using System;
using System.Net.Http;
using Autofac;
using Holo.Sdk.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;

namespace Holo.Sdk.Modules;

/// <summary>
/// Abstract base class for modules.
/// </summary>
public abstract class ModuleBase : IModule
{
    protected delegate void AddHttpClientDelegate(
        string name,
        Func<AsyncPolicy<HttpResponseMessage>>? policyFactory = null,
        Action<HttpClient>? configure = null);

    /// <inheritdoc cref="IModule.ConfigureContainer(ContainerBuilder, IConfigurationProvider)"/>
    public void ConfigureContainer(
        ContainerBuilder containerBuilder,
        IConfigurationProvider configurationProvider)
    {
        OnConfigureContainer(containerBuilder, configurationProvider);
    }

    /// <inheritdoc cref="IModule.ConfigureServiceCollection(IServiceCollection, IConfigurationProvider, IPolicyRegistry{string})"/>
    public void ConfigureServiceCollection(
        IServiceCollection serviceCollection,
        IConfigurationProvider configurationProvider,
        IPolicyRegistry<string> policyRegistry)
    {
        OnConfigureHttpClients(
            (name, policyFactory, configure) => AddHttpClient(
                serviceCollection,
                policyRegistry,
                name,
                policyFactory,
                configure),
            configurationProvider);
    }

    /// <summary>
    /// Invoked when the module should be configured. This happens only once.
    /// The default implementation does nothing.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/> used to register services.</param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    protected virtual void OnConfigureContainer(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider)
    {
    }

    /// <summary>
    /// Invoked when any required HTTP clients should be configured. This happens only once.
    /// The default implementation does nothing.
    /// </summary>
    /// <param name="addHttpClient">A delegate used to add HTTP clients.</param>
    /// <param name="configurationProvider">
    /// The <see cref="IConfigurationProvider"/> used to get configurations.
    /// </param>
    protected virtual void OnConfigureHttpClients(
        AddHttpClientDelegate addHttpClient,
        IConfigurationProvider configurationProvider)
    {
    }

    private static void AddHttpClient(
        IServiceCollection serviceCollection,
        IPolicyRegistry<string> policyRegistry,
        string name,
        Func<AsyncPolicy<HttpResponseMessage>>? policyFactory,
        Action<HttpClient>? configure)
    {
        var httpClientBuilder = serviceCollection.AddHttpClient(name);
        if (configure != null)
            httpClientBuilder.ConfigureHttpClient(configure);

        if (policyFactory != null)
        {
            policyRegistry.Add(name, policyFactory());
            httpClientBuilder.AddPolicyHandlerFromRegistry(name);
        }
    }
}