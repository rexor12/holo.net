using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General;

public sealed class SummonUserInteraction : InteractionGroupBase
{
    private const int MessageMaxLength = 512;

    public SummonUserInteraction(
        ILocalizationService localizationService,
        ILogger<SummonUserInteraction> logger)
        : base(localizationService, logger)
    {
    }

    [Cooldown(10, LocalizationKey = "Modules.General.SummonUser.CooldownError")]
    [SlashCommand("summon", "Requests a user's presence via a direct message.")]
    public async Task SummonUserAsync(
        [Summary(description: "The user to summon.")] IUser user,
        [Summary(description: "An optional message the bot forwards to the user.")] string? message = null,
        [Summary(description: "An optional mention of the channel where the user should be summoned.")] IChannel? channel = null)
    {
        var targetUser = await Context.GetRelevantUserAsync(user);
        if (targetUser == null)
        {
            await RespondAsync(LocalizationService.Localize("UserNotFoundError"), ephemeral: true);
            return;
        }

        if (targetUser.Id == Context.Client.CurrentUser.Id)
        {
            await RespondAsync(LocalizationService.Localize("Modules.General.SummonUser.CannotSummonSelfError"));
            return;
        }

        if (targetUser.IsBot)
        {
            await RespondAsync(LocalizationService.Localize("Modules.General.SummonUser.CannotSummonBotError"));
            return;
        }

        if (targetUser.Id == Context.User.Id)
        {
            await RespondAsync(
                LocalizationService.Localize(
                    "Modules.General.SummonUser.SelfSummon",
                    ("UserId", user.Id)),
                allowedMentions: AllowedMentions.All);
            return;
        }

        var targetChannel = channel ?? Context.Channel;
        var dmMessageKey = "Modules.General.SummonUser.DmSummon";
        if (message != null)
        {
            dmMessageKey = "Modules.General.SummonUser.DmSummonWithMessage";
            if (message.Length > MessageMaxLength)
                message = message[..MessageMaxLength];
        }

        try
        {
            await targetUser.SendMessageAsync(
                LocalizationService.Localize(
                    dmMessageKey,
                    ("UserName", Context.User.GlobalName),
                    ("ChannelId", targetChannel.Id),
                    ("Message", message)));

            await RespondAsync(
                LocalizationService.Localize(
                    "Modules.General.SummonUser.SuccessfulSummon",
                    ("UserId", user.Id)));
        }
        catch (HttpException e) when (e.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
        {
            await RespondAsync(
                LocalizationService.Localize(
                    "Modules.General.SummonUser.CannotDmError",
                    ("UserId", user.Id)));
        }
    }
}