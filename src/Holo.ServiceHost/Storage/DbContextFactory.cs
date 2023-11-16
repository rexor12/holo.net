using System;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;
using Holo.ServiceHost.Storage.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.ServiceHost.Storage;

/// <summary>
/// A factory used to create instances of <see cref="DbContext"/> inheritors.
/// </summary>
[Service(typeof(IDbContextFactory))]
public sealed class DbContextFactory : IDbContextFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IOptions<DatabaseOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="DbContextFactory"/>.
    /// </summary>
    /// <param name="options">The <see cref="DatabaseOptions"/>.</param>
    public DbContextFactory(ILoggerFactory loggerFactory, IOptions<DatabaseOptions> options)
    {
        _loggerFactory = loggerFactory;
        _options = options;
    }

    /// <inheritdoc cref="IDbContextFactory.Create{TDbContext}"/>
    public TDbContext Create<TDbContext>()
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        if (_options.Value.EnableSensitiveDataLogging)
            optionsBuilder.EnableSensitiveDataLogging();

        if (_options.Value.EnableEfCoreLogging)
        {
            var logger = _loggerFactory.CreateLogger<TDbContext>();
            optionsBuilder.LogTo(
                (EventId _, LogLevel _) => true,
                (EventData eventData) => logger.Log(
                    eventData.LogLevel,
                    "EFCore log entry, message: {Message}",
                    eventData.ToString()));
        }

        optionsBuilder.UseNpgsql(_options.Value.ConnectionString);

        var instance = Activator.CreateInstance(typeof(TDbContext), new object?[] { optionsBuilder.Options });
        if (instance is not TDbContext typedInstance)
            throw new ArgumentException(
                $"Failed to create an instance of '{typeof(TDbContext)}'.",
                nameof(TDbContext));

        return typedInstance;
    }
}