// <copyright file="ImperialGuardianEnterResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// The result of a request to enter the imperial guardian event.
/// </summary>
public enum ImperialGuardianEnterResult
{
    /// <summary>
    /// The player entered the event.
    /// </summary>
    Success,

    /// <summary>
    /// The event can not be entered yet.
    /// </summary>
    NotOpen,

    /// <summary>
    /// The player has no Gaion's Order, or no Complete Secromicon on sunday.
    /// </summary>
    MissingTicket,

    /// <summary>
    /// The event is full.
    /// </summary>
    Full,

    /// <summary>
    /// There is still time remaining in the zone.
    /// </summary>
    ZoneTimeRemaining,

    /// <summary>
    /// The player can only enter as a member of a party.
    /// </summary>
    PartyRequired,

    /// <summary>
    /// The character level is too low.
    /// </summary>
    CharacterLevelTooLow,
}
