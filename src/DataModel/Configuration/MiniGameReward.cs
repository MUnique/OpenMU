// <copyright file="MiniGameReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The success flags of a mini game player.
/// </summary>
[Flags]
public enum MiniGameSuccessFlags
{
    /// <summary>
    /// No defined flag.
    /// </summary>
    None,

    /// <summary>
    /// The player submitted the required quest item to the NPC.
    /// </summary>
    Winner,

    /// <summary>
    /// The player is part of the party of a player classified as <see cref="Winner"/>.
    /// </summary>
    WinningParty,

    /// <summary>
    /// The player is the winner or in the party of a player classified as <see cref="Winner"/>.
    /// </summary>
    WinnerOrInWinningParty,

    /// <summary>
    /// The player is not the winner and not in the winners party.
    /// </summary>
    Loser,

    /// <summary>
    /// The player managed to stay alive until the end.
    /// </summary>
    Alive,

    /// <summary>
    /// The player died during the game.
    /// </summary>
    Dead,

    /// <summary>
    /// A winner exists in the game.
    /// </summary>
    WinnerExists,

    /// <summary>
    /// A winner doesn't exist in the game.
    /// </summary>
    WinnerNotExists,
}

/// <summary>
/// Defines a reward of a <see cref="MiniGameDefinition"/>.
/// </summary>
public class MiniGameReward
{
    /// <summary>
    /// Gets or sets the rank to which this reward is applicable.
    /// It's applicable, when more than one player can complete the mini game and
    /// you'd want to give different awards for a differently ranked players.
    /// </summary>
    public int? Rank { get; set; }

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
            result.Append("Rank ").Append(this.Rank.Value).Append(": ");
        }

        result.Append(this.RewardType.ToString()).Append(" ").Append(this.RewardAmount);

        if (this.ItemReward != null)
        {
            result.Append(" ").Append(this.ItemReward.Description);
        }

        return result.ToString();
    }
}