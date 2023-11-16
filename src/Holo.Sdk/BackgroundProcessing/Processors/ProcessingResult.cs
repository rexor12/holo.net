namespace Holo.Sdk.BackgroundProcessing.Processors;

/// <summary>
/// Defines the valid values for the result of the processing of an item.
/// </summary>
public enum ProcessingResult
{
    /// <summary>
    /// Marks that the item has been processed successfully.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Marks that the processing of the item has failed and no further attempts should be made.
    /// </summary>
    Failure,

    /// <summary>
    /// Marks that another attempt to process the item should be made at a later point in time.
    /// </summary>
    RetryLater
}
