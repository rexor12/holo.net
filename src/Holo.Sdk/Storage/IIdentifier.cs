namespace Holo.Sdk.Storage;

/// <summary>
/// Interface for an identifier.
/// </summary>
public interface IIdentifier
{
}

/// <summary>
/// Generic interface for an identifier.
/// </summary>
/// <typeparam name="T">The type of the underlying data.</typeparam>
public interface IIdentifier<T> : IIdentifier
    where T : notnull
{
    /// <summary>
    /// Gets the value the identifier represents.
    /// </summary>
    T Value { get; }
}
