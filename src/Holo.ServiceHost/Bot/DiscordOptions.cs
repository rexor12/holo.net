namespace Holo.ServiceHost.Bot;

public sealed class DiscordOptions
{
    public const string SectionName = "DiscordOptions";

    public required string BotToken { get; set; }
    public ulong DevelopmentServerId { get; set; }
}