// <copyright file="ShowShopItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowShopItemListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowShopItemListPlugIn", "The default implementation of the IShowShopItemListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("14220c30-8a5e-458b-8fb5-f4c6bbc2c4a9")]
    public class ShowShopItemListPlugIn : IShowShopItemListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowShopItemListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowShopItemListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        /// <remarks>
        /// Maybe cache the result, because a lot of players could request the same list. However, this isn't critical.
        /// </remarks>
        public void ShowShopItemList(Player requestedPlayer, bool isUpdate)
        {
            var itemSerializer = this.player.ItemSerializer;
            var playerId = requestedPlayer.GetId(this.player);
            var items = requestedPlayer.ShopStorage.Items.ToList();
            using var writer = this.player.Connection.StartSafeWrite(PlayerShopItemList.HeaderType, PlayerShopItemList.GetRequiredSize(items.Count));

            var packet = new PlayerShopItemList(writer.Span)
            {
                Action = isUpdate ? PlayerShopItemList.ActionKind.UpdateAfterItemChange : PlayerShopItemList.ActionKind.ByRequest,
                ItemCount = (byte)items.Count,
                PlayerId = playerId,
                PlayerName = requestedPlayer.SelectedCharacter.Name,
                ShopName = requestedPlayer.ShopStorage.StoreName,
            };

            int i = 0;
            foreach (var item in items)
            {
                var itemBlock = packet[i];
                itemBlock.ItemSlot = item.ItemSlot;
                itemBlock.Price = (uint)(item.StorePrice ?? 0);
                itemSerializer.SerializeItem(itemBlock.ItemData, item);
                i++;
            }

            writer.Commit();
        }
    }
}