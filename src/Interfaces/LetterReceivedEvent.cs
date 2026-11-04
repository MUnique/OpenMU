// <copyright file="LetterReceivedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A character received a letter.
/// </summary>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="TimestampUtc">The timestamp, in UTC.</param>
/// <param name="ReceiverName">The name of the character which received the letter.</param>
/// <param name="SenderName">The name of the character which sent the letter.</param>
/// <param name="Subject">The subject of the letter.</param>
public sealed record LetterReceivedEvent(byte ServerId, DateTime TimestampUtc, string ReceiverName, string SenderName, string Subject)
    : GameEvent(ServerId, TimestampUtc);
