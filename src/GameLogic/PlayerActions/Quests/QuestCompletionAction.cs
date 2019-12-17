// <copyright file="QuestCompletionAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Views.Quest;

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
            var questState = player.GetQuestState(group, number);
            if (questState == null)
            {
                // todo log
                return;
            }

            var activeQuest = questState.ActiveQuest;
            if (activeQuest == null)
            {
                // todo log
                return;
            }

            foreach (var requiredItem in activeQuest.RequiredItems)
            {
                if (requiredItem.MinimumNumber > player.Inventory.Items.Count(i => i.Definition == requiredItem.Item))
                {
                    // todo log
                    return;
                }
            }

            foreach (var requiredKills in activeQuest.RequiredMonsterKills)
            {
                var currentKillCount = questState.RequirementStates.FirstOrDefault(r => r.Requirement == requiredKills)?.KillCount ?? 0;
                if (currentKillCount < requiredKills.MinimumNumber)
                {
                    // todo log
                    return;
                }
            }

            if (activeQuest.RequiresClientAction && !questState.ClientActionPerformed)
            {
                // todo log
                return;
            }

            questState.LastFinishedQuest = activeQuest;
            questState.Clear(player.PersistenceContext);
            player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()?.QuestCompleted(activeQuest);
        }
    }
}