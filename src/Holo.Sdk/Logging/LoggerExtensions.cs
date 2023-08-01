using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Logging;

/// <summary>
/// Extension methods for <see cref="ILogger"/>.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs the given Discord log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to be used.</param>
    /// <param name="message">The <see cref="LogMessage"/> to be logged.</param>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public static Task LogAsync(this ILogger logger, LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                logger.LogCritical(message.Exception, message.Message);
                break;
            case LogSeverity.Error:
                logger.LogError(message.Exception, message.Message);
                break;
            case LogSeverity.Warning:
                logger.LogWarning(message.Exception, message.Message);
                break;
            case LogSeverity.Info:
                logger.LogInformation(message.Exception, message.Message);
                break;
            case LogSeverity.Debug:
                logger.LogDebug(message.Exception, message.Message);
                break;
            default:
                logger.LogTrace(message.Exception, message.Message);
                break;
        }

        return Task.CompletedTask;
    }
}