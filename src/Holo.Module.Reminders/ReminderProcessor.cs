using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Holo.Module.Reminders.Configuration;
using Holo.Module.Reminders.Models;
using Holo.Module.Reminders.Storage.Repositories;
using Holo.Sdk.BackgroundProcessing;
using Holo.Sdk.BackgroundProcessing.Processors;
using Holo.Sdk.Chrono;
using Holo.Sdk.DI;
using Holo.Sdk.Lifecycle;
using Holo.Sdk.Localization;
using Holo.Sdk.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.Module.Reminders;

[Service(typeof(IBackgroundItemProcessor), typeof(IStartable))]
public sealed class ReminderProcessor :
    BackgroundItemProcessorBase<ReminderProcessor.ReminderProcessingItem>,
    IStartable
{
    private const int BatchSize = 50;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IItemManager _itemManager;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<ReminderProcessor> _logger;
    private readonly IOptions<ReminderOptions> _options;
    private readonly IReminderRepository _reminderRepository;
    private readonly IUnitOfWorkProvider _unitOfWorkProvider;

    public int Priority { get; } = 1000;

    public ReminderProcessor(
        IDateTimeProvider dateTimeProvider,
        DiscordSocketClient discordSocketClient,
        IItemManager itemManager,
        ILocalizationService localizationService,
        ILogger<ReminderProcessor> logger,
        IOptions<ReminderOptions> options,
        IReminderRepository reminderRepository,
        IUnitOfWorkProvider unitOfWorkProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _discordSocketClient = discordSocketClient;
        _itemManager = itemManager;
        _localizationService = localizationService;
        _logger = logger;
        _options = options;
        _reminderRepository = reminderRepository;
        _unitOfWorkProvider = unitOfWorkProvider;
    }

    public override async Task<ProcessingResult> ProcessAsync(
        ReminderProcessingItem item,
        CancellationToken cancellationToken)
    {
        if (!_options.Value.IsRemindersEnabled)
            return ProcessingResult.Success;

        var processedReminderCount = 0;
        bool hasProcessedReminders;
        do
        {
            var now = _dateTimeProvider.Now();
            var reminders = await _reminderRepository.GetTriggerableRemindersAsync(now, BatchSize);
            if (reminders.Count == 0)
                break;

            hasProcessedReminders = true;
            foreach (var reminder in reminders)
            {
                try
                {
                    await TryProcessReminderAsync(reminder).ConfigureAwait(false);
                    processedReminderCount++;
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Unexpected failure while processing reminder '{ReminderId}'",
                        reminder.Identifier.Value);
                }
            }
        } while (hasProcessedReminders);

        return ProcessingResult.RetryLater;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var unitOfWork = _unitOfWorkProvider.GetOrCreate();
        if (await _itemManager.HasItemAsync(ItemType).ConfigureAwait(false))
            return;

        await _itemManager.EnqueueAsync(new ReminderProcessingItem(), ItemType, "1").ConfigureAwait(false);
        unitOfWork.Complete();
    }

    public Task StopAsync()
        => Task.CompletedTask;

    private static string GetMessageLocalizationKey(bool hasMessage, bool isBelated)
        => (hasMessage, isBelated) switch
        {
            (true, true) => "Modules.Reminders.ReminderProcessor.ReminderBelated",
            (true, false) => "Modules.Reminders.ReminderProcessor.Reminder",
            (false, true) => "Modules.Reminders.ReminderProcessor.ReminderEmptyBelated",
            _ => "Modules.Reminders.ReminderProcessor.ReminderEmpty"
        };

    private async Task TryProcessReminderAsync(Reminder reminder)
    {
        try
        {
            await ProcessReminderAsync(reminder).ConfigureAwait(false);
        }
        catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
        {
            _logger.LogDebug(
                e,
                "Failed to send reminder notification for reminder '{ReminderId}' to user '{UserId}', will not retry",
                reminder.Identifier.Value,
                reminder.UserId.Value);
            await _reminderRepository.RemoveAsync(reminder.Identifier).ConfigureAwait(false);
        }
    }

    private async Task ProcessReminderAsync(Reminder reminder)
    {
        var isNotificationSuccessful = await TrySendNotificationAsync(reminder).ConfigureAwait(false);
        if (!isNotificationSuccessful || !reminder.IsRepeating)
        {
            await _reminderRepository.RemoveAsync(reminder.Identifier).ConfigureAwait(false);
            return;
        }

        var now = _dateTimeProvider.Now();
        reminder.LastTrigger = now;
        reminder.UpdateNextTrigger(now);
        await _reminderRepository.UpdateAsync(reminder).ConfigureAwait(false);
        _logger.LogTrace(
            "Processed reminder '{ReminderId}', next trigger at '{NextTrigger}'",
            reminder.Identifier.Value,
            reminder.NextTrigger.ToUnixTimeSeconds());
    }

    private async Task<bool> TrySendNotificationAsync(Reminder reminder)
    {
        var reminderDelay = _dateTimeProvider.Now() - reminder.NextTrigger;
        var isBelated = reminderDelay.TotalSeconds > _options.Value.BelatedReminderAfterSeconds;
        var localizedMessage = _localizationService.Localize(
            GetMessageLocalizationKey(reminder.Message != null, isBelated),
            ("UserId", reminder.UserId.Value),
            ("Message", reminder.Message),
            ("TriggerTime", reminder.NextTrigger.ToUnixTimeSeconds()));
        if (reminder.Location == Enums.ReminderLocation.DirectMessage)
        {
            var result = await TrySendDirectMessage(reminder, localizedMessage).ConfigureAwait(false);
            if (result.HasValue)
                return result.Value;
        }

        return await TrySendChannelMessage(reminder, localizedMessage).ConfigureAwait(false);
    }

    private async Task<bool?> TrySendDirectMessage(Reminder reminder, string localizedMessage)
    {
        try
        {
            var user = await _discordSocketClient.GetUserAsync(reminder.UserId.Value).ConfigureAwait(false);
            if (user == null)
                return false;

            var dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
            await dmChannel.SendMessageAsync(localizedMessage).ConfigureAwait(false);

            return true;
        }
        catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
        {
            // Try to send a message in the source channel.
            if (reminder.ServerId == null || reminder.ChannelId == null)
                throw;

            return null;
        }
    }

    private async Task<bool> TrySendChannelMessage(Reminder reminder, string localizedMessage)
    {
        if (reminder.ServerId == null || reminder.ChannelId == null)
            return false;

        var guild = _discordSocketClient.GetGuild(reminder.ServerId.Value.Value);
        if (guild.GetUser(reminder.UserId.Value) == null)
            return false;

        var channel = guild.GetChannel(reminder.ChannelId.Value.Value);
        if (channel is not ITextChannel textChannel)
            return false;

        await textChannel.SendMessageAsync(localizedMessage).ConfigureAwait(false);

        return true;
    }

    public sealed record ReminderProcessingItem();
}