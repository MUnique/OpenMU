// <copyright file="UpdateVaultMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Vault
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Vault;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateVaultMoneyPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateVaultMoneyPlugIn", "The default implementation of the IUpdateVaultMoneyPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("78b0567d-4976-4861-bea2-9561ea166199")]
    public class UpdateVaultMoneyPlugIn : IUpdateVaultMoneyPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVaultMoneyPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateVaultMoneyPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateVaultMoney(bool success)
        {
            this.player.Connection?.SendVaultMoneyUpdate(
                success,
                success ? (uint)this.player.Account!.Vault!.Money : 0,
                success ? (uint)this.player.Money : 0);
        }
    }
}