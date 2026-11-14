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
    private const short ChaosGoblinNumber = 238;

    /// <summary>
    /// Closes the currently opened npc dialog.
    /// </summary>
    /// <param name="player">The player who wants to close the dialog.</param>
    public ValueTask CloseNpcDialogAsync(Player player) => this.CloseNpcDialogAsync(player, player.OpenedNpcContext);

    /// <summary>Closes only the captured opening, without clearing a successor's dialog resources.</summary>
    /// <param name="player">The player.</param>
    /// <param name="expectedDialog">The immutable opening to close; null never selects another opening.</param>
    internal async ValueTask CloseNpcDialogAsync(Player player, Player.NpcDialogContext? expectedDialog)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (expectedDialog is not { } dialog)
        {
            player.Logger.LogDebug($"Captured dialog could not be closed by player {player.SelectedCharacter?.Name} in state {player.PlayerState}.");
            return;
        }

        var view = player.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>();
        var response = ValueTask.CompletedTask;
        var closed = false;
        try
        {
            await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
            {
                context.Allowed &= ReferenceEquals(player.OpenedNpcContext, dialog)
                    && player.PlayerState.CurrentState == PlayerState.NpcDialogOpened;
                if (context.Allowed)
                {
                    player.OpenedNpc = null;
                    player.Vault = null;
                    if (dialog.Npc.Definition.ItemCraftings.Any())
                    {
                        // Only this opening owns the disconnect snapshot; never clear a successor's backup.
                        player.BackupInventory = null;
                    }

                    try
                    {
                        response = view?.DialogClosedAsync(dialog.Npc.Definition) ?? ValueTask.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        response = ValueTask.FromException(ex);
                    }

                    closed = true;
                }
            }

            if (closed)
            {
                player.Logger.LogDebug($"Player {player.SelectedCharacter?.Name} closes NPC {dialog.Npc}");
            }
            else
            {
                player.Logger.LogDebug($"Captured dialog could not be closed by player {player.SelectedCharacter?.Name} in state {player.PlayerState}.");
            }
        }
        finally
        {
            // Capture once before World is published; completion and persistence never hold state exclusion.
            try
            {
                await response.ConfigureAwait(false);
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("Writing is not allowed after writer was completed", StringComparison.Ordinal))
            {
                // The connection closed while its result was pending, as in ordinary view invocation.
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Error when invoking view action of {viewType}", typeof(INpcDialogClosedPlugIn));
            }
        }

        if (closed && dialog.Npc.Definition.Number == ChaosGoblinNumber)
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
}
