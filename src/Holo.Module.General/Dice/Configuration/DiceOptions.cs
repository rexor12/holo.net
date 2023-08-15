namespace Holo.Module.General.Dice.Configuration;

public sealed class DiceOptions
{
    public const string SectionName = "Extensions:General:DiceOptions";

    public required string EmbedThumbnailUrl { get; set; }
}