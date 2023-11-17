using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions.Attributes;

namespace Holo.Module.General.ServerInfo;

public partial class ServerInteractionGroup
{
    [Cooldown(10)]
    [SlashCommand("banner", "Displays the server's banner.")]
    public async Task ViewServerBannerAsync()
    {
        if (Context.Guild == null)
        {
            await RespondAsync(LocalizationService.Localize("Interactions.ServerOnlyInteractionError"));
            return;
        }

        var (text, embed) = GetBannerResponseView(Context.User, Context.Guild);

        await RespondAsync(
            text: text,
            embeds: embed != null ? new[] { embed } : null,
            allowedMentions: AllowedMentions.None);
    }

    private (string? Text, Embed? Embed) GetBannerResponseView(IUser user, IGuild guild)
    {
        var bannerUrl = !string.IsNullOrWhiteSpace(guild.BannerId)
            ? CDN.GetGuildBannerUrl(guild.Id, guild.BannerId, ImageFormat.Auto, 2048)
            : null;
        if (string.IsNullOrWhiteSpace(bannerUrl))
            return (
                LocalizationService.Localize("Modules.General.Server.ViewServerBanner.NoBannerError"),
                null);

        return (
            null,
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize(
                    "Modules.General.Server.ViewServerBanner.EmbedTitle",
                    ("Name", guild.Name)))
                .WithImageUrl(bannerUrl)
                .WithColor((user as IGuildUser).GetAccent(Color.Blue))
                .Build());
    }
}