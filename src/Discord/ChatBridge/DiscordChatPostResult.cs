// <copyright file="DiscordChatPostResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// The result of sending a message from Discord to the game.
/// </summary>
public enum DiscordChatPostResult
{
    /// <summary>
    /// The message was sent.
    /// </summary>
    Sent,

    /// <summary>
    /// The channel isn't bound to a chat of the game.
    /// </summary>
    NotBridged,

    /// <summary>
    /// The Discord user isn't linked to a game account.
    /// </summary>
    NotLinked,

    /// <summary>
    /// The Discord user has no character of its account selected.
    /// </summary>
    NoCharacter,

    /// <summary>
    /// The character isn't a member of the guild or alliance of the channel.
    /// </summary>
    NotMember,

    /// <summary>
    /// The chat of the account is banned.
    /// </summary>
    ChatBanned,

    /// <summary>
    /// The user sent too many messages in a short time.
    /// </summary>
    TooFast,

    /// <summary>
    /// The message is empty, e.g. after removing what the game can't show.
    /// </summary>
    Empty,
}
