// <copyright file="FocusCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System.Linq;

    /// <summary>
    /// Action to focus a character in the character selection screen.
    /// </summary>
    public class FocusCharacterAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FocusCharacterAction"/> class.
        /// </summary>
        public FocusCharacterAction()
        {
        }

        /// <summary>
        /// Focuses the character.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">Name of the character.</param>
        public void FocusCharacter(Player player, string characterName)
        {
            if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
            {
                return;
            }

            var character = player.Account.Characters.FirstOrDefault(c => c.Name == characterName);
            if (character != null)
            {
                player.PlayerView.CharacterFocused(character);
            }
        }
    }
}
