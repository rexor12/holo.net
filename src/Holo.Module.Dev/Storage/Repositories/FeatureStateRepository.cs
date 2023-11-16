using System;
using System.Linq.Expressions;
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
    StringIdentifierBasedRepositoryBase<FeatureStateId, FeatureState, DevDbContext>,
    IFeatureStateRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="FeatureStateRepository"/>.
    /// </summary>
    /// <param name="databaseServices">
    /// The <see cref="IDatabaseServices"/> used to access units of work.
    /// </param>
    public FeatureStateRepository(IDatabaseServices databaseServices)
        : base(databaseServices)
    {
    }

    /// <inheritdoc cref="RepositoryBase{TAggregateRoot, TDbContext}.GetDbSet(TDbContext)"/>
    protected override DbSet<FeatureState> GetDbSet(DevDbContext dbContext)
        => dbContext.FeatureStates;

    protected override Expression<Func<FeatureState, bool>> GetEqualByIdExpression(FeatureStateId identifier)
        => entity => entity.Identifier == identifier;
}