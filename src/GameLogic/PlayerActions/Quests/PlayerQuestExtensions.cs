// <copyright file="PlayerQuestExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Quest related extension methods.
    /// </summary>
    public static class PlayerQuestExtensions
    {
        /// <summary>
        /// Gets the state of the quest.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="group">The group of the quest.</param>
        /// <param name="number">The number of the quest.</param>
        /// <returns>The quest state, if found; Otherwise, <c>null</c>.</returns>
        public static CharacterQuestState GetQuestState(this Player player, short group, short number)
        {
            return player.SelectedCharacter?.QuestStates.FirstOrDefault(state => state.Group == group && state.ActiveQuest?.Number == number);
        }
    }
}