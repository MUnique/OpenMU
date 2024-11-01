// <copyright file="QuestStructExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using QuestReward = MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward;

/// <summary>
/// Extensions for quest message structs.
/// </summary>
public static class QuestStructExtensions
{
    /// <summary>
    /// Sends the quest states of the legacy quest system.
    /// </summary>
    /// <param name="questState">State of the quest.</param>
    /// <param name="player">The player.</param>
    internal static async ValueTask SendLegacyQuestStateAsync(this CharacterQuestState? questState, RemotePlayer player)
    {
        var connection = player.Connection;
        if (connection is null)
        {
            return;
        }

        var questCount = 7;
        int Write()
        {
            var size = LegacyQuestStateListRef.GetRequiredSize(questCount);
            var span = connection.Output.GetSpan(size)[..size];
            var message = new LegacyQuestStateListRef(span)
            {
                QuestCount = (byte)questCount,
            };

            if (questState?.LastFinishedQuest != null)
            {
                for (int i = 0; i <= questState.LastFinishedQuest.Number; i++)
                {
                    message[i] = LegacyQuestState.Complete;
                }
            }

            if (questState?.ActiveQuest != null)
            {
                message[questState.ActiveQuest.Number] = LegacyQuestState.Active;
            }

            if (player.SelectedCharacter?.CharacterClass?.GetBaseClass(player.GameContext.Configuration).Number != (byte)CharacterClassNumber.DarkKnight)
            {
                message.SecretOfDarkStoneState = LegacyQuestState.Undefined;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <summary>
    /// Assigns the <paramref name="questState"/> to this <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="questState">State of the quest.</param>
    /// <param name="player">The player.</param>
    internal static void AssignActiveQuestData(this QuestStateRef message, CharacterQuestState? questState, RemotePlayer player)
    {
        var activeQuest = questState?.ActiveQuest;
        if (activeQuest is null)
        {
            return;
        }

        message.QuestNumber = (ushort)activeQuest.Number;

        var itemSerializer = player.ItemSerializer;
        if (activeQuest.RequiredItems.Any())
        {
            message.ConditionCount = (byte)activeQuest.RequiredItems.Count;
            int i = 0;
            foreach (var requiredItem in activeQuest.RequiredItems)
            {
                requiredItem.AssignTo(message.GetQuestCondition(i), player);
                i++;
            }
        }
        else if (activeQuest.RequiredMonsterKills.Any())
        {
            message.ConditionCount = (byte)activeQuest.RequiredMonsterKills.Count;
            int i = 0;
            foreach (var requiredKill in activeQuest.RequiredMonsterKills)
            {
                requiredKill.AssignTo(message.GetQuestCondition(i), questState!);
                i++;
            }
        }
        else
        {
            // no requirement states available
        }

        int r = 0;
        foreach (var reward in activeQuest.Rewards)
        {
            reward.AssignTo(message.GetQuestReward(r), itemSerializer);
            r++;
        }

        message.RewardCount = (byte)r;
    }

    /// <summary>
    /// Assigns the <paramref name="questState"/> to this <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="questState">State of the quest.</param>
    /// <param name="player">The player.</param>
    internal static void AssignActiveQuestData(this QuestStateExtendedRef message, CharacterQuestState? questState, RemotePlayer player)
    {
        var activeQuest = questState?.ActiveQuest;
        if (activeQuest is null)
        {
            return;
        }

        message.QuestNumber = (ushort)activeQuest.Number;

        var itemSerializer = player.ItemSerializer;
        if (activeQuest.RequiredItems.Any())
        {
            message.ConditionCount = (byte)activeQuest.RequiredItems.Count;
            int i = 0;
            foreach (var requiredItem in activeQuest.RequiredItems)
            {
                requiredItem.AssignTo(message.GetQuestConditionExtended(i), player);
                i++;
            }
        }
        else if (activeQuest.RequiredMonsterKills.Any())
        {
            message.ConditionCount = (byte)activeQuest.RequiredMonsterKills.Count;
            int i = 0;
            foreach (var requiredKill in activeQuest.RequiredMonsterKills)
            {
                requiredKill.AssignTo(message.GetQuestConditionExtended(i), questState!);
                i++;
            }
        }
        else
        {
            // no requirement states available
        }

        int r = 0;
        foreach (var reward in activeQuest.Rewards)
        {
            reward.AssignTo(message.GetQuestRewardExtended(r), itemSerializer);
            r++;
        }

        message.RewardCount = (byte)r;
    }

    private static void AssignTo(this QuestItemRequirement itemRequirement, QuestConditionRef condition, RemotePlayer player)
    {
        condition.Type = ConditionType.Item;
        condition.RequiredCount = (uint)itemRequirement.MinimumNumber;
        condition.CurrentCount = (uint)(player.Inventory?.Items.Count(item => Equals(item.Definition, itemRequirement.Item)) ?? 0);
        condition.RequirementId = itemRequirement.Item!.GetItemType();
        var temporaryItem = new TemporaryItem { Definition = itemRequirement.Item };
        temporaryItem.Durability = temporaryItem.GetMaximumDurabilityOfOnePiece();
        player.ItemSerializer.SerializeItem(condition.RequiredItemData, temporaryItem);
    }

    private static void AssignTo(this QuestItemRequirement itemRequirement, QuestConditionExtendedRef condition, RemotePlayer player)
    {
        condition.Type = ConditionType.Item;
        condition.RequiredCount = (uint)itemRequirement.MinimumNumber;
        condition.CurrentCount = (uint)(player.Inventory?.Items.Count(item => Equals(item.Definition, itemRequirement.Item)) ?? 0);
        condition.RequirementId = itemRequirement.Item!.GetItemType();
        var temporaryItem = new TemporaryItem { Definition = itemRequirement.Item };
        temporaryItem.Durability = temporaryItem.GetMaximumDurabilityOfOnePiece();
        player.ItemSerializer.SerializeItem(condition.RequiredItemData, temporaryItem);
    }

    private static void AssignTo(this QuestMonsterKillRequirement killRequirement, QuestConditionRef condition, CharacterQuestState questState)
    {
        condition.Type = ConditionType.MonsterKills;
        condition.RequiredCount = (uint)killRequirement.MinimumNumber;
        condition.RequirementId = (ushort)killRequirement.Monster!.Number;
        condition.CurrentCount = (uint)(questState.RequirementStates.FirstOrDefault(s => s.Requirement != null && s.Requirement.Equals(killRequirement))?.KillCount ?? 0);
    }

    private static void AssignTo(this QuestMonsterKillRequirement killRequirement, QuestConditionExtendedRef condition, CharacterQuestState questState)
    {
        condition.Type = ConditionType.MonsterKills;
        condition.RequiredCount = (uint)killRequirement.MinimumNumber;
        condition.RequirementId = (ushort)killRequirement.Monster!.Number;
        condition.CurrentCount = (uint)(questState.RequirementStates.FirstOrDefault(s => s.Requirement != null && s.Requirement.Equals(killRequirement))?.KillCount ?? 0);
    }

    private static void AssignTo(this QuestReward questReward, Network.Packets.ServerToClient.QuestRewardRef rewardStruct, IItemSerializer itemSerializer)
    {
        rewardStruct.Type = questReward.RewardType.Convert();
        rewardStruct.RewardCount = (uint)questReward.Value;
        if (questReward.RewardType == QuestRewardType.Item && questReward.ItemReward is { } itemReward)
        {
            rewardStruct.RewardId = itemReward.Definition!.GetItemType();
            itemSerializer.SerializeItem(rewardStruct.RewardedItemData, itemReward);
        }
    }

    private static void AssignTo(this QuestReward questReward, Network.Packets.ServerToClient.QuestRewardExtendedRef rewardStruct, IItemSerializer itemSerializer)
    {
        rewardStruct.Type = questReward.RewardType.Convert();
        rewardStruct.RewardCount = (uint)questReward.Value;
        if (questReward.RewardType == QuestRewardType.Item && questReward.ItemReward is { } itemReward)
        {
            rewardStruct.RewardId = itemReward.Definition!.GetItemType();
            itemSerializer.SerializeItem(rewardStruct.RewardedItemData, itemReward);
        }
    }

    private static ushort GetItemType(this ItemDefinition item)
    {
        // since the original game server/client supports up to 512 items per group, we have to shift the group by 9 bytes instead of 8.
        return (ushort)((ushort)(item.Group << 9) | (ushort)item.Number);
    }
}