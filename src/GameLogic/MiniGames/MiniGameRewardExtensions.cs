// <copyright file="MiniGameRewardExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Extension methods for <see cref="MiniGameReward"/>.
/// </summary>
public static class MiniGameRewardExtensions
{
    /// <summary>
    /// Determines whether the reward is applicable to a player of the specified rank, considering
    /// <see cref="MiniGameReward.Rank"/>, <see cref="MiniGameReward.MaximumRank"/>, the number of ranked players
    /// and the rank percentage.
    /// </summary>
    /// <param name="reward">The reward.</param>
    /// <param name="rank">The rank of the player, starting at 1.</param>
    /// <param name="rankedPlayers">The number of ranked players.</param>
    /// <returns><c>true</c>, if the reward is applicable to the rank; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// The rank percentage is rank * 100 / ranked players, rounded down, like the original server
    /// calculates the tiers of the Devil Square rewards for more than six players.
    /// </remarks>
    public static bool IsApplicableToRank(this MiniGameReward reward, int rank, int rankedPlayers)
    {
        if (reward.Rank is { } minimumRank
            && (rank < minimumRank || rank > (reward.MaximumRank ?? minimumRank)))
        {
            return false;
        }

        if (reward.Rank is null && reward.MaximumRank is { } maximumRank && rank > maximumRank)
        {
            return false;
        }

        if (rankedPlayers < reward.MinimumRankedPlayers || rankedPlayers > reward.MaximumRankedPlayers)
        {
            return false;
        }

        if (reward.MinimumRankPercentage is null && reward.MaximumRankPercentage is null)
        {
            return true;
        }

        var rankPercentage = rankedPlayers > 0 ? rank * 100 / rankedPlayers : 100;
        return rankPercentage >= (reward.MinimumRankPercentage ?? 0)
               && rankPercentage <= (reward.MaximumRankPercentage ?? int.MaxValue);
    }
}
