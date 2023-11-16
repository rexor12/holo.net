using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.Repositories;

/// <summary>
/// Abstract base class for a repository.
/// </summary>
/// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
/// <typeparam name="TAggregateRoot">The type of the entity.</typeparam>
/// <typeparam name="TDbContext">The type of the containing <see cref="DbContext"/>.</typeparam>
public abstract class RepositoryBase<TIdentifier, TAggregateRoot, TDbContext> : IRepository<TIdentifier, TAggregateRoot, TDbContext>
    where TIdentifier : struct, IIdentifier
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// The <see cref="IDatabaseServices"/> used to access database services.
    /// </summary>
    protected IDatabaseServices DatabaseServices { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RepositoryBase{TIdentifier, TAggregateRoot, TDbContext}"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="unitOfWorkProvider">
    /// The <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </param>
    protected RepositoryBase(IDatabaseServices databaseServices)
    {
        DatabaseServices = databaseServices;
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.GetAsync(TIdentifier)"/>
    public async Task<TAggregateRoot> GetAsync(TIdentifier identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        return await GetDbSet(dbContextWrapper).FirstAsync(GetEqualByIdExpression(identifier));
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.TryGetAsync(TIdentifier)"/>
    public async Task<TAggregateRoot?> TryGetAsync(TIdentifier identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        return await GetDbSet(dbContextWrapper).FirstOrDefaultAsync(GetEqualByIdExpression(identifier));
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.AddAsync(TAggregateRoot)"/>
    public async Task AddAsync(TAggregateRoot entity)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        await GetDbSet(dbContextWrapper).AddAsync(entity);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.UpdateAsync(TAggregateRoot)"/>
    public async Task UpdateAsync(TAggregateRoot entity)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        GetDbSet(dbContextWrapper).Update(entity);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.RemoveAsync(TIdentifier)"/>
    public async Task RemoveAsync(TIdentifier identifier)
    {
        await using var dbContextWrapper = GetDbContextWrapper();
        var dbSet = GetDbSet(dbContextWrapper);
        var entity = await dbSet.FirstOrDefaultAsync(GetEqualByIdExpression(identifier));
        if (entity == null)
            return;

        dbSet.Remove(entity);
    }

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> used by this repository.
    /// </summary>
    /// <param name="dbContext">The current <see cref="DbContext"/>.</param>
    /// <returns>The <see cref="DbSet{TEntity}"/> used by this repository.</returns>
    protected abstract DbSet<TAggregateRoot> GetDbSet(TDbContext dbContext);

    protected abstract Expression<Func<TAggregateRoot, bool>> GetEqualByIdExpression(TIdentifier identifier);

    /// <summary>
    /// Gets a <see cref="DbContextWrapper"/> that either wraps an existing <see cref="DbContext"/>
    /// or a newly created one. When disposed as a wrapper in a unit of work, the underlying
    /// <see cref="DbContext"/> is not disposed.
    /// </summary>
    /// <returns>A new instance of <see cref="DbContextWrapper"/>.</returns>
    protected DbContextWrapper GetDbContextWrapper()
    {
        var unitOfWork = DatabaseServices.UnitOfWorkProvider.Current;

        return unitOfWork == null
            ? new DbContextWrapper(DatabaseServices.DbContextFactory.Create<TDbContext>(), true, true)
            : new DbContextWrapper(unitOfWork.GetDbContext<TDbContext>(), false);
    }

    /// <summary>
    /// A wrapper for the currently active <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
    /// </summary>
    protected sealed class DbContextWrapper : IAsyncDisposable
    {
        private readonly bool _shouldDispose;
        private readonly bool _shouldSaveChanges;

        /// <summary>
        /// Gets the instance of the DbContext.
        /// </summary>
        public TDbContext DbContext { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DbContextWrapper"/>.
        /// </summary>
        /// <param name="dbContext">The wrapped <typeparamref name="TDbContext"/>.</param>
        /// <param name="shouldDispose">
        /// Whether the DbContext should be disposed on dispose.
        /// When we are in a unit of work, it should be left intact.
        /// </param>
        /// <param name="shouldSaveChanges">
        /// Whether the changes should be saved before disposal of the DbContext.
        /// This parameter is taken into consideration only when <paramref name="shouldDispose"/> is <c>true</c>.
        /// </param>
        public DbContextWrapper(TDbContext dbContext, bool shouldDispose, bool shouldSaveChanges = true)
        {
            DbContext = dbContext;
            _shouldDispose = shouldDispose;
            _shouldSaveChanges = shouldSaveChanges;
        }

        public static implicit operator TDbContext(DbContextWrapper wrapper)
            => wrapper.DbContext;

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync()
        {
            if (!_shouldDispose)
                return;

            if (_shouldSaveChanges)
                await DbContext.SaveChangesAsync();

            await DbContext.DisposeAsync();
        }
    }
}