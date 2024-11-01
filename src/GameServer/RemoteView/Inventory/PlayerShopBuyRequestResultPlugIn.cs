// <copyright file="PlayerShopBuyRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IPlayerShopBuyRequestResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(PlayerShopBuyRequestResultPlugIn), "The default implementation of the IPlayerShopBuyRequestResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("3b2498f2-3ae8-4700-8a61-1ffe49822caf")]
public class PlayerShopBuyRequestResultPlugIn : IPlayerShopBuyRequestResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerShopBuyRequestResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerShopBuyRequestResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowResultAsync(IIdentifiable? seller, ItemBuyResult result, Item? item)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        if (item is not null)
        {
            var itemSerializer = this._player.ItemSerializer;
            var array = new byte[itemSerializer.NeededSpace];
            itemSerializer.SerializeItem(array, item);
            await connection.SendPlayerShopBuyResultAsync(Convert(result), seller?.GetId(this._player) ?? ushort.MaxValue, array, item.ItemSlot).ConfigureAwait(false);
            return;
        }

        await connection.SendPlayerShopBuyResultAsync(Convert(result), seller?.GetId(this._player) ?? ushort.MaxValue, Array.Empty<byte>(), 0).ConfigureAwait(false);
    }

    private static PlayerShopBuyResult.ResultKind Convert(ItemBuyResult result)
    {
        return result switch
        {
            ItemBuyResult.Success => PlayerShopBuyResult.ResultKind.Success,
            ItemBuyResult.NotAvailable => PlayerShopBuyResult.ResultKind.NotAvailable,
            ItemBuyResult.ShopNotOpened => PlayerShopBuyResult.ResultKind.ShopNotOpened,
            ItemBuyResult.InTransaction => PlayerShopBuyResult.ResultKind.InTransaction,
            ItemBuyResult.InvalidShopSlot => PlayerShopBuyResult.ResultKind.InvalidShopSlot,
            ItemBuyResult.NameMismatchOrPriceMissing => PlayerShopBuyResult.ResultKind.NameMismatchOrPriceMissing,
            ItemBuyResult.LackOfMoney => PlayerShopBuyResult.ResultKind.LackOfMoney,
            ItemBuyResult.MoneyOverflowOrNotEnoughSpace => PlayerShopBuyResult.ResultKind.MoneyOverflowOrNotEnoughSpace,
            ItemBuyResult.ItemBlock => PlayerShopBuyResult.ResultKind.ItemBlock,
            _ => PlayerShopBuyResult.ResultKind.Undefined,
        };
    }
}