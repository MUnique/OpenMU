// <copyright file="DiscordIntegrationConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// The configuration of the Discord integration, which the game client gets with the Discord integration info.
/// It holds no secrets: every player can read these values.
/// </summary>
public class DiscordIntegrationConfiguration
{
    /// <summary>
    /// Gets or sets the invite link to the Discord server of the game server, e.g. <c>https://discord.gg/abc123</c>.
    /// The client opens it with its Discord button. Empty for none.
    /// </summary>
    public string InviteUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id of the Discord application which the client uses for the Rich Presence.
    /// Usually it's the application of the bot. Empty for none, then the client uses its own configuration.
    /// </summary>
    public string RichPresenceApplicationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key of the large image of the Rich Presence, as uploaded to the Discord application.
    /// </summary>
    public string RichPresenceLargeImageKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key of the small image of the Rich Presence, as uploaded to the Discord application.
    /// </summary>
    public string RichPresenceSmallImageKey { get; set; } = string.Empty;
}
