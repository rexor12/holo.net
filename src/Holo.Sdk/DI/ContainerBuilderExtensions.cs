using Autofac;
using Holo.Sdk.Configurations;

namespace Holo.Sdk.DI;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Registers the specified option type.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to add the registration to.</param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> for reading the configurations.
    /// </param>
    /// <param name="sectionName">The name of the configuration section.</param>
    /// <typeparam name="TOptions">The type of the options to register.</typeparam>
    /// <returns>The same instance of <see cref="ContainerBuilder"/>.</returns>
    public static ContainerBuilder RegisterOptions<TOptions>(
        this ContainerBuilder containerBuilder,
        IConfigurationProvider configurationProvider,
        string sectionName)
        where TOptions : class
    {
        containerBuilder.RegisterOptions(configurationProvider.GetOptions<TOptions>(sectionName));
        return containerBuilder;
    }
}