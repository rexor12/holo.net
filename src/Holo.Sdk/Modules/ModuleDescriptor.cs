using System;
using System.Linq;
using System.Reflection;

namespace Holo.Sdk.Modules;

/// <summary>
/// Holds information about a module.
/// </summary>
/// <param name="Assembly">The <see cref="System.Reflection.Assembly"/> that contains the module.</param>
public sealed class ModuleDescriptor
{
    public Assembly Assembly { get; }

    public IModule Module { get; }

    public ModuleDescriptor(Assembly assembly)
    {
        Assembly = assembly;
        Module = GetModule(assembly);
    }

    private static IModule GetModule(Assembly assembly)
    {
        var moduleType = assembly
            .GetTypes()
            .Single(type => type.IsClass
                            && !type.IsAbstract
                            && type.IsAssignableTo(typeof(IModule)));
        if (Activator.CreateInstance(moduleType) is not IModule module)
            throw new ArgumentException(
                $"Failed to create an instance of module '{moduleType.AssemblyQualifiedName}'.",
                nameof(assembly));

        return module;
    }
}