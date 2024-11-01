// <copyright file="DuelStartResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// The result of the duel request.
/// </summary>
public enum DuelStartResult
{
    Undefined,

    Success,

    Refused,

    FailedByError,

    FailedByTooLowLevel,

    FailedByNoFreeRoom,

    FailedByNotEnoughMoney,
}