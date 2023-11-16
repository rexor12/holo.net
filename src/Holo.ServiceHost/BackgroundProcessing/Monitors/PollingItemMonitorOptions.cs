namespace Holo.ServiceHost.BackgroundProcessing.Monitors;

/// <summary>
/// Options for <see cref="PollingItemMonitor"/>.
/// </summary>
public sealed class PollingItemMonitorOptions
{
    /// <summary>
    /// The name of the associated section.
    /// </summary>
    public const string SectionName = "BackgroundProcessing:Monitors:PollingItemMonitor";

    /// <summary>
    /// Gets or sets the interval between each polling request, in minutes.
    /// </summary>
    public int PollingIntervalInMinutes { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of asynchronous tasks
    /// that the monitor uses to process background processing items.
    /// </summary>
    public int MaxConcurrency { get; set; } = 4;

    /// <summary>
    /// Gets or sets the amount of time, in seconds, after which
    /// a background processing item fails with a timeout error.
    /// </summary>
    public int ProcessingTimeoutInSeconds { get; set; } = 60;
}