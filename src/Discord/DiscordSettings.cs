// <copyright file="DiscordSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;

/// <summary>
/// The settings of the Discord integration.
/// </summary>
/// <remarks>
/// The webhook URLs and the bot token are secrets: everybody who knows them can post into the channels.
/// So they are part of the application configuration (e.g. environment variables), not of the game configuration in the database.
/// </remarks>
public class DiscordSettings
{
    /// <summary>
    /// The name of the configuration section.
    /// </summary>
    public const string SectionName = "Discord";

    /// <summary>
    /// Gets or sets the webhook URLs per <see cref="DiscordChannelCategory"/>.
    /// A category without URL isn't posted, unless the bot posts it, see <see cref="DiscordBotSettings.Channels"/>.
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
    /// Gets or sets the maximum number of messages per channel which wait to be sent.
    /// When there are more, the oldest ones are dropped.
    /// </summary>
    public int MaximumQueuedMessages { get; set; } = 100;

    /// <summary>
    /// Gets or sets the settings of the bot.
    /// </summary>
    public DiscordBotSettings Bot { get; set; } = new();

    /// <summary>
    /// Gets a value indicating whether game events are posted, through webhooks or the bot.
    /// </summary>
    public bool IsNotificationEnabled => this.Webhooks.Values.Any(url => !string.IsNullOrWhiteSpace(url))
                                         || (this.Bot.IsEnabled && this.Bot.Channels.Count > 0);

    /// <summary>
    /// Gets the culture of the <see cref="Language"/>; or the invariant culture, if the language is unknown.
    /// </summary>
    /// <returns>The culture.</returns>
    public CultureInfo GetCulture()
    {
        try
        {
            return CultureInfo.GetCultureInfo(this.Language);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}
