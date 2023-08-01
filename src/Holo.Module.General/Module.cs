using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Holo.Module.General.Cookies.Configuration;
using Holo.Sdk.Configurations;
using Holo.Sdk.DI;
using Holo.Sdk.Modules;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General;

[Service(typeof(IModule))]
public sealed class Module : ModuleBase
{
    private readonly ILogger<Module> _logger;

    public Module(ILogger<Module> logger)
    {
        _logger = logger;
    }

    protected override void OnConfigure(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider)
    {
        containerBuilder.RegisterOptions(
            configurationProvider.GetOptions<FortuneCookieOptions>(FortuneCookieOptions.SectionName));
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("General module started");
        return Task.CompletedTask;
    }
}