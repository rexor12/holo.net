using System;
using Holo.Sdk;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Holo.ServiceHost.Logging;

/// <summary>
/// A logger that outputs log entries to Serilog.
/// </summary>
/// <typeparam name="T">The type the logger belongs to.</typeparam>
public sealed class SerilogLogger<T> : ILogger<T>
{
    /// <summary>
    /// The default instance of <see cref="SerilogLogger{T}"/>.
    /// </summary>
    public static readonly ILogger<T> Instance = new SerilogLogger<T>();

    /// <inheritdoc cref="ILogger.BeginScope{TState}(TState)"/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => AnonymousDisposable.Instance;

    /// <inheritdoc cref="ILogger.IsEnabled(LogLevel)"/>
    public bool IsEnabled(LogLevel logLevel)
        => true;

    /// <inheritdoc cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var serilogLogLevel = logLevel switch
        {
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Verbose
        };

        Serilog.Log.Logger.Write(serilogLogLevel, exception, "{Message}", formatter(state, exception));
    }
}
