// <copyright file="DiscordGuildChatBindingMode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// Defines where the chats of guilds can be bound to.
/// </summary>
public enum DiscordGuildChatBindingMode
{
    /// <summary>
    /// The chats can be bound to channels on the Discord server of the game server, and to channels on Discord servers of the guilds.
    /// </summary>
    Both,

    /// <summary>
    /// The chats can only be bound to channels on the Discord server of the game server, which the bot creates for the guilds.
    /// </summary>
    HostedOnly,

    /// <summary>
    /// The chats can only be bound to channels on Discord servers of the guilds.
    /// </summary>
    GuildOwnedOnly,
}
