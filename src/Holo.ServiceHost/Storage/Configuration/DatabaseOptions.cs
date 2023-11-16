namespace Holo.ServiceHost.Storage.Configuration;

/// <summary>
/// Represents the database options.
/// </summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "DatabaseOptions";

    /// <summary>
    /// Gets or sets the connection string of the database.
    /// </summary>
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets whether EFCore sensitive data logging is enabled.
    /// </summary>
    public required bool EnableSensitiveDataLogging { get; set; }

    /// <summary>
    /// Gets or sets whether EFCore logging is enabled.
    /// </summary>
    public required bool EnableEfCoreLogging { get; set; }

    /// <summary>
    /// Gets or sets the options of the Hi/Lo sequence generator.
    /// </summary>
    public required HiLoSequenceGeneratorOptions HiLoSequenceGeneratorOptions { get; set; }
}