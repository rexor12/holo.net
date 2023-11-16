using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Module.Reminders.Enums;
using Holo.Module.Reminders.Models;
using Holo.Sdk.Chrono;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Storage.Ids;

namespace Holo.Module.Reminders.Interactions;

public partial class ReminderInteractionGroup
{
    [Cooldown(3, 1, LocalizationKey = "Modules.Reminders.AddReminder.CooldownError")]
    [SlashCommand("single", "Sets a one-time reminder.")]
    public async Task AddSingleReminderAsync(
        [Summary(description: "When you'd like me to remind you, in the format of NdNhNm.")] string when,
        [Summary(description: "The message you'd like me to send you.")] string? message = null,
        [Summary(description: "Where you'd like to get the notification.")] ReminderLocation location = ReminderLocation.Channel)
    {
        if (!await ValidateMessageAsync(message).ConfigureAwait(false))
            return;

        if (!TimeSpanUtils.TryParse(when, out var dueTime))
        {
            await RespondAsync(LocalizationService.Localize("Modules.Reminders.AddReminder.InvalidTimeInputError"));
            return;
        }

        var maxDueTimeMinutes = _options.Value.MaxReminderDueTimeInMinutes;
        var maxDueTime = TimeSpan.FromMinutes(maxDueTimeMinutes);
        if (dueTime < TimeSpan.Zero || dueTime > maxDueTime)
        {
            await RespondAsync(LocalizationService.Localize(
                "Modules.Reminders.AddReminder.DueTimeOutOfRangeError",
                ("MaxDueTimeMinutes", maxDueTimeMinutes)));
            return;
        }

        SnowflakeId? guildId = null;
        SnowflakeId? channelId = null;
        var reminderLocation = ReminderLocation.DirectMessage;
        if (Context.Guild != null)
        {
            guildId = new SnowflakeId(Context.Guild.Id);
            channelId = new SnowflakeId(Context.Channel.Id);
            reminderLocation = location;
        }

        var userId = new SnowflakeId(Context.User.Id);
        Reminder reminder;
        await using (var unitOfWork = _unitOfWorkProvider.GetOrCreate())
        {
            if (!await ValidateReminderCountAsync(userId).ConfigureAwait(false))
                return;

            var now = _dateTimeProvider.Now();
            reminder = new Reminder
            {
                Identifier = await _sequenceGenerator.GetNextIdentifierAsync(),
                UserId = userId,
                CreatedAt = now,
                Message = message,
                BaseTrigger = now,
                LastTrigger = now,
                NextTrigger = now + dueTime.Value,
                Location = reminderLocation,
                ServerId = guildId,
                ChannelId = channelId
            };

            await _reminderRepository.AddAsync(reminder);

            unitOfWork.Complete();
        }

        await RespondAsync(
            LocalizationService.Localize(
                "Modules.Reminders.AddReminder.ReminderAdded",
                ("Timestamp", reminder.NextTrigger.ToUnixTimeSeconds())),
            components: new ComponentBuilder()
                .WithButton(GetViewRemindersButton(userId.Value))
                .WithButton(GetRemoveReminderButton(userId.Value, reminder.Identifier.Value))
                .Build());
    }
    [Cooldown(3, 1, LocalizationKey = "Modules.Reminders.AddReminder.CooldownError")]
    [SlashCommand("recurring", "Sets a recurring reminder.")]
    public async Task AddRecurringReminderAsync(
        [Summary(description: "The interval of the reminders, in the foramt of NdNhNm.")] string interval,
        [Summary(description: "The message you'd like me to send you.")] string? message = null,
        [Summary(description: "Where you'd like to get the notification.")] ReminderLocation location = ReminderLocation.Channel)
    {
        if (!await ValidateMessageAsync(message).ConfigureAwait(false))
            return;

        if (!TimeSpanUtils.TryParse(interval, out var intervalTime))
        {
            await RespondAsync(LocalizationService.Localize("Modules.Reminders.AddReminder.InvalidTimeInputError"));
            return;
        }

        var maxIntervalMinutes = _options.Value.MaxReminderIntervalInMinutes;
        var maxInterval = TimeSpan.FromMinutes(maxIntervalMinutes);
        if (intervalTime < TimeSpan.Zero || intervalTime > maxInterval)
        {
            await RespondAsync(LocalizationService.Localize(
                "Modules.Reminders.AddReminder.IntervalOutOfRangeError",
                ("MaxIntervalMinutes", maxIntervalMinutes)));
            return;
        }

        SnowflakeId? guildId = null;
        SnowflakeId? channelId = null;
        var reminderLocation = ReminderLocation.DirectMessage;
        if (Context.Guild != null)
        {
            guildId = new SnowflakeId(Context.Guild.Id);
            channelId = new SnowflakeId(Context.Channel.Id);
            reminderLocation = location;
        }

        var userId = new SnowflakeId(Context.User.Id);
        Reminder reminder;
        await using (var unitOfWork = _unitOfWorkProvider.GetOrCreate())
        {
            if (!await ValidateReminderCountAsync(userId).ConfigureAwait(false))
                return;

            var now = _dateTimeProvider.Now();
            var baseTrigger = now + intervalTime.Value;
            reminder = new Reminder
            {
                Identifier = await _sequenceGenerator.GetNextIdentifierAsync(),
                UserId = userId,
                CreatedAt = now,
                Message = message,
                IsRepeating = true,
                FrequencyTime = intervalTime,
                BaseTrigger = now,
                LastTrigger = now,
                NextTrigger = baseTrigger,
                Location = reminderLocation,
                ServerId = guildId,
                ChannelId = channelId
            };

            await _reminderRepository.AddAsync(reminder);

            unitOfWork.Complete();
        }

        await RespondAsync(
            LocalizationService.Localize(
                "Modules.Reminders.AddReminder.ReminderAdded",
                ("Timestamp", reminder.NextTrigger.ToUnixTimeSeconds())),
            components: new ComponentBuilder()
                .WithButton(GetViewRemindersButton(userId.Value))
                .WithButton(GetRemoveReminderButton(userId.Value, reminder.Identifier.Value))
                .Build());
    }

    private async Task<bool> ValidateMessageAsync(string? message)
    {
        if (message == null)
            return true;

        if (message.Length < _options.Value.MessageLengthMin || message.Length > _options.Value.MessageLengthMax)
        {
            await RespondAsync(LocalizationService.Localize(
                "Modules.Reminders.AddReminder.MessageLengthOutOfRangeError",
                ("MinLength", _options.Value.MessageLengthMin),
                ("MaxLength", _options.Value.MessageLengthMax)));

            return false;
        }

        return true;
    }

    private async Task<bool> ValidateReminderCountAsync(SnowflakeId userId)
    {
        var existingReminderCount = await _reminderRepository.GetCountByUserAsync(userId);
        if (existingReminderCount < _options.Value.RemindersPerUserMax)
            return true;

        await RespondAsync(
            LocalizationService.Localize("Modules.Reminders.AddReminder.TooManyRemindersError"),
            components: new ComponentBuilder()
                .WithButton(GetViewRemindersButton(userId.Value))
                .Build());

        return false;
    }
}