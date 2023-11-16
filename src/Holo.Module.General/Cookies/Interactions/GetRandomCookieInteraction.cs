using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Holo.Module.General.Cookies.Configuration;
using Holo.Module.General.Cookies.Storage.Repositories;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.Module.General.Cookies.Interactions;

public sealed class GetRandomCookieInteraction : InteractionGroupBase
{
    private readonly IFortuneCookieRepository _fortuneCookieRepository;
    private readonly IOptions<FortuneCookieOptions> _options;

    public GetRandomCookieInteraction(
        IFortuneCookieRepository fortuneCookieRepository,
        ILocalizationService localizationService,
        ILogger<GetRandomCookieInteraction> logger,
        IOptions<FortuneCookieOptions> options)
        : base(localizationService, logger)
    {
        _fortuneCookieRepository = fortuneCookieRepository;
        _options = options;
    }

    [Cooldown(30, 1, LocalizationKey = "Modules.General.GetRandomCookie.CooldownError")]
    [SlashCommand("cookie", "Crack open a fortune cookie.")]
    public async Task GetRandomCookieAsync()
    {
        var fortuneCookie = await _fortuneCookieRepository.GetRandomFortuneCookieAsync();
        await RespondAsync(
            embeds: new[]
            {
                new EmbedBuilder()
                    .WithTitle(LocalizationService.Localize("Modules.General.GetRandomCookie.EmbedTitle"))
                    .WithDescription(fortuneCookie.Message)
                    .WithColor(Color.Gold)
                    .WithThumbnailUrl(_options.Value.EmbedThumbnailUrl)
                    .WithFooter(new EmbedFooterBuilder().WithText(
                        LocalizationService.Localize(
                            "Modules.General.GetRandomCookie.EmbedFooter",
                            ("Id", fortuneCookie.Identifier.Value))))
                    .Build()
            });
    }
}