using System;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Logging;

/// <summary>
/// A logger that outputs log entries to <see cref="Console"/>.
/// </summary>
/// <typeparam name="T">The type the logger belongs to.</typeparam>
public sealed class ConsoleLogger<T> : ILogger<T>
{
    /// <summary>
    /// The default instance of <see cref="ConsoleLogger{T}"/>.
    /// </summary>
    public static readonly ILogger<T> Instance = new ConsoleLogger<T>();

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
        Console.WriteLine("[{0}] {1}", logLevel, formatter(state, exception));
    }
}
