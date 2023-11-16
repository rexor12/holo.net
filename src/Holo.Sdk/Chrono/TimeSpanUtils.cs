using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Holo.Sdk.Chrono;

/// <summary>
/// Utilities for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanUtils
{
    private static readonly Regex TimeSpanRegex = new(
        @"^(?:(?<days>\d+)\s*d)?\s*(?:(?<hours>\d+)\s*h)?\s*(?:(?<minutes>\d+)\s*m)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Attempts to parse the day, hour and minute components from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="timeSpan">If successful, the resulting <see cref="TimeSpan"/>.</param>
    /// <returns><c>true</c>, if the operation was successful.</returns>
    public static bool TryParse(string? value, [NotNullWhen(true)] out TimeSpan? timeSpan)
    {
        timeSpan = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var match = TimeSpanRegex.Match(value);
        if (!match.Success)
            return false;

        timeSpan = new TimeSpan(
            int.TryParse(match.Groups["days"].Value, out var days) ? days : 0,
            int.TryParse(match.Groups["hours"].Value, out var hours) ? hours : 0,
            int.TryParse(match.Groups["minutes"].Value, out var minutes) ? minutes : 0,
            0);

        return true;
    }
}