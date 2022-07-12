// <copyright file="ILegacyQuestRewardPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Interface of a view whose implementation informs about quest rewards after completion of a quest.
/// </summary>
/// <remarks>
/// Sends C1A4.
/// </remarks>
public interface ILegacyQuestRewardPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the quest reward for the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="rewardType">Type of the reward.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributeReward">The attribute reward.</param>
    ValueTask ShowAsync(Player player, QuestRewardType rewardType, int value, AttributeDefinition? attributeReward);
}