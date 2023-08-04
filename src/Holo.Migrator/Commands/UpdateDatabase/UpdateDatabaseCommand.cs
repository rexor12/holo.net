using System;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Holo.Sdk.Assemblies;
using Holo.Sdk.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Holo.Migrator.Commands.UpdateDatabase;

public sealed class UpdateDatabaseCommand : CommandBase<UpdateDatabaseOptions>
{
    public override Task OnExecuteAsync(UpdateDatabaseOptions options)
    {
        using var serviceProvider = CreateServiceProvider(options);
        using var scope = serviceProvider.CreateScope();
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        Console.WriteLine("Successfully finished the migration process");

        return Task.CompletedTask;
    }

    private static ServiceProvider CreateServiceProvider(UpdateDatabaseOptions options)
    {
        var assemblyLoader = new AssemblyLoader(
            EmptyLogger<AssemblyLoader>.Instance,
            options.AssemblyGlobPatterns.ToArray(),
            options.AssemblyNamePattern,
            options.AssemblyBaseDirectoryPath);
        var migrationAssemblies = assemblyLoader.LoadAssemblies().ToArray();
        foreach (var assembly in migrationAssemblies)
            Console.WriteLine($"Discovered migration assembly '{assembly.FullName}'");

        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(options.ConnectionString)
                .ScanIn(migrationAssemblies)
                    .For.Migrations()
                    .For.EmbeddedResources())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}