using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Holo.ServiceHost.Bot;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Holo.ServiceHost;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        var host = new Host();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddLogging(configure => configure.AddConsole());
        builder.Services.AddOptions();
        builder.Services.AddControllers();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(
            (c, b) => host.Configure(b, builder.Services, builder.Environment.EnvironmentName));

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        var logger = app.Services.GetService<ILogger<Program>>()!;
        logger.LogInformation("Starting host...");

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

        logger.LogInformation("Successfully shut down host");
    }
}