using System;
using System.Diagnostics;
using Holo.Sdk.Storage;

namespace Holo.ServiceHost.BackgroundProcessing;

[DebuggerDisplay("{Value}")]
public readonly struct ItemId : IIdentifier<Guid>, IEquatable<ItemId>
{
    public Guid Value { get; }

    public ItemId(Guid value)
    {
        Value = value;
    }

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (other is not ItemId typedOther)
            return false;

        return Equals(typedOther);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(ItemId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(ItemId? first, ItemId? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(ItemId? first, ItemId? second)
        => !(first == second);

    public override string ToString()
        => Value.ToString();
}
