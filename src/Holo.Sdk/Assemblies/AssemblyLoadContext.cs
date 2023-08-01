using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Holo.Sdk.Assemblies;

public sealed class AssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;

    public AssemblyLoadContext(string assemblyPath)
    {
        _resolver = new AssemblyDependencyResolver(assemblyPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        try
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }
        catch (Exception e) when (e is FileNotFoundException or FileLoadException)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
        }
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}
