using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Holo.Sdk.Interactions.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CooldownAttribute : PreconditionAttribute
{
    private readonly ConcurrentDictionary<ulong, CooldownInfo> Cooldowns = new();

    public TimeSpan Duration { get; }

    public int Uses { get; }

    public GuildPermission? IgnoreGuildPermissions { get; init; }

    public string LocalizationKey { get; init; } = "Interactions.CooldownError";

    public CooldownAttribute(uint duration, int uses = 1)
    {
        if (uses < 1)
            throw new ArgumentOutOfRangeException(
                nameof(uses),
                uses,
                "Command usage must be at least 1.");

        Duration = TimeSpan.FromSeconds(duration);
        Uses = uses;
    }

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

        var cooldownInfo = Cooldowns.GetOrAdd(context.User.Id, userId => new CooldownInfo
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