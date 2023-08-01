using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Tasks;

public static class TaskHelper
{
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