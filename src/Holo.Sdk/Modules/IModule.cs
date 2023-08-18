using Autofac;
using Holo.Sdk.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Polly.Registry;

namespace Holo.Sdk.Modules;

/// <summary>
/// Interface for a module.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Configures the service collection.
    /// </summary>
    /// <param name="serviceCollection">
    /// The <see cref="IServiceCollection"/> to configure.
    /// </param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    /// <param name="policyRegistry">
    /// The <see cref="IPolicyRegistry{TKey}"/> where named Polly policies can be registered.
    /// </param>
    void ConfigureServiceCollection(
        IServiceCollection serviceCollection,
        IConfigurationProvider configurationProvider,
        IPolicyRegistry<string> policyRegistry);

    /// <summary>
    /// Configures the container.
    /// </summary>
    /// <param name="containerBuilder">
    /// The <see cref="ContainerBuilder"/> to hold the registrations.
    /// </param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    void ConfigureContainer(
        ContainerBuilder containerBuilder,
        IConfigurationProvider configurationProvider);
}
