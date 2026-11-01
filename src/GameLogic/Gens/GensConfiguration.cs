// <copyright file="GensConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The configuration of the gens system.
/// </summary>
public class GensConfiguration
{
    /// <summary>
    /// Gets or sets the minimum level of a character to join a gens.
    /// </summary>
    public int MinimumLevel { get; set; } = 50;

    /// <summary>
    /// Gets or sets the contribution points which a character gets when it joins a gens.
    /// </summary>
    public int StartingContribution { get; set; } = 10;

    /// <summary>
    /// Gets or sets the rank which a character gets when it joins a gens.
    /// The game client knows the ranks 1 (highest, Grand Duke) to 14 (lowest, Private).
    /// </summary>
    public byte StartingRank { get; set; } = 14;

    /// <summary>
    /// Gets or sets the time which a character has to wait after leaving a gens, until it can join a gens again.
    /// </summary>
    public TimeSpan RejoinWaitTime { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the number of the npc of the Duprian gens.
    /// </summary>
    public short DuprianNpcNumber { get; set; } = 543;

    /// <summary>
    /// Gets or sets the number of the npc of the Vanert gens.
    /// </summary>
    public short VanertNpcNumber { get; set; } = 544;

    /// <summary>
    /// Gets or sets the numbers of the maps of the battle zone.
    /// Only gens members can enter them, and kills between the members of different gens change their contribution points.
    /// </summary>
    /// <remarks>
    /// The game client shows Vulcanus (63) as battle zone.
    /// </remarks>
    public IList<short> BattleZoneMapNumbers { get; set; } = new List<short> { 63 };

    /// <summary>
    /// Gets or sets the contribution points of a kill, depending on the level difference.
    /// </summary>
    public IList<GensKillContribution> KillContributions { get; set; } = new List<GensKillContribution>
    {
        new() { MinimumLevelDifference = int.MinValue, KillerContribution = 7, VictimContributionLoss = 3 },
        new() { MinimumLevelDifference = -50, KillerContribution = 6, VictimContributionLoss = 3 },
        new() { MinimumLevelDifference = -10, KillerContribution = 5, VictimContributionLoss = 3 },
        new() { MinimumLevelDifference = 11, KillerContribution = 3, VictimContributionLoss = 3 },
        new() { MinimumLevelDifference = 31, KillerContribution = 2, VictimContributionLoss = 1 },
        new() { MinimumLevelDifference = 51, KillerContribution = 1, VictimContributionLoss = 1 },
    };

    /// <summary>
    /// Gets or sets the bonus contribution points of a kill of a victim with a better rank.
    /// </summary>
    public IList<GensRankBonus> RankBonuses { get; set; } = new List<GensRankBonus>
    {
        new() { MinimumRankDifference = 1, Bonus = 3, LowRankBonus = 1 },
        new() { MinimumRankDifference = 2, Bonus = 4, LowRankBonus = 2 },
        new() { MinimumRankDifference = 3, Bonus = 5, LowRankBonus = 3 },
    };

    /// <summary>
    /// Gets or sets the rank, from which a killer gets the <see cref="GensRankBonus.LowRankBonus"/> instead of the <see cref="GensRankBonus.Bonus"/>.
    /// </summary>
    public byte LowRankBonusFromRank { get; set; } = 9;

    /// <summary>
    /// Gets or sets the minimum contribution points of a victim, so that the killer gets contribution points.
    /// It prevents getting points by killing members which have no points anymore.
    /// </summary>
    public int MinimumVictimContribution { get; set; } = 1;

    /// <summary>
    /// Gets or sets the count of recent kills of the same victim, from which the killer is warned that it gets no points anymore soon.
    /// </summary>
    public int AbuseWarningKillCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the count of recent kills of the same victim, from which the kills don't change the contribution points anymore.
    /// </summary>
    public int AbuseLimitKillCount { get; set; } = 6;

    /// <summary>
    /// Gets or sets the time after the last kill of the same victim, after which the count of recent kills starts again.
    /// </summary>
    public TimeSpan AbuseResetTime { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    /// Gets or sets the ranks and their requirements.
    /// </summary>
    public IList<GensRankDefinition> Ranks { get; set; } = new List<GensRankDefinition>
    {
        new() { Rank = 1, MinimumContribution = 10000, MaximumRankingPosition = 1 },
        new() { Rank = 2, MinimumContribution = 10000, MaximumRankingPosition = 5 },
        new() { Rank = 3, MinimumContribution = 10000, MaximumRankingPosition = 10 },
        new() { Rank = 4, MinimumContribution = 10000, MaximumRankingPosition = 30 },
        new() { Rank = 5, MinimumContribution = 10000, MaximumRankingPosition = 50 },
        new() { Rank = 6, MinimumContribution = 10000, MaximumRankingPosition = 100 },
        new() { Rank = 7, MinimumContribution = 10000, MaximumRankingPosition = 200 },
        new() { Rank = 8, MinimumContribution = 10000, MaximumRankingPosition = 300 },
        new() { Rank = 9, MinimumContribution = 10000 },
        new() { Rank = 10, MinimumContribution = 6000 },
        new() { Rank = 11, MinimumContribution = 3000 },
        new() { Rank = 12, MinimumContribution = 1500 },
        new() { Rank = 13, MinimumContribution = 500 },
        new() { Rank = 14, MinimumContribution = 0 },
    };

    /// <summary>
    /// Gets or sets the interval in which the ranking of the gens is calculated.
    /// </summary>
    public TimeSpan RankingInterval { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Gets or sets the first day of a month, on which the members can claim their monthly reward (UTC).
    /// </summary>
    public int RewardStartDay { get; set; } = 1;

    /// <summary>
    /// Gets or sets the last day of a month, on which the members can claim their monthly reward (UTC).
    /// </summary>
    public int RewardEndDay { get; set; } = 7;

    /// <summary>
    /// Gets or sets the monthly rewards of the ranks. A member gets the reward of its rank at the time it claims it.
    /// </summary>
    public IList<GensRankReward> Rewards { get; set; } = new List<GensRankReward>
    {
        new() { Rank = 1, ItemGroup = 14, ItemNumber = 141, Count = 30 },
        new() { Rank = 2, ItemGroup = 14, ItemNumber = 141, Count = 20 },
        new() { Rank = 3, ItemGroup = 14, ItemNumber = 142, Count = 20 },
        new() { Rank = 4, ItemGroup = 14, ItemNumber = 142, Count = 10 },
        new() { Rank = 5, ItemGroup = 14, ItemNumber = 143, Count = 10 },
        new() { Rank = 6, ItemGroup = 14, ItemNumber = 143, Count = 5 },
        new() { Rank = 7, ItemGroup = 14, ItemNumber = 144, Count = 5 },
        new() { Rank = 8, ItemGroup = 14, ItemNumber = 144, Count = 3 },
    };

    /// <summary>
    /// Gets or sets a value indicating whether members of different gens can form a party.
    /// </summary>
    public bool AllowPartyWithOtherGens { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parties can be formed in the battle zone.
    /// When it's <c>false</c>, a player also leaves its party when it enters the battle zone.
    /// </summary>
    public bool AllowPartyInBattleZone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a character has to be a member of a gens to create a guild,
    /// and to be a member of the gens of the guild master to join a guild.
    /// </summary>
    /// <remarks>
    /// The original game requires it. It's not the default, because it affects all players, also the ones which don't care about the gens.
    /// </remarks>
    public bool GuildRequiresGens { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the masters of two guilds have to be members of the same gens to form an alliance.
    /// </summary>
    /// <remarks>
    /// The original game requires it. It's not the default, because it affects all guilds, also the ones which don't care about the gens.
    /// </remarks>
    public bool AllianceRequiresSameGens { get; set; }

    /// <summary>
    /// Gets the gens of the npc with the specified number.
    /// </summary>
    /// <param name="npcNumber">The number of the npc.</param>
    /// <returns>The gens of the npc; <see cref="GensType.None"/>, if it's not a gens npc.</returns>
    public GensType GetGensOfNpc(short npcNumber)
    {
        if (npcNumber == this.DuprianNpcNumber)
        {
            return GensType.Duprian;
        }

        if (npcNumber == this.VanertNpcNumber)
        {
            return GensType.Vanert;
        }

        return GensType.None;
    }

    /// <summary>
    /// Determines whether the map with the specified number is part of the battle zone.
    /// </summary>
    /// <param name="mapNumber">The number of the map.</param>
    /// <returns><c>true</c>, if the map is part of the battle zone; otherwise, <c>false</c>.</returns>
    public bool IsBattleZone(short mapNumber)
    {
        return this.BattleZoneMapNumbers.Contains(mapNumber);
    }

    /// <summary>
    /// Gets the best rank which a member gets with the specified contribution and ranking position.
    /// </summary>
    /// <param name="contribution">The contribution points.</param>
    /// <param name="rankingPosition">The position in the ranking of the gens; 0, if it's not ranked yet.</param>
    /// <returns>The rank.</returns>
    public byte GetRank(int contribution, int rankingPosition)
    {
        var rank = this.Ranks
            .Where(definition => contribution >= definition.MinimumContribution
                                 && (definition.MaximumRankingPosition <= 0
                                     || (rankingPosition > 0 && rankingPosition <= definition.MaximumRankingPosition)))
            .OrderBy(definition => definition.Rank)
            .FirstOrDefault();
        return rank?.Rank ?? this.StartingRank;
    }

    /// <summary>
    /// Gets the contribution points which a member still needs for the next better rank.
    /// </summary>
    /// <param name="contribution">The contribution points.</param>
    /// <param name="rank">The current rank.</param>
    /// <returns>
    /// The missing contribution points; 0, if the next better rank also depends on the ranking position,
    /// or if there is no better rank.
    /// </returns>
    public int GetMissingContributionForNextRank(int contribution, byte rank)
    {
        var nextRank = this.Ranks
            .Where(definition => definition.Rank < rank)
            .OrderByDescending(definition => definition.Rank)
            .FirstOrDefault();
        if (nextRank is null || nextRank.MaximumRankingPosition > 0)
        {
            return 0;
        }

        return Math.Max(nextRank.MinimumContribution - contribution, 0);
    }

    /// <summary>
    /// Gets the contribution points of a kill of a member of the other gens.
    /// </summary>
    /// <param name="killerLevel">The level of the killer.</param>
    /// <param name="victimLevel">The level of the victim.</param>
    /// <param name="killerRank">The rank of the killer.</param>
    /// <param name="victimRank">The rank of the victim.</param>
    /// <returns>The points which the killer gets and which the victim loses.</returns>
    public (int KillerContribution, int VictimContributionLoss) GetKillContribution(int killerLevel, int victimLevel, byte killerRank, byte victimRank)
    {
        var levelDifference = killerLevel - victimLevel;
        var contribution = this.KillContributions
            .Where(entry => entry.MinimumLevelDifference <= levelDifference)
            .MaxBy(entry => entry.MinimumLevelDifference);
        if (contribution is null)
        {
            return (0, 0);
        }

        // A smaller rank is a better rank.
        var rankDifference = killerRank - victimRank;
        var rankBonus = this.RankBonuses
            .Where(entry => entry.MinimumRankDifference <= rankDifference)
            .MaxBy(entry => entry.MinimumRankDifference);
        var bonus = rankBonus is null
            ? 0
            : killerRank >= this.LowRankBonusFromRank ? rankBonus.LowRankBonus : rankBonus.Bonus;

        return (contribution.KillerContribution + bonus, contribution.VictimContributionLoss);
    }
}
