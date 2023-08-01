namespace Holo.ServiceHost.Modules;

public sealed class ModuleOptions
{
    public const string SectionName = "ModuleOptions";

    public required string[] ModuleAssemblyGlobPatterns { get; set; }

    public required string ModuleAssemblyNamePattern { get; set; }
}