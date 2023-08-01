using System.Threading;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;

namespace Holo.ServiceHost.Storage;

[Service(typeof(IUnitOfWorkProvider))]
public sealed class UnitOfWorkProvider : IUnitOfWorkProvider
{
    private static readonly AsyncLocal<IUnitOfWork?> _unitOfWork = new AsyncLocal<IUnitOfWork?>();
    private readonly IDbContextFactory _dbContextFactory;

    public IUnitOfWork? Current
        => _unitOfWork.Value;

    public UnitOfWorkProvider(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IUnitOfWork GetOrCreate()
    {
        if (_unitOfWork.Value != null)
            return _unitOfWork.Value;

        return _unitOfWork.Value = new UnitOfWork(_dbContextFactory);
    }
}