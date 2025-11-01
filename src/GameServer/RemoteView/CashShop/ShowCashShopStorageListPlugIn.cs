// <copyright file="ShowCashShopStorageListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopStorageListPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopStorageListPlugIn), "The default implementation of the IShowCashShopStorageListPlugIn.")]
[Guid("E5F6A7B8-C9D0-1E2F-3A4B-5C6D7E8F9A0B")]
public class ShowCashShopStorageListPlugIn : IShowCashShopStorageListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopStorageListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopStorageListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopStorageListAsync()
    {
        var connection = this._player.Connection;
        if (connection is not { Connected: true })
        {
            return;
        }

        var storage = this._player.SelectedCharacter?.CashShopStorage;
        if (storage is null)
        {
            return;
        }

        var items = storage.Items;

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            int sizePerItem = StoredItemRef.GetRequiredSize(itemSerializer.NeededSpace);
            var size = CashShopStorageListResponseRef.GetRequiredSize(items.Count, sizePerItem);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CashShopStorageListResponseRef(span)
            {
                ItemCount = (byte)items.Count,
            };

            int headerSize = CashShopStorageListResponseRef.GetRequiredSize(0, 0);
            int actualSize = headerSize;
            foreach (var item in items)
            {
                if (item.Definition is null)
                {
                    this._player.Logger.LogWarning("Cash shop storage item at slot {0} has no definition.", item.ItemSlot);
                    packet.ItemCount--;
                    continue;
                }

                var storedItem = new StoredItemRef(span[actualSize..]);
                storedItem.ItemSlot = item.ItemSlot;
                var itemSize = itemSerializer.SerializeItem(storedItem.ItemData, item);
                actualSize += StoredItemRef.GetRequiredSize(itemSize);
            }

            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
