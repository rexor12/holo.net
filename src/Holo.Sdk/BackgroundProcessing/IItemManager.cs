using System.Threading;
using System.Threading.Tasks;
using Holo.Sdk.BackgroundProcessing.Processors;

namespace Holo.Sdk.BackgroundProcessing;

/// <summary>
/// Interface for a service providing functionality for manipulating background processing items.
/// </summary>
public interface IItemManager
{
    /// <summary>
    /// Gets whether one or more items of the specified <paramref name="itemType"/> exist in the queue.
    /// </summary>
    /// <param name="itemType">The type of the item to check.</param>
    /// <param name="cancellationToken">
    /// An optional <see cref="CancellationToken"/> used to cancel the operation.
    /// </param>
    /// <returns><c>true</c>, if one or more items of the specified type exist in the queue.</returns>
    Task<bool> HasItemAsync(string itemType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues the given <paramref name="item"/> for processing in the background.
    /// </summary>
    /// <param name="item">The item to be enqueued for processing.</param>
    /// <param name="itemType">
    /// The unique type of the item. This determines which <see cref="IBackgroundItemProcessor"/>
    /// will process the item.
    /// </param>
    /// <param name="correlationId">
    /// A correlation identifier that is used to determine the order in which
    /// items should be processed, in a first-in-first-out manner.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional <see cref="CancellationToken"/> used to cancel the operation.
    /// </param>
    /// <returns>An awaitable <see cref="Task"/> that represents the enqueuing operation.</returns>
    Task EnqueueAsync(
        object item,
        string itemType,
        string correlationId,
        CancellationToken cancellationToken = default);
}