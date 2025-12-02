// <copyright file="PlayerShopBuyRequestResultExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IPlayerShopBuyRequestResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(PlayerShopBuyRequestResultExtendedPlugIn), "The extended implementation of the IPlayerShopBuyRequestResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("83C89473-5977-4E08-82CF-94BC68C50676")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class PlayerShopBuyRequestResultExtendedPlugIn : IPlayerShopBuyRequestResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerShopBuyRequestResultExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerShopBuyRequestResultExtendedPlugIn(RemotePlayer player) => this._player = player;

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
            await connection.SendPlayerShopBuyResultExtendedAsync(seller?.GetId(this._player) ?? ushort.MaxValue, Convert(result), item.ItemSlot, array).ConfigureAwait(false);
            return;
        }

        await connection.SendPlayerShopBuyResultExtendedAsync(seller?.GetId(this._player) ?? ushort.MaxValue, Convert(result), 0, Array.Empty<byte>()).ConfigureAwait(false);
    }

    private static PlayerShopBuyResultExtended.ResultKind Convert(ItemBuyResult result)
    {
        return result switch
        {
            ItemBuyResult.Success => PlayerShopBuyResultExtended.ResultKind.Success,
            ItemBuyResult.NotAvailable => PlayerShopBuyResultExtended.ResultKind.NotAvailable,
            ItemBuyResult.ShopNotOpened => PlayerShopBuyResultExtended.ResultKind.ShopNotOpened,
            ItemBuyResult.InTransaction => PlayerShopBuyResultExtended.ResultKind.InTransaction,
            ItemBuyResult.InvalidShopSlot => PlayerShopBuyResultExtended.ResultKind.InvalidShopSlot,
            ItemBuyResult.NameMismatchOrPriceMissing => PlayerShopBuyResultExtended.ResultKind.NameMismatchOrPriceMissing,
            ItemBuyResult.LackOfMoney => PlayerShopBuyResultExtended.ResultKind.LackOfMoney,
            ItemBuyResult.MoneyOverflowOrNotEnoughSpace => PlayerShopBuyResultExtended.ResultKind.MoneyOverflowOrNotEnoughSpace,
            ItemBuyResult.ItemBlock => PlayerShopBuyResultExtended.ResultKind.ItemBlock,
            _ => PlayerShopBuyResultExtended.ResultKind.Undefined,
        };
    }
}