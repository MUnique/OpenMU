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
    /// Sends a message with the embeds to the channel. It waits until the messenger is connected.
    /// Mentions in the message don't ping anybody.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="embeds">The embeds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task SendAsync(ulong channelId, IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken);
}
