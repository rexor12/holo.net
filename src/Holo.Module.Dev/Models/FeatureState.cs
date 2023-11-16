using System.ComponentModel.DataAnnotations.Schema;
using Holo.Sdk.Storage;

namespace Holo.Module.Dev.Models;

/// <summary>
/// Represents the state of a named feature.
/// </summary>
public sealed class FeatureState : AggregateRoot<FeatureStateId>
{
    public const string TableName = "feature_states";

    /// <summary>
    /// Gets or sets whether the feature is enabled.
    /// </summary>
    [Column("is_enabled")]
    public required bool IsEnabled { get; set; }
}