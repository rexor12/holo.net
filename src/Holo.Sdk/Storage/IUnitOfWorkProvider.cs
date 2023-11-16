namespace Holo.Sdk.Storage;

/// <summary>
/// Interface for a service used to access units of work.
/// </summary>
public interface IUnitOfWorkProvider
{
    /// <summary>
    /// Gets whether there is an active <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <value></value>
    bool HasUnitOfWork { get; }

    /// <summary>
    /// Gets the currently active <see cref="IUnitOfWork"/>, if any.
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// Gets the currently active unit of work, if any; otherwise, creates a new one.
    /// </summary>
    /// <returns>The currently active <see cref="IUnitOfWork"/>.</returns>
    IUnitOfWork GetOrCreate();
}
