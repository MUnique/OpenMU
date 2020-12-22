// <copyright file="SelectCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System.Linq;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Action to select a character and enter the world with it.
    /// </summary>
    public class SelectCharacterAction
    {
        /// <summary>
        /// Selects the character and enters the world.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">Name of the character.</param>
        public void SelectCharacter(Player player, string characterName)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
            {
                player.Logger.LogError("Could not select character because of wrong current player state: {0}", player.PlayerState.CurrentState);
                player.Disconnect();
                return;
            }

            player.SelectedCharacter = player.Account?.Characters.FirstOrDefault(c => c.Name.Equals(characterName));
            if (player.SelectedCharacter is null)
            {
                player.Logger.LogError("Could not select character because character not found: [{0}]", characterName);
                player.Disconnect();
            }
        }
    }
}
