using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage;

/// <summary>
/// Interface for a factory used to create instances of <see cref="DbContext"/> inheritors.
/// </summary>
public interface IDbContextFactory
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="TDbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    /// <returns>The new instance of <typeparamref name="TDbContext"/>.</returns>
    TDbContext Create<TDbContext>()
        where TDbContext : DbContext;
}