using Microsoft.Extensions.Options;

namespace Holo.Sdk.Configurations;

public sealed class OptionsConfiguration<TOptions>
    where TOptions : class
{
    public ConfigurationChangeTokenSource<TOptions> ConfigurationChangeTokenSource { get; }
    public NamedConfigureFromConfigurationOptions<TOptions> NamedConfigureFromConfigurationOptions { get; }

    public OptionsConfiguration(
        ConfigurationChangeTokenSource<TOptions> configurationChangeTokenSource,
        NamedConfigureFromConfigurationOptions<TOptions> namedConfigureFromConfigurationOptions)
    {
        this.ConfigurationChangeTokenSource = configurationChangeTokenSource;
        NamedConfigureFromConfigurationOptions = namedConfigureFromConfigurationOptions;
    }
}
