using System.Linq;
using Holo.Module.General.Cookies.Models;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.General.Cookies.Storage;

public sealed class CookiesDbContext : DbContext
{
    private const string SchemaName = "general";

    public required DbSet<FortuneCookie> FortuneCookies { get; set; }

    public CookiesDbContext()
    {
    }

    public CookiesDbContext(DbContextOptions<CookiesDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets a randomly selected fortune cookie.
    /// </summary>
    /// <returns>A randomly selected <see cref="FortuneCookie"/>.</returns>
    public IQueryable<FortuneCookie> GetRandomFortuneCookie()
        => FromExpression(() => GetRandomFortuneCookie());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<FortuneCookie>()
            .ToTable(FortuneCookie.TableName, SchemaName)
            .HasKey(entity => entity.Identifier);

        modelBuilder
            .HasDbFunction(() => GetRandomFortuneCookie())
            .HasSchema(SchemaName)
            .HasName("get_fortune_cookie");
    }
}