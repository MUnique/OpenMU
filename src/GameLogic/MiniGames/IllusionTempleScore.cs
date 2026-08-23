// <copyright file="IllusionTempleScore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;

/// <summary>
/// The score of an illusion temple game.
/// </summary>
/// <remarks>
/// The counters are kept as integers, so that concurrent scoring can't wrap them around, while the
/// game client can only show a byte per team - the properties clamp them accordingly.
/// </remarks>
public class IllusionTempleScore
{
    private int _alliedForcesScore;

    private int _illusionForcesScore;

    /// <summary>
    /// Gets the score of the allied forces.
    /// </summary>
    public byte AlliedForcesScore => (byte)Math.Min(byte.MaxValue, this._alliedForcesScore);

    /// <summary>
    /// Gets the score of the illusion forces.
    /// </summary>
    public byte IllusionForcesScore => (byte)Math.Min(byte.MaxValue, this._illusionForcesScore);

    /// <summary>
    /// The minimum score a team needs to be declared the winner - a single relic delivered ahead of the
    /// other team (e.g. 1:0) isn't enough on its own and counts as a draw, just like in the original
    /// event.
    /// </summary>
    private const int MinimumWinningScore = 2;

    /// <summary>
    /// Gets the team which is currently in the lead, or <c>null</c>, if neither team has both scored at
    /// least <see cref="MinimumWinningScore"/> points and more than the other team.
    /// </summary>
    public IllusionTempleTeam? LeadingTeam
    {
        get
        {
            if (this._alliedForcesScore >= MinimumWinningScore && this._alliedForcesScore > this._illusionForcesScore)
            {
                return IllusionTempleTeam.AlliedForces;
            }

            if (this._illusionForcesScore >= MinimumWinningScore && this._illusionForcesScore > this._alliedForcesScore)
            {
                return IllusionTempleTeam.IllusionForces;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the score of the specified team.
    /// </summary>
    /// <param name="team">The team.</param>
    /// <returns>The score of the team.</returns>
    public byte GetScore(IllusionTempleTeam team) => team == IllusionTempleTeam.AlliedForces
        ? this.AlliedForcesScore
        : this.IllusionForcesScore;

    /// <summary>
    /// Increases the score of the specified team.
    /// </summary>
    /// <param name="team">The team which scored.</param>
    /// <param name="value">The value by which the score is increased.</param>
    public void IncreaseScore(IllusionTempleTeam team, int value = 1)
    {
        if (team == IllusionTempleTeam.AlliedForces)
        {
            Interlocked.Add(ref this._alliedForcesScore, value);
        }
        else
        {
            Interlocked.Add(ref this._illusionForcesScore, value);
        }
    }
}
