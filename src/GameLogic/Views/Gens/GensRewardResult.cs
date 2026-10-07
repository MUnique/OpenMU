// <copyright file="GensRewardResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// The result of a request of the gens ranking reward.
/// </summary>
public enum GensRewardResult
{
    /// <summary>
    /// The player got its reward.
    /// </summary>
    Success,

    /// <summary>
    /// The rewards are only given in the reward period of a month.
    /// </summary>
    OutsideRewardPeriod,

    /// <summary>
    /// The player is not eligible for a reward.
    /// </summary>
    NotEligible,

    /// <summary>
    /// The inventory of the player has not enough space for the reward.
    /// </summary>
    InventoryFull,

    /// <summary>
    /// The player already got its reward in this month.
    /// </summary>
    AlreadyClaimed,

    /// <summary>
    /// The player is member of a different gens than the one of the npc.
    /// </summary>
    DifferentGensNpc,

    /// <summary>
    /// The player is not member of a gens.
    /// </summary>
    NotJoined,
}
