using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Holo.Sdk.Collections;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Creates a <see cref="Task{TResult}"/> that completes when all the specified tasks have completed.
    /// </summary>
    /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
    /// <typeparam name="T">The return type of the tasks.</typeparam>
    /// <returns>The <see cref="Task"/> that represents the completion of all the supplied tasks.</returns>
    public static Task<T[]> WhenAllAsync<T>(this IEnumerable<Task<T>> source)
        => Task.WhenAll(source);

    /// <summary>
    /// Awaits the specified <see cref="Task{TResult}"/> and creates a
    /// <see cref="Dictionary{TKey, TValue}"/> from the resulting items.
    /// </summary>
    /// <param name="source">The source <see cref="Task{TResult}"/>.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="valueSelector">A transform function to produce a result element value from each element.</param>
    /// <typeparam name="TItem">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the dictionary's key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary's value.</typeparam>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the selected key-value pairs.</returns>
    public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TItem, TKey, TValue>(
        this Task<TItem[]> source,
        Func<TItem, TKey> keySelector,
        Func<TItem, TValue> valueSelector)
        where TKey : notnull
    {
        var result = await source.ConfigureAwait(false);

        return result.ToDictionary(keySelector, valueSelector);
    }

    /// <summary>
    /// Projects each element of a sequence into the element paired with its index.
    /// </summary>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to enumerate.</param>
    /// <typeparam name="TSource">The type of the enumerable's elements.</typeparam>
    public static IEnumerable<(TSource Entry, int Index)> WithIndex<TSource>(this IEnumerable<TSource> source)
        => source.Select((item, index) => (item, index));
}