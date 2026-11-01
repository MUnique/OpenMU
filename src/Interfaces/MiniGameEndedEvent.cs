// <copyright file="MiniGameEndedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A mini game ended.
/// </summary>
/// <remarks>
/// Names of configuration objects, like maps or monsters, are passed as the full value of a <see cref="LocalizedString"/>,
/// so that a receiver can translate them with <c>new LocalizedString(name).GetTranslation(culture)</c>.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="MiniGameType">The name of the type of the mini game, e.g. <c>BloodCastle</c>.</param>
/// <param name="MiniGameName">The name of the mini game.</param>
/// <param name="GameLevel">The level of the mini game, e.g. 1 for Blood Castle 1.</param>
/// <param name="WinnerName">The character name of the winner, if the game has one.</param>
/// <param name="FinisherNames">The character names of the players which were still in the game when it ended.</param>
public sealed record MiniGameEndedEvent(byte ServerId, DateTime TimestampUtc, string MiniGameType, string MiniGameName, byte GameLevel, string? WinnerName, IReadOnlyList<string> FinisherNames)
    : GameEvent(ServerId, TimestampUtc);
