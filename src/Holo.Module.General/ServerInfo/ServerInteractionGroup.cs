using Discord.Interactions;
using Holo.Sdk.Interactions;
using Holo.Sdk.Localization;
using Microsoft.Extensions.Logging;

namespace Holo.Module.General.ServerInfo;

[Group("server", "Command related to the server.")]
public partial class ServerInteractionGroup : InteractionGroupBase
{
    public ServerInteractionGroup(
        ILocalizationService localizationService,
        ILogger<ServerInteractionGroup> logger)
        : base(localizationService, logger)
    {
    }
}