// <copyright file="SaveKeyConfigurationAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    /// <summary>
    /// Action to save the players key configuration (hotkeys for skills, potions etc.).
    /// </summary>
    public class SaveKeyConfigurationAction
    {
        /// <summary>
        /// Saves the key configuration.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="keyConfiguration">The key configuration.</param>
        public void SaveKeyConfiguration(Player player, byte[] keyConfiguration)
        {
            player.SelectedCharacter.KeyConfiguration = keyConfiguration;
        }
    }
}
