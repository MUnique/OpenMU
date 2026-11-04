// <copyright file="UnlockVaultAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Vault;

using MUnique.OpenMU.GameLogic.Views.Vault;

/// <summary>
/// Action which unlocks the vault of a player, if the correct pin is provided.
/// </summary>
public class UnlockVaultAction
{
    /// <summary>
    /// Unlocks the vault of a player, if the correct pin is provided.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="pin">The pin.</param>
    public async ValueTask UnlockVaultAsync(Player player, string pin)
    {
        VaultLockChangeResult result;
        if (player.Account?.VaultPassword == pin)
        {
            player.IsVaultLocked = false;
            result = VaultLockChangeResult.Unlocked;
        }
        else
        {
            result = VaultLockChangeResult.UnlockFailedByWrongPin;
        }

        await player.InvokeViewPlugInAsync<IShowVaultLockChangeResponse>(p => p.ShowResponseAsync(result)).ConfigureAwait(false);
    }
}