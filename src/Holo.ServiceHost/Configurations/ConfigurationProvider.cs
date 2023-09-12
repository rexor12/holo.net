using System;
using System.Diagnostics.CodeAnalysis;
using Holo.Sdk.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IConfigurationProvider = Holo.Sdk.Configurations.IConfigurationProvider;

namespace Holo.ServiceHost.Configurations;

public sealed class ConfigurationProvider : IConfigurationProvider
{
    private readonly IConfigurationRoot _configurationRoot;

    public ConfigurationProvider(IConfigurationRoot configurationRoot)
    {
        _configurationRoot = configurationRoot;
    }

    public static ConfigurationProvider Create(
        string environmentName,
        string environmentVariablePrefix = "HOLO_",
        IConfigurationSource[]? extraConfigurationSources = null)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.Add(new ExpandJsonConfigurationSource("appsettings.json", false, false));
        configurationBuilder.Add(new ExpandJsonConfigurationSource($"appsettings.{environmentName}.json", true, false));
        configurationBuilder.Add(new ExpandJsonConfigurationSource($"appsettings.override.json", true, false));
        configurationBuilder.AddEnvironmentVariables(environmentVariablePrefix);
        if (extraConfigurationSources?.Length is not null and > 0)
            foreach (var extraConfigurationSource in extraConfigurationSources)
                configurationBuilder.Add(extraConfigurationSource);

        return new ConfigurationProvider(configurationBuilder.Build());
    }

    public bool TryGetValue<T>(string key, [NotNullWhen(true)] out T? value)
    {
        value = _configurationRoot.GetValue<T>(key);
        return value != null;
    }

    public T GetValue<T>(string key)
    {
        if (!TryGetValue<T>(key, out var value))
            throw new ArgumentOutOfRangeException(nameof(key), value, "No configuration for the specified key can be found.");

        return value;
    }

    public IConfigurationSection GetSection(string key)
        => _configurationRoot.GetSection(key);

    public OptionsConfiguration<TOptions> GetOptions<TOptions>(string configurationSectionName)
        where TOptions : class
    {
        var section = _configurationRoot.GetSection(configurationSectionName);

        return new OptionsConfiguration<TOptions>(
            new ConfigurationChangeTokenSource<TOptions>(Options.DefaultName, section),
            new NamedConfigureFromConfigurationOptions<TOptions>(Options.DefaultName, section));
    }
}