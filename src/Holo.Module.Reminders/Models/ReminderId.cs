using System;
using System.Diagnostics;
using Holo.Sdk.Storage;

namespace Holo.Module.Reminders.Models;

[DebuggerDisplay("{Value}")]
public readonly struct ReminderId : IIdentifier<ulong>, IEquatable<ReminderId>
{
    public ulong Value { get; }

    public ReminderId(ulong value)
    {
        Value = value;
    }

    public static implicit operator ReminderId(long id)
        => new((ulong)id);

    public static implicit operator long(ReminderId id)
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
        if (other is not ReminderId typedOther)
            return false;

        return Equals(typedOther);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(ReminderId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(ReminderId? first, ReminderId? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(ReminderId? first, ReminderId? second)
        => !(first == second);

    public override string ToString()
        => Value.ToString();
}