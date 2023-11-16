using Discord.Interactions;

namespace Holo.Module.Reminders.Enums;

public enum ReminderLocation
{
    [ChoiceDisplay("Direct Message")]
    DirectMessage = 0,

    [ChoiceDisplay("Current Channel")]
    Channel
}