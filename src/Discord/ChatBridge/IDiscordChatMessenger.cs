// <copyright file="IDiscordChatMessenger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Threading;

/// <summary>
/// Sends chat messages as text to Discord channels.
/// </summary>
public interface IDiscordChatMessenger
{
    /// <summary>
    /// Sends a text to the channel. Mentions in the text don't ping anybody.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="text">The text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task SendTextAsync(ulong channelId, string text, CancellationToken cancellationToken);
}
