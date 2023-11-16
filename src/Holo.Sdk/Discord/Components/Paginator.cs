using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Holo.Sdk.Interactions;

namespace Holo.Sdk.Discord.Components;

/// <summary>
/// A wrapper component for building a pagination action row.
/// </summary>
public class Paginator
{
    /// <summary>
    /// Gets or sets the index of the current page, starting from 0.
    /// </summary>
    public uint CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items displayed on the page.
    /// </summary>
    public uint PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of available items.
    /// </summary>
    public ulong TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a list of custom data to be encoded in the paginator.
    /// </summary>
    public IEnumerable<string>? CustomData { get; set; }

    /// <summary>
    /// Determines whether the current page is the first page.
    /// </summary>
    public bool IsFirstPage
        => CurrentPage == 0;

    /// <summary>
    /// Determines whether the current page is the last page.
    /// </summary>
    public bool IsLastPage
    {
        get
        {
            var lastPageIndex = Math.Ceiling((double)TotalCount / PageSize) - 1;

            return CurrentPage >= lastPageIndex;
        }
    }

    /// <summary>
    /// Determines whether the current page is the only available page.
    /// </summary>
    public bool IsOnlyPage
        => IsFirstPage && IsLastPage;

    public ComponentBuilder ToComponentBuilder(
        string pagingId,
        Emote? previousButtonEmoji = null,
        string? previousButtonLabel = null,
        Emote? nextButtonEmoji = null,
        string? nextButtonLabel = null,
        ulong? boundUserId = null)
    {
        var previousButtonCustomData = CustomData?.Prepend<object>(CurrentPage - 1).ToArray() ?? new object[] { CurrentPage - 1 };
        var nextButtonCustomData = CustomData?.Prepend<object>(CurrentPage + 1).ToArray() ?? new object[] { CurrentPage + 1 };
        var boundUserIdValue = boundUserId ?? 0;

        return new ComponentBuilder()
            .WithButton(
                customId: ComponentHelper.CreateCustomId(pagingId, boundUserIdValue, previousButtonCustomData),
                label: previousButtonLabel,
                emote: previousButtonEmoji,
                style: ButtonStyle.Secondary,
                disabled: IsFirstPage)
            .WithButton(
                customId: ComponentHelper.CreateCustomId(pagingId, boundUserIdValue, nextButtonCustomData),
                label: nextButtonLabel,
                emote: nextButtonEmoji,
                style: ButtonStyle.Secondary,
                disabled: IsLastPage);
    }
}