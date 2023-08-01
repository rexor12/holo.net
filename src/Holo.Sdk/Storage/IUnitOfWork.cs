using System;
using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage;

/// <summary>
/// Interface for a unit of work.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets whether the unit of work has been completed yet.
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    /// Completes the unit of work. Further invocations of certain methods will throw an exception.
    /// </summary>
    void Complete();

    /// <summary>
    /// Gets the <see cref="DbContext"/> of the specified type.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    /// <returns>The <see cref="DbContext"/> instance.</returns>
    TDbContext GetDbContext<TDbContext>()
        where TDbContext : DbContext;
}