// <copyright file="CastleSiegeUpgradeType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The type of upgrade applied to a castle siege NPC (gate or statue).
/// </summary>
public enum CastleSiegeUpgradeType : byte
{
    /// <summary>
    /// Increases the defense stat of the NPC.
    /// </summary>
    Defense = 1,

    /// <summary>
    /// Increases the HP regeneration rate of the NPC.
    /// </summary>
    Regen = 2,

    /// <summary>
    /// Increases the maximum HP of the NPC.
    /// </summary>
    Life = 3,
}
