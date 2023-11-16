using Holo.Sdk.Storage.EntityFramework;
using Holo.ServiceHost.BackgroundProcessing;
using Microsoft.EntityFrameworkCore;

namespace Holo.ServiceHost.Storage;

public sealed class ServiceHostDbContext : DbContextBase<ServiceHostDbContext>
{
    public required DbSet<Item> BackgroundProcessingItems { get; set; }

    public ServiceHostDbContext()
        : base("service_host")
    {
    }

    public ServiceHostDbContext(DbContextOptions<ServiceHostDbContext> options)
        : base("service_host", options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(builder =>
        {
            builder
                .ToTable(Item.TableName, SchemaName)
                .HasKey(entity => entity.Identifier);

            builder
                .Property(entity => entity.Identifier)
                .HasConversion(v => v.Value, v => new ItemId(v));
        });
    }
}