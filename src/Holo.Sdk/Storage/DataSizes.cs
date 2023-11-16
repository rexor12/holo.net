namespace Holo.Sdk.Storage;

/// <summary>
/// A collection of data size related constants.
/// </summary>
public static class DataSizes
{
    /// <summary>
    /// Recommended size for medium length user input.
    /// </summary>
    public static readonly int UserText = 512;

    /// <summary>
    /// Recommended size for string type identifiers.
    /// </summary>
    public static readonly int StringId = 250;

    /// <summary>
    /// Recommended size for snowflakes (Discord identifiers).
    /// </summary>
    public static readonly int Snowflake = 20;
}