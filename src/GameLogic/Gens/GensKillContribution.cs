// <copyright file="GensKillContribution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

/// <summary>
/// The contribution points of a kill of a member of the other gens in a battle zone,
/// depending on the level difference between the killer and the victim.
/// </summary>
public class GensKillContribution
{
    /// <summary>
    /// Gets or sets the minimum level difference (level of the killer minus level of the victim),
    /// from which these points apply. It's negative, when the victim has the higher level.
    /// The entry with the highest minimum which isn't greater than the level difference applies.
    /// </summary>
    public int MinimumLevelDifference { get; set; }

    /// <summary>
    /// Gets or sets the contribution points which the killer gets.
    /// </summary>
    public int KillerContribution { get; set; }

    /// <summary>
    /// Gets or sets the contribution points which the victim loses.
    /// </summary>
    public int VictimContributionLoss { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"From {this.MinimumLevelDifference}: +{this.KillerContribution} / -{this.VictimContributionLoss}";
    }
}
