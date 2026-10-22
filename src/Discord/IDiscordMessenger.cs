// <copyright file="IDiscordMessenger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Threading;

/// <summary>
/// Sends messages to Discord channels, e.g. through the bot.
/// </summary>
public interface IDiscordMessenger
{
    /// <summary>
    /// Gets the identifier of the channel into which the notifications of the category are posted.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The identifier of the channel; or <c>null</c>, if the messenger doesn't post the category.</returns>
    ulong? GetChannelId(DiscordChannelCategory category);

    /// <summary>
    /// Sends a message with the embeds to the channel. It waits until the messenger is connected.
    /// Mentions in the message don't ping anybody.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="embeds">The embeds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task SendAsync(ulong channelId, IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken);
}
