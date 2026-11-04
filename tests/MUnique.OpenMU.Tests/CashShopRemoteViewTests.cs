// <copyright file="CashShopRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.GameServer.RemoteView.CashShop;

/// <summary>
/// Tests for the packets of the <see cref="CashShopViewPlugIn"/>.
/// </summary>
[TestFixture]
public class CashShopRemoteViewTests
{
    /// <summary>
    /// Tests that the script and banner versions are sent, so that the client unlocks its cash shop.
    /// </summary>
    [Test]
    public async Task VersionsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);
        var configuration = new CashShopConfiguration
        {
            ScriptSaleZone = 512,
            ScriptYear = 2012,
            ScriptYearId = 84,
            BannerSaleZone = 583,
            BannerYear = 2011,
            BannerYearId = 1,
        };

        await view.ShowVersionsAsync(configuration).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 10, 0xD2, 0x0C, 0x00, 0x02, 0xDC, 0x07, 84, 0,
            0xC1, 10, 0xD2, 0x15, 0x47, 0x02, 0xDB, 0x07, 1, 0,
        }));
    }

    /// <summary>
    /// Tests the points, which are sent as doubles at the unaligned offsets of the packed client structure.
    /// </summary>
    [Test]
    public async Task PointsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);

        await view.ShowPointsAsync(new Account { WCoinC = 100, WCoinP = 20, GoblinPoints = 3 }).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(45));
        Assert.That(data[..5], Is.EqualTo(new byte[] { 0xC1, 45, 0xD2, 0x01, 0 }));
        Assert.That(BitConverter.ToDouble(data, 5), Is.EqualTo(120));
        Assert.That(BitConverter.ToDouble(data, 13), Is.EqualTo(100));
        Assert.That(BitConverter.ToDouble(data, 21), Is.EqualTo(20));
        Assert.That(BitConverter.ToDouble(data, 29), Is.EqualTo(123));
        Assert.That(BitConverter.ToDouble(data, 37), Is.EqualTo(3));
    }

    /// <summary>
    /// Tests the buy results, which have the left count at the unaligned offset 5 of the packed client structure.
    /// </summary>
    [Test]
    public async Task BuyResultsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);

        await view.ShowBuyResultAsync(CashShopBuyResult.Success).ConfigureAwait(false);
        await view.ShowBuyResultAsync(CashShopBuyResult.NotEnoughCoins).ConfigureAwait(false);
        await view.ShowBuyResultAsync(CashShopBuyResult.WrongCoinType).ConfigureAwait(false);
        await view.ShowBuyResultAsync(CashShopBuyResult.SaveFailed).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 9, 0xD2, 0x03, 0, 0, 0, 0, 0,
            0xC1, 9, 0xD2, 0x03, 1, 0, 0, 0, 0,
            0xC1, 9, 0xD2, 0x03, 9, 0, 0, 0, 0,
            0xC1, 9, 0xD2, 0x03, 0xFF, 0, 0, 0, 0,
        }));
    }

    /// <summary>
    /// Tests the gift results, which have the left count at offset 5 and the limited cash at offset 9.
    /// </summary>
    [Test]
    public async Task GiftResultsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);

        await view.ShowGiftResultAsync(CashShopGiftResult.Success).ConfigureAwait(false);
        await view.ShowGiftResultAsync(CashShopGiftResult.RecipientNotFound).ConfigureAwait(false);
        await view.ShowGiftResultAsync(CashShopGiftResult.NotGiftable).ConfigureAwait(false);
        await view.ShowGiftResultAsync(CashShopGiftResult.WrongCoinType).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(4 * 17));
        Assert.That(data[..17], Is.EqualTo(new byte[] { 0xC1, 17, 0xD2, 0x04, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        Assert.That(new[] { data[4], data[17 + 4], data[34 + 4], data[51 + 4] }, Is.EqualTo(new byte[] { 0, 3, 7, 10 }));
    }

    /// <summary>
    /// Tests the results of the requests to use a storage item.
    /// </summary>
    [Test]
    public async Task UseResultsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);

        await view.ShowUseResultAsync(CashShopUseResult.Success).ConfigureAwait(false);
        await view.ShowUseResultAsync(CashShopUseResult.ItemNotFound).ConfigureAwait(false);
        await view.ShowUseResultAsync(CashShopUseResult.InventoryFull).ConfigureAwait(false);
        await view.ShowUseResultAsync(CashShopUseResult.CannotUse).ConfigureAwait(false);
        await view.ShowUseResultAsync(CashShopUseResult.SaveFailed).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 5, 0xD2, 0x0B, 0,
            0xC1, 5, 0xD2, 0x0B, 1,
            0xC1, 5, 0xD2, 0x0B, 21,
            0xC1, 5, 0xD2, 0x0B, 22,
            0xC1, 5, 0xD2, 0x0B, 0xFF,
        }));
    }

    /// <summary>
    /// Tests that the storage index of an item is its index in all items of the account, because the client
    /// doesn't send whether an item to use is in the storage or the gift storage.
    /// </summary>
    [Test]
    public async Task StorageIndexIsUniqueAcrossBothStoragesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);
        var items = new List<CashShopStorageItem>
        {
            new() { PriceSequence = 1, IsGift = true },
            new() { PriceSequence = 2 },
            new() { PriceSequence = 3, IsGift = true },
            new() { PriceSequence = 4 },
        };

        await view.ShowStorageAsync(items, 1, false).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data[..12], Is.EqualTo(new byte[] { 0xC1, 12, 0xD2, 0x06, 2, 0, 2, 0, 1, 0, 1, 0 }));
        Assert.That(data[(12 + 4)..(12 + 12)], Is.EqualTo(new byte[] { 1, 0, 0, 0, 2, 0, 0, 0 }));
        Assert.That(data[(12 + 33 + 4)..(12 + 33 + 12)], Is.EqualTo(new byte[] { 3, 0, 0, 0, 4, 0, 0, 0 }));
    }

    /// <summary>
    /// Tests that the requested storage page is sent with up to 9 items, which the client shows on one page.
    /// </summary>
    [Test]
    public async Task StoragePageAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);
        var items = Enumerable.Range(1, 11).Select(i => new CashShopStorageItem { ProductSequence = 100 + i, PriceSequence = 200 + i }).ToList();

        await view.ShowStorageAsync(items, 2, false).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(12 + (2 * 33)));
        Assert.That(data[..12], Is.EqualTo(new byte[] { 0xC1, 12, 0xD2, 0x06, 11, 0, 2, 0, 2, 0, 2, 0 }));
        var lastItem = data[(12 + 33)..];
        Assert.That(lastItem[..24], Is.EqualTo(new byte[] { 0xC1, 33, 0xD2, 0x0D, 10, 0, 0, 0, 211, 0, 0, 0, 0, 0, 0, 0, 111, 0, 0, 0, 211, 0, 0, 0 }));
        Assert.That(lastItem[32], Is.EqualTo((byte)'P'));
    }

    /// <summary>
    /// Tests that an empty storage is sent as one empty page, and that gifts are sent with sender and message.
    /// </summary>
    [Test]
    public async Task EmptyStorageAndGiftsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new CashShopViewPlugIn(player);
        var gift = new CashShopStorageItem { ProductSequence = 1, PriceSequence = 2, IsGift = true, GiftSenderName = "Sender", GiftMessage = "Hello" };

        await view.ShowStorageAsync([], 1, false).ConfigureAwait(false);
        await view.ShowStorageAsync([gift], 5, true).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(12 + 12 + 244));
        Assert.That(data[..12], Is.EqualTo(new byte[] { 0xC1, 12, 0xD2, 0x06, 0, 0, 0, 0, 1, 0, 1, 0 }));
        Assert.That(data[12..24], Is.EqualTo(new byte[] { 0xC1, 12, 0xD2, 0x06, 1, 0, 1, 0, 1, 0, 1, 0 }));
        var giftPacket = data[24..];
        Assert.That(giftPacket[..4], Is.EqualTo(new byte[] { 0xC1, 244, 0xD2, 0x0E }));
        Assert.That(giftPacket[33..39], Is.EqualTo("Sender"u8.ToArray()));
        Assert.That(giftPacket[44..49], Is.EqualTo("Hello"u8.ToArray()));
    }
}
