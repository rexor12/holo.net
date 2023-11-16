namespace Holo.Module.Reminders.Configuration;

public sealed class ReminderOptions
{
    public const string SectionName = "Extensions:Reminders:ReminderOptions";

    /// <summary>
    /// Gets or sets whether reminders are enabled.
    /// </summary>
    public bool IsRemindersEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of reminders a user can have at a time.
    /// </summary>
    public int RemindersPerUserMax { get; set; } = 5;

    /// <summary>
    /// Gets or sets the minimum allowed length for reminder messages.
    /// </summary>
    /// <remarks>A value of "0" means messages aren't mandatory.</remarks>
    public int MessageLengthMin { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum allowed length for reminder messages.
    /// </summary>
    public int MessageLengthMax { get; set; } = 120;

    /// <summary>
    /// Gets or sets the amount of time, in seconds, after which
    /// a reminder classifies as a belated reminder.
    /// </summary>
    public int BelatedReminderAfterSeconds { get; set; } = 300;

    /// <summary>
    /// Gets or sets the URL of the image to be displayed in the embeds.
    /// </summary>
    public required string EmbedThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed due time for one-time reminders, in minutes.
    /// </summary>
    /// <remarks>The default value is 1 year, in minutes.</remarks>
    public required int MaxReminderDueTimeInMinutes { get; set; } = 525960;

    /// <summary>
    /// Gets or sets the maximum allowed interval for recurring reminders, in minutes.
    /// </summary>
    /// <remarks>The default value is 1 year, in minutes.</remarks>
    public required int MaxReminderIntervalInMinutes { get; set; } = 525960;
}