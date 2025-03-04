// <copyright file="CloseNpcDialogAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Action to close a npc dialog.
/// </summary>
public class CloseNpcDialogAction
{
    private const ushort ChaosGoblinId = 238;

    /// <summary>
    /// Closes the currently opened npc dialog.
    /// </summary>
    /// <param name="player">The player who wants to close the dialog.</param>
    public async ValueTask CloseNpcDialogAsync(Player player)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var npc = player.OpenedNpc;
        if (npc != null && await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            player.Logger.LogDebug($"Player {player.SelectedCharacter?.Name} closes NPC {player.OpenedNpc}");
            player.OpenedNpc = null;
            player.Vault = null;
            await player.InvokeViewPlugInAsync<INpcDialogClosedPlugIn>(p => p.DialogClosedAsync(npc.Definition)).ConfigureAwait(false);
            if (npc.Id == ChaosGoblinId)
            {
                try
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    player.Logger.LogInformation("Saving changes after closing the chaos goblin ...");
                    await player.SaveProgressAsync().ConfigureAwait(false);
                    player.Logger.LogInformation("Saved changes after closing the chaos goblin ...");
                }
                catch (Exception ex)
                {
                    player.Logger.LogError(ex, "Couldn't save changes after closing the chaos goblin for player {player}", player);
                }
            }
        }
        else
        {
            player.Logger.LogDebug($"Dialog of NPC {player.OpenedNpc} could not be closed by player {player.SelectedCharacter?.Name} because the player has the wrong state {player.PlayerState}");
        }
    }
}