using System.Collections.Generic;

namespace Holo.Sdk.Localization;

/// <summary>
/// Interface for a service used for localization.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the localization for the given key.
    /// </summary>
    /// <param name="key">The key of the resource.</param>
    /// <param name="arguments">An optional list of arguments.</param>
    /// <returns>The localized text.</returns>
    string Localize(string key, params (string Name, object? Value)[]? arguments);

    /// <summary>
    /// Gets the localization for the given key.
    /// </summary>
    /// <param name="key">The key of the resource.</param>
    /// <param name="arguments">An optional list of arguments.</param>
    /// <returns>The localized text.</returns>
    string Localize(string key, IEnumerable<(string Name, object? Value)> arguments);

    /// <summary>
    /// Gets the localization for the list-like resource with the given key.
    /// </summary>
    /// <param name="key">The key of the resource.</param>
    /// <param name="itemIndex">The index of the item in the list.</param>
    /// <param name="arguments">An optional list of arguments.</param>
    /// <returns>The localized text.</returns>
    string Localize(string key, int itemIndex, params (string Name, object? Value)[]? arguments);
}