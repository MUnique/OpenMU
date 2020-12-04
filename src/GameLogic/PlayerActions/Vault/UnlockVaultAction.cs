// <copyright file="UnlockVaultAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Vault
{
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
        public void UnlockVault(Player player, string pin)
        {
            if (player.Account.VaultPassword == pin)
            {
                player.IsVaultLocked = false;
                player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.Unlocked);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowVaultLockChangeResponse>()?.ShowResponse(VaultLockChangeResult.UnlockFailedByWrongPin);
            }
        }
    }
}
