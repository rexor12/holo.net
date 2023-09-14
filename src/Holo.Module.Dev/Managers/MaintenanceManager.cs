using System.Threading.Tasks;
using Holo.Module.Dev.Models;
using Holo.Module.Dev.Storage.Repositories;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;

namespace Holo.Module.Dev.Managers;

/// <summary>
/// A service that can be used to turn application maintenance mode on or off.
/// </summary>
[Service(typeof(IMaintenanceManager))]
public sealed class MaintenanceManager : IMaintenanceManager
{
    private const string MaintenanceModeFeatureName = "MaintenanceMode";

    private readonly IFeatureStateRepository _featureStateRepository;
    private readonly IUnitOfWorkProvider _unitOfWorkProvider;

    public MaintenanceManager(
        IFeatureStateRepository featureStateRepository,
        IUnitOfWorkProvider unitOfWorkProvider)
    {
        _featureStateRepository = featureStateRepository;
        _unitOfWorkProvider = unitOfWorkProvider;
    }

    /// <inheritdoc cref="IMaintenanceManager.IsMaintenanceModeEnabledAsync"/>
    public async Task<bool> IsMaintenanceModeEnabledAsync()
    {
        var featureState = await _featureStateRepository.TryGetAsync(MaintenanceModeFeatureName);

        return featureState != null && featureState.IsEnabled;
    }

    /// <inheritdoc cref="IMaintenanceManager.SetOperationModeAsync(bool)"/>
    public async Task SetOperationModeAsync(bool isMaintenanceMode)
    {
        await using var unitOfWork = _unitOfWorkProvider.GetOrCreate();

        var featureState = await _featureStateRepository.TryGetAsync(MaintenanceModeFeatureName);
        if (featureState == null)
        {
            await _featureStateRepository.AddAsync(new FeatureState
            {
                Identifier = MaintenanceModeFeatureName,
                IsEnabled = isMaintenanceMode
            });

            unitOfWork.Complete();

            return;
        }

        if (featureState.IsEnabled == isMaintenanceMode)
            return;

        featureState.IsEnabled = isMaintenanceMode;
        await _featureStateRepository.UpdateAsync(featureState);

        unitOfWork.Complete();
    }
}