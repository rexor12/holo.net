using System.Linq;
using Discord;

namespace Holo.Sdk.Discord;

/// <summary>
/// Extension methods for <see cref="IGuildUser"/>.
/// </summary>
public static class GuildUserExtensions
{
    /// <summary>
    /// Gets the accent color of the user based on their roles.
    /// </summary>
    /// <param name="user">The user for whom to get the accent color.</param>
    /// <param name="fallbackColor">
    /// The color to return if the user doesn't have a non-default one.
    /// This defaults to <see cref="Color.Default"/>.
    /// </param>
    /// <returns>
    /// If any, the color of the highest position guild role with a non-default color;
    /// otherwise, the specified fallback color.
    /// </returns>
    public static Color GetAccent(this IGuildUser user, Color? fallbackColor = null)
        => user.Guild.Roles
            .Where(role => role.Color != Color.Default && user.RoleIds.Contains(role.Id))
            .OrderByDescending(role => role.Position)
            .Select(role => role.Color)
            .FirstOrDefault(Color.Default);
}