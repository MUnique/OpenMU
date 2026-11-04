// <copyright file="QuestSelectAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests;

using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// A player action which implements the selecting of a quest in the quest list.
/// </summary>
public class QuestSelectAction
{
    /// <summary>
    /// Tries to select the quest of the given group and number for the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The <see cref="QuestDefinition.Group"/>.</param>
    /// <param name="number">The <see cref="QuestDefinition.StartingNumber"/>.</param>
    public async ValueTask SelectQuestAsync(Player player, short group, short number)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var quest = player.GetQuest(group, number);
        if (quest is null)
        {
            player.Logger.LogDebug("Failed, quest not found");
            return;
        }

        var questState = player.SelectedCharacter?.QuestStates.FirstOrDefault(q => q.Group == group);
        if (questState?.ActiveQuest != null)
        {
            player.Logger.LogDebug("There is already an active quest of this group.");
            await player.InvokeViewPlugInAsync<IQuestProgressPlugIn>(p => p.ShowQuestProgressAsync(questState.ActiveQuest, false)).ConfigureAwait(false);
            return;
        }

        if (quest.Equals(questState?.LastFinishedQuest) && !quest.Repeatable)
        {
            player.Logger.LogDebug("The quest is not repeatable.");
            return;
        }

        if (quest.MinimumCharacterLevel > player.Level || (quest.MaximumCharacterLevel > 0 && quest.MaximumCharacterLevel < player.Level))
        {
            player.Logger.LogDebug("Failed, character level {0} not in allowed range {1} to {2}.", player.Level, quest.MinimumCharacterLevel, quest.MaximumCharacterLevel);
            return;
        }

        if (quest.StartingNumber == number && quest.Number != number)
        {
            await player.InvokeViewPlugInAsync<IQuestStepInfoPlugIn>(p => p.ShowQuestStepInfoAsync(quest.Group, quest.StartingNumber)).ConfigureAwait(false);
        }
    }
}