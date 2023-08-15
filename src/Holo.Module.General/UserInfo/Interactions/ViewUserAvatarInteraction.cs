using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Holo.Module.General.UserInfo.Configuration;
using Holo.Sdk;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.Module.General.UserInfo.Interactions;

public sealed class ViewUserAvatarInteraction : InteractionGroupBase
{
    private static readonly IReadOnlyCollection<AvatarKind> AvatarKindPriority = new[]
    {
        AvatarKind.ServerSpecific,
        AvatarKind.Global
    };

    private readonly IOptions<AvatarOptions> _options;

    public ViewUserAvatarInteraction(
        ILocalizationService localizationService,
        ILogger<ViewUserAvatarInteraction> logger,
        IOptions<AvatarOptions> options)
        : base(localizationService, logger)
    {
        _options = options;
    }

    [Cooldown(10)]
    [SlashCommand("avatar", "Displays a user's avatar.")]
    public async Task ViewUserAvatarAsync(
        [Summary(description: "The user to view. By default, it's yourself.")] IUser? user = null,
        [Summary("type", "The type of the avatar to view. By default, it's the server specific avatar.")] AvatarKind avatarKind = AvatarKind.ServerSpecific)
    {
        user = await Context.GetRelevantUserAsync(user, Context.User);
        var (text, embed, component) = GetResponseView(Context.User.Id, user, avatarKind);

        await RespondAsync(
            text: text,
            embeds: embed != null ? new[] { embed } : null,
            components: component,
            allowedMentions: AllowedMentions.None);
    }

    [Cooldown(3)]
    [ExtendedComponentInteraction("viewavatar:*,*")]
    public async Task ChangeUserAvatarAsync(AvatarKind avatarKind, ulong userId)
    {
        var interaction = (SocketMessageComponent)Context.Interaction;
        var user = await Context.GetRelevantUserAsync(userId);
        if (user == null)
        {
            await interaction.UpdateAsync(m =>
            {
                m.Content = LocalizationService.Localize("UserNotFoundError");
                m.Embeds = null;
                m.Components = null;
            });
            return;
        }

        var (text, embed, component) = GetResponseView(
            Context.ComponentInfo.AssertNotNull().BoundUserId,
            user,
            avatarKind);

        await interaction.UpdateAsync(m =>
        {
            m.Content = text;
            m.Embeds = new[] { embed };
            m.Components = component;
        });
    }

    private static AvatarInfo GetAvatarInfo(IUser user, AvatarKind requestedAvatarKind)
    {
        var avatarUrls = new Dictionary<AvatarKind, string?>
        {
            [AvatarKind.Global] = user.GetAvatarUrl(ImageFormat.Auto, 2048)
        };
        if (user is IGuildUser guildUser)
            avatarUrls[AvatarKind.ServerSpecific] = guildUser.GetGuildAvatarUrl(ImageFormat.Auto, 2048);

        var (effectiveKind, effectiveUrl) = GetEffectiveAvatar(avatarUrls, requestedAvatarKind);

        return new AvatarInfo
        {
            AvatarUrls = avatarUrls,
            EffectiveAvatarKind = effectiveKind,
            EffectiveAvatarUrl = effectiveUrl
        };
    }

    private static (AvatarKind? Kind, string? Url) GetEffectiveAvatar(
        IReadOnlyDictionary<AvatarKind, string?> avatarUrls,
        AvatarKind requestedAvatarKind)
    {
        if (avatarUrls.TryGetValue(requestedAvatarKind, out var url) && !string.IsNullOrWhiteSpace(url))
            return (requestedAvatarKind, url);

        foreach (var avatarKind in AvatarKindPriority)
            if (avatarUrls.TryGetValue(avatarKind, out url) && !string.IsNullOrWhiteSpace(url))
                return (avatarKind, url);

        return (null, null);
    }

    private (string? Text, Embed? Embed, MessageComponent? Component) GetResponseView(
        ulong boundUserId,
        IUser user,
        AvatarKind avatarKind)
    {
        var avatarInfo = GetAvatarInfo(user, avatarKind);
        if (avatarInfo.EffectiveAvatarKind == null || avatarInfo.EffectiveAvatarUrl == null)
            return (
                LocalizationService.Localize(
                    "Modules.General.ViewUserAvatar.NoAvatarError",
                    ("UserId", user.Id)),
                null,
                null);

        EmbedFooterBuilder? embedFooter = null;
        if (user.Id == Context.Client.CurrentUser.Id)
            embedFooter = new EmbedFooterBuilder()
                .WithText(LocalizationService.Localize(
                    "Modules.General.ViewUserAvatar.EmbedFooter",
                    ("Artist", _options.Value.ArtworkArtistName)));

        var (userName, color) = user is IGuildUser guildUser
            ? (guildUser.DisplayName, guildUser.GetAccent(Color.Blue))
            : (user.GlobalName, Color.Blue);

        return (
            null,
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize(
                    avatarInfo.EffectiveAvatarKind == AvatarKind.ServerSpecific
                        ? "Modules.General.ViewUserAvatar.EmbedTitleServer"
                        : "Modules.General.ViewUserAvatar.EmbedTitleGlobal",
                    ("User", userName)))
                .WithFooter(embedFooter)
                .WithImageUrl(avatarInfo.EffectiveAvatarUrl)
                .WithColor(color)
                .Build(),
            GetResponseComponent(boundUserId, user.Id, avatarInfo));
    }

    private MessageComponent? GetResponseComponent(
        ulong boundUserId,
        ulong userId,
        AvatarInfo avatarInfo)
    {
        var isGlobalButtonEnabled = avatarInfo.CanSwitchTo(AvatarKind.Global);
        var isServerButtonEnabled = avatarInfo.CanSwitchTo(AvatarKind.ServerSpecific);
        if (!isGlobalButtonEnabled && !isServerButtonEnabled)
            return null;

        return new ComponentBuilder()
            .WithButton(
                customId: ComponentHelper.CreateCustomId("viewavatar", boundUserId, "0", userId),
                label: LocalizationService.Localize("Modules.General.ViewUserAvatar.GlobalButton"),
                style: ButtonStyle.Secondary,
                disabled: !isGlobalButtonEnabled)
            .WithButton(
                customId: ComponentHelper.CreateCustomId("viewavatar", boundUserId, "1", userId),
                label: LocalizationService.Localize("Modules.General.ViewUserAvatar.ServerButton"),
                style: ButtonStyle.Secondary,
                disabled: !isServerButtonEnabled)
            .Build();
    }

    public enum AvatarKind
    {
        [ChoiceDisplay("Global")]
        Global = 0,

        [ChoiceDisplay("Server specific")]
        ServerSpecific
    }

    private sealed class AvatarInfo
    {
        public required Dictionary<AvatarKind, string?> AvatarUrls { get; init; }
        public AvatarKind? EffectiveAvatarKind { get; init; }
        public string? EffectiveAvatarUrl { get; init; }

        public bool CanSwitchTo(AvatarKind avatarKind)
            => EffectiveAvatarKind.HasValue
               && EffectiveAvatarKind != avatarKind
               && AvatarUrls.TryGetValue(avatarKind, out var url)
               && !string.IsNullOrWhiteSpace(url);
    }
}