using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Tasks;

/// <summary>
/// Helper methods for <see cref="Task"/>.
/// </summary>
public static class TaskHelper
{
    /// <summary>
    /// Awaits the <see cref="Task"/> produced by the given <see cref="Func{TResult}"/>
    /// within a try-block, ignoring the specified exceptions; or, all, if none are specified.
    /// </summary>
    /// <param name="funcAsync">The factory method for the <see cref="Task"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used to log exceptions.</param>
    /// <param name="silent">Determines whether the caught exceptions should be logged.</param>
    /// <param name="ignoredExceptions">The list of exceptions to be caught and ignored.</param>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public static async Task TryAwaitAsync(
        this Func<Task> funcAsync,
        ILogger logger,
        bool silent = false,
        params Type[]? ignoredExceptions)
    {
        try
        {
            await funcAsync();
        }
        catch (Exception e) when (ignoredExceptions == null || ignoredExceptions.Contains(e.GetType()))
        {
            if (!silent)
                logger.LogError(e, "Asynchronous method failed with an error");
        }
    }

    /// <summary>
    /// Awaits the <see cref="ValueTask"/> produced by the given <see cref="Func{TResult}"/>
    /// within a try-block, ignoring the specified exceptions; or, all, if none are specified.
    /// </summary>
    /// <param name="funcAsync">The factory method for the <see cref="ValueTask"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used to log exceptions.</param>
    /// <param name="silent">Determines whether the caught exceptions should be logged.</param>
    /// <param name="ignoredExceptions">The list of exceptions to be caught and ignored.</param>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public static async Task TryAwaitAsync(
        this Func<ValueTask> funcAsync,
        ILogger logger,
        bool silent = false,
        params Type[]? ignoredExceptions)
    {
        try
        {
            await funcAsync();
        }
        catch (Exception e) when (ignoredExceptions == null || ignoredExceptions.Contains(e.GetType()))
        {
            if (!silent)
                logger.LogError(e, "Asynchronous method failed with an error");
        }
    }

    /// <summary>
    /// Awaits the <see cref="Task"/> produced by the given <see cref="Func{T, TResult}"/>
    /// within a try-block, ignoring the specified exceptions; or, all, if none are specified.
    /// </summary>
    /// <param name="funcAsync">The factory method for the <see cref="Task"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used to log exceptions.</param>
    /// <param name="state">An object passed to the factory method.</param>
    /// <param name="silent">Determines whether the caught exceptions should be logged.</param>
    /// <param name="ignoredExceptions">The list of exceptions to be caught and ignored.</param>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public static async Task TryAwaitAsync<T>(
        this Func<T, Task> funcAsync,
        ILogger logger,
        T state,
        bool silent = false,
        params Type[]? ignoredExceptions)
    {
        try
        {
            await funcAsync(state);
        }
        catch (Exception e) when (ignoredExceptions == null || ignoredExceptions.Contains(e.GetType()))
        {
            if (!silent)
                logger.LogError(e, "Asynchronous method failed with an error");
        }
    }

    /// <summary>
    /// Awaits the <see cref="Task"/> produced by the given <see cref="Func{T, TResult}"/>
    /// within a try-block, ignoring the specified exceptions; or, all, if none are specified.
    /// </summary>
    /// <param name="funcAsync">The factory method for the <see cref="ValueTask"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> used to log exceptions.</param>
    /// <param name="state">An object passed to the factory method.</param>
    /// <param name="silent">Determines whether the caught exceptions should be logged.</param>
    /// <param name="ignoredExceptions">The list of exceptions to be caught and ignored.</param>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public static async Task TryAwaitAsync<T>(
        this Func<T, ValueTask> funcAsync,
        ILogger logger,
        T state,
        bool silent = false,
        params Type[]? ignoredExceptions)
    {
        try
        {
            await funcAsync(state);
        }
        catch (Exception e) when (ignoredExceptions == null || ignoredExceptions.Contains(e.GetType()))
        {
            if (!silent)
                logger.LogError(e, "Asynchronous method failed with an error");
        }
    }
}