using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Holo.Sdk.Configurations;

namespace Holo.Sdk.Modules;

/// <summary>
/// Interface for a module.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Configures the module by registering the required services.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to hold the registrations.</param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    void Configure(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider);

    /// <summary>
    /// Starts the module.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> used to signal cancellation.
    /// </param>
    /// <remarks>This method is invoked by the framework.</remarks>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the module.
    /// </summary>
    /// <remarks>This method is invoked by the framework.</remarks>
    Task StopAsync();
}
