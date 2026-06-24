// <copyright file="DuelStartResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// The result of the duel request.
/// </summary>
public enum DuelStartResult
{
    /// <summary>
    /// Undefined result.
    /// </summary>
    Undefined,

    /// <summary>
    /// The duel was accepted.
    /// </summary>
    Success,

    /// <summary>
    /// The duel was refused.
    /// </summary>
    Refused,

    /// <summary>
    /// Failed by an error.
    /// </summary>
    FailedByError,

    /// <summary>
    /// Failed because the player level is too low.
    /// </summary>
    FailedByTooLowLevel,

    /// <summary>
    /// Failed because there is no free room.
    /// </summary>
    FailedByNoFreeRoom,

    /// <summary>
    /// Failed because the player does not have enough money.
    /// </summary>
    FailedByNotEnoughMoney,
}