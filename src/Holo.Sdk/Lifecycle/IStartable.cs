using System.Threading;
using System.Threading.Tasks;

namespace Holo.Sdk.Lifecycle;

/// <summary>
/// Interface for a service that requires to be started before modules are started.
/// </summary>
/// <remarks>
/// While shutting down the application, these services are stopped in reverse order
/// based on their priorities, and only after the modules are stopped.
/// </remarks>
public interface IStartable
{
    /// <summary>
    /// Gets the priority of the service.
    /// </summary>
    /// <remarks>A lower number means a higher priority.</remarks>
    int Priority { get; }

    /// <summary>
    /// Starts the <see cref="IStartable"/>.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the <see cref="IStartable"/>.
    /// </summary>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task StopAsync();
}