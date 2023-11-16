using System;
using System.ComponentModel.DataAnnotations.Schema;
using Holo.Module.Reminders.Enums;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Ids;

namespace Holo.Module.Reminders.Models;

public sealed class Reminder : AggregateRoot<ReminderId>
{
    public const string TableName = "reminders";

    [Column("user_id")]
    public SnowflakeId UserId { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("is_repeating")]
    public bool IsRepeating { get; set; }

    [Column("frequency_time")]
    public TimeSpan? FrequencyTime { get; set; }

    [Column("day_of_week")]
    public DayOfWeek DayOfWeek { get; set; }

    [Column("until_date")]
    public DateOnly? UntilDate { get; set; }

    [Column("base_trigger")]
    public DateTimeOffset BaseTrigger { get; set; }

    [Column("last_trigger")]
    public DateTimeOffset LastTrigger { get; set; }

    [Column("next_trigger")]
    public DateTimeOffset NextTrigger { get; set; }

    [Column("location")]
    public ReminderLocation Location { get; set; }

    [Column("server_id")]
    public SnowflakeId? ServerId { get; set; }

    [Column("channel_id")]
    public SnowflakeId? ChannelId { get; set; }

    public bool IsExpired()
        => UntilDate.HasValue && DateOnly.FromDateTime(NextTrigger.Date) >= UntilDate.Value;

    public void UpdateNextTrigger(DateTimeOffset currentTime)
    {
        if (!IsRepeating || !FrequencyTime.HasValue)
            throw new InvalidOperationException(
                $"Non-recurring reminder '{Identifier}' cannot have a new trigger date-time.");

        var repeatCount = (int)((currentTime - BaseTrigger) / FrequencyTime.Value);
        NextTrigger = BaseTrigger + (repeatCount + 1) * FrequencyTime.Value;
    }
}