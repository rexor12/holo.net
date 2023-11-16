using Holo.Sdk.Lifecycle;

namespace Holo.Sdk.BackgroundProcessing.Monitors;

/// <summary>
/// Interface for a service that automatically discovers items
/// that are ready to be processed in the background.
/// </summary>
public interface IItemMonitor : IStartable
{
}
