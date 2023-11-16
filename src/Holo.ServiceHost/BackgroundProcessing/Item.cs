using System;
using System.ComponentModel.DataAnnotations.Schema;
using Holo.Sdk.BackgroundProcessing;
using Holo.Sdk.Storage;

namespace Holo.ServiceHost.BackgroundProcessing;

/// <summary>
/// A background processing item.
/// </summary>
public sealed class Item : AggregateRoot<ItemId>, IItem<ItemId>
{
    public const string TableName = "background_processing_items";

    /// <inheritdoc cref="IItem{TIdentifier}.CreatedAt"/>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc cref="IItem{TIdentifier}.CorrelationId"/>
    [Column("correlation_id")]
    public required string CorrelationId { get; set; }

    /// <inheritdoc cref="IItem{TIdentifier}.ItemType"/>
    [Column("item_type")]
    public required string ItemType { get; set; }

    /// <inheritdoc cref="IItem{TIdentifier}.SerializedItemData"/>
    [Column("serialized_item_data")]
    public required string SerializedItemData { get; set; }
}