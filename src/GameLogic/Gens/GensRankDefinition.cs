// <copyright file="GensRankDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

/// <summary>
/// The requirements of a gens rank.
/// </summary>
public class GensRankDefinition
{
    /// <summary>
    /// Gets or sets the rank, from 1 (highest, Grand Duke) to 14 (lowest, Private), as the game client knows them.
    /// </summary>
    public byte Rank { get; set; }

    /// <summary>
    /// Gets or sets the minimum contribution points.
    /// </summary>
    public int MinimumContribution { get; set; }

    /// <summary>
    /// Gets or sets the worst position in the ranking of the gens, with which a member gets this rank.
    /// 0, when the rank doesn't depend on the ranking.
    /// </summary>
    public int MaximumRankingPosition { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.MaximumRankingPosition > 0
            ? $"Rank {this.Rank}: {this.MinimumContribution} points, ranking position up to {this.MaximumRankingPosition}"
            : $"Rank {this.Rank}: {this.MinimumContribution} points";
    }
}
