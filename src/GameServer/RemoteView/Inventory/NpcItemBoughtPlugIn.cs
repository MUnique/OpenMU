// <copyright file="NpcItemBoughtPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="INpcItemBoughtPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("NpcItemBoughtPlugIn", "The default implementation of the INpcItemBoughtPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("cf45b5e2-158a-4998-bc73-fed4d4d31c0c")]
    public class NpcItemBoughtPlugIn : INpcItemBoughtPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcItemBoughtPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public NpcItemBoughtPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void NpcItemBought(Item newItem)
        {
            var itemSerializer = this.player.ItemSerializer;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 4 + itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0x32;
                message[3] = newItem.ItemSlot;
                itemSerializer.SerializeItem(message.Slice(4), newItem);
                writer.Commit();
            }
        }
    }
}