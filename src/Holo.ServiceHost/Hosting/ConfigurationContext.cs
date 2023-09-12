using System.Collections.Generic;
using Holo.Sdk.Modules;
using Holo.ServiceHost.Configurations;
using Polly.Registry;
using Serilog.Core;

namespace Holo.ServiceHost.Hosting;

/// <summary>
/// The context to hold data or services that are required by
/// host configuration phases down the chain.
/// </summary>
public sealed class ConfigurationContext
{
    /// <summary>
    /// Gets the name of the currently active application environment.
    /// </summary>
    public required string EnvironmentName { get; init; }

    /// <summary>
    /// Gets the <see cref="Configurations.ConfigurationProvider"/> used to get configurations.
    /// </summary>
    public required ConfigurationProvider ConfigurationProvider { get; init; }

    /// <summary>
    /// Gets the list of available module descriptors.
    /// </summary>
    public required IReadOnlyList<ModuleDescriptor> ModuleDescriptors { get; init; }

    /// <summary>
    /// Gets or sets the Polly policy registry.
    /// </summary>
    public IPolicyRegistry<string>? PolicyRegistry { get; set; }

    /// <summary>
    /// Gets or sets the mappings of logging level switches to their names.
    /// </summary>
    public required IReadOnlyDictionary<string, LoggingLevelSwitch> LevelSwitches { get; init; }
}