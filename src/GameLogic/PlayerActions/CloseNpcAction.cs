// <copyright file="CloseNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using log4net;

    /// <summary>
    /// Action to close a npc dialog.
    /// </summary>
    public class CloseNpcAction
    {
        private static ILog log = LogManager.GetLogger(typeof(CloseNpcAction));

        /// <summary>
        /// Closes the Monster dialog.
        /// </summary>
        /// <param name="player">The player who wants to close the dialog.</param>
        public void CloseNpcDialog(Player player)
        {
            if (player.OpenedNpc != null && player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld))
            {
                log.Debug($"Player {player.SelectedCharacter.Name} closes Monster {player.OpenedNpc}");
                player.OpenedNpc = null;
                player.Vault = null;
            }
            else
            {
                log.Debug($"Dialog of Monster {player.OpenedNpc} could not be closed by player {player.SelectedCharacter?.Name} because the player has the wrong state {player.PlayerState}");
            }
        }
    }
}
