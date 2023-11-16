using System.Threading;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;

namespace Holo.ServiceHost.Storage;

/// <summary>
/// A service used to access units of work.
/// </summary>
[Service(typeof(IUnitOfWorkProvider))]
public sealed class UnitOfWorkProvider : IUnitOfWorkProvider
{
    private static readonly AsyncLocal<IUnitOfWork?> _unitOfWork = new AsyncLocal<IUnitOfWork?>();
    private readonly IDbContextFactory _dbContextFactory;

    /// <inheritdoc cref="IUnitOfWorkProvider.HasUnitOfWork"/>
    public bool HasUnitOfWork
        => _unitOfWork.Value != null;

    /// <inheritdoc cref="IUnitOfWorkProvider.Current"/>
    public IUnitOfWork? Current
        => _unitOfWork.Value;

    public UnitOfWorkProvider(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc cref="IUnitOfWorkProvider.GetOrCreate"/>
    public IUnitOfWork GetOrCreate()
    {
        if (_unitOfWork.Value != null)
            return _unitOfWork.Value;

        return _unitOfWork.Value = new UnitOfWork(_dbContextFactory);
    }
}