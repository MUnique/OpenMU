// <copyright file="QuestReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Quests;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Defines the reward of a completed quest.
/// </summary>
[Cloneable]
public partial class QuestReward
{
    /// <summary>
    /// Gets or sets the type of the reward.
    /// </summary>
    /// <value>
    /// The type of the reward.
    /// </value>
    public QuestRewardType RewardType { get; set; }

    /// <summary>
    /// Gets or sets the value of the reward. It may have a different meaning, depending on the <see cref="RewardType"/>.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the item reward.
    /// The <see cref="Value"/> contains how often the item is rewarded.
    /// </summary>
    [MemberOfAggregate]
    public virtual Item? ItemReward { get; set; }

    /// <summary>
    /// Gets or sets the attribute reward. It's set when <see cref="RewardType"/> is <see cref="QuestRewardType.Attribute"/>.
    /// </summary>
    public virtual AttributeDefinition? AttributeReward { get; set; }

    /// <summary>
    /// Gets or sets the attribute reward. It's set when <see cref="RewardType"/> is <see cref="QuestRewardType.Skill"/>.
    /// </summary>
    public virtual Skill? SkillReward { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.RewardType == QuestRewardType.Item)
        {
            return $"{this.Value} x {this.ItemReward}";
        }

        if (this.RewardType == QuestRewardType.Skill)
        {
            return $"Skill: {this.SkillReward?.Name}";
        }

        if (this.RewardType == QuestRewardType.Attribute)
        {
            return $"Attribute: {this.Value} x {this.AttributeReward}";
        }

        if (this.RewardType is QuestRewardType.Experience or QuestRewardType.Money or QuestRewardType.LevelUpPoints or QuestRewardType.GensAttribution)
        {
            return $"{this.Value} x {this.RewardType}";
        }

        return $"{this.RewardType}";
    }
}