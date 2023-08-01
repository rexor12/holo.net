using Autofac;
using Microsoft.Extensions.Options;

namespace Holo.Sdk.Configurations;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Registers an options type that can be injected via <see cref="IOptions{TOptions}"/>.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> to use for registration.</param>
    /// <param name="optionsConfiguration">The <see cref="OptionsConfiguration{TOptions}"/> to be registered.</param>
    /// <typeparam name="TOptions">The type of the options class.</typeparam>
    /// <returns>The same <see cref="ContainerBuilder"/>.</returns>
    public static ContainerBuilder RegisterOptions<TOptions>(
        this ContainerBuilder containerBuilder,
        OptionsConfiguration<TOptions> optionsConfiguration)
        where TOptions : class
    {
        containerBuilder.RegisterInstance(optionsConfiguration.ConfigurationChangeTokenSource).As<IOptionsChangeTokenSource<TOptions>>();
        containerBuilder.RegisterInstance(optionsConfiguration.NamedConfigureFromConfigurationOptions).As<IConfigureOptions<TOptions>>();

        return containerBuilder;
    }
}