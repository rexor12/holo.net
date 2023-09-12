using System;
using System.Collections.Generic;
using Discord.Interactions;

namespace Holo.Sdk.Interactions.Attributes;

/// <summary>
/// Used for interaction groups that are only available in specific guilds.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GuildBoundAttribute : DontAutoRegisterAttribute
{
    /// <summary>
    /// Gets the identifier of the permission the availability of the annotated
    /// interaction group is determined by.
    /// </summary>
    public string PermissionId { get; }

    /// <summary>
    /// Gets the list of guild identifiers for which guilds teh annotated
    /// interaction group is always available, regardless of permissions.
    /// </summary>
    public IReadOnlyCollection<ulong> GuildIds { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="GuildBoundAttribute"/>.
    /// </summary>
    /// <param name="permissionId">
    /// The identifier of the permission the availability of the annotated
    /// interaction group is determined by.
    /// </param>
    /// <param name="additionalGuildIds">
    /// An optional list of guild identifiers for which guilds the annotated
    /// interaction group is always available, regardless of permissions.
    /// </param>
    public GuildBoundAttribute(string permissionId, params ulong[] additionalGuildIds)
    {
        PermissionId = permissionId;
        GuildIds = additionalGuildIds ?? Array.Empty<ulong>();
    }
}