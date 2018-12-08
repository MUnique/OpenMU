// <copyright file="SelectCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System.Linq;
    using log4net;

    /// <summary>
    /// Action to select a character and enter the world with it.
    /// </summary>
    public class SelectCharacterAction
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SelectCharacterAction));

        /// <summary>
        /// Selects the character and enters the world.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">Name of the character.</param>
        public void SelectCharacter(Player player, string characterName)
        {
            if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
            {
                Logger.ErrorFormat("Could not select character because of wrong current player state: {0}", player.PlayerState.CurrentState);
                player.Disconnect();
                return;
            }

            player.SelectedCharacter = player.Account.Characters.FirstOrDefault(c => c.Name.Equals(characterName));
            if (player.SelectedCharacter == null)
            {
                Logger.ErrorFormat("Could not select character because character not found: [{0}]", characterName);
                player.Disconnect();
            }
        }
    }
}
