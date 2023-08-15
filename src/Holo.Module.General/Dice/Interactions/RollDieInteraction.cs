using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Module.General.Dice.Configuration;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.Module.General.Dice.Interactions;

public sealed class RollDieInteraction : InteractionGroupBase
{
    private static readonly IReadOnlySet<int> NamedDice = new HashSet<int>
    {
        1, 4, 6, 8, 10, 12, 20, 24, 50, 120
    };

    private readonly IOptions<DiceOptions> _options;

    public RollDieInteraction(
        ILocalizationService localizationService,
        ILogger<RollDieInteraction> logger,
        IOptions<DiceOptions> options)
        : base(localizationService, logger)
    {
        _options = options;
    }

    [Cooldown(5, 1, LocalizationKey = "Modules.General.RollDie.CooldownError")]
    [SlashCommand("roll", "Generates a random integer between the specified bounds.")]
    public Task RollDieAsync(
        [Summary("max", "The upper bound.")] int upperBound,
        [Summary("min", "The lower bound. By default, it's 1.")] int lowerBound = 1)
    {
        if (upperBound < lowerBound)
            (lowerBound, upperBound) = (upperBound, lowerBound);

        var result = Random.Shared.Next(lowerBound, upperBound);
        string? dieName = null;
        var localizationKey = "Modules.General.RollDie.UnusualRollResult";
        if (lowerBound == 1 && NamedDice.Contains(upperBound))
        {
            localizationKey = "Modules.General.RollDie.UsualRollResult";
            dieName = LocalizationService.Localize($"Modules.General.RollDie.Die{upperBound}");
        }

        return RespondAsync(
            embeds: new[]
            {
                new EmbedBuilder()
                    .WithTitle(LocalizationService.Localize("Modules.General.RollDie.EmbedTitle"))
                    .WithDescription(LocalizationService.Localize(
                        localizationKey,
                        ("Name", dieName),
                        ("Value", result)))
                    .WithThumbnailUrl(_options.Value.EmbedThumbnailUrl)
                    .WithColor(Color.Purple)
                    .Build()
            });
    }
}