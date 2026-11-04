// <copyright file="ImperialGuardianTimerType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// The type of the timer of the imperial guardian event.
/// </summary>
public enum ImperialGuardianTimerType
{
    /// <summary>
    /// The time to collect the loot.
    /// </summary>
    LootTime,

    /// <summary>
    /// The time until the monsters appear.
    /// </summary>
    Standby,

    /// <summary>
    /// The time to kill the monsters.
    /// </summary>
    TimeAttack,
}
