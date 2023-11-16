using System.Threading;
using System.Threading.Tasks;

namespace Holo.Sdk.BackgroundProcessing.Processors;

/// <summary>
/// Interface for a service used to process a specific type of item in the background.
/// </summary>
public interface IBackgroundItemProcessor
{
    /// <summary>
    /// Gets the name that identifies items associated to this processor.
    /// </summary>
    string ItemType { get; }

    /// <summary>
    /// Processes the specified <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item to be processed.</param>
    /// <param name="cancellationToken">
    /// An optional <see cref="CancellationToken"/> used to cancel the operation.
    /// </param>
    /// <returns>The <see cref="ProcessingResult"/> of the operation.</returns>
    Task<ProcessingResult> ProcessAsync(object item, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for a service used to process a specific type of item in the background.
/// </summary>
/// <typeparam name="TItem">The type of the items the processor can process.</typeparam>
public interface IBackgroundItemProcessor<in TItem> : IBackgroundItemProcessor
{
    /// <summary>
    /// Processes the specified <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item to be processed.</param>
    /// <param name="cancellationToken">
    /// An optional <see cref="CancellationToken"/> used to cancel the operation.
    /// </param>
    /// <returns>The <see cref="ProcessingResult"/> of the operation.</returns>
    Task<ProcessingResult> ProcessAsync(TItem item, CancellationToken cancellationToken = default);
}
