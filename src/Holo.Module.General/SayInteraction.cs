using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General;

public sealed class SayInteraction : InteractionGroupBase
{
    public SayInteraction(
        ILocalizationService localizationService,
        ILogger<SayInteraction> logger)
        : base(localizationService, logger)
    {
    }

    [Cooldown(10, LocalizationKey = "Modules.General.Say.CooldownError")]
    [SlashCommand("say", "Repeats the specified text as the bot.")]
    public async Task SayAsync(
        [Summary(description: "The message to be repeated.")] string message)
    {
        await DeferAsync(true);
        await Context.Channel.SendMessageAsync(text: message, allowedMentions: AllowedMentions.None);
        await DeleteOriginalResponseAsync();
    }
}