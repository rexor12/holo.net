using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holo.Sdk.Storage;

/// <summary>
/// Abstract base class for aggregate root entities.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier of the entity.</typeparam>
public abstract class AggregateRoot<TIdentifier> : IEquatable<AggregateRoot<TIdentifier>>
    where TIdentifier : struct
{
    [Column("id")]
    public TIdentifier Identifier { get; set; }

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (other.GetType() != GetType())
            return false;

        return Equals((AggregateRoot<TIdentifier>)other);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(AggregateRoot<TIdentifier>? other)
    {
        if (other is null)
            return false;

        return Identifier.Equals(other.Identifier);
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Identifier.GetHashCode();

    public static bool operator ==(AggregateRoot<TIdentifier>? first, AggregateRoot<TIdentifier>? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(AggregateRoot<TIdentifier>? first, AggregateRoot<TIdentifier>? second)
        => !(first == second);
}