using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.Repositories;

/// <summary>
/// Abstract base class for a numeric identifier based repository.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
/// <typeparam name="TAggregateRoot">The type of the entity.</typeparam>
/// <typeparam name="TDbContext">The type of the containing <see cref="DbContext"/>.</typeparam>
public abstract class NumericIdentifierBasedRepositoryBase<TIdentifier, TAggregateRoot, TDbContext> : RepositoryBase<TIdentifier, TAggregateRoot, TDbContext>
    where TIdentifier : struct, IIdentifier<ulong>
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="NumericIdentifierBasedRepositoryBase{TAggregateRoot, TDbContext}"/>.
    /// </summary>
    /// <param name="databaseServices">
    /// The <see cref="IDatabaseServices"/> used to access database services.
    /// </param>
    protected NumericIdentifierBasedRepositoryBase(IDatabaseServices databaseServices)
        : base(databaseServices)
    {
    }
}
