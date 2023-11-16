using System;
using System.Threading;
using System.Threading.Tasks;
using Holo.Sdk.BackgroundProcessing;
using Holo.Sdk.Chrono;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;
using Holo.ServiceHost.Storage;
using Microsoft.EntityFrameworkCore;

namespace Holo.ServiceHost.BackgroundProcessing;

/// <summary>
/// A service providing functionality for manipulating background processing items.
/// </summary>
[Service(typeof(IItemManager))]
public sealed class ItemManager : IItemManager
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDbContextFactory _dbContextFactory;
    private readonly IUnitOfWorkProvider _unitOfWorkProvider;

    public ItemManager(
        IDateTimeProvider dateTimeProvider,
        IDbContextFactory dbContextFactory,
        IUnitOfWorkProvider unitOfWorkProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _dbContextFactory = dbContextFactory;
        _unitOfWorkProvider = unitOfWorkProvider;
    }

    /// <inheritdoc cref="IItemManager.HasItemAsync(string, CancellationToken)"/>
    public async Task<bool> HasItemAsync(string itemType, CancellationToken cancellationToken)
    {
        ServiceHostDbContext dbContext;
        var disposeDbContext = true;
        var unitOfWork = _unitOfWorkProvider.Current;
        if (unitOfWork != null)
        {
            dbContext = unitOfWork.GetDbContext<ServiceHostDbContext>();
            disposeDbContext = false;
        }
        else
        {
            dbContext = _dbContextFactory.Create<ServiceHostDbContext>();
        }

        var result = await dbContext.BackgroundProcessingItems
            .AnyAsync(entity => entity.ItemType == itemType, cancellationToken)
            .ConfigureAwait(false);

        if (disposeDbContext)
            await dbContext.DisposeAsync().ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc cref="IItemManager.EnqueueAsync(object, string, string, CancellationToken)"/>
    public async Task EnqueueAsync(
        object item,
        string itemType,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var itemWrapper = new Item
        {
            Identifier = new ItemId(Guid.NewGuid()),
            CreatedAt = _dateTimeProvider.Now(),
            CorrelationId = correlationId,
            ItemType = itemType,
            SerializedItemData = ItemHelper.SerializeItemData(item)
        };
        var unitOfWork = _unitOfWorkProvider.Current;
        if (unitOfWork != null)
        {
            unitOfWork.GetDbContext<ServiceHostDbContext>().BackgroundProcessingItems.Add(itemWrapper);
            return;
        }

        await using var dbContext = _dbContextFactory.Create<ServiceHostDbContext>();
        dbContext.BackgroundProcessingItems.Add(itemWrapper);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}