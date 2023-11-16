using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Holo.Sdk.Collections;
using Holo.Sdk.Discord;
using Holo.Sdk.Discord.Components;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Storage.Ids;

namespace Holo.Module.Reminders.Interactions;

public partial class ReminderInteractionGroup
{
    // NOTE: If this is changed to anything >5, the way the remove action row is constructed HAS TO be revised,
    // because no more than 5 buttons can be in a single action row.
    private const int RemindersPerPage = 3;

    [Cooldown(10, 1, LocalizationKey = "Modules.Reminders.ViewReminders.CooldownError")]
    [SlashCommand("view", "Displays your reminders.")]
    public async Task ViewRemindersAsync()
    {
        await DeferAsync();

        var (content, embed, component) = await GetPageAsync(Context.User.Id, 0, RemindersPerPage);

        await ModifyOriginalResponseAsync(m =>
        {
            m.Content = content;
            m.Embeds = embed != null ? new[] { embed } : null;
            m.Components = component;
        });
    }

    [UserBound]
    [Cooldown(1, 1, LocalizationKey = "Modules.Reminders.ViewReminders.CooldownError")]
    [ExtendedComponentInteraction("viewreminders:*", true)]
    public async Task ChangeRemindersPageAsync(uint pageIndex)
    {
        await DeferAsync();

        var interaction = (SocketMessageComponent)Context.Interaction;
        var (content, embed, component) = await GetPageAsync(Context.User.Id, pageIndex, RemindersPerPage);

        await interaction.ModifyOriginalResponseAsync(m =>
        {
            m.Content = content;
            m.Embeds = embed != null ? new[] { embed } : null;
            m.Components = component;
        });
    }

    private async Task<(string? Content, Embed? Embed, MessageComponent? Component)> GetPageAsync(
        ulong userId,
        uint pageIndex,
        uint pageSize)
    {
        var userIdSnowflake = new SnowflakeId(userId);
        var reminders = await _reminderRepository.PaginateAsync(userIdSnowflake, pageIndex, pageSize);
        if (reminders.Items.Count == 0 && pageIndex == 0)
            reminders = await _reminderRepository.PaginateAsync(userIdSnowflake, 0, pageSize);

        if (reminders.Items.Count == 0)
            return (
                LocalizationService.Localize("Modules.Reminders.ViewReminders.NoReminders"),
                null,
                null);

        var embedFields = new List<EmbedFieldBuilder>();
        var removeButtons = new ActionRowBuilder();
        foreach (var reminder in reminders.Items)
        {
            embedFields.Add(new EmbedFieldBuilder()
                .WithName(LocalizationService.Localize(
                    reminder.IsRepeating
                        ? "Modules.Reminders.ViewReminders.EmbedFieldNameRepeating"
                        : "Modules.Reminders.ViewReminders.EmbedFieldName",
                    ("ReminderId", reminder.Identifier.Value)))
                .WithValue(LocalizationService.Localize(
                    "Modules.Reminders.ViewReminders.EmbedFieldValue",
                    ("Message", reminder.Message ?? string.Empty),
                    ("Timestamp", reminder.NextTrigger.ToUnixTimeSeconds())))
                .WithIsInline(false));

            removeButtons.WithButton(GetRemoveReminderButton(userId, reminder.Identifier.Value));
        }

        var paginator = new Paginator
        {
            CurrentPage = reminders.PageIndex,
            PageSize = reminders.PageSize,
            TotalCount = reminders.TotalCount
        };
        ActionRowBuilder? paginatorBuilder = null;
        if (!paginator.IsOnlyPage)
        {
            paginatorBuilder = paginator
                .ToComponentBuilder(
                    "viewreminders",
                    previousButtonLabel: "<<",
                    nextButtonLabel: ">>",
                    boundUserId: userId)
                .ActionRows
                .Single();
        }

        return (
            null,
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize("Modules.Reminders.ViewReminders.EmbedTitle"))
                .WithDescription(LocalizationService
                    .Localize("Modules.Reminders.ViewReminders.EmbedDescription", ("UserId", userId)))
                .WithColor((Context.User as IGuildUser).GetAccent(Color.Blue))
                .WithThumbnailUrl(_options.Value.EmbedThumbnailUrl)
                .WithFields(embedFields)
                .WithFooter(new EmbedFooterBuilder()
                    .WithText(LocalizationService.Localize("Modules.Reminders.ViewReminders.EmbedFooter")))
                .Build(),
            new ComponentBuilder()
                .WithRows(Enumerable
                    .Empty<ActionRowBuilder>()
                    .Append(removeButtons)
                    .AppendNotNull(paginatorBuilder))
                .Build());
    }
}