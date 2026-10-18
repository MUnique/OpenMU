// <copyright file="CrywolfArmyStage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The stage of the army of Balgass during the crywolf event.
/// </summary>
public enum CrywolfArmyStage
{
    /// <summary>
    /// The army waits for the battle and doesn't move or attack.
    /// </summary>
    Waiting,

    /// <summary>
    /// The army advances towards the fortress.
    /// </summary>
    Advancing,

    /// <summary>
    /// The army attacks the statue of the holy wolf.
    /// </summary>
    AttackingStatue,
}
