using System.Threading.Tasks;

namespace Holo.Module.Dev.Managers;

/// <summary>
/// Interface for a service that can be used to turn application maintenance mode on or off.
/// </summary>
public interface IMaintenanceManager
{
    /// <summary>
    /// Determines whether application maintenance mode is enabled.
    /// </summary>
    /// <returns><c>true</c>, if maintenance mode is enabled.</returns>
    Task<bool> IsMaintenanceModeEnabledAsync();

    /// <summary>
    /// Sets the operation mode of the application.
    /// </summary>
    /// <param name="isMaintenanceMode"><c>true</c>, if maintenance mode is enabled.</param>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task SetOperationModeAsync(bool isMaintenanceMode);
}