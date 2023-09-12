using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Logging;

/// <summary>
/// Interface for a service used to manage <see cref="ILogger"/> instances.
/// </summary>
public interface ILoggerManager
{
    /// <summary>
    /// Changes the log level of the specified <see cref="ILogger"/>s.
    /// </summary>
    /// <param name="switchName">The name of the switch to update.</param>
    /// <param name="logLevel">The new <see cref="LogLevel"/> to be used.</param>
    void ChangeLogLevel(string switchName, LogLevel logLevel);
}