// <copyright file="MiniGameRewardType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines the type of the <see cref="MiniGameReward"/>.
/// </summary>
public enum MiniGameRewardType
{
    /// <summary>
    /// Undefined reward type.
    /// </summary>
    Undefined,

    /// <summary>
    /// The reward is money which is added to the character of the player.
    /// </summary>
    Money,

    /// <summary>
    /// The reward is experience which is added to the character of the player.
    /// </summary>
    Experience,

    /// <summary>
    /// The reward is experience which is added to the character of the player,
    /// which is calculated based on the remaining seconds.
    /// Reward Value = <see cref="MiniGameReward.RewardAmount"/> * Remaining Seconds.
    /// </summary>
    ExperiencePerRemainingSeconds,

    /// <summary>
    /// The reward is an item.
    /// </summary>
    Item,

    /// <summary>
    /// The reward is an item to be dropped.
    /// </summary>
    ItemDrop,

    /// <summary>
    /// The reward is a score for the mini game.
    /// </summary>
    Score,
}