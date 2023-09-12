using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Holo.ServiceHost.Configurations;

public sealed class ExpandJsonConfigurationSource : JsonConfigurationSource
{
    public ExpandJsonConfigurationSource(string path, bool optional, bool reloadOnChange)
    {
        Path = path;
        Optional = optional;
        ReloadOnChange = reloadOnChange;
    }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);

        return base.Build(builder);
    }
}
