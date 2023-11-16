using Discord;
using Discord.Interactions;
using Holo.Module.Reminders.Configuration;
using Holo.Module.Reminders.Models;
using Holo.Module.Reminders.Storage.Repositories;
using Holo.Sdk.Chrono;
using Holo.Sdk.Interactions;
using Holo.Sdk.Localization;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Sequences;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.Module.Reminders.Interactions;

[Group("reminder", "Commands related to reminders.")]
public partial class ReminderInteractionGroup : InteractionGroupBase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHiLoSequenceGenerator<ReminderId> _sequenceGenerator;
    private readonly IOptions<ReminderOptions> _options;
    private readonly IReminderRepository _reminderRepository;
    private readonly IUnitOfWorkProvider _unitOfWorkProvider;

    public ReminderInteractionGroup(
        IDateTimeProvider dateTimeProvider,
        IHiLoSequenceGenerator<ReminderId> sequenceGenerator,
        ILocalizationService localizationService,
        ILogger<ReminderInteractionGroup> logger,
        IOptions<ReminderOptions> options,
        IReminderRepository reminderRepository,
        IUnitOfWorkProvider unitOfWorkProvider)
        : base(localizationService, logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _sequenceGenerator = sequenceGenerator;
        _options = options;
        _reminderRepository = reminderRepository;
        _unitOfWorkProvider = unitOfWorkProvider;
    }

    private ButtonBuilder GetViewRemindersButton(ulong userId)
        => new(
                customId: ComponentHelper.CreateCustomId("viewreminders", userId, 0),
                label: LocalizationService.Localize("Modules.Reminders.ViewAllRemindersButton"),
                style: ButtonStyle.Secondary);

    private ButtonBuilder GetRemoveReminderButton(ulong userId, ulong reminderId)
        => new(
                customId: ComponentHelper.CreateCustomId("removereminder", userId, reminderId),
                label: LocalizationService.Localize(
                    "Modules.Reminders.ViewReminders.RemoveReminderButton",
                    ("ReminderId", reminderId)),
                emote: Emoji.Parse("🚮"),
                style: ButtonStyle.Secondary);
}