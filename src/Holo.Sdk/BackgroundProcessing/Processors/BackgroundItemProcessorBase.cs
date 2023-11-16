using System;
using System.Threading;
using System.Threading.Tasks;

namespace Holo.Sdk.BackgroundProcessing.Processors;

/// <summary>
/// Abstract base class for a <see cref="IBackgroundItemProcessor"/>.
/// </summary>
/// <typeparam name="TItem">The type of the items the processor can process.</typeparam>
public abstract class BackgroundItemProcessorBase<TItem> : IBackgroundItemProcessor<TItem>
{
    /// <inheritdoc cref="IBackgroundItemProcessor.ItemType"/>
    public string ItemType { get; } = typeof(TItem).Name;

    /// <inheritdoc cref="IBackgroundItemProcessor.ProcessAsync(object, CancellationToken)"/>
    public Task<ProcessingResult> ProcessAsync(object item, CancellationToken cancellationToken = default)
    {
        if (item is not TItem typedItem)
            throw new ArgumentException(
                $"Expected an item of type '{typeof(TItem).FullName}', but got '{item.GetType().FullName}'.",
                nameof(item));

        return ProcessAsync(typedItem, cancellationToken);
    }

    /// <inheritdoc cref="IBackgroundItemProcessor{TItem}.ProcessAsync(TItem, CancellationToken)"/>
    public abstract Task<ProcessingResult> ProcessAsync(TItem item, CancellationToken cancellationToken = default);
}