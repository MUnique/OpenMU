// <copyright file="DiscordDeliveryRoute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Selects the same available destination for notifications and delivery tests.
/// </summary>
public static class DiscordDeliveryRoute
{
    /// <summary>
    /// Resolves a category to a connected bot channel, otherwise its webhook.
    /// </summary>
    /// <param name="settings">The current settings.</param>
    /// <param name="messenger">The bot messenger, when configured.</param>
    /// <param name="category">The notification category.</param>
    /// <returns>The available channel or webhook; both null when delivery is disabled or unconfigured.</returns>
    public static (ulong? ChannelId, string? WebhookUrl) Resolve(DiscordSettings settings, IDiscordMessenger? messenger, DiscordChannelCategory category)
    {
        if (settings.DisabledNotifications.Contains(category))
        {
            return (null, null);
        }

        if (settings.UseBot.GetValueOrDefault(true)
            && messenger is not IManageableServer { ServerState: not ServerState.Started }
            && messenger?.GetChannelId(category) is { } channelId)
        {
            return (channelId, null);
        }

        return settings.Webhooks.TryGetValue(category, out var url) && !string.IsNullOrWhiteSpace(url)
            ? (null, url)
            : (null, null);
    }
}
