// <copyright file="DiscordWebhookSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The settings of the Discord webhook notifications.
/// </summary>
/// <remarks>
/// The webhook URLs are secrets: everybody who knows them can post into the channel.
/// So they are part of the application configuration (e.g. environment variables), not of the game configuration in the database.
/// </remarks>
public class DiscordWebhookSettings
{
    /// <summary>
    /// The name of the configuration section.
    /// </summary>
    public const string SectionName = "Discord";

    /// <summary>
    /// Gets or sets the webhook URLs per <see cref="DiscordChannelCategory"/>.
    /// A category without URL isn't posted.
    /// </summary>
    public Dictionary<DiscordChannelCategory, string> Webhooks { get; set; } = new();

    /// <summary>
    /// Gets or sets the language of the messages, e.g. <c>en</c> or <c>de</c>.
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Gets or sets the identifiers of the game servers which announce events like mini games and invasions.
    /// When several game servers run the same events, this avoids duplicate announcements.
    /// If it's empty, all game servers announce them.
    /// </summary>
    public List<byte> EventAnnouncingServerIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the maximum number of messages per webhook which wait to be sent.
    /// When there are more, the oldest ones are dropped.
    /// </summary>
    public int MaximumQueuedMessages { get; set; } = 100;

    /// <summary>
    /// Gets a value indicating whether at least one webhook is configured.
    /// </summary>
    public bool IsEnabled => this.Webhooks.Values.Any(url => !string.IsNullOrWhiteSpace(url));
}
