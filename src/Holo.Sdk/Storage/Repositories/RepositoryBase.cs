using System;
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
    where TIdentifier : notnull
    where TAggregateRoot : AggregateRoot<TIdentifier>
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </summary>
    protected IDbContextFactory DbContextFactory { get; }

    /// <summary>
    /// Gets the <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </summary>
    protected IUnitOfWorkProvider UnitOfWorkProvider { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RepositoryBase{TIdentifier, TAggregateRoot, TDbContext}"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="unitOfWorkProvider">
    /// The <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </param>
    protected RepositoryBase(
        IDbContextFactory dbContextFactory,
        IUnitOfWorkProvider unitOfWorkProvider)
    {
        DbContextFactory = dbContextFactory;
        UnitOfWorkProvider = unitOfWorkProvider;
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.GetAsync(TIdentifier)"/>
    public abstract Task<TAggregateRoot> GetAsync(TIdentifier identifier);

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.TryGetAsync(TIdentifier)"/>
    public abstract Task<TAggregateRoot?> TryGetAsync(TIdentifier identifier);

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.AddAsync(TAggregateRoot)"/>
    public async Task AddAsync(TAggregateRoot entity)
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);

        await GetDbSet(dbContextWrapper).AddAsync(entity);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.UpdateAsync(TAggregateRoot)"/>
    public async Task UpdateAsync(TAggregateRoot entity)
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);

        GetDbSet(dbContextWrapper).Update(entity);
    }

    /// <inheritdoc cref="IRepository{TIdentifier, TAggregateRoot, TDbContext}.RemoveAsync(TIdentifier)"/>
    public abstract Task RemoveAsync(TIdentifier identifier);

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> used by this repository.
    /// </summary>
    /// <param name="dbContext">The current <see cref="DbContext"/>.</param>
    /// <returns>The <see cref="DbSet{TEntity}"/> used by this repository.</returns>
    protected abstract DbSet<TAggregateRoot> GetDbSet(TDbContext dbContext);

    /// <summary>
    /// Gets a <see cref="DbContextWrapper"/>.
    /// </summary>
    /// <param name="shouldSaveChanges">
    /// Whether the changes should be saves before the disposal of the <see cref="DbContext"/>.
    /// This parameter is ignored when the queries are executed within a unit of work.
    /// </param>
    /// <returns></returns>
    protected DbContextWrapper GetDbContextWrapper(bool shouldSaveChanges = true)
    {
        var unitOfWork = UnitOfWorkProvider.Current;
        if (unitOfWork != null)
            return new DbContextWrapper(unitOfWork.GetDbContext<TDbContext>(), false);

        return new DbContextWrapper(DbContextFactory.Create<TDbContext>(), true, shouldSaveChanges);
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