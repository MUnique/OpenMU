// <copyright file="WeeklyQuestRewarder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Hands out the rewards of a <see cref="WeeklyQuestDefinition"/>.
/// </summary>
/// <remarks>
/// Unlike the classic quests, items are never dropped on the ground when the inventory is full.
/// The rewards are handed out completely or not at all, so they can be retried later.
/// </remarks>
public static class WeeklyQuestRewarder
{
    /// <summary>
    /// Tries to give all rewards of the quest to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="quest">The quest.</param>
    /// <returns><c>true</c>, if all rewards have been given; <c>false</c>, if nothing has been given.</returns>
    public static async ValueTask<bool> TryGiveRewardsAsync(Player player, WeeklyQuestDefinition quest)
    {
        if (player.Inventory is null || player.SelectedCharacter is null)
        {
            return false;
        }

        var money = quest.Rewards
            .Where(r => r.RewardType == WeeklyQuestRewardType.Money)
            .Sum(r => (long)r.Amount);
        if (money > 0 && player.Money + money > player.GameContext.Configuration.MaximumInventoryMoney)
        {
            return false;
        }

        var addedItems = new List<Item>();
        foreach (var reward in quest.Rewards.Where(r => r.RewardType == WeeklyQuestRewardType.Item))
        {
            if (ResolveItemDefinition(player, reward) is not { } itemDefinition)
            {
                player.Logger.LogWarning("Weekly quest {quest} has an item reward without a valid item.", quest.Id);
                continue;
            }

            for (var i = 0; i < reward.Amount; i++)
            {
                var item = CreateItem(itemDefinition, reward).MakePersistent(player.PersistenceContext);
                if (!await player.Inventory.AddItemAsync(item).ConfigureAwait(false))
                {
                    await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                    await RollbackAsync(player, addedItems).ConfigureAwait(false);
                    return false;
                }

                addedItems.Add(item);
            }
        }

        foreach (var item in addedItems)
        {
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        }

        if (money > 0)
        {
            player.TryAddMoney((int)money);
        }

        foreach (var reward in quest.Rewards)
        {
            switch (reward.RewardType)
            {
                case WeeklyQuestRewardType.Experience:
                    await player.AddExperienceAsync(reward.Amount, null).ConfigureAwait(false);
                    break;
                case WeeklyQuestRewardType.MasterExperience:
                    await player.AddMasterExperienceAsync(reward.Amount, null).ConfigureAwait(false);
                    break;
                default:
                    // Items and money are handled above.
                    break;
            }
        }

        return true;
    }

    private static async ValueTask RollbackAsync(Player player, List<Item> addedItems)
    {
        foreach (var item in addedItems)
        {
            await player.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
            await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        }
    }

    private static ItemDefinition? ResolveItemDefinition(Player player, WeeklyQuestReward reward)
    {
        if (reward.Item is not { } configured)
        {
            return null;
        }

        // The configuration of the plugin may hold an instance of another context, so we use the one of the game.
        return player.GameContext.Configuration.Items
            .FirstOrDefault(i => i.Group == configured.Group && i.Number == configured.Number)
            ?? configured;
    }

    private static TemporaryItem CreateItem(ItemDefinition itemDefinition, WeeklyQuestReward reward)
    {
        var item = new TemporaryItem
        {
            Definition = itemDefinition,
            Level = Math.Min(reward.ItemLevel, itemDefinition.MaximumItemLevel),
            SocketCount = itemDefinition.MaximumSockets,
        };
        item.Durability = item.IsStackable() ? 1 : itemDefinition.Durability;
        item.HasSkill = itemDefinition.Skill is not null && reward.Skill;

        var possibleOptions = itemDefinition.PossibleItemOptions.SelectMany(o => o.PossibleOptions).ToList();
        if (reward.OptionLevel > 0
            && possibleOptions.FirstOrDefault(o => o.OptionType == ItemOptionTypes.Option) is { } option)
        {
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = option, Level = reward.OptionLevel });
        }

        if (reward.Luck
            && possibleOptions.FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck) is { } luck)
        {
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = luck });
        }

        if (reward.ExcellentOptions > 0)
        {
            var excellentOptions = possibleOptions
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .Where(o => ((1 << (o.Number - 1)) & reward.ExcellentOptions) > 0)
                .ToList();
            foreach (var excellentOption in excellentOptions)
            {
                item.ItemOptions.Add(new ItemOptionLink { ItemOption = excellentOption });
            }

            // Like the /item command: every excellent item has its skill, if the item definition has one.
            item.HasSkill |= excellentOptions.Count > 0 && itemDefinition.Skill is not null;
        }

        return item;
    }
}
