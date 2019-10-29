// <copyright file="CloseVaultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ICloseVaultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("CloseVaultPlugIn", "The default implementation of the ICloseVaultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("3030e5aa-01a2-4523-b42e-2eef16f4b58f")]
    public class CloseVaultPlugIn : ICloseVaultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseVaultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public CloseVaultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void CloseVault()
        {
            using var writer = this.player.Connection.StartSafeWrite(VaultClosed.HeaderType, VaultClosed.Length);
            _ = new VaultClosed(writer.Span);
            writer.Commit();
        }
    }
}