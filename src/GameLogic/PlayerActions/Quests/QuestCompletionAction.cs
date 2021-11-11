// <copyright file="QuestCompletionAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A player action which implements the completion of a quest.
/// </summary>
public class QuestCompletionAction
{
    /// <summary>
    /// Tries to completes the quest of the given group and number for the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The group.</param>
    /// <param name="number">The number.</param>
    public void CompleteQuest(Player player, short group, short number)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var questState = player.GetQuestState(group, number);
        var activeQuest = questState?.ActiveQuest;
        if (activeQuest is null)
        {
            player.Logger.LogDebug("Failed, no active quest");
            return;
        }

        foreach (var requiredItem in activeQuest.RequiredItems)
        {
            if (requiredItem.MinimumNumber > player.Inventory?.Items.Count(i => i.Definition == requiredItem.Item))
            {
                player.Logger.LogDebug("Failed, required item not found: {0}", requiredItem.Item!.Name);
                return;
            }
        }

        foreach (var requiredKills in activeQuest.RequiredMonsterKills)
        {
            var currentKillCount = questState!.RequirementStates.FirstOrDefault(r => r.Requirement == requiredKills)?.KillCount ?? 0;
            if (currentKillCount < requiredKills.MinimumNumber)
            {
                player.Logger.LogDebug("Failed, required kills of monster {0}: {1}/{2};", requiredKills.Monster?.Designation, currentKillCount, requiredKills.MinimumNumber);
                return;
            }
        }

        if (activeQuest.RequiresClientAction && !questState!.ClientActionPerformed)
        {
            player.Logger.LogDebug("Failed, client action not performed.");
            return;
        }

        foreach (var requiredItem in activeQuest.RequiredItems)
        {
            var items = player.Inventory!.Items
                .Where(item => item.Definition == requiredItem.Item)
                .Take(requiredItem.MinimumNumber)
                .ToList();

            foreach (var item in items)
            {
                player.ViewPlugIns.GetPlugIn<IItemRemovedPlugIn>()?.RemoveItem(item.ItemSlot);
                player.Inventory.RemoveItem(item);
                player.PersistenceContext.Delete(item);
            }
        }

        foreach (var reward in activeQuest.Rewards)
        {
            AddReward(player, reward);
        }

        questState!.LastFinishedQuest = activeQuest;
        questState.Clear(player.PersistenceContext);
        player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()?.QuestCompleted(activeQuest);
    }

    private static void AddReward(Player player, QuestReward reward)
    {
        switch (reward.RewardType)
        {
            case QuestRewardType.Attribute:
                var attribute = player.SelectedCharacter!.Attributes.FirstOrDefault(a => a.Definition == reward.AttributeReward);
                if (attribute is null)
                {
                    attribute = player.PersistenceContext.CreateNew<StatAttribute>(reward.AttributeReward, 0);
                    player.SelectedCharacter.Attributes.Add(attribute);
                }

                attribute.Value += reward.Value;

                player.ViewPlugIns.GetPlugIn<ILegacyQuestRewardPlugIn>()?.Show(player, QuestRewardType.Attribute, reward.Value, attribute.Definition);
                break;
            case QuestRewardType.Item:
                var item = player.PersistenceContext.CreateNew<Item>();
                item.AssignValues(reward.ItemReward ?? throw new InvalidOperationException($"Reward {reward.GetId()} is defined as item reward, but has no item assigned"));
                if (player.Inventory!.AddItem(item))
                {
                    player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()?.ItemAppear(item);
                }
                else
                {
                    player.CurrentMap?.Add(new DroppedItem(item, player.Position, player.CurrentMap, player, player.GetAsEnumerable()));
                }

                break;
            case QuestRewardType.LevelUpPoints:
                player.SelectedCharacter!.LevelUpPoints += reward.Value;
                player.ViewPlugIns.GetPlugIn<ILegacyQuestRewardPlugIn>()
                    ?.Show(player, QuestRewardType.LevelUpPoints, reward.Value, null);
                break;
            case QuestRewardType.CharacterEvolutionFirstToSecond:
                player.SelectedCharacter!.CharacterClass = player.SelectedCharacter.CharacterClass?.NextGenerationClass
                                                           ?? throw new InvalidOperationException($"Current character class has no next generation");
                player.ForEachWorldObserver(
                    o => o.ViewPlugIns.GetPlugIn<ILegacyQuestRewardPlugIn>()?.Show(
                        player,
                        QuestRewardType.CharacterEvolutionFirstToSecond,
                        reward.Value,
                        null), true);
                break;
            case QuestRewardType.CharacterEvolutionSecondToThird:
                player.SelectedCharacter!.CharacterClass = player.SelectedCharacter.CharacterClass?.NextGenerationClass
                                                           ?? throw new InvalidOperationException($"Current character class has no next generation");
                player.ForEachWorldObserver(
                    o => o.ViewPlugIns.GetPlugIn<ILegacyQuestRewardPlugIn>()?.Show(
                        player,
                        QuestRewardType.CharacterEvolutionSecondToThird,
                        reward.Value,
                        null), true);
                player.ViewPlugIns.GetPlugIn<IUpdateMasterStatsPlugIn>()?.SendMasterStats();
                break;
            case QuestRewardType.Experience:
                player.AddExperience(reward.Value, null);
                break;
            case QuestRewardType.Money:
                player.TryAddMoney(reward.Value);
                player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                break;
            case QuestRewardType.GensAttribution:
                // not yet implemented.
                break;
            default:
                player.Logger.LogWarning("Unknown reward type: {0}", reward.RewardType);
                break;
        }
    }
}