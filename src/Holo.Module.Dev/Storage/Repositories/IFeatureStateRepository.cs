using Holo.Module.Dev.Models;
using Holo.Sdk.Storage.Repositories;

namespace Holo.Module.Dev.Storage.Repositories;

/// <summary>
/// Interface for a repository used for accessing <see cref="FeatureState"/> entities.
/// </summary>
public interface IFeatureStateRepository : IRepository<FeatureStateId, FeatureState, DevDbContext>
{
}