using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
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

    public DevInteractionGroup(
        ILocalizationService localizationService,
        ILogger<DevInteractionGroup> logger,
        ILoggerManager loggerManager)
        : base(localizationService, logger)
    {
        _loggerManager = loggerManager;
    }

    [SlashCommand("setloglevel", "Changes the current log level of the application.")]
    public Task SetLogLevelAsync(
        [Summary("switch", "The name of the level switch.")] string switchName,
        [Summary("level", "The new log level.")] LogLevelArg logLevel)
    {
        var newLogLevel = MapLogLevel(logLevel);
        _loggerManager.ChangeLogLevel(switchName, newLogLevel);

        Logger.Log(newLogLevel, "The log level has been changed to '{NewLogLevel}'.", newLogLevel);

        return RespondAsync($"The log level has been changed to '{newLogLevel}'.");
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
}