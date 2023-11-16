using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Holo.Sdk.BackgroundProcessing.Monitors;
using Holo.Sdk.BackgroundProcessing.Processors;
using Holo.Sdk.DI;
using Holo.Sdk.Lifecycle;
using Holo.Sdk.Storage;
using Holo.ServiceHost.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.ServiceHost.BackgroundProcessing.Monitors;

/// <summary>
/// An <see cref="IItemMonitor"/> that discovers items
/// that are ready to be processed in the background
/// by periodically polling the database.
/// </summary>
[Service(typeof(IItemMonitor), typeof(IStartable))]
public sealed class PollingItemMonitor : IItemMonitor
{
    private WorkItem? _workItem;
    private readonly IReadOnlyDictionary<string, IBackgroundItemProcessor> _processorsByItemType;
    private readonly IDbContextFactory _dbContextFactory;
    private readonly ILogger<PollingItemMonitor> _logger;
    private readonly IOptions<PollingItemMonitorOptions> _options;

    public int Priority { get; } = 100;

    public PollingItemMonitor(
        IEnumerable<IBackgroundItemProcessor> processors,
        IDbContextFactory dbContextFactory,
        ILogger<PollingItemMonitor> logger,
        IOptions<PollingItemMonitorOptions> options)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _options = options;
        _processorsByItemType = processors.ToDictionary(p => p.ItemType, p => p);
    }

    /// <inheritdoc cref="IStartable.StartAsync(CancellationToken)"/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_workItem != null)
            throw new InvalidOperationException("The monitor has been started already.");

        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var linkedCancellationToken = tokenSource.Token;
        var workTask = Task.Factory.StartNew(
            () => ContinuouslyProcessItemsAsync(linkedCancellationToken),
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);

        _workItem = new WorkItem(workTask, tokenSource);

        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IStartable.StopAsync"/>
    public async Task StopAsync()
    {
        var workItem = Interlocked.Exchange(ref _workItem, null);
        if (workItem == null)
            return;

        workItem.TokenSource.Cancel();
        await workItem.Task.ConfigureAwait(false);
        workItem.TokenSource.Dispose();
    }

    private async Task ContinuouslyProcessItemsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await TryDelayAsync(cancellationToken))
                continue;

            await using var dbContext = _dbContextFactory.Create<ServiceHostDbContext>();
            var processableItems = await dbContext.BackgroundProcessingItems
                .GroupBy(entity => entity.CorrelationId)
                .Take(_options.Value.MaxConcurrency)
                .Select(grouping => grouping.OrderBy(entity => entity.CreatedAt).First())
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);
            var results = await ProcessItemsAsync(processableItems, cancellationToken).ConfigureAwait(false);
            foreach (var (item, result) in results)
            {
                if (result == ProcessingResult.RetryLater)
                    continue;

                dbContext.BackgroundProcessingItems.Remove(item);
            }

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyList<(Item Item, ProcessingResult Result)>> ProcessItemsAsync(
        IEnumerable<Item> items,
        CancellationToken cancellationToken)
    {
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var linkedCancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.CancelAfter(_options.Value.ProcessingTimeoutInSeconds * 1000);

        var tasks = items.Select(async item => (
            Item: item,
            Result: await ProcessItemAsync(item, linkedCancellationToken).ConfigureAwait(false)));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        cancellationTokenSource.Dispose();

        return results;
    }

    private async Task<ProcessingResult> ProcessItemAsync(Item item, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogDebug(
                "Executing background processing item '{ItemId}' of type '{ItemType}'",
                item.Identifier.Value,
                item.ItemType);
            if (!_processorsByItemType.TryGetValue(item.ItemType, out var processor))
            {
                _logger.LogError(
                    "Failed to execute the background processing item '{ItemId}',"
                    + " because there is no matching processor named '{ProcessorName}'",
                    item.Identifier.Value,
                    item.ItemType);

                return ProcessingResult.Failure;
            }

            if (!ItemHelper.TryDeserializeItemData(item.SerializedItemData, out var itemData))
            {
                _logger.LogError(
                    "Failed to execute the background processing item '{ItemId}',"
                    + " because the deserialized item data was null",
                    item.Identifier.Value);

                return ProcessingResult.Failure;
            }

            var result = await processor.ProcessAsync(itemData, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug(
                "Executed background processing item '{ItemId}' of type '{ItemType}' in {Elapsed} ms",
                item.Identifier.Value,
                item.ItemType,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning(
                "Failed to execute the background processing item '{ItemId}' due to cancellation",
                item.Identifier.Value);

            return ProcessingResult.Failure;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Failed to execute the background processing item '{ItemId}' due to an unexpected error",
                item.Identifier.Value);

            return ProcessingResult.Failure;
        }
    }

    private async Task<bool> TryDelayAsync(CancellationToken cancellationToken)
    {
        var timeSpan = TimeSpan.FromMinutes(_options.Value.PollingIntervalInMinutes);

        try
        {
            await Task.Delay(timeSpan, cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private sealed record WorkItem(Task Task, CancellationTokenSource TokenSource);
}