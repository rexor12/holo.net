using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Holo.Sdk.Interactions.Attributes;

/// <summary>
/// Used for interactions that impose a cooldown between consecutive invocations.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class CooldownAttribute : PreconditionAttribute
{
    private readonly ConcurrentDictionary<ulong, CooldownInfo> _cooldowns = new();

    /// <summary>
    /// Gets the duration of the cooldown.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Gets the number of uses allowed before the cooldown expires.
    /// </summary>
    public uint Uses { get; }

    /// <summary>
    /// Gets the guild permissions a user must have to avoid the cooldown.
    /// </summary>
    public GuildPermission? IgnoreGuildPermissions { get; init; }

    /// <summary>
    /// Gets the key of the localization resource returned during a precondition failure.
    /// </summary>
    public string LocalizationKey { get; init; } = "Interactions.CooldownError";

    /// <summary>
    /// Initializes a new instance of <see cref="CooldownAttribute"/>.
    /// </summary>
    /// <param name="duration">The duration of the cooldown, in seconds.</param>
    /// <param name="uses">The number of uses allowed before the cooldown expires.</param>
    public CooldownAttribute(uint duration, uint uses = 1)
    {
        if (uses < 1)
            throw new ArgumentOutOfRangeException(
                nameof(uses),
                uses,
                "Command usage must be at least 1.");

        Duration = TimeSpan.FromSeconds(duration);
        Uses = uses;
    }

    /// <inheritdoc cref="PreconditionAttribute.CheckRequirementsAsync(IInteractionContext, ICommandInfo, IServiceProvider)"/>
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        // TODO IDateTimeService
        if (IgnoreGuildPermissions.HasValue
            && context.User is IGuildUser guildUser
            && guildUser.GuildPermissions.Has(IgnoreGuildPermissions.Value))
            return Task.FromResult(PreconditionResult.FromSuccess());

        var cooldownInfo = _cooldowns.GetOrAdd(context.User.Id, userId => new CooldownInfo
        {
            UserId = context.User.Id,
            EndsAt = DateTime.UtcNow + Duration
        });

        lock (cooldownInfo.LockObject)
        {
            var timeLeft = cooldownInfo.EndsAt - DateTime.UtcNow;
            if (timeLeft <= TimeSpan.Zero)
            {
                cooldownInfo.Uses = 1;
                cooldownInfo.EndsAt = DateTime.UtcNow + Duration;
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            cooldownInfo.Uses++;
            if (cooldownInfo.Uses <= Uses)
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult<PreconditionResult>(
                new LocalizedPreconditionResult(
                    LocalizationKey,
                    ("SecondsLeft", timeLeft.TotalSeconds)));
        }
    }

    private sealed class CooldownInfo
    {
        public object LockObject { get; } = new object();
        public required ulong UserId { get; init; }
        public required DateTime EndsAt { get; set; }
        public int Uses { get; set; }
    }
}