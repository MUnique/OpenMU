// <copyright file="QuestClientAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests;

/// <summary>
/// Is called when the player completed the an action (e.g. the tutorial).
/// The server checks if the specified quest is currently in progress.
/// If the quest got a Condition(condition type 0x10) for this flag,
/// the condition is flagged as fulfilled.
/// </summary>
public class QuestClientAction
{
    /// <summary>
    /// Clients the action.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The group.</param>
    /// <param name="number">The number.</param>
    public void ClientAction(Player player, short group, short number)
    {
        var questState = player.SelectedCharacter?.QuestStates.FirstOrDefault(s => s.Group == group && s.ActiveQuest?.Number == number && s.ActiveQuest.RequiresClientAction);
        if (questState is null)
        {
            return;
        }

        questState.ClientActionPerformed = true;
    }
}