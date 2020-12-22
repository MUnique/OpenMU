// <copyright file="RemoveVaultPinAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Vault
{
    using MUnique.OpenMU.GameLogic.Views.Vault;

    /// <summary>
    /// Action which removes the pin for the vault of a player, if the correct account password is provided.
    /// </summary>
    public class RemoveVaultPinAction
    {
        /// <summary>
        ///  Removes the pin for the vault of a player, if the correct account password is provided.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="accountPassword">The account password.</param>
        public void RemovePin(Player player, string accountPassword)
        {
            if (player.Account is not null && BCrypt.Net.BCrypt.Verify(accountPassword, player.Account.PasswordHash))
            {
                player.Account.VaultPassword = null;
                player.IsVaultLocked = false;
                player.ViewPlugIns.GetPlugIn<IUpdateVaultStatePlugIn>()?.UpdateState();
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.RemovePinFailedByWrongPassword);
            }
        }
    }
}