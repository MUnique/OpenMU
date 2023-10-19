// <copyright file="LogoutAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Views.Login;

/// <summary>
/// Action to log the player out of the game.
/// </summary>
public class LogoutAction
{
    /// <summary>
    /// Logs out the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="logoutType">Type of the logout.</param>
    public async ValueTask LogoutAsync(Player player, LogoutType logoutType)
    {
        player.CurrentMap?.RemoveAsync(player);
        player.Party?.KickMySelfAsync(player);
        await player.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await player.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);

        try
        {
            await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Couldn't Save at Disconnect. Player: {player}", player);
        }

        if (logoutType == LogoutType.CloseGame)
        {
            await player.InvokeViewPlugInAsync<ILogoutPlugIn>(p => p.LogoutAsync(logoutType)).ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
        }
        else
        {
            if (logoutType == LogoutType.BackToCharacterSelection)
            {
                await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<ILogoutPlugIn>(p => p.LogoutAsync(logoutType)).ConfigureAwait(false);
        }
    }
}