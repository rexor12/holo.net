using Holo.Module.Reminders.Models;
using Holo.Sdk.Storage.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.Reminders.Storage;

public sealed class RemindersDbContext : DbContextBase<RemindersDbContext>
{
    public required DbSet<Reminder> Reminders { get; set; }

    public RemindersDbContext()
        : base("reminders")
    {
    }

    public RemindersDbContext(DbContextOptions<RemindersDbContext> options)
        : base("reminders", options)
    {
    }

    // /// <summary>
    // /// Gets a randomly selected fortune cookie.
    // /// </summary>
    // /// <returns>A randomly selected <see cref="FortuneCookie"/>.</returns>
    // public IQueryable<Reminder> GetRandomFortuneCookie()
    //     => FromExpression(() => GetRandomFortuneCookie());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO https://github.com/npgsql/efcore.pg/issues/2617
        // modelBuilder.HasSequence(
        //     typeof(long),
        //     "reminders_hilo_seq",
        //     SchemaName,
        //     builder => builder.IncrementsBy(100));

        var reminderModel = modelBuilder.Entity<Reminder>(builder =>
        {
            builder
                .ToTable(Reminder.TableName, SchemaName)
                .HasKey(entity => entity.Identifier);

            builder
                .Property(entity => entity.Identifier)
                .HasConversion(v => (long)v, v => (ReminderId)v)
                // TODO https://github.com/npgsql/efcore.pg/issues/2617
                // .UseHiLo("reminders_hilo_seq", SchemaName)
                ;
        });

        // modelBuilder
        //     .HasDbFunction(() => GetRandomFortuneCookie())
        //     .HasSchema(SchemaName)
        //     .HasName("get_fortune_cookie");
    }
}