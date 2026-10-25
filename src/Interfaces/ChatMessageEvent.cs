// <copyright file="ChatMessageEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A player sent a message to the guild, alliance or world chat.
/// </summary>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="TimestampUtc">The timestamp, in UTC.</param>
/// <param name="Channel">The chat.</param>
/// <param name="GuildId">The (non-persistent) identifier of the guild of the sender, for the guild and alliance chat.</param>
/// <param name="Sender">The name of the character which sent the message.</param>
/// <param name="Message">The message, without the prefix of the chat.</param>
public sealed record ChatMessageEvent(byte ServerId, DateTime TimestampUtc, GameChatChannel Channel, uint GuildId, string Sender, string Message)
    : GameEvent(ServerId, TimestampUtc);
