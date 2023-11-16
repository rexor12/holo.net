using Holo.Sdk.Storage.EntityFramework.Converters;
using Holo.Sdk.Storage.Ids;
using Microsoft.EntityFrameworkCore;

namespace Holo.Sdk.Storage.EntityFramework;

/// <summary>
/// Abstract base class for <see cref="DbContext"/>s.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public abstract class DbContextBase<TDbContext> : DbContext
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the name of the associated database schema.
    /// </summary>
    protected string SchemaName { get; }

    protected DbContextBase(string schemaName)
        : base()
    {
        SchemaName = schemaName;
    }

    protected DbContextBase(string schemaName, DbContextOptions<TDbContext> options)
        : base(options)
    {
        SchemaName = schemaName;
    }

    /// <summary>
    /// Configures the globally applicable conventiens.
    /// </summary>
    /// <param name="configurationBuilder">
    /// The builder being used to set defaults and configure conventions
    /// that will be used to build the model for this context.
    /// </param>
    /// <remarks>Inheritors should invoke the base method.</remarks>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<SnowflakeId>().HaveConversion<SnowflakeIdConverter>();
    }
}