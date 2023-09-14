using System.Collections.Generic;
using System.Linq;

namespace Holo.Sdk.Interactions.Rules;

/// <summary>
/// Represents the result of an interaction rule.
/// </summary>
/// <param name="ShouldHalt"><c>true</c>, if the interaction shouldn't be executed.</param>
/// <param name="ResponseLocalizationKey">
/// They key of the localization resource that describes the problem.
/// </param>
/// <param name="ResponseLocalizationArguments">
/// Optional arguments fr the localization resource.
/// </param>
public record InteractionRuleResult(
    bool ShouldHalt,
    string? ResponseLocalizationKey,
    IEnumerable<(string Key, object? Value)> ResponseLocalizationArguments)
{
    /// <summary>
    /// A singleton instance of a non-halting result.
    /// </summary>
    public static InteractionRuleResult OkResult = new(false, null, Enumerable.Empty<(string, object?)>());
}