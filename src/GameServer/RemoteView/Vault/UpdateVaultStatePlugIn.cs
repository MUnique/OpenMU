// <copyright file="UpdateVaultStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Vault
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Vault;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateVaultStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(UpdateVaultStatePlugIn), "The default implementation of the IUpdateVaultStatePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("1B0A5F5B-85A0-4228-9392-584E89FB8D88")]
    public class UpdateVaultStatePlugIn : IUpdateVaultStatePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVaultStatePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateVaultStatePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void UpdateState()
        {
            this.player.Connection?.SendVaultProtectionInformation(this.GetVaultState());
        }

        private VaultProtectionInformation.VaultProtectionState GetVaultState()
        {
            return this.player.IsVaultLocked switch
            {
                true => VaultProtectionInformation.VaultProtectionState.Locked,
                false when string.IsNullOrWhiteSpace(this.player.Account?.VaultPassword) => VaultProtectionInformation.VaultProtectionState.Unprotected,
                _ => VaultProtectionInformation.VaultProtectionState.Unlocked,
            };
        }
    }
}