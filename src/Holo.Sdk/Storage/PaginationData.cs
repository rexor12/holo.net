using System.Collections.Generic;

namespace Holo.Sdk.Storage;

/// <summary>
/// Holds information about a pagination.
/// </summary>
/// <param name="PageIndex">The index of the current page, starting from 0.</param>
/// <param name="PageSize">The size of each page.</param>
/// <param name="TotalCount">The total number of items.</param>
/// <param name="Items">The items on the current page.</param>
/// <typeparam name="TIdentifier">The type of the identifiers of the items.</typeparam>
/// <typeparam name="TItem">The type of the items.</typeparam>
public record PaginationData<TIdentifier, TItem>(
    uint PageIndex,
    uint PageSize,
    ulong TotalCount,
    IReadOnlyCollection<TItem> Items)
    where TIdentifier : struct, IIdentifier
    where TItem : AggregateRoot<TIdentifier>;