using System;
using System.Diagnostics;
using Holo.Sdk.Storage;

namespace Holo.Module.Dev.Models;

[DebuggerDisplay("{Value}")]
public readonly struct FeatureStateId : IIdentifier<string>, IEquatable<FeatureStateId>
{
    public string Value { get; }

    public FeatureStateId(string value)
    {
        Value = value;
    }

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (other is not FeatureStateId typedOther)
            return false;

        return Equals(typedOther);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(FeatureStateId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(FeatureStateId? first, FeatureStateId? second)
    {
        if (first is null)
        {
            if (second is null)
                return true;

            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(FeatureStateId? first, FeatureStateId? second)
        => !(first == second);

    public override string ToString()
        => Value.ToString();
}
