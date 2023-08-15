using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General;

public sealed class ViewBotInfoInteraction : InteractionGroupBase
{
    public ViewBotInfoInteraction(
        ILocalizationService localizationService,
        ILogger<ViewBotInfoInteraction> logger)
        : base(localizationService, logger)
    {
    }

    [Cooldown(60)]
    [SlashCommand("info", "Displays some information about the bot.")]
    public Task ViewBotInfoAsync() => RespondAsync(
        embeds: new[]
        {
            new EmbedBuilder()
                .WithTitle(LocalizationService.Localize("Modules.General.ViewBotInfo.EmbedTitle"))
                .WithDescription(LocalizationService.Localize(
                    "Modules.General.ViewBotInfo.GeneralValue",
                    ("Version", "0.1.0.0"),
                    ("Latency", Context.Client.Latency),
                    ("Servers", Context.Client.Guilds.Count),
                    ("ServerTime", DateTimeOffset.UtcNow)))
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .WithFooter(
                    LocalizationService.Localize("Modules.General.ViewBotInfo.EmbedFooter"),
                    "https://avatars.githubusercontent.com/u/15330052")
                .Build()
        },
        components: new ComponentBuilder()
            .WithButton(
                label: LocalizationService.Localize("Modules.General.ViewBotInfo.ProjectHome"),
                style: ButtonStyle.Link,
                url: "https://github.com/rexor12/holobot")
            .WithButton(
                label: LocalizationService.Localize("Modules.General.ViewBotInfo.ReportBug"),
                style: ButtonStyle.Link,
                url: "https://github.com/rexor12/holobot/issues/new?assignees=&labels=bug%2Fbugfix&template=bug_report.yml")
            .WithButton(
                label: LocalizationService.Localize("Modules.General.ViewBotInfo.RequestFeature"),
                style: ButtonStyle.Link,
                url: "https://github.com/rexor12/holobot/issues/new?assignees=&labels=feature+request&template=feature_request.yml")
            .Build());
}