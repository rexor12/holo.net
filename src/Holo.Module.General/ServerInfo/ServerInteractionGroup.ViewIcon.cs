using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions.Attributes;

namespace Holo.Module.General.ServerInfo;

public partial class ServerInteractionGroup
{
    [Cooldown(10)]
    [SlashCommand("icon", "Displays the server's icon.")]
    public async Task ViewServerIconAsync()
    {
        if (Context.Guild == null)
        {
            await RespondAsync(LocalizationService.Localize("Interactions.ServerOnlyInteractionError"));
            return;
        }

        var (text, embed) = GetIconResponseView(Context.User, Context.Guild);

        await RespondAsync(
            text: text,
            embeds: embed != null ? new[] { embed } : null,
            allowedMentions: AllowedMentions.None);
    }

    private (string? Text, Embed? Embed) GetIconResponseView(IUser user, IGuild guild)
    {
        var iconUrl = !string.IsNullOrWhiteSpace(guild.IconId)
            ? $"https://cdn.discordapp.com/icons/{guild.Id}/{guild.IconId}.jpg?size=2048"
            : null;
        if (string.IsNullOrWhiteSpace(iconUrl))
            return (
                LocalizationService.Localize("Modules.General.Server.ViewServerIcon.NoIconError"),
                null);

        return (
            null,
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize(
                    "Modules.General.Server.ViewServerIcon.EmbedTitle",
                    ("Name", guild.Name)))
                .WithImageUrl(iconUrl)
                .WithColor((user as IGuildUser).GetAccent(Color.Blue))
                .Build());
    }
}