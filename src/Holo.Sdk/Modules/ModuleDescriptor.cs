using System.Reflection;

namespace Holo.Sdk.Modules;

/// <summary>
/// Holds information about a module.
/// </summary>
/// <param name="Assembly">The <see cref="System.Reflection.Assembly"/> that contains the module.</param>
public sealed record ModuleDescriptor(
    Assembly Assembly);