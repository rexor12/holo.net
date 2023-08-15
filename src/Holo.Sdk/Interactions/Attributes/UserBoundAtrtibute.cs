using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Holo.Sdk.Interactions.Attributes;

/// <summary>
/// Used for component interactions where the interactability should be bound to a user.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class UserBoundAttribute : PreconditionAttribute
{
    /// <summary>
    /// Gets the key of the localization resource returned during a precondition failure.
    /// </summary>
    public string LocalizationKey { get; init; } = "Interactions.BoundInteractionUserError";

    /// <summary>
    /// Initializes a new instance of <see cref="UserBoundAttribute"/>.
    /// </summary>
    public UserBoundAttribute()
    {
    }

    /// <inheritdoc cref="PreconditionAttribute.CheckRequirementsAsync(IInteractionContext, ICommandInfo, IServiceProvider)"/>
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        if (context is not ExtendedInteractionContext extendedContext
            || extendedContext.ComponentInfo == null
            || extendedContext.ComponentInfo.BoundUserId == 0
            || extendedContext.ComponentInfo.BoundUserId == extendedContext.User.Id)
            return Task.FromResult(PreconditionResult.FromSuccess());

        return Task.FromResult<PreconditionResult>(new LocalizedPreconditionResult(LocalizationKey));
    }
}