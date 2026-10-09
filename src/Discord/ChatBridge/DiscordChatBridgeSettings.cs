// <copyright file="DiscordChatBridgeSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// The settings of the chat bridge between the game and Discord.
/// </summary>
/// <remarks>
/// The game servers only publish the chat messages, if it's enabled in the configuration of the game event publisher plugin.
/// </remarks>
public class DiscordChatBridgeSettings
{
    /// <summary>
    /// Gets or sets where the chats of guilds can be bound to.
    /// </summary>
    public DiscordGuildChatBindingMode BindingMode { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of the Discord servers of guilds, on which chats can be bound.
    /// If it's empty, all Discord servers are allowed.
    /// </summary>
    public List<ulong> AllowedDiscordServerIds { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the bot reads the messages in the bound channels.
    /// It requires the privileged <i>Message Content</i> intent, which has to be enabled for the bot in the Discord developer portal.
    /// Without it, the messages are sent with the command <c>/say</c>.
    /// </summary>
    public bool ReadMessages { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of a message from Discord to the game. Longer messages are cut.
    /// </summary>
    public int MaximumMessageLength { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum number of messages per minute, which a Discord user can send to the game.
    /// </summary>
    public int MaximumMessagesPerMinute { get; set; } = 10;
}
