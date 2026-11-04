// <copyright file="ImperialGuardianMonsterScaling.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// The scaling of the monsters of the imperial guardian event for players up to a level.
/// </summary>
public class ImperialGuardianMonsterScaling
{
    /// <summary>
    /// Gets or sets the maximum level of the highest player, including the master level.
    /// </summary>
    public int MaximumPlayerLevel { get; set; }

    /// <summary>
    /// Gets or sets the multiplier of the level of the monsters.
    /// </summary>
    public float LevelMultiplier { get; set; } = 1;

    /// <summary>
    /// Gets or sets the multiplier of the health of the monsters.
    /// </summary>
    public float HealthMultiplier { get; set; } = 1;

    /// <summary>
    /// Gets or sets the multiplier of the damage of the monsters.
    /// </summary>
    public float DamageMultiplier { get; set; } = 1;

    /// <summary>
    /// Gets or sets the multiplier of the defense of the monsters.
    /// </summary>
    public float DefenseMultiplier { get; set; } = 1;
}
