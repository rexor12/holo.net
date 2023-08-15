namespace Holo.Sdk.Interactions;

/// <summary>
/// Holds information about a component interaction.
/// </summary>
public sealed class ComponentInfo
{
    /// <summary>
    /// The value that can be used in place of the bound user identifier
    /// when the component isn't bound to a user.
    /// </summary>
    public static readonly ulong UnboundUserId = 0;

    /// <summary>
    /// Gets the identifier of the component.
    /// </summary>
    /// <remarks>This isn't the full custom ID, it doesn't contain the arguments.</remarks>
    public string ComponentId { get; }

    /// <summary>
    /// Gest the identifier of the user the component is bound to.
    /// </summary>
    /// <remarks><see cref="UnboundUserId"/> is used if the component isn't bound to a user.</remarks>
    public ulong BoundUserId { get; }

    /// <summary>
    /// Gets the unprocessed arguments of the component interaction.
    /// </summary>
    public string CustomData { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ComponentInfo"/>.
    /// </summary>
    /// <param name="componentId">The identifier of the component.</param>
    /// <param name="boundUserId">The identifier of the user the component is bound to.</param>
    /// <param name="customData">The unprocessed arguments of the interaction.</param>
    public ComponentInfo(
        string componentId,
        ulong boundUserId,
        string customData)
    {
        ComponentId = componentId;
        BoundUserId = boundUserId;
        CustomData = customData;
    }
}
