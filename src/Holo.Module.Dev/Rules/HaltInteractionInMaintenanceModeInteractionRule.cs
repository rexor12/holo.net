using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Holo.Module.Dev.Managers;
using Holo.Sdk.DI;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Rules;

namespace Holo.Module.Dev.Rules;

/// <summary>
/// Halts interactions other then the "dev" command group
/// when the application is in maintenance mode.
/// </summary>
[Service(typeof(IInteractionRule))]
public sealed class HaltInteractionInMaintenanceModeInteractionRule : IInteractionRule
{
    private readonly IMaintenanceManager _maintenanceManager;

    public HaltInteractionInMaintenanceModeInteractionRule(IMaintenanceManager maintenanceManager)
    {
        _maintenanceManager = maintenanceManager;
    }

    /// <inheritdoc cref="IInteractionRule.EvaluateAsync(ExtendedInteractionContext)"/>
    public async Task<InteractionRuleResult> EvaluateAsync(ExtendedInteractionContext interactionContext)
    {
        if (interactionContext.Interaction is IApplicationCommandInteraction interaction
            && interaction.Data.Name.Equals("dev", StringComparison.OrdinalIgnoreCase))
        {
            return InteractionRuleResult.OkResult;
        }

        return !await _maintenanceManager.IsMaintenanceModeEnabledAsync()
            ? InteractionRuleResult.OkResult
            : new InteractionRuleResult(
                true,
                "Modules.Dev.MaintenanceModeEnabledError",
                Enumerable.Empty<(string, object?)>());
    }
}