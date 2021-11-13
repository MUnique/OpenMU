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
    public void SetPin(Player player, string newPin, string accountPassword)
    {
        if (player.Account is null || player.IsVaultLocked)
        {
            player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.SetPinFailedBecauseLock);
            return;
        }

        if (BCrypt.Net.BCrypt.Verify(accountPassword, player.Account.PasswordHash))
        {
            player.Account.VaultPassword = newPin;
            player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.Unlocked);
        }
        else
        {
            player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.SetPinFailedBecauseLock);
        }
    }
}