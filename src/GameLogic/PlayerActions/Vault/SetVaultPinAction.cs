// <copyright file="SetVaultPinAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Vault;

using MUnique.OpenMU.GameLogic.Views.Vault;

/// <summary>
/// Action which sets a new pin for the vault of a player, if the correct account password is provided.
/// </summary>
public class SetVaultPinAction
{
    /// <summary>
    /// Sets a new pin for the vault of a player, if the correct account password is provided.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="newPin">The new pin.</param>
    /// <param name="accountPassword">The account password.</param>
    public async ValueTask SetPinAsync(Player player, string newPin, string accountPassword)
    {
        if (player.Account is null || player.IsVaultLocked)
        {
            await player.InvokeViewPlugInAsync<IShowVaultLockChangeResponse>(p => p.ShowResponseAsync(VaultLockChangeResult.SetPinFailedBecauseLock)).ConfigureAwait(false);
            return;
        }

        if (BCrypt.Net.BCrypt.Verify(accountPassword, player.Account.PasswordHash))
        {
            player.Account.VaultPassword = newPin;
            await player.InvokeViewPlugInAsync<IShowVaultLockChangeResponse>(p => p.ShowResponseAsync(VaultLockChangeResult.Unlocked)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowVaultLockChangeResponse>(p => p.ShowResponseAsync(VaultLockChangeResult.SetPinFailedBecauseLock)).ConfigureAwait(false);
        }
    }
}