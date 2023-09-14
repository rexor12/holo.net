using Holo.Module.Dev.Models;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.Dev.Storage;

public sealed class DevDbContext : DbContext
{
    private const string SchemaName = "dev";

    public required DbSet<FeatureState> FeatureStates { get; set; }

    public DevDbContext()
    {
    }

    public DevDbContext(DbContextOptions<DevDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<FeatureState>()
            .ToTable(FeatureState.TableName, SchemaName)
            .HasKey(entity => entity.Identifier);
    }
}