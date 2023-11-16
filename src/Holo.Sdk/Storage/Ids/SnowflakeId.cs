using System;
using System.Diagnostics;

namespace Holo.Sdk.Storage.Ids;

/// <summary>
/// A Discord specific identifier. This may represent entities such as users, servers, channels, etc.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly struct SnowflakeId : IIdentifier<ulong>, IEquatable<SnowflakeId>
{
    /// <inheritdoc cref="IIdentifier{T}.Value"/>
    public ulong Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SnowflakeId"/>.
    /// </summary>
    /// <param name="value">The value the identifier represents.</param>
    public SnowflakeId(ulong value)
    {
        Value = value;
    }

    public static implicit operator SnowflakeId(long id)
        => new((ulong)id);

    public static implicit operator long(SnowflakeId id)
    {
        unchecked
        {
            return (long)id.Value;
        }
    }

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (other is not SnowflakeId typedOther)
            return false;

        return Equals(typedOther);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(SnowflakeId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(SnowflakeId? first, SnowflakeId? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(SnowflakeId? first, SnowflakeId? second)
        => !(first == second);

    public override string ToString()
        => Value.ToString();
}