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
    where TIdentifier : notnull
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the entity.</param>
    /// <returns>The entity with the given identifier.</returns>
    Task<TAggregateRoot> GetAsync(TIdentifier identifier);

    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the entity.</param>
    /// <returns>If exists, the entity with the given identifier; otherwise, <c>null</c>.</returns>
    Task<TAggregateRoot?> TryGetAsync(TIdentifier identifier);

    /// <summary>
    /// Adds the specified entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task AddAsync(TAggregateRoot entity);

    /// <summary>
    /// Updates the specified entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task UpdateAsync(TAggregateRoot entity);

    /// <summary>
    /// Removes the entity specified by the given identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the entity to be removed.</param>
    /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
    Task RemoveAsync(TIdentifier identifier);
}
