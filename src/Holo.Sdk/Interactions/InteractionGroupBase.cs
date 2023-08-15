using Discord.Interactions;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Sdk.Interactions;

/// <summary>
/// Abstract base class for an interaction group.
/// </summary>
public abstract class InteractionGroupBase : InteractionModuleBase<ExtendedInteractionContext>, IInteractionGroup
{
    protected ILocalizationService LocalizationService { get; }
    protected ILogger Logger { get; }

    protected InteractionGroupBase(
        ILocalizationService localizationService,
        ILogger logger)
    {
        LocalizationService = localizationService;
        Logger = logger;
    }
}