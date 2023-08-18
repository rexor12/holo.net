using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Holo.Sdk.Assemblies;
using Holo.Sdk.Configurations;
using Holo.Sdk.Logging;
using Holo.Sdk.Modules;
using Holo.Sdk.Storage;
using Holo.Sdk.Tasks;
using Holo.ServiceHost.Bot;
using Holo.ServiceHost.Configurations;
using Holo.ServiceHost.Modules;
using Holo.ServiceHost.Reflection;
using Holo.ServiceHost.Resources;
using Holo.ServiceHost.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using IStartable = Holo.Sdk.Lifecycle.IStartable;

namespace Holo.ServiceHost.Hosting;

/// <summary>
/// Responsible for hosting a Discord client.
/// </summary>
public sealed class Host
{
    private static readonly DiscordSocketConfig SocketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged
    };

    private static readonly InteractionServiceConfig InteractionServiceConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        UseCompiledLambda = true,
        AutoServiceScopes = false
    };

    /// <summary>
    /// Creates a <see cref="ConfigurationContext"/> to be shared between the configuration phases.
    /// </summary>
    /// <param name="environmentName">The name of the currently active application environment.</param>
    /// <returns>
    /// A new instance of <see cref="ConfigurationContext"/> to be shared between the configuration phases.
    /// </returns>
    public ConfigurationContext BuildContext(string environmentName)
    {
        var configurationProvider = ConfigurationProvider.Create(environmentName);
        var moduleAssemblyGlobPatterns = configurationProvider
            .GetSection("ModuleOptions:ModuleAssemblyGlobPatterns")
            .GetChildren()
            .Select(i => i.Value)
            .ToArray();
        var moduleAssemblyNamePattern = configurationProvider.GetValue<string>("ModuleOptions:ModuleAssemblyNamePattern");
        var assemblyLoader = new AssemblyLoader(
            ConsoleLogger<AssemblyLoader>.Instance,
            moduleAssemblyGlobPatterns!,
            moduleAssemblyNamePattern);
        var moduleDescriptors = assemblyLoader
            .LoadAssemblies()
            .Select(assembly => new ModuleDescriptor(assembly))
            .ToArray();

        return new ConfigurationContext
        {
            EnvironmentName = environmentName,
            ConfigurationProvider = configurationProvider,
            ModuleDescriptors = moduleDescriptors
        };
    }

    /// <summary>
    /// Configures the service collection for dependency injection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <param name="context">The <see cref="ConfigurationContext"/> shared between the configuration phases.</param>
    public void ConfigureServiceCollection(IServiceCollection serviceCollection, ConfigurationContext context)
    {
        context.PolicyRegistry = serviceCollection.AddPolicyRegistry();
        foreach (var moduleDescriptor in context.ModuleDescriptors)
        {
            moduleDescriptor.Module.ConfigureServiceCollection(
                serviceCollection,
                context.ConfigurationProvider,
                context.PolicyRegistry);
            RegisterDbContexts(serviceCollection, context.ConfigurationProvider, moduleDescriptor.Assembly);
        }
    }

    /// <summary>
    /// Configures the container for dependency injection.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/>.</param>
    /// <param name="context">The <see cref="ConfigurationContext"/> shared between the configuration phases.</param>
    public void ConfigureContainer(ContainerBuilder containerBuilder, ConfigurationContext context)
    {
        containerBuilder.RegisterInstance(context.ConfigurationProvider).As<IConfigurationProvider>();
        containerBuilder.RegisterType<DbContextFactory>().As<IDbContextFactory>().SingleInstance();
        containerBuilder.RegisterType<UnitOfWorkProvider>().As<IUnitOfWorkProvider>().SingleInstance();
        containerBuilder.RegisterInstance(SocketConfig).As<DiscordSocketConfig>();
        containerBuilder.RegisterType<DiscordSocketClient>().SingleInstance();
        containerBuilder.Register(c => new InteractionService(c.Resolve<DiscordSocketClient>(), InteractionServiceConfig));
        containerBuilder.RegisterOptions(context.ConfigurationProvider.GetOptions<DiscordOptions>(DiscordOptions.SectionName));
        containerBuilder.RegisterOptions(context.ConfigurationProvider.GetOptions<ModuleOptions>(ModuleOptions.SectionName));
        containerBuilder.RegisterOptions(context.ConfigurationProvider.GetOptions<DatabaseOptions>(DatabaseOptions.SectionName));
        containerBuilder.RegisterOptions(context.ConfigurationProvider.GetOptions<ResourceOptions>(ResourceOptions.SectionName));

        RegisterServices(containerBuilder, Assembly.GetAssembly(typeof(Program))!);
        RegisterModules(containerBuilder, context);
    }

    /// <summary>
    /// Starts the Discord client with the configured services.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to access services.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to shut down the client.</param>
    /// <returns>A <see cref="Task"/> that completes when the client is shut down.</returns>
    public async Task<Task> StartAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Host>>();
        var startables = serviceProvider.GetServices<IStartable>();
        var client = await StartClientAsync(serviceProvider, startables, logger, cancellationToken);

        return ShutdownLaterAsync(client, startables, logger, cancellationToken);
    }

    private static void RegisterServices(ContainerBuilder containerBuilder, Assembly assembly)
    {
        foreach (var descriptor in assembly.GetServiceDescriptors())
        {
            var registration = containerBuilder.RegisterType(descriptor.ServiceType);
            foreach (var contract in descriptor.ContractTypes)
                registration = registration.As(contract);

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case ServiceLifetime.Transient:
                    registration.InstancePerDependency();
                    break;
                case ServiceLifetime.Scoped:
                    registration.InstancePerLifetimeScope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(Modules.ServiceDescriptor.Lifetime),
                        descriptor.Lifetime,
                        "Unknown type of service lifetime.");
            }
        }
    }

    private static void RegisterModules(
        ContainerBuilder containerBuilder,
        ConfigurationContext context)
    {
        foreach (var descriptor in context.ModuleDescriptors)
        {
            containerBuilder.RegisterInstance(descriptor).As<ModuleDescriptor>();
            descriptor.Module.ConfigureContainer(containerBuilder, context.ConfigurationProvider);
            RegisterServices(containerBuilder, descriptor.Assembly);
        }
    }

    private static void RegisterDbContexts(
        IServiceCollection serviceCollection,
        IConfigurationProvider configurationProvider,
        Assembly assembly)
    {
        var connectionString = configurationProvider.GetValue<string>("DatabaseOptions:ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("The connection string must be specified in the configuration.", "DatabaseOptions:ConnectionString");

        var addNpgsqlMethod = typeof(NpgsqlServiceCollectionExtensions).GetMethod(
            nameof(NpgsqlServiceCollectionExtensions.AddNpgsql),
            new[]
            {
                typeof(IServiceCollection),
                typeof(string),
                typeof(Action<NpgsqlDbContextOptionsBuilder>),
                typeof(Action<DbContextOptionsBuilder>)
            });
        if (addNpgsqlMethod == null)
            throw new NotSupportedException("The required 'AddNpgsql' method cannot be found. The code may need to be updated.");

        foreach (var dbContextType in assembly.GetDbContexts())
        {
            var closedGenericMethod = addNpgsqlMethod.MakeGenericMethod(dbContextType);
            closedGenericMethod.Invoke(null, new object?[] { serviceCollection, connectionString, null, null });
        }
    }

    private static async Task<DiscordSocketClient> StartClientAsync(
        IServiceProvider serviceProvider,
        IEnumerable<IStartable> startables,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var options = serviceProvider.GetRequiredService<IOptions<DiscordOptions>>();
        var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        client.Log += message => logger.LogAsync(message);

        foreach (var startable in startables)
            await startable.StartAsync(cancellationToken);

        await serviceProvider.GetRequiredService<InteractionHandler>().InitializeAsync();
        await client.LoginAsync(TokenType.Bot, options.Value.BotToken);
        await client.StartAsync();

        return client;
    }

    private static async Task ShutdownLaterAsync(
        DiscordSocketClient client,
        IEnumerable<IStartable> startables,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        await TaskHelper.TryAwaitAsync(
            ct => Task.Delay(Timeout.Infinite, ct),
            logger,
            cancellationToken,
            true,
            typeof(TaskCanceledException),
            typeof(OperationCanceledException));

        await TaskHelper.TryAwaitAsync(c => c.DisposeAsync(), logger, client);

        foreach (var startable in startables)
            await TaskHelper.TryAwaitAsync(s => s.StopAsync(), logger, startable);
    }
}