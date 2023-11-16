using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage;

/// <summary>
/// Interface for accessing database services.
/// </summary>
public interface IDatabaseServices
{
    /// <summary>
    /// Gets the factory used to create instances of <see cref="DbContext"/> inheritors.
    /// </summary>
    IDbContextFactory DbContextFactory { get; }

    /// <summary>
    /// Gets the service used to access units of work.
    /// </summary>
    IUnitOfWorkProvider UnitOfWorkProvider { get; }
}