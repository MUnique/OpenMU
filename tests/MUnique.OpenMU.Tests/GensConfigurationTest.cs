// <copyright file="GensConfigurationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Gens;

/// <summary>
/// Tests for the rules of the <see cref="GensConfiguration"/>.
/// </summary>
[TestFixture]
public class GensConfigurationTest
{
    private readonly GensConfiguration _configuration = new();

    /// <summary>
    /// Tests the ranks by the contribution points, and by the ranking position from 10000 points.
    /// </summary>
    /// <param name="contribution">The contribution points.</param>
    /// <param name="position">The ranking position.</param>
    /// <returns>The rank.</returns>
    [TestCase(0, 0, ExpectedResult = 14)]
    [TestCase(499, 1, ExpectedResult = 14)]
    [TestCase(500, 0, ExpectedResult = 13)]
    [TestCase(1500, 0, ExpectedResult = 12)]
    [TestCase(3000, 0, ExpectedResult = 11)]
    [TestCase(6000, 0, ExpectedResult = 10)]
    [TestCase(9999, 1, ExpectedResult = 10)]
    [TestCase(10000, 0, ExpectedResult = 9)]
    [TestCase(10000, 301, ExpectedResult = 9)]
    [TestCase(10000, 300, ExpectedResult = 8)]
    [TestCase(10000, 101, ExpectedResult = 7)]
    [TestCase(10000, 51, ExpectedResult = 6)]
    [TestCase(10000, 31, ExpectedResult = 5)]
    [TestCase(10000, 11, ExpectedResult = 4)]
    [TestCase(10000, 6, ExpectedResult = 3)]
    [TestCase(10000, 2, ExpectedResult = 2)]
    [TestCase(10000, 1, ExpectedResult = 1)]
    public byte GetRank(int contribution, int position)
    {
        return this._configuration.GetRank(contribution, position);
    }

    /// <summary>
    /// Tests the contribution points which are missing for the next rank.
    /// From 6000 points, the next rank also needs a ranking position, so no points are shown.
    /// </summary>
    /// <param name="contribution">The contribution points.</param>
    /// <param name="rank">The rank.</param>
    /// <returns>The missing points.</returns>
    [TestCase(10, 14, ExpectedResult = 490)]
    [TestCase(500, 13, ExpectedResult = 1000)]
    [TestCase(5999, 11, ExpectedResult = 1)]
    [TestCase(6000, 10, ExpectedResult = 4000)]
    [TestCase(10000, 9, ExpectedResult = 0)]
    [TestCase(10000, 1, ExpectedResult = 0)]
    public int GetMissingContributionForNextRank(int contribution, byte rank)
    {
        return this._configuration.GetMissingContributionForNextRank(contribution, rank);
    }

    /// <summary>
    /// Tests the contribution points of a kill by the level difference.
    /// </summary>
    /// <param name="killerLevel">The level of the killer.</param>
    /// <param name="victimLevel">The level of the victim.</param>
    /// <param name="expectedGain">The expected gain of the killer.</param>
    /// <param name="expectedLoss">The expected loss of the victim.</param>
    [TestCase(100, 100, 5, 3)]
    [TestCase(110, 100, 5, 3)]
    [TestCase(111, 100, 3, 3)]
    [TestCase(131, 100, 2, 1)]
    [TestCase(151, 100, 1, 1)]
    [TestCase(90, 100, 5, 3)]
    [TestCase(89, 100, 6, 3)]
    [TestCase(49, 100, 7, 3)]
    public void KillContributionByLevel(int killerLevel, int victimLevel, int expectedGain, int expectedLoss)
    {
        var (gain, loss) = this._configuration.GetKillContribution(killerLevel, victimLevel, 14, 14);

        Assert.That(gain, Is.EqualTo(expectedGain));
        Assert.That(loss, Is.EqualTo(expectedLoss));
    }

    /// <summary>
    /// Tests the bonus for a victim with a better rank. Unlike the known server sources, a victim
    /// which is just one rank better gives a bonus, too.
    /// </summary>
    /// <param name="killerRank">The rank of the killer.</param>
    /// <param name="victimRank">The rank of the victim.</param>
    /// <returns>The gain of the killer at the same level.</returns>
    [TestCase(5, 5, ExpectedResult = 5)]
    [TestCase(5, 6, ExpectedResult = 5)]
    [TestCase(5, 4, ExpectedResult = 8)]
    [TestCase(5, 3, ExpectedResult = 9)]
    [TestCase(5, 1, ExpectedResult = 10)]
    [TestCase(10, 9, ExpectedResult = 6)]
    [TestCase(14, 8, ExpectedResult = 8)]
    public int KillContributionRankBonus(byte killerRank, byte victimRank)
    {
        return this._configuration.GetKillContribution(100, 100, killerRank, victimRank).KillerContribution;
    }
}
