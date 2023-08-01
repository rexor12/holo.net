using System;
using Holo.Sdk.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Holo.ServiceHost.Storage;

/// <summary>
/// A factory used to create instances of <see cref="DbContext"/> inheritors.
/// </summary>
public sealed class DbContextFactory : IDbContextFactory
{
    private readonly IOptions<DatabaseOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="DbContextFactory"/>.
    /// </summary>
    /// <param name="options">The <see cref="DatabaseOptions"/>.</param>
    public DbContextFactory(IOptions<DatabaseOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc cref="IDbContextFactory.Create{TDbContext}"/>
    public TDbContext Create<TDbContext>()
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString);

        var instance = Activator.CreateInstance(typeof(TDbContext), new object?[] { optionsBuilder.Options });
        if (instance is not TDbContext typedInstance)
            throw new ArgumentException(
                $"Failed to create an instance of '{typeof(TDbContext)}'.",
                nameof(TDbContext));

        return typedInstance;
    }
}