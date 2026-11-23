// <copyright file="DevilSquareRewardTiers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Configures the rewards of a devil square, so that every ranked player gets one of the four reward tiers.
/// </summary>
/// <remarks>
/// With up to six ranked players, the ranks 1 to 3 get the tiers 1 to 3 and every other rank the tier 4.
/// With more players, the rank 1 gets the tier 1 and the others get their tier by their rank percentage
/// (rank * 100 / ranked players): up to 30 the tier 2, up to 50 the tier 3, above the tier 4.
/// That's how the original server distributes the rewards; before, only the ranks 1 to 4 got a reward.
/// </remarks>
internal static class DevilSquareRewardTiers
{
    /// <summary>
    /// The number of reward tiers. The rewards of the ranks 1 to 4 define the rewards of the tiers 1 to 4.
    /// </summary>
    internal const int TierCount = 4;

    /// <summary>
    /// The maximum number of ranked players, for which the tiers are assigned by rank.
    /// </summary>
    internal const int MaximumPlayersForTiersByRank = 6;

    /// <summary>
    /// The maximum rank percentage of the tier 2, when the tiers are assigned by rank percentage.
    /// </summary>
    internal const int SecondTierMaximumRankPercentage = 30;

    /// <summary>
    /// The maximum rank percentage of the tier 3, when the tiers are assigned by rank percentage.
    /// </summary>
    internal const int ThirdTierMaximumRankPercentage = 50;

    /// <summary>
    /// Determines whether the rewards of the devil square are already configured by tiers.
    /// </summary>
    /// <param name="devilSquare">The devil square.</param>
    /// <returns><c>true</c>, if the rewards are already configured by tiers; otherwise, <c>false</c>.</returns>
    public static bool HasTiers(MiniGameDefinition devilSquare)
    {
        return devilSquare.Rewards.Any(reward => reward.MinimumRankedPlayers is not null || reward.MaximumRankedPlayers is not null);
    }

    /// <summary>
    /// Configures the rewards of the ranks 1 to 4 of the devil square as reward tiers.
    /// The reward amounts are kept; the rewards are applied for up to six ranked players and copied for more players.
    /// Does nothing, if the rewards are already configured by tiers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="devilSquare">The devil square.</param>
    public static void Apply(IContext context, MiniGameDefinition devilSquare)
    {
        if (HasTiers(devilSquare))
        {
            return;
        }

        var tierRewards = devilSquare.Rewards
            .Where(reward => reward.Rank is >= 1 and <= TierCount
                             && reward.MaximumRank is null
                             && reward.RewardType is MiniGameRewardType.Experience or MiniGameRewardType.Money)
            .ToList();
        var maximumRank = Math.Max(devilSquare.MaximumPlayerCount, MaximumPlayersForTiersByRank + 1);
        foreach (var reward in tierRewards)
        {
            var tier = reward.Rank!.Value;

            var rewardForMorePlayers = context.CreateNew<MiniGameReward>();
            rewardForMorePlayers.RewardType = reward.RewardType;
            rewardForMorePlayers.RewardAmount = reward.RewardAmount;
            rewardForMorePlayers.RequiredSuccess = reward.RequiredSuccess;
            rewardForMorePlayers.ItemReward = reward.ItemReward;
            rewardForMorePlayers.RequiredKill = reward.RequiredKill;
            SetTierByRankPercentage(rewardForMorePlayers, tier, maximumRank);
            devilSquare.Rewards.Add(rewardForMorePlayers);

            SetTierByRank(reward, tier);
        }
    }

    private static void SetTierByRank(MiniGameReward reward, int tier)
    {
        reward.MaximumRankedPlayers = MaximumPlayersForTiersByRank;
        reward.Rank = tier;
        reward.MaximumRank = tier == TierCount ? MaximumPlayersForTiersByRank : null;
    }

    private static void SetTierByRankPercentage(MiniGameReward reward, int tier, int maximumRank)
    {
        reward.MinimumRankedPlayers = MaximumPlayersForTiersByRank + 1;
        if (tier == 1)
        {
            reward.Rank = 1;
            return;
        }

        // The rank 1 always gets the tier 1, even if its percentage is in the range of another tier.
        reward.Rank = 2;
        reward.MaximumRank = maximumRank;
        switch (tier)
        {
            case 2:
                reward.MaximumRankPercentage = SecondTierMaximumRankPercentage;
                break;
            case 3:
                reward.MinimumRankPercentage = SecondTierMaximumRankPercentage + 1;
                reward.MaximumRankPercentage = ThirdTierMaximumRankPercentage;
                break;
            default:
                reward.MinimumRankPercentage = ThirdTierMaximumRankPercentage + 1;
                break;
        }
    }
}
