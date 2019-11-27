// <copyright file="ItemSoldToNpcPlugIn.cs" company="MUnique">
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
            using var writer = this.player.Connection.StartSafeWrite(NpcItemSellResult.HeaderType, NpcItemSellResult.Length);
            _ = new NpcItemSellResult(writer.Span)
            {
                Success = success,
                Money = (uint)this.player.Money,
            };

            writer.Commit();
        }
    }
}