using System.Collections.Generic;
using CommandLine;

namespace Holo.Migrator.Commands.UpdateDatabase;

[Verb("update-database", HelpText = "Applies any new database migrations.")]
public sealed class UpdateDatabaseOptions
{
    [Option(
        'c',
        nameof(ConnectionString),
        Required = true,
        HelpText = "The PostgreSQL database connection string.")]
    public required string ConnectionString { get; set; }

    [Option(
        'a',
        nameof(AssemblyGlobPatterns),
        Required = true,
        HelpText = "A list of glob patterns for assemblies containing the migrations.")]
    public required IEnumerable<string> AssemblyGlobPatterns { get; set; }

    [Option(
        'n',
        nameof(AssemblyNamePattern),
        Required = true,
        HelpText = "A regular expression pattern for matching assemblies.")]
    public required string AssemblyNamePattern { get; set; }

    [Option(
        'd',
        nameof(AssemblyBaseDirectoryPath),
        Required = false,
        HelpText = "The path to the base directory for discovering assemblies. Defaults to the current working directory.")]
    public string? AssemblyBaseDirectoryPath { get; set; }
}
