using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Assemblies;

public sealed class AssemblyLoader
{
    private readonly ILogger<AssemblyLoader> _logger;
    private readonly IReadOnlyCollection<string> _assemblyGlobPatterns;
    private readonly string _moduleAssemblyNamePattern;
    private readonly string? _assemblyBaseDirectoryPath;

    public AssemblyLoader(
        ILogger<AssemblyLoader> logger,
        IReadOnlyCollection<string> assemblyGlobPatterns,
        string moduleAssemblyNamePattern,
        string? assemblyBaseDirectoryPath = null)
    {
        _logger = logger;
        _assemblyGlobPatterns = assemblyGlobPatterns;
        _moduleAssemblyNamePattern = moduleAssemblyNamePattern;
        _assemblyBaseDirectoryPath = assemblyBaseDirectoryPath;
    }

    public IReadOnlyList<Assembly> LoadAssemblies()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Module assembly glob paths: {Paths}", string.Join(";", _assemblyGlobPatterns));
        }

        var rootDirectory = new DirectoryInfo(_assemblyBaseDirectoryPath ?? Environment.CurrentDirectory);
        var matcher = new Matcher();
        matcher.AddIncludePatterns(_assemblyGlobPatterns);
        var results = matcher.Execute(new DirectoryInfoWrapper(rootDirectory));
        if (!results.HasMatches)
        {
            _logger.LogInformation("No module assemblies have been found");
            return Array.Empty<Assembly>();
        }

        var moduleAssemblyNameRegex = new Regex(_moduleAssemblyNamePattern);
        var assemblies = new List<Assembly>();
        foreach (var result in results.Files)
        {
            var filePath = Path.Combine(rootDirectory.FullName, result.Path);
            var assemblyName = Path.GetFileNameWithoutExtension(filePath);
            if (!moduleAssemblyNameRegex.IsMatch(assemblyName))
                continue;

            var loadContext = new AssemblyLoadContext(filePath);
            _logger.LogDebug("Loading assembly '{AssemblyName}'", assemblyName);

            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(assemblyName));
            assemblies.Add(assembly);

            _logger.LogDebug("Loaded assembly '{AssemblyName}'", assemblyName);
        }

        _logger.LogInformation("Loaded {Count} assemblies", assemblies.Count);
        return assemblies;
    }
}