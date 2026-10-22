// <copyright file="DiscordChannelRouting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// Knows the channels into which the bot posts: the channels which are configured in the <see cref="DiscordBotSettings"/>,
/// and otherwise the channels of the <see cref="DiscordServerLayout"/>.
/// </summary>
public sealed class DiscordChannelRouting
{
    private readonly IReadOnlyDictionary<DiscordChannelCategory, ulong> _notificationChannels;

    private DiscordChannelRouting(IReadOnlyDictionary<DiscordChannelCategory, ulong> notificationChannels, ulong? statusChannelId, ulong? worldChatChannelId)
    {
        this._notificationChannels = notificationChannels;
        this.StatusChannelId = statusChannelId;
        this.WorldChatChannelId = worldChatChannelId;
    }

    /// <summary>
    /// Gets the identifier of the channel in which the bot shows the status of the game servers.
    /// </summary>
    public ulong? StatusChannelId { get; }

    /// <summary>
    /// Gets the identifier of the channel which is bound to the world chat of the game.
    /// </summary>
    public ulong? WorldChatChannelId { get; }

    /// <summary>
    /// Creates the routing.
    /// </summary>
    /// <param name="settings">The settings of the bot. Its channels take precedence over the ones of the layout.</param>
    /// <param name="layout">The layout.</param>
    /// <param name="layoutChannelIds">The identifiers of the existing channels of the layout, by their key.</param>
    /// <returns>The routing.</returns>
    public static DiscordChannelRouting Create(DiscordBotSettings settings, DiscordServerLayout layout, IReadOnlyDictionary<string, ulong> layoutChannelIds)
    {
        var notificationChannels = new Dictionary<DiscordChannelCategory, ulong>();
        ulong? statusChannelId = null;
        ulong? worldChatChannelId = null;
        foreach (var channel in layout.Channels)
        {
            if (!layoutChannelIds.TryGetValue(channel.Key, out var channelId))
            {
                continue;
            }

            foreach (var category in channel.Notifications)
            {
                notificationChannels.TryAdd(category, channelId);
            }

            if (channel.ShowsStatus)
            {
                statusChannelId ??= channelId;
            }

            if (channel.IsWorldChat)
            {
                worldChatChannelId ??= channelId;
            }
        }

        foreach (var (category, channelId) in settings.Channels)
        {
            notificationChannels[category] = channelId;
        }

        return new DiscordChannelRouting(notificationChannels, settings.StatusChannelId ?? statusChannelId, worldChatChannelId);
    }

    /// <summary>
    /// Gets the identifier of the channel into which the notifications of the category are posted.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The identifier of the channel; or <c>null</c>, if there is none.</returns>
    public ulong? GetChannelId(DiscordChannelCategory category)
    {
        return this._notificationChannels.TryGetValue(category, out var channelId) ? channelId : null;
    }
}
