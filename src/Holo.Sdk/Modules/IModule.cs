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
}
