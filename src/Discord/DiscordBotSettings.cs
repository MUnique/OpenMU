// <copyright file="DiscordBotSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// The settings of the Discord bot.
/// </summary>
public class DiscordBotSettings
{
    /// <summary>
    /// Gets or sets the token of the bot, from the Discord developer portal. Without token, the bot doesn't run.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the Discord server (guild) of the game server.
    /// The slash commands are registered for it, which makes them available immediately.
    /// Without it, they are registered globally, which can take up to an hour.
    /// The bot posts into the channels of the layout of this Discord server; without it, of the only Discord server which the bot is in.
    /// </summary>
    public ulong? GuildId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the channel in which the bot keeps a message with the status of the game servers up to date.
    /// Without it, the bot uses the status channel of the layout.
    /// </summary>
    public ulong? StatusChannelId { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of the channels per <see cref="DiscordChannelCategory"/>, into which the bot posts the game events.
    /// They take precedence over the channels of the layout.
    /// A category with a channel is posted by the bot instead of its webhook.
    /// </summary>
    public Dictionary<DiscordChannelCategory, ulong> Channels { get; set; } = new();

    /// <summary>
    /// Gets or sets the path of a JSON file with the layout of the Discord server, which the command <c>/openmu setup</c> sets up.
    /// Without it, the default layout is used.
    /// </summary>
    public string? LayoutFile { get; set; }

    /// <summary>
    /// Gets or sets the settings of the chat bridge between the game and Discord.
    /// </summary>
    public DiscordChatBridgeSettings ChatBridge { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the bot creates a scheduled event on the Discord server for the next castle siege.
    /// It requires the permission to create events.
    /// </summary>
    public bool CreateScheduledEvents { get; set; }

    /// <summary>
    /// Gets or sets the interval in which the status message and the presence of the bot are updated.
    /// </summary>
    public TimeSpan StatusUpdateInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets a value indicating whether the bot is enabled.
    /// </summary>
    public bool IsEnabled => !string.IsNullOrWhiteSpace(this.Token);
}
