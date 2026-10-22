// <copyright file="DiscordChannelLayout.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// A text channel of the <see cref="DiscordServerLayout"/>.
/// </summary>
public sealed class DiscordChannelLayout
{
    /// <summary>
    /// Gets or sets the key, which stays the same when the name changes.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name, e.g. <c>events</c>. An existing channel with this name is adopted.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the topic.
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the channel is an announcement channel, which other Discord servers can follow.
    /// It requires a community server; otherwise, a normal text channel is created.
    /// </summary>
    public bool IsAnnouncement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only the bot can write into the channel.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the categories of notifications which are posted into the channel.
    /// </summary>
    public List<DiscordChannelCategory> Notifications { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the bot shows the status of the game servers in the channel.
    /// </summary>
    public bool ShowsStatus { get; set; }
}
