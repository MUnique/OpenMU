// <copyright file="CloseNpcDialogAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using log4net;

    /// <summary>
    /// Action to close a npc dialog.
    /// </summary>
    public class CloseNpcDialogAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CloseNpcDialogAction));

        /// <summary>
        /// Closes the currently opened npc dialog.
        /// </summary>
        /// <param name="player">The player who wants to close the dialog.</param>
        public void CloseNpcDialog(Player player)
        {
            if (player.OpenedNpc != null && player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld))
            {
                Log.Debug($"Player {player.SelectedCharacter?.Name} closes NPC {player.OpenedNpc}");
                player.OpenedNpc = null;
                player.Vault = null;
            }
            else
            {
                Log.Debug($"Dialog of NPC {player.OpenedNpc} could not be closed by player {player.SelectedCharacter?.Name} because the player has the wrong state {player.PlayerState}");
            }
        }
    }
}
