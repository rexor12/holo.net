using System.Collections.Generic;
using Holo.Sdk.Logging;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Holo.ServiceHost.Logging;

public sealed class LoggerManager : ILoggerManager
{
    private readonly IReadOnlyDictionary<string, LoggingLevelSwitch> _levelSwitches;

    public LoggerManager(IReadOnlyDictionary<string, LoggingLevelSwitch> levelSwitches)
    {
        _levelSwitches = levelSwitches;
    }

    public void ChangeLogLevel(string switchName, LogLevel logLevel)
    {
        if (!_levelSwitches.TryGetValue(switchName, out var levelSwitch))
            return;

        levelSwitch.MinimumLevel = logLevel switch
        {
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Trace => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };
    }
}