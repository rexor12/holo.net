using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Module.General.Reactions.DataProviders;
using Holo.Module.General.Reactions.Enums;
using Holo.Sdk.Discord;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Polly.CircuitBreaker;

namespace Holo.Module.General.Reactions.Interactions;

[Group("react", "Shows a reaction with an animated picture.")]
public sealed class ShowReactionInteraction : InteractionGroupBase
{
    private const string DefaultCooldownKey = "Modules.General.ShowReaction.CooldownError";

    private readonly IReactionDataProvider _reactionDataProvider;

    public ShowReactionInteraction(
        ILocalizationService localizationService,
        ILogger<ShowReactionInteraction> logger,
        IReactionDataProvider reactionDataProvider)
        : base(localizationService, logger)
    {
        _reactionDataProvider = reactionDataProvider;
    }

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("awoo", "Summon a lovely wolfgirl.")]
    public Task ShowAwooReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Awoo, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("bite", "Bite someone.")]
    public Task ShowBiteReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Bite, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("blush", "Show your embarrassment.")]
    public Task ShowBlushReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Blush, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("bonk", "Bonk someone.")]
    public Task ShowBonkReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Bonk, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("bully", "Bully someone.")]
    public Task ShowBullyReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Bully, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("cry", "Cry on someone's shoulder.")]
    public Task ShowCryReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Cry, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("cuddle", "Cuddle with someone.")]
    public Task ShowCuddleReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Cuddle, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("dance", "Dance with someone.")]
    public Task ShowDanceReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Dance, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("handhold", "Hold someone's hand.")]
    public Task ShowHandholdReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Handhold, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("happy", "Show your happiness to someone.")]
    public Task ShowHappyReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Happy, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("highfive", "Give someone a high-five.")]
    public Task ShowHighfiveReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Highfive, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("hug", "Hug someone.")]
    public Task ShowHugReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Hug, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("kill", "Kill someone.")]
    public Task ShowKillReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Kill, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("kiss", "Kiss someone.")]
    public Task ShowKissReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Kiss, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("lick", "Lick someone.")]
    public Task ShowLickReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Lick, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("neko", "Summon a lovely catgirl.")]
    public Task ShowNekoReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Neko, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("nom", "Eat someone.")]
    public Task ShowNomReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Nom, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("pat", "Pat someone on the head.")]
    public Task ShowPatReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Pat, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("poke", "Poke someone.")]
    public Task ShowPokeReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Poke, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("slap", "Slap someone.")]
    public Task ShowSlapReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Slap, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("smile", "Smile at someone.")]
    public Task ShowSmileReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Smile, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("smug", "Show your most smug face.")]
    public Task ShowSmugReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Smug, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("waifu", "Summon a lovely waifu.")]
    public Task ShowWaifuReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Waifu, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("wave", "Wave at someone.")]
    public Task ShowWaveReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Wave, user);

    [Cooldown(10, LocalizationKey = DefaultCooldownKey)]
    [SlashCommand("wink", "Wink at someone.")]
    public Task ShowWinkReactionAsync([Summary(description: "The target user.")] IUser? user = null)
        => ShowReactionAsync(ReactionType.Wink, user);

    private async Task ShowReactionAsync(ReactionType reactionType, IUser? user)
    {
        var targetUser = user?.Id == Context.User.Id
            ? null
            : await Context.GetRelevantUserAsync(user);
        string? pictureUrl = null;
        try
        {
            pictureUrl = await _reactionDataProvider.TryGetPictureUrlAsync(reactionType);
        }
        catch (Exception e) when (e is BulkheadRejectedException or BrokenCircuitException or HttpRequestException)
        {
            await RespondAsync(
                LocalizationService.Localize("Modules.General.ShowReaction.RemoteApiError"),
                ephemeral: true);
        }

        if (string.IsNullOrEmpty(pictureUrl))
        {
            await RespondAsync(
                LocalizationService.Localize("Modules.General.ShowReaction.NoImageError"),
                ephemeral: true);
            return;
        }

        var descriptionKey = targetUser == null
            ? $"Modules.General.ShowReaction.Actions.{reactionType}"
            : $"Modules.General.ShowReaction.Actions.{reactionType}Target";
        var color = (targetUser ?? Context.User) is IGuildUser guildUser
            ? guildUser.GetAccent(Color.Blue)
            : Color.Blue;

        await RespondAsync(
            embeds: new[]
            {
                new EmbedBuilder()
                    .WithTitle(LocalizationService.Localize("Modules.General.ShowReaction.EmbedTitle"))
                    .WithDescription(LocalizationService.Localize(
                        descriptionKey,
                        ("UserId", Context.User.Id),
                        ("TargetUserId", targetUser?.Id)))
                    .WithImageUrl(pictureUrl)
                    .WithFooter(LocalizationService.Localize("Modules.General.ShowReaction.EmbedFooter"))
                    .WithColor(color)
                    .Build()
            });
    }
}