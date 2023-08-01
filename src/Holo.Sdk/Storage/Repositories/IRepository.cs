using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.Repositories;

/// <summary>
/// Interface for a repository used for accessing database entities.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
/// <typeparam name="TAggregateRoot">The type of the entity.</typeparam>
/// <typeparam name="TDbContext">The type of the containing <see cref="DbContext"/>.</typeparam>
public interface IRepository<TIdentifier, TAggregateRoot, TDbContext>
    where TIdentifier : struct
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the entity.</param>
    /// <returns>The entity with the given identifier.</returns>
    Task<TAggregateRoot> GetAsync(TIdentifier identifier);
}
