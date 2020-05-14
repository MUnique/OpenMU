// <copyright file="QuestSelectAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System.Linq;
    using System.Reflection;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic.Views.Quest;

    /// <summary>
    /// A player action which implements the selecting of a quest in the quest list.
    /// </summary>
    public class QuestSelectAction
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Tries to select the quest of the given group and number for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="group">The <see cref="QuestDefinition.Group"/>.</param>
        /// <param name="number">The <see cref="QuestDefinition.StartingNumber"/>.</param>
        public void SelectQuest(Player player, short group, short number)
        {
            var quest = player.GetQuest(group, number);
            if (quest == null)
            {
                Log.Debug("Failed, quest not found");
                return;
            }

            var questState = player.SelectedCharacter.QuestStates.FirstOrDefault(q => q.Group == group);
            if (questState?.ActiveQuest != null)
            {
                Log.Debug("There is already an active quest of this group.");
                player.ViewPlugIns.GetPlugIn<IQuestProgressPlugIn>()?.ShowQuestProgress(questState.ActiveQuest, false);
                return;
            }

            if (questState?.LastFinishedQuest == quest && !quest.Repeatable)
            {
                Log.Debug("The quest is not repeatable.");
                return;
            }

            if (quest.MinimumCharacterLevel > player.Level || (quest.MaximumCharacterLevel > 0 && quest.MaximumCharacterLevel < player.Level))
            {
                Log.DebugFormat("Failed, character level {0} not in allowed range {1} to {2}.", player.Level, quest.MinimumCharacterLevel, quest.MaximumCharacterLevel);
                return;
            }

            if (quest.StartingNumber == number && quest.Number != number)
            {
                player.ViewPlugIns.GetPlugIn<IQuestStepInfoPlugIn>()?.ShowQuestStepInfo(quest.Group, quest.StartingNumber);
            }
        }
    }
}