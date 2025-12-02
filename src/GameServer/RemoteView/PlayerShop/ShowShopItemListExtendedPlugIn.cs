// <copyright file="ShowShopItemListExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IShowShopItemListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowShopItemListExtendedPlugIn), "The extended implementation of the IShowShopItemListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("D64E9027-4801-46EE-9FD0-2FC66C33FE32")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ShowShopItemListExtendedPlugIn : IShowShopItemListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowShopItemListExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowShopItemListExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    /// <remarks>
    /// Maybe cache the result, because a lot of players could request the same list. However, this isn't critical.
    /// </remarks>
    public async ValueTask ShowShopItemListAsync(Player requestedPlayer, bool isUpdate)
    {
        var connection = this._player.Connection;
        if (connection is null || requestedPlayer.ShopStorage is null || requestedPlayer.SelectedCharacter is null)
        {
            return;
        }

        var itemSerializer = this._player.ItemSerializer;
        var playerId = requestedPlayer.GetId(this._player);
        var items = requestedPlayer.ShopStorage.Items.ToList();
        int Write()
        {
            var size = PlayerShopItemListExtendedRef.GetRequiredSize(items.Count, PlayerShopItemExtendedRef.GetRequiredSize(itemSerializer.NeededSpace));
            var span = connection.Output.GetSpan(size)[..size];
            _ = new PlayerShopItemListExtendedRef(span)
            {
                Action = isUpdate
                    ? PlayerShopItemListExtended.ActionKind.UpdateAfterItemChange
                    : PlayerShopItemListExtended.ActionKind.ByRequest,
                ItemCount = (byte)items.Count,
                PlayerId = playerId,
                PlayerName = requestedPlayer.SelectedCharacter.Name,
                ShopName = requestedPlayer.SelectedCharacter.StoreName,
            };

            int headerSize = PlayerShopItemListExtendedRef.GetRequiredSize(0, 0);
            int actualSize = headerSize;
            int i = 0;
            foreach (var item in items)
            {
                var itemBlock = new PlayerShopItemExtendedRef(span[actualSize..]);
                itemBlock.ItemSlot = item.ItemSlot;
                itemBlock.MoneyPrice = (uint)(item.StorePrice ?? 0);

                // todo: when we can define a price in items, set PriceItemType and RequiredItemAmount
                var itemSize = itemSerializer.SerializeItem(itemBlock.ItemData, item);
                actualSize += PlayerShopItemExtendedRef.GetRequiredSize(itemSize);
                i++;
            }

            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}