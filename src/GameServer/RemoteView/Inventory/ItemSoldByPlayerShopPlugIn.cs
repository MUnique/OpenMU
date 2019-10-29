// <copyright file="ItemSoldByPlayerShopPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemSoldByPlayerShopPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemSoldByPlayerShopPlugIn", "The default implementation of the IItemSoldByPlayerShopPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9594f5db-53b3-491f-a99c-11554c077942")]
    public class ItemSoldByPlayerShopPlugIn : IItemSoldByPlayerShopPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSoldByPlayerShopPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemSoldByPlayerShopPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemSoldByPlayerShop(byte slot, Player buyer)
        {
            using var writer = this.player.Connection.StartSafeWrite(PlayerShopItemSoldToPlayer.HeaderType, PlayerShopItemSoldToPlayer.Length);
            _ = new PlayerShopItemSoldToPlayer(writer.Span)
            {
                InventorySlot = slot,
                BuyerName = buyer.SelectedCharacter.Name,
            };
            writer.Commit();
        }
    }
}