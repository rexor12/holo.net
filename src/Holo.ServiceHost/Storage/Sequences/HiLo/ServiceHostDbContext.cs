using Holo.Sdk.Storage.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Holo.ServiceHost.Storage.Sequences.HiLo;

public sealed class ServiceHostDbContext : DbContextBase<ServiceHostDbContext>
{
    public ServiceHostDbContext()
        : base("service_host")
    {
    }

    public ServiceHostDbContext(DbContextOptions<ServiceHostDbContext> options)
        : base("service_host", options)
    {
    }
}
