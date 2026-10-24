// <copyright file="BuiltInLevelBonusTable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemFormulas;

/// <summary>
/// The level bonus tables which the <see cref="BuiltInItemStatProfiles"/> name.
/// The names are the keys of the level bonus names of the initialization data.
/// </summary>
public enum BuiltInLevelBonusTable
{
    /// <summary>
    /// The damage increase of weapons.
    /// </summary>
    DamageIncreaseWeapons,

    /// <summary>
    /// The defense rate increase of shields.
    /// </summary>
    DefenseRateIncreaseShields,

    /// <summary>
    /// The defense increase of armor pieces.
    /// </summary>
    DefenseIncreaseArmors,

    /// <summary>
    /// The defense increase of shields.
    /// </summary>
    DefenseIncreaseShields,

    /// <summary>
    /// The defense bonus of small, first and second wings and of capes.
    /// </summary>
    DefenseBonus1stAnd2ndWings,

    /// <summary>
    /// The defense bonus of third wings.
    /// </summary>
    DefenseBonus3rdWings,
}
