// <copyright file="ItemPriceSetResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemPriceSetResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemPriceSetResponsePlugIn", "The default implementation of the IItemPriceSetResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f83daf12-28cb-47bc-bb23-7f8eba21c97c")]
    public class ItemPriceSetResponsePlugIn : IItemPriceSetResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPriceSetResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemPriceSetResponsePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ItemPriceSetResponse(byte itemSlot, ItemPriceResult result)
        {
            using var writer = this.player.Connection.StartSafeWrite(PlayerShopSetItemPriceResponse.HeaderType, PlayerShopSetItemPriceResponse.Length);
            _ = new PlayerShopSetItemPriceResponse(writer.Span)
            {
                InventorySlot = itemSlot,
                Result = result.Convert(),
            };

            writer.Commit();
        }
    }
}