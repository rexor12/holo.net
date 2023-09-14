using System.Threading.Tasks;

namespace Holo.Sdk.Interactions.Rules;

/// <summary>
/// Interface for a rule that determines whether an interaction can be executed.
/// </summary>
public interface IInteractionRule
{
    /// <summary>
    /// Determines whether the interaction with the specified context can be executed.
    /// </summary>
    /// <param name="interactionContext">The <see cref="ExtendedInteractionContext"/> of the interaction.</param>
    /// <returns>An <see cref="InteractionRuleResult"/>.</returns>
    Task<InteractionRuleResult> EvaluateAsync(ExtendedInteractionContext interactionContext);
}