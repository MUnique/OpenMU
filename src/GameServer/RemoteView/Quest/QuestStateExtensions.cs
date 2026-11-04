// <copyright file="QuestStateExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network.Packets;

/// <summary>
/// Extensions regarding the quest state of a player.
/// </summary>
internal static class QuestStateExtensions
{
    /// <summary>
    /// Gets the quest state byte for the current quest state of the legacy quest group.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The quest state byte for the current quest state of the legacy quest group.</returns>
    /// <remarks>
    /// Coming to this solution was pure brainfuck!.
    /// </remarks>
    public static byte GetLegacyQuestStateByte(this Player player)
    {
        var legacyQuestState = player.GetQuestState(QuestConstants.LegacyQuestGroup);

        // The first case is the easiest:
        if (legacyQuestState is null)
        {
            // In this case, all Quests are inactive, including the starting one
            return 0xFF;
        }

        // The next one is obvious, too:
        if (legacyQuestState.LastFinishedQuest is null && legacyQuestState.ActiveQuest is null)
        {
            // In this case, it depends if we have an active quest
            return (byte)(0b1111_1100 | (legacyQuestState.ActiveQuest is { }
                ? (byte)LegacyQuestState.Active
                : (byte)LegacyQuestState.Inactive));
        }

        var quest = legacyQuestState.ActiveQuest ?? player.GetNextLegacyQuest() ?? legacyQuestState.LastFinishedQuest;
        if (quest is null)
        {
            // Should never happen.
            return 0xFF;
        }

        var startOffset = (quest.Number / 4) * 4; // 4 quests per byte
        var result = 0;
        for (int i = 0; i < 4; i++)
        {
            var shift = i * 2;
            if ((legacyQuestState.LastFinishedQuest?.Number >= i + startOffset)
                || (legacyQuestState.ActiveQuest?.Number > i + startOffset))
            {
                // This case is a bit simplified. There may be previous quests which are not meant for the character class.
                // For the sake of simplicity, we leave it like that, for now.
                result |= (byte)LegacyQuestState.Complete << shift;
            }
            else if (legacyQuestState.ActiveQuest == quest && quest.Number == i + startOffset)
            {
                result |= (byte)LegacyQuestState.Active << shift;
            }
            else
            {
                // This is also simplified. This quest may not exist,
                // then we wouldn't OR a new state, and just leave these bits as 0.
                result |= (byte)LegacyQuestState.Inactive << shift;
            }
        }

        return (byte)result;
    }

    /// <summary>
    /// Gets the next legacy quest of the currently opened NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The next legacy quest of the currently opened NPC.</returns>
    public static QuestDefinition? GetNextLegacyQuest(this Player player)
    {
        var legacyQuestState = player.GetQuestState(QuestConstants.LegacyQuestGroup);
        return player.GetAvailableQuestsOfOpenedNpc()
            .OrderBy(quest => quest.Number)
            .FirstOrDefault(quest => quest.Number > (legacyQuestState?.LastFinishedQuest?.Number ?? -1));
    }
}