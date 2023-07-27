// <copyright file="QuestDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Extension methods to make quest creation easier.
/// </summary>
internal static class QuestDefinitionExtensions
{
    /// <summary>
    /// Adds a <see cref="QuestMonsterKillRequirement"/> to this quest definition.
    /// </summary>
    /// <param name="questDefinition">The quest definition.</param>
    /// <param name="amount">The amount.</param>
    /// <param name="monsterNumber">The monster number.</param>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>This quest definition.</returns>
    public static QuestDefinition WithMonsterKillRequirement(this QuestDefinition questDefinition, int amount, short monsterNumber, IContext context, GameConfiguration gameConfiguration)
    {
        var killRequirement = context.CreateNew<QuestMonsterKillRequirement>();
        var parentNumber = (ushort)((ushort)questDefinition.Number | (questDefinition.Group << 10));
        killRequirement.SetGuid(parentNumber.ToSigned(), monsterNumber, questDefinition.QualifiedCharacter?.Number ?? 0);
        killRequirement.MinimumNumber = amount;
        killRequirement.Monster = gameConfiguration.Monsters.First(m => m.Number == monsterNumber);
        questDefinition.RequiredMonsterKills.Add(killRequirement);
        return questDefinition;
    }

    /// <summary>
    /// Adds a <see cref="QuestReward"/> of <see cref="QuestRewardType.Money"/> to this quest definition.
    /// </summary>
    /// <param name="questDefinition">The quest definition.</param>
    /// <param name="moneyAmount">The money amount.</param>
    /// <param name="context">The context.</param>
    /// <returns>This quest definition.</returns>
    public static QuestDefinition WithMoneyReward(this QuestDefinition questDefinition, int moneyAmount, IContext context)
    {
        var reward = context.CreateNew<QuestReward>();
        reward.Value = moneyAmount;
        reward.RewardType = QuestRewardType.Money;
        questDefinition.Rewards.Add(reward);
        return questDefinition;
    }

    /// <summary>
    /// Adds a <see cref="QuestReward"/> of <see cref="QuestRewardType.Experience"/> to this quest definition.
    /// </summary>
    /// <param name="questDefinition">The quest definition.</param>
    /// <param name="experienceAmount">The experience amount.</param>
    /// <param name="context">The context.</param>
    /// <returns>This quest definition.</returns>
    public static QuestDefinition WithExperienceReward(this QuestDefinition questDefinition, int experienceAmount, IContext context)
    {
        var reward = context.CreateNew<QuestReward>();
        reward.Value = experienceAmount;
        reward.RewardType = QuestRewardType.Experience;
        questDefinition.Rewards.Add(reward);
        return questDefinition;
    }

    /// <summary>
    /// Adds a <see cref="QuestReward"/> of <see cref="QuestRewardType.Item"/> to this quest definition.
    /// </summary>
    /// <param name="questDefinition">The quest definition.</param>
    /// <param name="itemGroup">The item group.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="itemOption">The item option.</param>
    /// <param name="hasLuck">if set to <c>true</c> [has luck].</param>
    /// <param name="hasSkill">if set to <c>true</c> [has skill].</param>
    /// <returns>This quest definition.</returns>
    public static QuestDefinition WithItemReward(this QuestDefinition questDefinition, int itemGroup,
        int itemNumber, IContext context, GameConfiguration gameConfiguration,
        byte itemLevel = 0, int itemOption = 0, bool hasLuck = false, bool hasSkill = false)
    {
        var reward = context.CreateNew<QuestReward>();
        reward.RewardType = QuestRewardType.Item;
        var item = context.CreateNew<Item>();
        item.Definition = gameConfiguration.Items.First(def => def.Group == itemGroup && def.Number == itemNumber);
        item.HasSkill = item.Definition.Skill != null && hasSkill;
        item.Durability = item.IsWearable() ? item.GetMaximumDurabilityOfOnePiece() : item.Durability;
        item.Level = itemLevel;
        if (hasLuck
            && item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck) is { } luckOption)
        {
            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = luckOption;
            item.ItemOptions.Add(optionLink);
        }

        if (itemOption > 0
            && item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Option) is { } optionOption)
        {
            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = optionOption;
            optionLink.Level = itemOption / 4;
            item.ItemOptions.Add(optionLink);
        }

        reward.ItemReward = item;
        questDefinition.Rewards.Add(reward);
        return questDefinition;
    }

    /// <summary>
    /// Adds a <see cref="QuestItemRequirement"/> to this quest definition.
    /// </summary>
    /// <param name="questDefinition">The quest definition.</param>
    /// <param name="itemGroup">The item group.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="itemOption">The item option (4 ~ 12).</param>
    /// <param name="hasLuck">If set to <c>true</c>, the item must have luck.</param>
    /// <param name="hasSkill">If set to <c>true</c>, the item must have skill.</param>
    /// <param name="itemCount">The item count, default 1.</param>
    /// <returns>This quest definition.</returns>
    public static QuestDefinition WithItemRequirement(this QuestDefinition questDefinition, int itemGroup,
        int itemNumber, IContext context, GameConfiguration gameConfiguration, byte durability = 0, byte itemLevel = 0,
        int itemOption = 0, bool hasLuck = false, bool hasSkill = false, byte itemCount = 1)
    {
        var requirement = context.CreateNew<QuestItemRequirement>();
        requirement.MinimumNumber = itemCount;
        var item = context.CreateNew<Item>();
        item.Definition = gameConfiguration.Items.First(def => def.Group == itemGroup && def.Number == itemNumber);
        item.HasSkill = item.Definition.Skill != null && hasSkill;
        item.Durability = durability;
        item.Level = itemLevel;
        if (hasLuck
            && item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck) is { } luckOption)
        {
            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = luckOption;
            item.ItemOptions.Add(optionLink);
        }

        if (itemOption > 0
            && item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Option) is { } optionOption)
        {
            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = optionOption;
            optionLink.Level = itemOption / 4;
            item.ItemOptions.Add(optionLink);
        }

        requirement.Item = item.Definition; // TODO?

        questDefinition.RequiredItems.Add(requirement);
        return questDefinition;
    }
}