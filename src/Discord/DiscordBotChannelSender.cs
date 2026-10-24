// <copyright file="DiscordBotChannelSender.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Threading;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sends <see cref="DiscordEmbed"/>s to a channel through the bot, in the background.
/// </summary>
/// <remarks>
/// The rate limits are handled by the Discord library of the bot.
/// </remarks>
public sealed class DiscordBotChannelSender : DiscordMessageQueue
{
    private readonly ulong _channelId;
    private readonly IDiscordMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordBotChannelSender"/> class.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="messenger">The messenger of the bot.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="capacity">The maximum number of queued embeds.</param>
    public DiscordBotChannelSender(ulong channelId, IDiscordMessenger messenger, ILogger logger, int capacity)
        : base(logger, capacity)
    {
        this._channelId = channelId;
        this._messenger = messenger;
    }

    /// <inheritdoc />
    protected override Task SendAsync(IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken)
    {
        return this._messenger.SendAsync(this._channelId, embeds, cancellationToken);
    }
}
