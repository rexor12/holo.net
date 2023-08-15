using Discord.Interactions;
using Discord.WebSocket;

namespace Holo.Sdk.Interactions;

/// <summary>
/// An <see cref="SocketInteractionContext"/> with additional information
/// specific to the framework.
/// </summary>
public class ExtendedInteractionContext : SocketInteractionContext
{
    /// <summary>
    /// Gets information about the component.
    /// </summary>
    /// <remarks>If the interaction isn't a component interaction, this is <c>null</c>.</remarks>
    public ComponentInfo? ComponentInfo { get; init; }

    public ExtendedInteractionContext(
        DiscordSocketClient client,
        SocketInteraction interaction)
        : base(client, interaction)
    {
    }
}