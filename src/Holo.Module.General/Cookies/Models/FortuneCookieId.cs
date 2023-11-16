using System;
using System.Diagnostics;
using Holo.Sdk.Storage;

namespace Holo.Module.General.Cookies.Models;

[DebuggerDisplay("{Value}")]
public readonly struct FortuneCookieId : IIdentifier<ulong>, IEquatable<FortuneCookieId>
{
    public ulong Value { get; }

    public FortuneCookieId(ulong value)
    {
        Value = value;
    }

    public static implicit operator FortuneCookieId(long id)
        => new((ulong)id);

    public static implicit operator long(FortuneCookieId id)
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
        if (other is not FortuneCookieId typedOther)
            return false;

        return Equals(typedOther);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(FortuneCookieId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(FortuneCookieId? first, FortuneCookieId? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(FortuneCookieId? first, FortuneCookieId? second)
        => !(first == second);

    public override string ToString()
        => Value.ToString();
}
