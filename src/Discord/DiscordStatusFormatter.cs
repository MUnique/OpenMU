// <copyright file="DiscordStatusFormatter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using MUnique.OpenMU.Discord.Properties;

/// <summary>
/// Formats the status of the game servers, for the status message and the presence of the bot.
/// </summary>
public sealed class DiscordStatusFormatter
{
    /// <summary>
    /// The color of the status message, when at least one game server is online (green).
    /// </summary>
    internal const int OnlineColor = 0x2ECC71;

    /// <summary>
    /// The color of the status message, when no game server is online (red).
    /// </summary>
    internal const int OfflineColor = 0xE74C3C;

    private readonly CultureInfo _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordStatusFormatter"/> class.
    /// </summary>
    /// <param name="culture">The culture of the texts.</param>
    public DiscordStatusFormatter(CultureInfo culture)
    {
        this._culture = culture;
    }

    /// <summary>
    /// Creates the status message.
    /// </summary>
    /// <param name="servers">The status of the game servers.</param>
    /// <param name="utcNow">The current time, which is shown as time of the last update.</param>
    /// <returns>The status message.</returns>
    public DiscordEmbed CreateStatus(IReadOnlyList<GameServerStatus> servers, DateTime utcNow)
    {
        var description = servers.Count == 0
            ? this.Text(nameof(Resources.Status_NoServers))
            : string.Join('\n', servers.Select(server => server.IsOnline
                ? this.Text(nameof(Resources.Status_Online), DiscordMessageFormatter.Escape(server.Name), server.CurrentPlayers, server.MaximumPlayers)
                : this.Text(nameof(Resources.Status_Offline), DiscordMessageFormatter.Escape(server.Name))));
        var color = servers.Any(server => server.IsOnline) ? OnlineColor : OfflineColor;
        return new DiscordEmbed(this.Text(nameof(Resources.Status_Title)), description, color, utcNow, null);
    }

    /// <summary>
    /// Creates the text of the presence of the bot.
    /// </summary>
    /// <param name="servers">The status of the game servers.</param>
    /// <returns>The text.</returns>
    public string CreatePresence(IReadOnlyList<GameServerStatus> servers)
    {
        return this.Text(nameof(Resources.Presence), servers.Where(server => server.IsOnline).Sum(server => server.CurrentPlayers));
    }

    private string Text(string resourceKey, params object[] args)
    {
        var text = Resources.ResourceManager.GetString(resourceKey, this._culture) ?? resourceKey;
        return string.Format(this._culture, text, args);
    }
}
