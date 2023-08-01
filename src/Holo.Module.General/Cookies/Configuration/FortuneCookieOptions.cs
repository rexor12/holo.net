namespace Holo.Module.General.Cookies.Configuration;

public sealed class FortuneCookieOptions
{
    public const string SectionName = "Extensions:General:FortuneCookieOptions";

    public required string EmbedThumbnailUrl { get; set; }
}