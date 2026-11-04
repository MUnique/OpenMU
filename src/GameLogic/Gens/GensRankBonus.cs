// <copyright file="GensRankBonus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

/// <summary>
/// The bonus contribution points of a kill of a member of the other gens with a better rank.
/// </summary>
public class GensRankBonus
{
    /// <summary>
    /// Gets or sets the minimum difference of the ranks (rank of the killer minus rank of the victim),
    /// from which this bonus applies. The entry with the highest minimum which isn't greater than the difference applies.
    /// </summary>
    public int MinimumRankDifference { get; set; }

    /// <summary>
    /// Gets or sets the bonus points.
    /// </summary>
    public int Bonus { get; set; }

    /// <summary>
    /// Gets or sets the bonus points, when the killer has a low rank, see <see cref="GensConfiguration.LowRankBonusFromRank"/>.
    /// </summary>
    public int LowRankBonus { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"From {this.MinimumRankDifference}: +{this.Bonus} (low rank: +{this.LowRankBonus})";
    }
}
