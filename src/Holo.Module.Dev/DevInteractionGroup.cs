using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Module.Dev.Managers;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Holo.Sdk.Logging;
using Microsoft.Extensions.Logging;

namespace Holo.Module.Dev;

[GuildBound("development", 999259836439081030)]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("dev", "A group of commands for assisting with development.")]
public sealed class DevInteractionGroup : InteractionGroupBase
{
    private readonly ILoggerManager _loggerManager;
    private readonly IMaintenanceManager _maintenanceManager;

    public DevInteractionGroup(
        ILocalizationService localizationService,
        ILogger<DevInteractionGroup> logger,
        ILoggerManager loggerManager,
        IMaintenanceManager maintenanceManager)
        : base(localizationService, logger)
    {
        _loggerManager = loggerManager;
        _maintenanceManager = maintenanceManager;
    }

    [SlashCommand("setloglevel", "Changes the current log level of the application.")]
    public Task SetLogLevelAsync(
        [Summary("switch", "The name of the level switch.")] string switchName,
        [Summary("level", "The new log level.")] LogLevelArg logLevel)
    {
        var newLogLevel = MapLogLevel(logLevel);
        _loggerManager.ChangeLogLevel(switchName, newLogLevel);
        Logger.LogInformation("The log level has been changed to '{NewLogLevel}'", newLogLevel);

        return RespondAsync(LocalizationService.Localize(
            "Modules.Dev.SetLogLevel.LogLevelChanged",
            ("LogLevel", newLogLevel)));
    }

    [SlashCommand("setmode", "Sets the operation mode of the application.")]
    public async Task SetModeAsync([Summary("mode", "The new operation mode.")] OperationModeArg operationMode)
    {
        await _maintenanceManager.SetOperationModeAsync(operationMode == OperationModeArg.Maintenance);
        Logger.LogInformation("Changed application mode to '{NewMode}'", operationMode);

        await RespondAsync(LocalizationService.Localize(
            operationMode == OperationModeArg.Maintenance
                ? "Modules.Dev.SetOperationMode.MaintenanceModeEnabled"
                : "Modules.Dev.SetOperationMode.NormalModeEnabled"));
    }

    private static LogLevel MapLogLevel(LogLevelArg logLevelArg)
        => logLevelArg switch
        {
            LogLevelArg.Trace => LogLevel.Trace,
            LogLevelArg.Debug => LogLevel.Debug,
            LogLevelArg.Warning => LogLevel.Warning,
            LogLevelArg.Error => LogLevel.Error,
            _ => LogLevel.Information
        };

    public enum LogLevelArg
    {
        Trace = 0,
        Debug,
        Information,
        Warning,
        Error
    }

    public enum OperationModeArg
    {
        Normal = 0,
        Maintenance
    }
}