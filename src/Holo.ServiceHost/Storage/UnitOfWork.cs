using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Holo.Sdk.Storage;
using Microsoft.EntityFrameworkCore;

namespace Holo.ServiceHost.Storage;

public class UnitOfWork : IUnitOfWork
{
    private int _isDisposed;
    private readonly ConcurrentDictionary<Type, DbContext> _dbContexts = new();
    private readonly TransactionScope _transactionScope;
    private readonly IDbContextFactory _dbContextFactory;

    public bool IsComplete { get; private set; }

    public UnitOfWork(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        _transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    public void Complete()
    {
        IsComplete = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
            return;

        Func<DbContext, Task> completeFunc = IsComplete
            ? dbContext => dbContext.SaveChangesAsync()
            : _ => Task.CompletedTask;
        foreach (var dbContext in _dbContexts.Values)
        {
            await completeFunc(dbContext);
            await dbContext.DisposeAsync();
        }

        if (IsComplete)
        {
            _transactionScope.Complete();
        }

        _transactionScope.Dispose();
    }

    public TDbContext GetDbContext<TDbContext>()
        where TDbContext : DbContext
    {
        if (IsComplete)
            throw new InvalidOperationException("The unit of work has been completed already.");

        return (TDbContext)_dbContexts.GetOrAdd(typeof(TDbContext), _ => _dbContextFactory.Create<TDbContext>());
    }
}
