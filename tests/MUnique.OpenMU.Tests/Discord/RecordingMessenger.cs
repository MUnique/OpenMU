// <copyright file="RecordingMessenger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Threading;
using MUnique.OpenMU.Discord;

/// <summary>
/// A <see cref="IDiscordMessenger"/> which records the sent messages.
/// </summary>
internal sealed class RecordingMessenger : IDiscordMessenger
{
    /// <summary>
    /// Gets the sent messages.
    /// </summary>
    public List<(ulong ChannelId, IReadOnlyList<DiscordEmbed> Embeds)> Messages { get; } = new();

    /// <summary>
    /// Gets the channels per category.
    /// </summary>
    public Dictionary<DiscordChannelCategory, ulong> Channels { get; } = new();

    /// <inheritdoc />
    public ulong? GetChannelId(DiscordChannelCategory category) => this.Channels.TryGetValue(category, out var channelId) ? channelId : null;

    /// <inheritdoc />
    public Task SendAsync(ulong channelId, IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken)
    {
        lock (this.Messages)
        {
            this.Messages.Add((channelId, embeds));
        }

        return Task.CompletedTask;
    }
}
