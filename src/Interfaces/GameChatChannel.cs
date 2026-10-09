// <copyright file="GameChatChannel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The chats of the game which reach more than the players nearby.
/// </summary>
public enum GameChatChannel
{
    /// <summary>
    /// The chat of a guild.
    /// </summary>
    Guild,

    /// <summary>
    /// The chat of an alliance of guilds.
    /// </summary>
    Alliance,

    /// <summary>
    /// The world chat, which all players of all game servers can read.
    /// </summary>
    World,
}
