using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Holo.Module.Reminders.Models;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Ids;
using Holo.Sdk.Storage.Repositories;

namespace Holo.Module.Reminders.Storage.Repositories;

public interface IReminderRepository : IRepository<ReminderId, Reminder, RemindersDbContext>
{
    Task<int> GetCountByUserAsync(SnowflakeId userId);

    Task<PaginationData<ReminderId, Reminder>> PaginateAsync(SnowflakeId userId, uint pageIndex, uint pageSize);

    Task<IReadOnlyCollection<Reminder>> GetTriggerableRemindersAsync(DateTimeOffset beforeTime, int count);

    Task<bool> RemoveByUserAsync(SnowflakeId userId, ReminderId reminderId);
}