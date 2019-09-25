// <copyright file="ItemSoldToNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemSoldToNpcPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemSoldToNpcPlugIn", "The default implementation of the IItemSoldToNpcPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("8372476a-7fb9-4f6e-a857-41c39c7d377c")]
    public class ItemSoldToNpcPlugIn : IItemSoldToNpcPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSoldToNpcPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemSoldToNpcPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemSoldToNpc(bool success)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 8))
            {
                var message = writer.Span;
                message[2] = 0x33;
                message[3] = success ? (byte)1 : (byte)0;
                message.Slice(4).SetIntegerBigEndian((uint)this.player.Money);
                writer.Commit();
            }
        }
    }
}