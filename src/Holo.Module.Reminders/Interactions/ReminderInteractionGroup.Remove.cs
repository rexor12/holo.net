using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Holo.Module.Reminders.Models;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Storage.Ids;

namespace Holo.Module.Reminders.Interactions;

public partial class ReminderInteractionGroup
{
    [UserBound]
    [Cooldown(3, 1, LocalizationKey = "Modules.Reminders.RemoveReminder.CooldownError")]
    [ExtendedComponentInteraction("removereminder:*", true)]
    public async Task RemoveReminderAsync(uint reminderId)
    {
        var interaction = (SocketMessageComponent)Context.Interaction;
        var isDeleted = await _reminderRepository.RemoveByUserAsync(
            new SnowflakeId(Context.User.Id),
            new ReminderId(reminderId));
        var localizationKey = isDeleted
            ? "Modules.Reminders.RemoveReminder.DeletedMessage"
            : "Modules.Reminders.RemoveReminder.NotFoundError";

        await interaction.UpdateAsync(m =>
        {
            m.Content = LocalizationService.Localize(localizationKey);
            m.Embeds = null;
            m.Components = new ComponentBuilder()
                .WithButton(GetViewRemindersButton(Context.User.Id))
                .Build();
        });
    }
}