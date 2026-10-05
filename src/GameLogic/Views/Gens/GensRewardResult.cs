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
    /// The player is not eligible for a reward.
    /// </summary>
    NotEligible,

    /// <summary>
    /// The player is member of a different gens than the one of the npc.
    /// </summary>
    DifferentGensNpc,

    /// <summary>
    /// The player is not member of a gens.
    /// </summary>
    NotJoined,
}
