using Holo.Module.Dev.Models;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.Dev.Storage.Repositories;

/// <summary>
/// A repository used for accessing <see cref="FeatureState"/> entities.
/// </summary>
[Service(typeof(IFeatureStateRepository))]
public sealed class FeatureStateRepository :
    StringIdentifierBasedRepositoryBase<FeatureState, DevDbContext>,
    IFeatureStateRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="FeatureStateRepository"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="unitOfWorkProvider">
    /// The <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </param>
    public FeatureStateRepository(
        IDbContextFactory dbContextFactory,
        IUnitOfWorkProvider unitOfWorkProvider)
        : base(dbContextFactory, unitOfWorkProvider)
    {
    }

    /// <inheritdoc cref="RepositoryBase{TAggregateRoot, TDbContext}.GetDbSet(TDbContext)"/>
    protected override DbSet<FeatureState> GetDbSet(DevDbContext dbContext)
        => dbContext.FeatureStates;
}