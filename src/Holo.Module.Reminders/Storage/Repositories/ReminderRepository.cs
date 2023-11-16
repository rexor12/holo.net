using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Holo.Module.Reminders.Models;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Ids;
using Holo.Sdk.Storage.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.Reminders.Storage.Repositories;

[Service(typeof(IReminderRepository))]
public sealed class ReminderRepository
    : NumericIdentifierBasedRepositoryBase<ReminderId, Reminder, RemindersDbContext>, IReminderRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="ReminderRepository"/>.
    /// </summary>
    /// <param name="databaseServices">
    /// The <see cref="IDatabaseServices"/> used to access units of work.
    /// </param>
    public ReminderRepository(IDatabaseServices databaseServices)
        : base(databaseServices)
    {
    }

    public async Task<bool> RemoveByUserAsync(SnowflakeId userId, ReminderId reminderId)
    {
        await using var dbContextWrapper = GetDbContextWrapper();
        var dbSet = GetDbSet(dbContextWrapper);
        var entity = await dbSet.FirstOrDefaultAsync(entity =>
            entity.UserId == userId
            && entity.Identifier == reminderId);
        if (entity == null)
            return false;

        dbSet.Remove(entity);

        return true;
    }

    public async Task<int> GetCountByUserAsync(SnowflakeId userId)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        return await GetDbSet(dbContextWrapper)
            .Where(entity => entity.UserId == userId)
            .CountAsync();
    }

    public async Task<IReadOnlyCollection<Reminder>> GetTriggerableRemindersAsync(
        DateTimeOffset beforeTime,
        int count)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        return await GetDbSet(dbContextWrapper)
            .Where(entity => entity.NextTrigger <= beforeTime)
            .Take(count)
            .ToArrayAsync();
    }

    public async Task<PaginationData<ReminderId, Reminder>> PaginateAsync(
        SnowflakeId userId,
        uint pageIndex,
        uint pageSize)
    {
        await using var dbContextWrapper = GetDbContextWrapper();

        var query = GetDbSet(dbContextWrapper).Where(entity => entity.UserId == userId);
        var result = await query
            .OrderBy(entity => entity.Identifier)
            .Select(entity => new
            {
                Reminder = entity,
                TotalCount = query.LongCount()
            })
            .Skip((int)(pageIndex * pageSize))
            .Take((int)pageSize)
            .ToArrayAsync();

        return new PaginationData<ReminderId, Reminder>(
            pageIndex,
            pageSize,
            (ulong)(result.Length > 0 ? result[0].TotalCount : await query.LongCountAsync()),
            result.Select(item => item.Reminder).ToArray());
    }

    protected override DbSet<Reminder> GetDbSet(RemindersDbContext dbContext)
        => dbContext.Reminders;

    protected override Expression<Func<Reminder, bool>> GetEqualByIdExpression(ReminderId identifier)
        => entity => entity.Identifier == identifier;
}