using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.Repositories;

/// <summary>
/// Abstract base class for a string identifier based repository.
/// </summary>
/// <typeparam name="TAggregateRoot">The type of the entity.</typeparam>
/// <typeparam name="TDbContext">The type of the containing <see cref="DbContext"/>.</typeparam>
public abstract class StringIdentifierBasedRepositoryBase<TIdentifier, TAggregateRoot, TDbContext> : RepositoryBase<TIdentifier, TAggregateRoot, TDbContext>
    where TIdentifier : struct, IIdentifier<string>
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringIdentifierBasedRepositoryBase{TAggregateRoot, TDbContext}"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="databaseServices">
    /// The <see cref="IDatabaseServices"/> used to access database services.
    /// </param>
    protected StringIdentifierBasedRepositoryBase(IDatabaseServices databaseServices)
        : base(databaseServices)
    {
    }
}
