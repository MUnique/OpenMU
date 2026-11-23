// <copyright file="MiniGameRewardExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the <see cref="MiniGameRewardExtensions.IsApplicableToRank"/>.
/// </summary>
[TestFixture]
public class MiniGameRewardExtensionsTest
{
    /// <summary>
    /// Tests that a reward without any rank condition applies to every rank.
    /// </summary>
    [Test]
    public void RewardWithoutRankAppliesToEveryRank()
    {
        var reward = new BasicModel.MiniGameReward();

        Assert.That(Enumerable.Range(1, 10).Select(rank => reward.IsApplicableToRank(rank, 10)), Has.All.True);
    }

    /// <summary>
    /// Tests that a reward with a rank, but without a maximum rank, applies only to this rank, as before.
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(1, false)]
    [TestCase(2, true)]
    [TestCase(3, false)]
    public void RewardWithRankAppliesOnlyToRank(int rank, bool expected)
    {
        var reward = new BasicModel.MiniGameReward { Rank = 2 };

        Assert.That(reward.IsApplicableToRank(rank, 10), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a reward with a rank and a maximum rank applies to the ranks in between.
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(3, false)]
    [TestCase(4, true)]
    [TestCase(6, true)]
    [TestCase(7, false)]
    public void RewardWithRankRangeAppliesToRanksInBetween(int rank, bool expected)
    {
        var reward = new BasicModel.MiniGameReward { Rank = 4, MaximumRank = 6 };

        Assert.That(reward.IsApplicableToRank(rank, 10), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a reward with a range of ranked players applies only when the number of ranked players is in the range.
    /// </summary>
    /// <param name="rankedPlayers">The number of ranked players.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(1, false)]
    [TestCase(2, true)]
    [TestCase(6, true)]
    [TestCase(7, false)]
    public void RewardWithRankedPlayersRangeAppliesOnlyInRange(int rankedPlayers, bool expected)
    {
        var reward = new BasicModel.MiniGameReward { Rank = 1, MinimumRankedPlayers = 2, MaximumRankedPlayers = 6 };

        Assert.That(reward.IsApplicableToRank(1, rankedPlayers), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a reward with a rank percentage range applies to the ranks whose rank * 100 / ranked players,
    /// rounded down, is in the range.
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <param name="rankedPlayers">The number of ranked players.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(2, 7, false)] // 28
    [TestCase(3, 7, true)] // 42
    [TestCase(3, 10, false)] // 30
    [TestCase(5, 10, true)] // 50
    [TestCase(6, 10, false)] // 60
    public void RewardWithRankPercentageRangeAppliesOnlyInRange(int rank, int rankedPlayers, bool expected)
    {
        var reward = new BasicModel.MiniGameReward { MinimumRankPercentage = 31, MaximumRankPercentage = 50 };

        Assert.That(reward.IsApplicableToRank(rank, rankedPlayers), Is.EqualTo(expected));
    }
}
