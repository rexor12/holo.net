using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Holo.ServiceHost.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Settings.Configuration;

namespace Holo.ServiceHost;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        // Bootstrap logger to avoid losing logs during host startup.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var host = new Host();
        var app = CreateAppBuilder(args, host).Build();
        ConfigureApp(app);

        var logger = app.Services.GetService<ILogger<Program>>()!;
        logger.LogInformation("Starting host...");
        try
        {
            await RunAppAsync(app, host, logger);
        }
        finally
        {
            logger.LogInformation("Successfully shut down host");
            await Log.CloseAndFlushAsync();
        }
    }

    private static WebApplicationBuilder CreateAppBuilder(string[] args, Host host)
    {
        var builder = WebApplication.CreateBuilder(args);
        var levelSwitches = new Dictionary<string, LoggingLevelSwitch>();
        var configurationContext = host.BuildContext(
            builder.Environment.EnvironmentName,
            levelSwitches);

        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions
            {
                OnLevelSwitchCreated = (name, levelSwitch) => levelSwitches[name] = levelSwitch
            })
            .ReadFrom.Services(services));
        builder.Services.AddOptions();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        host.ConfigureServiceCollection(builder.Services, configurationContext);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(
            (c, b) => host.ConfigureContainer(b, configurationContext));

        return builder;
    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static async Task RunAppAsync(WebApplication app, Host host, Microsoft.Extensions.Logging.ILogger logger)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        Task hostTask = await host.StartAsync(app.Services, cancellationTokenSource.Token);

        app.Run();

        logger.LogInformation("Shutting down host...");
        cancellationTokenSource.Cancel();

        try
        {
            await hostTask;
        }
        catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
        {
            // Ignored.
        }
        finally
        {
            cancellationTokenSource.Dispose();
            hostTask.Dispose();
        }
    }
}