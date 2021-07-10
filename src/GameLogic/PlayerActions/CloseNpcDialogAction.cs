// <copyright file="CloseNpcDialogAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic.Views.NPC;

    /// <summary>
    /// Action to close a npc dialog.
    /// </summary>
    public class CloseNpcDialogAction
    {
        /// <summary>
        /// Closes the currently opened npc dialog.
        /// </summary>
        /// <param name="player">The player who wants to close the dialog.</param>
        public void CloseNpcDialog(Player player)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            var npc = player.OpenedNpc;
            if (npc != null && player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld))
            {
                player.Logger.LogDebug($"Player {player.SelectedCharacter?.Name} closes NPC {player.OpenedNpc}");
                player.OpenedNpc = null;
                player.Vault = null;
                player.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>()?.DialogClosed(npc.Definition);
            }
            else
            {
                player.Logger.LogDebug($"Dialog of NPC {player.OpenedNpc} could not be closed by player {player.SelectedCharacter?.Name} because the player has the wrong state {player.PlayerState}");
            }
        }
    }
}
