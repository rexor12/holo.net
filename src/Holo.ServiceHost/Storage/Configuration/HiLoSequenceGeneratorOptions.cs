using System.Collections.Generic;

namespace Holo.ServiceHost.Storage.Configuration;

/// <summary>
/// Represents the options of the Hi/Lo sequence generator.
/// </summary>
public sealed class HiLoSequenceGeneratorOptions
{
    /// <summary>
    /// The default window size for identifier types.
    /// </summary>
    public const ulong DefaultWindowSize = 100;

    /// <summary>
    /// Gets or sets the window sizes by the identifier types. The default value is 100.
    /// </summary>
    /// <remarks>
    /// If the value associated to an identifier type is changed,
    /// the database also needs to be updated to avoid duplicate identifiers.
    /// </remarks>
    /// <seealso cref="DefaultWindowSize"/>
    public Dictionary<string, ulong> WindowSizes { get; set; } = new Dictionary<string, ulong>();
}
