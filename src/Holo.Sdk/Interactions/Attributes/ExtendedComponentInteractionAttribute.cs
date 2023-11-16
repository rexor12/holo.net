using System;
using Discord.Interactions;

namespace Holo.Sdk.Interactions.Attributes;

/// <summary>
/// A <see cref="ComponentInteractionAttribute"/> that supports additional framework level features.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ExtendedComponentInteractionAttribute : ComponentInteractionAttribute
{
    public ExtendedComponentInteractionAttribute(string customId, bool ignoreGroupNames = false)
        : base(ExtendCustomId(customId), ignoreGroupNames, RunMode.Default)
    {
    }

    /// <summary>
    /// Extends the custom identifier with an additional regex capture group.
    /// </summary>
    /// <param name="customId">The custom identifier to extend.</param>
    /// <returns>The extended custom identifier.</returns>
    /// <seealso cref="ComponentHelper.TryParseCustomId(string, out ComponentInfo?)"/>
    private static string ExtendCustomId(string customId)
        => customId + ",*";
}