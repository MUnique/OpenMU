// <copyright file="EnterResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Defines the result of an enter request.
/// </summary>
public enum EnterResult
{
    /// <summary>
    /// The mini game has been entered.
    /// </summary>
    Success,

    /// <summary>
    /// Entering the mini game failed, e.g. by missing ticket or level range.
    /// </summary>
    Failed,

    /// <summary>
    /// Entering the mini game failed, because it's not opened.
    /// </summary>
    NotOpen,

    /// <summary>
    /// Entering the mini game failed, because the character level is too high for the requested game level.
    /// </summary>
    CharacterLevelTooHigh,

    /// <summary>
    /// Entering the mini game failed, because the character level is too low for the requested game level.
    /// </summary>
    CharacterLevelTooLow,

    /// <summary>
    /// Entering the mini game failed, because it's full.
    /// </summary>
    Full,

    /// <summary>
    /// Entering the event failed, because the player doesn't have enough money for the entrance fee.
    /// </summary>
    NotEnoughMoney,

    /// <summary>
    /// Entering the event failed, because the player has a pk state.
    /// </summary>
    PlayerKillerCantEnter,
}