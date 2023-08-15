using Autofac;
using Holo.Sdk.Configurations;

namespace Holo.Sdk.Modules;

/// <summary>
/// Abstract base class for modules.
/// </summary>
public abstract class ModuleBase : IModule
{
    /// <inheritdoc cref="IModule.Configure(ContainerBuilder, IConfigurationProvider)"/>
    public void Configure(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider)
        => OnConfigure(containerBuilder, configurationProvider);

    /// <summary>
    /// Invoked when the module needs to be configured. This happens only once.
    /// The default implementation does nothing.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> used to register services.</param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    protected virtual void OnConfigure(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider)
    {
    }
}