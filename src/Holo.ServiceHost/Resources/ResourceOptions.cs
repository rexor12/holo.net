namespace Holo.ServiceHost.Resources;

public sealed class ResourceOptions
{
    public const string SectionName = "ResourceOptions";

    public required string LocalizationGlobPattern { get; set; }

    public required string LocalizationFileNamePattern { get; set; }

    public required string DefaultCultureCode { get; set; }
}