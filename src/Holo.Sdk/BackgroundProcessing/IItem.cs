using System;
using Holo.Sdk.Storage;

namespace Holo.Sdk.BackgroundProcessing;

/// <summary>
/// Interface for a background processing item.
/// </summary>
/// <typeparam name="TIdentifier">The type of the item's identifier.</typeparam>
public interface IItem<TIdentifier>
    where TIdentifier : IIdentifier
{
    /// <summary>
    /// Gets the identifier of the item.
    /// </summary>
    TIdentifier Identifier { get; }

    /// <summary>
    /// Gets the date and time at which the item was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the correlation identifier of the item.
    /// </summary>
    /// <remarks>This identifier is used for processing items in the correct order.</remarks>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the type descriptor of the item.
    /// </summary>
    string ItemType { get; }

    /// <summary>
    /// Gets the serialized data of the item.
    /// </summary>
    string SerializedItemData { get; }
}