// <copyright file="GlobalNoticeEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A game master sent a notice to all players of a game server.
/// </summary>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="SenderName">The character name of the game master.</param>
/// <param name="Message">The message.</param>
public sealed record GlobalNoticeEvent(byte ServerId, DateTime TimestampUtc, string SenderName, string Message)
    : GameEvent(ServerId, TimestampUtc);
