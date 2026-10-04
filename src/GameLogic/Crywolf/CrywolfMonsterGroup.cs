// <copyright file="CrywolfMonsterGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// A group of the army of Balgass, which is led by a Dark Elf.
/// </summary>
/// <remarks>
/// The members of a group are defined by the monster spawns of the crywolf map with the trigger
/// <see cref="SpawnTrigger.OnceAtWaveStart"/> and the <see cref="WaveNumber"/> of the group.
/// When the battle starts, the leader marches to the <see cref="AdvanceGoalX"/>/<see cref="AdvanceGoalY"/>.
/// When the army starts to attack the statue, it marches to the <see cref="AttackGoalX"/>/<see cref="AttackGoalY"/>.
/// The other members of the group follow their leader.
/// </remarks>
public class CrywolfMonsterGroup
{
    /// <summary>
    /// Gets or sets the wave number of the monster spawns of the group.
    /// </summary>
    public byte WaveNumber { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate, to which the leader marches when the battle starts.
    /// </summary>
    public byte AdvanceGoalX { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate, to which the leader marches when the battle starts.
    /// </summary>
    public byte AdvanceGoalY { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate, to which the leader marches when the army attacks the statue.
    /// </summary>
    public byte AttackGoalX { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate, to which the leader marches when the army attacks the statue.
    /// </summary>
    public byte AttackGoalY { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Group {this.WaveNumber}: ({this.AdvanceGoalX}, {this.AdvanceGoalY}), ({this.AttackGoalX}, {this.AttackGoalY})";
    }
}
