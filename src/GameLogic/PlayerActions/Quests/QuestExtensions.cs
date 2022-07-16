// <copyright file="QuestExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests;

using MUnique.OpenMU.Persistence;

/// <summary>
/// Extensions regarding quests.
/// </summary>
public static class QuestExtensions
{
    /// <summary>
    /// Clears the quest state for the specified player.
    /// </summary>
    /// <param name="questState">State of the quest.</param>
    /// <param name="persistenceContext">The persistence context of the player.</param>
    public static async ValueTask ClearAsync(this CharacterQuestState questState, IContext persistenceContext)
    {
        questState.ActiveQuest = null;
        questState.ClientActionPerformed = false;
        foreach (var requirementState in questState.RequirementStates)
        {
            await persistenceContext.DeleteAsync(requirementState).ConfigureAwait(false);
        }

        questState.RequirementStates.Clear();
    }
}