using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General.UserInfo.Interactions;

public sealed class ViewUserBannerInteraction : InteractionGroupBase
{
    public ViewUserBannerInteraction(
        ILocalizationService localizationService,
        ILogger<ViewUserBannerInteraction> logger)
        : base(localizationService, logger)
    {
    }

    [Cooldown(10)]
    [SlashCommand("banner", "Displays a user's banner.")]
    public async Task ViewUserBannerAsync(
        [Summary(description: "The user to view. By default, it's yourself.")] IUser? user = null)
    {
        user = await Context.GetRelevantUserAsync(user, Context.User);
        var (text, embed) = await GetResponseView(Context.User.Id, user);

        await RespondAsync(
            text: text,
            embeds: embed != null ? new[] { embed } : null,
            allowedMentions: AllowedMentions.None);
    }

    private async Task<(string? Text, Embed? Embed)> GetResponseView(
        ulong boundUserId,
        IUser user)
    {
        var restUser = await Context.Client.Rest.GetUserAsync(user.Id);
        var bannerUrl = !string.IsNullOrWhiteSpace(restUser?.BannerId)
            ? restUser.GetBannerUrl(ImageFormat.Auto, 2048)
            : null;
        if (string.IsNullOrWhiteSpace(bannerUrl))
            return (
                LocalizationService.Localize(
                    "Modules.General.ViewUserBanner.NoBannerError",
                    ("UserId", user.Id)),
                null);

        var (userName, color) = user is IGuildUser guildUser
            ? (guildUser.DisplayName, guildUser.GetAccent(Color.Blue))
            : (user.GlobalName, Color.Blue);

        return (
            null,
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize(
                    "Modules.General.ViewUserBanner.EmbedTitle",
                    ("User", userName)))
                .WithImageUrl(bannerUrl)
                .WithColor(color)
                .Build());
    }
}