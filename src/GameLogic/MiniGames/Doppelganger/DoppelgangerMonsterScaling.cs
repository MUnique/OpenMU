// <copyright file="DoppelgangerMonsterScaling.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <summary>
/// The multipliers for the monsters of the doppelganger event, which apply up to a player level.
/// Each list contains one multiplier for each number of players, starting with one player.
/// </summary>
public class DoppelgangerMonsterScaling
{
    /// <summary>
    /// Gets or sets the highest player level (including the master level) for which the multipliers apply.
    /// </summary>
    public int MaximumPlayerLevel { get; set; }

    /// <summary>
    /// Gets or sets the multipliers for the level of the monsters.
    /// </summary>
    public IList<float> LevelMultipliers { get; set; } = new List<float>();

    /// <summary>
    /// Gets or sets the multipliers for the health of the monsters.
    /// </summary>
    public IList<float> HealthMultipliers { get; set; } = new List<float>();

    /// <summary>
    /// Gets or sets the multipliers for the damage of the monsters.
    /// </summary>
    public IList<float> DamageMultipliers { get; set; } = new List<float>();

    /// <summary>
    /// Gets or sets the multipliers for the defense of the monsters.
    /// </summary>
    public IList<float> DefenseMultipliers { get; set; } = new List<float>();
}
