using System.Diagnostics.CodeAnalysis;

namespace Holo.Sdk.Configurations;

/// <summary>
/// Interface for a service that provides access to the application's configuration.
/// </summary>
public interface IConfigurationProvider
{
    bool TryGetValue<T>(string key, [NotNullWhen(true)] out T? value);

    T GetValue<T>(string key);

    OptionsConfiguration<TOptions> GetOptions<TOptions>(string configurationSectionName)
        where TOptions : class;
}