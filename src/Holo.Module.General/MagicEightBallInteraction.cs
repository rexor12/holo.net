using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General;

public sealed class MagicEightBallInteraction : InteractionGroupBase
{
    private static readonly Regex StripRegex = new(@"[\?\.!\-+ ]", RegexOptions.Compiled);

    private readonly IReadOnlyList<string> _answers;

    public MagicEightBallInteraction(
        ILocalizationService localizationService,
        ILogger<MagicEightBallInteraction> logger)
        : base(localizationService, logger)
    {
        _answers = localizationService.GetList("Modules.General.MagicEightBall.Responses");
    }

    [Cooldown(10, LocalizationKey = "Modules.General.MagicEightBall.CooldownError")]
    [SlashCommand("8ball", "Answers your yes/no question.")]
    public Task GetMagicEightBallResponseAsync(
        [Summary(description: "The yes/no question to be answered.")] string question)
    {
        var seedQuestion = StripRegex.Replace(question, m => string.Empty).ToLower();
        var seed = seedQuestion.GetHashCode() + DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60;
        var random = new Random((int)seed);
        var chosenAnswer = _answers[random.Next(_answers.Count)];

        return RespondAsync(
            text: LocalizationService.Localize(
                "Modules.General.MagicEightBall.ResponseMessage",
                ("Question", question),
                ("Answer", chosenAnswer)),
            allowedMentions: AllowedMentions.None);
    }
}