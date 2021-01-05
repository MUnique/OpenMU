// <copyright file="QuestStartAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A player action which implements the starting of a quest.
    /// </summary>
    public class QuestStartAction
    {
        /// <summary>
        /// Tries to start the quest of the given group and number for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="group">The group.</param>
        /// <param name="number">The number.</param>
        public void StartQuest(Player player, short group, short number)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            var quest = player.GetQuest(group, number);
            if (quest is null)
            {
                player.Logger.LogWarning("Failed, quest not found");
                return;
            }

            if (quest.MinimumCharacterLevel > player.Level || (quest.MaximumCharacterLevel > 0 && quest.MaximumCharacterLevel < player.Level))
            {
                player.Logger.LogDebug("Failed, character level {0} not in allowed range {1} to {2}.", player.Level, quest.MinimumCharacterLevel, quest.MaximumCharacterLevel);
                return;
            }

            var questState = player.SelectedCharacter!.QuestStates.FirstOrDefault(q => q.Group == group);
            if (questState is null)
            {
                questState = player.PersistenceContext.CreateNew<CharacterQuestState>();
                questState.Group = group;
                player.SelectedCharacter.QuestStates.Add(questState);
            }

            if (questState.ActiveQuest != null)
            {
                player.Logger.LogDebug("There is already an active quest of this group.");
                player.ViewPlugIns.GetPlugIn<IQuestProgressPlugIn>()?.ShowQuestProgress(questState.ActiveQuest, false);
                return;
            }

            if (questState.LastFinishedQuest == quest && !quest.Repeatable)
            {
                player.Logger.LogDebug("The quest is not repeatable.");
                return;
            }

            if (quest.RequiredStartMoney > 0)
            {
                if (player.TryRemoveMoney(quest.RequiredStartMoney))
                {
                    player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                }
                else
                {
                    player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Not enough money to proceed", MessageType.BlueNormal);
                    return;
                }
            }

            questState.Clear(player.PersistenceContext);
            questState.ActiveQuest = quest;
            player.ViewPlugIns.GetPlugIn<IQuestStartedPlugIn>()?.QuestStarted(quest);
        }
    }
}