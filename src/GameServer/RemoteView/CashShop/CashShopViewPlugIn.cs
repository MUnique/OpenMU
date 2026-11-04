// <copyright file="CashShopViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using StorageItem = MUnique.OpenMU.DataModel.Entities.CashShopStorageItem;

/// <summary>
/// The default implementation of the <see cref="ICashShopViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopViewPlugIn_Name), Description = nameof(PlugInResources.CashShopViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("231D55B0-4861-4D22-AAB9-CDC80A106B85")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class CashShopViewPlugIn : ICashShopViewPlugIn
{
    /// <summary>
    /// The number of items which the client shows on one storage page.
    /// </summary>
    private const int ItemsPerStoragePage = 9;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CashShopViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowVersionsAsync(CashShopConfiguration configuration)
    {
        var connection = this._player.Connection;
        await connection.SendCashShopScriptVersionAsync((ushort)configuration.ScriptSaleZone, (ushort)configuration.ScriptYear, (ushort)configuration.ScriptYearId).ConfigureAwait(false);
        await connection.SendCashShopBannerVersionAsync((ushort)configuration.BannerSaleZone, (ushort)configuration.BannerYear, (ushort)configuration.BannerYearId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowOpenResultAsync(bool isAllowed)
    {
        await this._player.Connection.SendCashShopOpenStateResponseAsync(isAllowed).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowPointsAsync(Account account)
    {
        var wCoinC = (double)account.WCoinC;
        var wCoinP = (double)account.WCoinP;
        var goblinPoints = (double)account.GoblinPoints;
        await this._player.Connection.SendCashShopPointInfoAsync(0, wCoinC + wCoinP, wCoinC, wCoinP, wCoinC + wCoinP + goblinPoints, goblinPoints).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBuyResultAsync(CashShopBuyResult result)
    {
        var packetResult = result switch
        {
            CashShopBuyResult.Success => CashShopItemBuyResult.CashShopBuyResult.Success,
            CashShopBuyResult.NotEnoughCoins => CashShopItemBuyResult.CashShopBuyResult.NotEnoughCoins,
            CashShopBuyResult.NotForSale => CashShopItemBuyResult.CashShopBuyResult.NotAvailableCurrently,
            CashShopBuyResult.PackageNotFound => CashShopItemBuyResult.CashShopBuyResult.NoLongerAvailable,
            CashShopBuyResult.InvalidPriceOption => CashShopItemBuyResult.CashShopBuyResult.CannotBeBought,
            CashShopBuyResult.WrongCoinType => CashShopItemBuyResult.CashShopBuyResult.WrongCoinType,
            CashShopBuyResult.StorageFull => CashShopItemBuyResult.CashShopBuyResult.StorageFull,
            CashShopBuyResult.RefusedByPlugIn => CashShopItemBuyResult.CashShopBuyResult.CannotBeBought,
            _ => CashShopItemBuyResult.CashShopBuyResult.DatabaseError,
        };
        await this._player.Connection.SendCashShopItemBuyResultAsync(packetResult, 0).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowGiftResultAsync(CashShopGiftResult result)
    {
        var packetResult = result switch
        {
            CashShopGiftResult.Success => CashShopItemGiftResult.CashShopGiftResult.Success,
            CashShopGiftResult.NotEnoughCoins => CashShopItemGiftResult.CashShopGiftResult.NotEnoughCoins,
            CashShopGiftResult.RecipientNotFound => CashShopItemGiftResult.CashShopGiftResult.RecipientNotFound,
            CashShopGiftResult.NotForSale => CashShopItemGiftResult.CashShopGiftResult.NoLongerAvailable,
            CashShopGiftResult.NotGiftable => CashShopItemGiftResult.CashShopGiftResult.CannotBeGifted,
            CashShopGiftResult.PackageNotFound => CashShopItemGiftResult.CashShopGiftResult.NoLongerAvailable,
            CashShopGiftResult.InvalidPriceOption => CashShopItemGiftResult.CashShopGiftResult.NoLongerAvailable2,
            CashShopGiftResult.WrongCoinType => CashShopItemGiftResult.CashShopGiftResult.WrongCoinType,
            CashShopGiftResult.RecipientStorageFull => CashShopItemGiftResult.CashShopGiftResult.RecipientStorageFull,
            CashShopGiftResult.RefusedByPlugIn => CashShopItemGiftResult.CashShopGiftResult.CannotBeGifted,

            // The client has no message for this case.
            CashShopGiftResult.RecipientIsOwnAccount => CashShopItemGiftResult.CashShopGiftResult.RecipientNotFound,
            _ => CashShopItemGiftResult.CashShopGiftResult.DatabaseError,
        };
        await this._player.Connection.SendCashShopItemGiftResultAsync(packetResult, 0, 0).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowUseResultAsync(CashShopUseResult result)
    {
        var packetResult = result switch
        {
            CashShopUseResult.Success => CashShopStorageItemConsumeResult.CashShopConsumeResult.Success,
            CashShopUseResult.ItemNotFound => CashShopStorageItemConsumeResult.CashShopConsumeResult.ItemNotFound,
            CashShopUseResult.InventoryFull => CashShopStorageItemConsumeResult.CashShopConsumeResult.InventoryFull,
            CashShopUseResult.CannotUse => CashShopStorageItemConsumeResult.CashShopConsumeResult.CannotUse,
            _ => CashShopStorageItemConsumeResult.CashShopConsumeResult.Error,
        };
        await this._player.Connection.SendCashShopStorageItemConsumeResultAsync(packetResult).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The client doesn't send whether an item to use is in the storage or the gift storage. That's why
    /// the storage index of an item is its index in all <paramref name="items"/> of the account. The item
    /// sequence is its price sequence number. The client sends both back when the item should be used.
    /// </remarks>
    public async ValueTask ShowStorageAsync(IReadOnlyList<StorageItem> items, int pageNumber, bool isGiftStorage)
    {
        var connection = this._player.Connection;
        var storageIndexes = Enumerable.Range(0, items.Count).Where(index => items[index].IsGift == isGiftStorage).ToList();
        var pageCount = Math.Max(1, (storageIndexes.Count + ItemsPerStoragePage - 1) / ItemsPerStoragePage);
        pageNumber = Math.Clamp(pageNumber, 1, pageCount);
        var pageIndexes = storageIndexes.Skip((pageNumber - 1) * ItemsPerStoragePage).Take(ItemsPerStoragePage).ToList();

        await connection.SendCashShopStorageListResponseAsync((ushort)Math.Min(storageIndexes.Count, ushort.MaxValue), (ushort)pageIndexes.Count, (ushort)pageNumber, (ushort)Math.Min(pageCount, ushort.MaxValue)).ConfigureAwait(false);
        foreach (var index in pageIndexes)
        {
            var item = items[index];
            if (isGiftStorage)
            {
                await connection.SendCashShopGiftStorageItemAsync(
                    (uint)index,
                    (uint)item.PriceSequence,
                    0,
                    (uint)item.ProductSequence,
                    (uint)item.PriceSequence,
                    0,
                    CashShopGiftStorageItem.CashShopStorageItemType.Product,
                    item.GiftSenderName ?? string.Empty,
                    item.GiftMessage ?? string.Empty).ConfigureAwait(false);
            }
            else
            {
                await connection.SendCashShopStorageItemAsync(
                    (uint)index,
                    (uint)item.PriceSequence,
                    0,
                    (uint)item.ProductSequence,
                    (uint)item.PriceSequence,
                    0,
                    Network.Packets.ServerToClient.CashShopStorageItem.CashShopStorageItemType.Product).ConfigureAwait(false);
            }
        }
    }
}
