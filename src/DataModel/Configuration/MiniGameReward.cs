// <copyright file="MiniGameReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// The success flags of a mini game player.
/// </summary>
[Flags]
public enum MiniGameSuccessFlags
{
    /// <summary>
    /// No defined flag.
    /// </summary>
    None = 0,

    /// <summary>
    /// The player submitted the required quest item to the NPC.
    /// </summary>
    Winner = 1,

    /// <summary>
    /// The player is part of the party of a player classified as <see cref="Winner"/>.
    /// </summary>
    WinningParty = 1 << 1,

    /// <summary>
    /// The player is the winner or in the party of a player classified as <see cref="Winner"/>.
    /// </summary>
    WinnerOrInWinningParty = 1 << 2,

    /// <summary>
    /// The player is not the winner and not in the winners party.
    /// </summary>
    Loser = 1 << 3,

    /// <summary>
    /// The player managed to stay alive until the end.
    /// </summary>
    Alive = 1 << 4,

    /// <summary>
    /// The player died during the game.
    /// </summary>
    Dead = 1 << 5,

    /// <summary>
    /// A winner exists in the game.
    /// </summary>
    WinnerExists = 1 << 6,

    /// <summary>
    /// A winner doesn't exist in the game.
    /// </summary>
    WinnerNotExists = 1 << 7,
}

/// <summary>
/// Defines a reward of a <see cref="MiniGameDefinition"/>.
/// </summary>
[Cloneable]
public partial class MiniGameReward
{
    /// <summary>
    /// Gets or sets the rank to which this reward is applicable.
    /// It's applicable, when more than one player can complete the mini game and
    /// you'd want to give different awards for a differently ranked players.
    /// </summary>
    public int? Rank { get; set; }

    /// <summary>
    /// Gets or sets the maximum rank to which this reward is applicable.
    /// When it's set, the reward is applicable to the ranks from <see cref="Rank"/> to this rank;
    /// otherwise, only to <see cref="Rank"/>.
    /// </summary>
    public int? MaximumRank { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of ranked players, so that this reward is applicable.
    /// </summary>
    public int? MinimumRankedPlayers { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of ranked players, so that this reward is applicable.
    /// </summary>
    public int? MaximumRankedPlayers { get; set; }

    /// <summary>
    /// Gets or sets the minimum rank percentage, so that this reward is applicable.
    /// The rank percentage of a player is its rank * 100 / number of ranked players, rounded down,
    /// so the best players have the lowest percentage.
    /// </summary>
    public int? MinimumRankPercentage { get; set; }

    /// <summary>
    /// Gets or sets the maximum rank percentage, so that this reward is applicable.
    /// The rank percentage of a player is its rank * 100 / number of ranked players, rounded down,
    /// so the best players have the lowest percentage.
    /// </summary>
    public int? MaximumRankPercentage { get; set; }

    /// <summary>
    /// Gets or sets the reward type.
    /// </summary>
    public MiniGameRewardType RewardType { get; set; }

    /// <summary>
    /// Gets or sets the amount of the rewards.
    /// In case of <see cref="MiniGameRewardType.Money"/> it's the amount of money.
    /// In case of <see cref="MiniGameRewardType.Experience"/> it's the amount of experience.
    /// In case of <see cref="MiniGameRewardType.Item"/> it's the amount of <see cref="ItemReward"/>.
    /// </summary>
    public int RewardAmount { get; set; }

    /// <summary>
    /// Gets or sets the required success flags for the reward.
    /// </summary>
    public MiniGameSuccessFlags RequiredSuccess { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DropItemGroup"/> of this reward, if <see cref="RewardType"/> is <see cref="MiniGameRewardType.Item"/>.
    /// </summary>
    public virtual DropItemGroup? ItemReward { get; set; }

    /// <summary>
    /// Gets or sets the monster/gate/status which needs to be killed during the event, so that this reward applies.
    /// </summary>
    public virtual MonsterDefinition? RequiredKill { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var result = new StringBuilder();
        if (this.Rank.HasValue)
        {
            result.Append("Rank ").Append(this.Rank.Value);
            if (this.MaximumRank.HasValue)
            {
                result.Append('-').Append(this.MaximumRank.Value);
            }

            result.Append(": ");
        }

        AppendRange(result, "Players ", this.MinimumRankedPlayers, this.MaximumRankedPlayers);
        AppendRange(result, "Rank % ", this.MinimumRankPercentage, this.MaximumRankPercentage);

        if (this.RequiredSuccess > 0)
        {
            result.Append(this.RequiredSuccess).Append(": ");
        }

        result.Append(this.RewardType.ToString()).Append(" ").Append(this.RewardAmount);

        if (this.ItemReward != null)
        {
            result.Append(" ").Append(this.ItemReward.Description.ToString());
        }

        return result.ToString();
    }

    private static void AppendRange(StringBuilder result, string caption, int? minimum, int? maximum)
    {
        if (!minimum.HasValue && !maximum.HasValue)
        {
            return;
        }

        result.Append(caption);
        if (minimum.HasValue)
        {
            result.Append(minimum.Value);
        }

        result.Append('-');
        if (maximum.HasValue)
        {
            result.Append(maximum.Value);
        }

        result.Append(": ");
    }
}