using Holo.Sdk.DI;
using Holo.Sdk.Storage;

namespace Holo.ServiceHost.Storage;

/// <summary>
/// A short-hand service for accessing database services.
/// </summary>
[Service(typeof(IDatabaseServices))]
public sealed class DatabaseServices : IDatabaseServices
{
    /// <inheritdoc cref="IDatabaseServices.DbContextFactory"/>
    public IDbContextFactory DbContextFactory { get; }

    /// <inheritdoc cref="IDatabaseServices.UnitOfWorkProvider"/>
    public IUnitOfWorkProvider UnitOfWorkProvider { get; }

    public DatabaseServices(IDbContextFactory dbContextFactory, IUnitOfWorkProvider unitOfWorkProvider)
    {
        DbContextFactory = dbContextFactory;
        UnitOfWorkProvider = unitOfWorkProvider;
    }
}