namespace Holo.ServiceHost.Storage;

public sealed class DatabaseOptions
{
    public const string SectionName = "DatabaseOptions";

    public required string ConnectionString { get; set; }
}