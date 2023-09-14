using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.Repositories;

/// <summary>
/// Abstract base class for a string identifier based repository.
/// </summary>
/// <typeparam name="TAggregateRoot">The type of the entity.</typeparam>
/// <typeparam name="TDbContext">The type of the containing <see cref="DbContext"/>.</typeparam>
public abstract class StringIdentifierBasedRepositoryBase<TAggregateRoot, TDbContext> : RepositoryBase<string, TAggregateRoot, TDbContext>
    where TAggregateRoot : AggregateRoot<string>
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="StringIdentifierBasedRepositoryBase{TAggregateRoot, TDbContext}"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="unitOfWorkProvider">
    /// The <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </param>
    protected StringIdentifierBasedRepositoryBase(
        IDbContextFactory dbContextFactory,
        IUnitOfWorkProvider unitOfWorkProvider)
        : base(dbContextFactory, unitOfWorkProvider)
    {
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.GetAsync(TIdentifier)"/>
    public override async Task<TAggregateRoot> GetAsync(string identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);

        return await GetDbSet(dbContextWrapper).FirstAsync(entity => entity.Identifier == identifier);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.TryGetAsync(TIdentifier)"/>
    public override async Task<TAggregateRoot?> TryGetAsync(string identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);

        return await GetDbSet(dbContextWrapper).FirstOrDefaultAsync(entity => entity.Identifier == identifier);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.RemoveAsync(TIdentifier)"/>
    public override async Task RemoveAsync(string identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);
        var dbSet = GetDbSet(dbContextWrapper);
        var entity = await dbSet.FirstOrDefaultAsync(entity => entity.Identifier == identifier);
        if (entity == null)
            return;

        dbSet.Remove(entity);
    }
}
