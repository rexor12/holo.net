using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Holo.Sdk.Discord;

/// <summary>
/// Extension methods for <see cref="SocketInteractionContext"/>.
/// </summary>
public static class SocketInteractionContextExtensions
{
    /// <summary>
    /// Gets the user relevant to the specified context.
    /// </summary>
    /// <param name="context">The <see cref="SocketInteractionContext"/> of the invocation.</param>
    /// <param name="user">The <see cref="IUser"/> to look for.</param>
    /// <returns>The <see cref="IUser"/> or <see cref="IGuildUser"/> relevant to the context.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// If the command was invoked in a guild and the given user is a member of that guild,
    /// then the same user is returned as an <see cref="IGuildUser"/>.
    /// </item>
    /// <item>
    /// If the command was invoked in a DM channel and the given user is a member of that channel,
    /// then the same user is returned.
    /// </item>
    /// <item>
    /// In any other scenario, <c>null</c> is returned.
    /// </item>
    /// </list>
    /// </remarks>
    public static Task<IUser?> GetRelevantUserAsync(
        this SocketInteractionContext context,
        IUser? user)
        => InternalGetRelevantUserAsync(context, user, null);

    /// <summary>
    /// Gets the user relevant to the specified context.
    /// </summary>
    /// <param name="context">The <see cref="SocketInteractionContext"/> of the invocation.</param>
    /// <param name="user">The <see cref="IUser"/> to look for.</param>
    /// <param name="fallbackUser">The <see cref="IUser"/> to fallback to.</param>
    /// <returns>The <see cref="IUser"/> or <see cref="IGuildUser"/> relevant to the context.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// If the command was invoked in a guild and the given user is a member of that guild,
    /// then the same user is returned as an <see cref="IGuildUser"/>.
    /// </item>
    /// <item>
    /// If the command was invoked in a DM channel and the given user is a member of that channel,
    /// then the same user is returned.
    /// </item>
    /// <item>
    /// In any other scenario, the specified fallback user is returned.
    /// </item>
    /// </list>
    /// </remarks>
    public static async Task<IUser> GetRelevantUserAsync(
        this SocketInteractionContext context,
        IUser? user,
        IUser fallbackUser)
    {
        var result = await InternalGetRelevantUserAsync(context, user, fallbackUser);

        // The internal method must ensure that when it receives
        // a non-null fallback user, the same user is returned.
        Debug.Assert(result != null);

        return result!;
    }

    /// <summary>
    /// Gets the user relevant to the specified context.
    /// </summary>
    /// <param name="context">The <see cref="SocketInteractionContext"/> of the invocation.</param>
    /// <param name="userId">The identifier of the user to look for.</param>
    /// <returns>The <see cref="IUser"/> or <see cref="IGuildUser"/> relevant to the context.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// If the command was invoked in a guild and the given user is a member of that guild,
    /// then the guild member is returned as an <see cref="IGuildUser"/>.
    /// </item>
    /// <item>
    /// If the command was invoked in a DM channel and the given user is a member of that channel,
    /// then the channel member is returned as an <see cref="IUser"/>.
    /// </item>
    /// <item>
    /// In any other scenario, the <c>null</c> is returned.
    /// </item>
    /// </list>
    /// </remarks>
    public static Task<IUser?> GetRelevantUserAsync(
        this SocketInteractionContext context,
        ulong userId)
        => InternalGetRelevantUserAsync(context, userId, null);

    /// <summary>
    /// Gets the user relevant to the specified context.
    /// </summary>
    /// <param name="context">The <see cref="SocketInteractionContext"/> of the invocation.</param>
    /// <param name="userId">The identifier of the user to look for.</param>
    /// <param name="fallbackUser">The <see cref="IUser"/> to fallback to.</param>
    /// <returns>The <see cref="IUser"/> or <see cref="IGuildUser"/> relevant to the context.</returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// If the command was invoked in a guild and the given user is a member of that guild,
    /// then the guild member is returned as an <see cref="IGuildUser"/>.
    /// </item>
    /// <item>
    /// If the command was invoked in a DM channel and the given user is a member of that channel,
    /// then the channel member is returned as an <see cref="IUser"/>.
    /// </item>
    /// <item>
    /// In any other scenario, the specified fallback user is returned.
    /// </item>
    /// </list>
    /// </remarks>
    public static async Task<IUser> GetRelevantUserAsync(
        this SocketInteractionContext context,
        ulong userId,
        IUser fallbackUser)
    {
        var result = await InternalGetRelevantUserAsync(context, userId, fallbackUser);

        // The internal method must ensure that when it receives
        // a non-null fallback user, the same user is returned.
        Debug.Assert(result != null);

        return result!;
    }

    private static async Task<IUser?> InternalGetRelevantUserAsync(
        this SocketInteractionContext context,
        ulong userId,
        IUser? fallbackUser)
    {
        if (userId == context.User.Id)
            return context.User;

        if (context.Interaction.IsDMInteraction)
            return fallbackUser;

        if (context.Guild != null)
            return await ((IGuild)context.Guild).GetUserAsync(userId) ?? fallbackUser;

        return await context.Client.GetUserAsync(userId) ?? fallbackUser;
    }

    private static async Task<IUser?> InternalGetRelevantUserAsync(
        SocketInteractionContext context,
        IUser? user,
        IUser? fallbackUser)
    {
        if (user == null)
            return fallbackUser;

        if (user.Id == context.User.Id)
            return user;

        if (context.Interaction.IsDMInteraction)
            return fallbackUser;

        if (context.User is IGuildUser localGuildUser && user is IGuildUser guildUser)
            return guildUser.GuildId == localGuildUser.GuildId ? user : fallbackUser;

        if (context.Guild != null)
            return await ((IGuild)context.Guild).GetUserAsync(user.Id) ?? fallbackUser;

        return fallbackUser;
    }
}