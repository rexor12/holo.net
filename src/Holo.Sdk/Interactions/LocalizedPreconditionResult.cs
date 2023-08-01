using System;
using System.Collections.Generic;
using Discord.Interactions;

namespace Holo.Sdk.Interactions;

/// <summary>
/// Represents a result type of error <see cref="InteractionCommandError.UnmetPrecondition"/>
/// for interaction preconditions.
/// </summary>
public class LocalizedPreconditionResult : PreconditionResult
{
    private const string DefaultReason = "A precondition has been unmet.";

    /// <summary>
    /// Gets the key for the localization entry.
    /// </summary>
    public string LocalizationKey { get; }

    /// <summary>
    /// Gets the arguments for the localization entry.
    /// </summary>
    public IReadOnlyList<(string Key, object? Value)> LocalizationArguments { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="LocalizedPreconditionResult"/>.
    /// </summary>
    /// <param name="localizationKey">The key for the localization entry.</param>
    /// <param name="localizationArguments">The list of arguments for the localization entry.</param>
    public LocalizedPreconditionResult(
        string localizationKey,
        params (string Key, object? Value)[]? localizationArguments)
        : base(InteractionCommandError.UnmetPrecondition, DefaultReason)
    {
        LocalizationKey = localizationKey;
        LocalizationArguments = localizationArguments ?? Array.Empty<(string, object?)>();
    }
}