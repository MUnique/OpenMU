// <copyright file="CrywolfMonsterScore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The score which a player gets for killing a monster of the army of Balgass.
/// </summary>
public class CrywolfMonsterScore
{
    /// <summary>
    /// Gets or sets the number of the monster.
    /// </summary>
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets the score.
    /// </summary>
    public int Score { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.MonsterNumber}: {this.Score}";
    }
}
