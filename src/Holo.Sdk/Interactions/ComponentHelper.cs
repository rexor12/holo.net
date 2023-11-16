using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Holo.Sdk.Collections;

namespace Holo.Sdk.Interactions;

/// <summary>
/// Provides helper methods for components.
/// </summary>
public static class ComponentHelper
{
    private static readonly Regex CustomIdRegex = new(@"^(?<Id>[^:]+):(?<CustomData>.*?),?(?<BoundUserId>\d+)$", RegexOptions.Compiled);

    /// <summary>
    /// Creates a custom identifier from the specified information.
    /// </summary>
    /// <param name="componentId">The identifier of the component.</param>
    /// <param name="boundUserId">The identifier of the user the component is bound to.</param>
    /// <param name="customData">The optional arguments for the interaction handler's parameters.</param>
    /// <returns>The constructed custom identifier.</returns>
    /// <seealso cref="ComponentInfo.UnboundUserId"/>
    public static string CreateCustomId(
        string componentId,
        ulong boundUserId,
        params object[]? customData)
    {
        customData ??= Array.Empty<object>();
        var stringBuilder = new StringBuilder(componentId.Length + 2 * sizeof(ulong) + 5 * customData.Length);
        stringBuilder.Append(componentId);
        stringBuilder.Append(':');

        if (customData.Length > 0)
        {
            foreach (var (customDataItem, index) in customData.WithIndex())
            {
                stringBuilder.Append(customDataItem);
                if (index + 1 < customData.Length)
                    stringBuilder.Append(',');
            }

            stringBuilder.Append(',');
        }

        stringBuilder.Append(boundUserId);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Tries to parse the specified custom identifier.
    /// </summary>
    /// <param name="customId">The custom identifier to parse.</param>
    /// <param name="componentInfo">The resulting <see cref="ComponentInfo"/>, if parsing is successful.</param>
    /// <returns><c>true</c>, if parsing was successful.</returns>
    public static bool TryParseCustomId(string customId, [NotNullWhen(true)] out ComponentInfo? componentInfo)
    {
        var match = CustomIdRegex.Match(customId);
        if (!match.Success || !ulong.TryParse(match.Groups["BoundUserId"].Value, out var boundUserId))
        {
            componentInfo = null;
            return false;
        }

        componentInfo = new ComponentInfo(
            match.Groups["Id"].Value,
            boundUserId,
            match.Groups["CustomData"].Value);

        return true;
    }
}