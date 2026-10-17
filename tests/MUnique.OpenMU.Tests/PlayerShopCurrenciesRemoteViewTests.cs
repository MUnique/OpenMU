// <copyright file="PlayerShopCurrenciesRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.GameServer.MessageHandler.PlayerShop;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.PlayerShop;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the remote views of the player shop currencies.
/// </summary>
[TestFixture]
public class PlayerShopCurrenciesRemoteViewTests
{
    private static readonly ItemDefinition JewelOfBless = new Persistence.BasicModel.ItemDefinition { Number = 13, Group = 14, Width = 1, Height = 1, Durability = 1 };

    /// <summary>
    /// Tests that the allowed price items and account coins are sent with their price item types.
    /// </summary>
    [Test]
    public async Task PriceTypesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ShowShopCurrenciesExtendedPlugIn(player);

        await view.ShowShopCurrenciesAsync([JewelOfBless], [CashShopCoinType.GoblinPoints]).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[] { 0xC1, 9, 0x3F, 0x0A, 2, 0x0D, 0xE0, 0xF2, 0xFF }));
    }

    /// <summary>
    /// Tests that a truncated set price request is ignored instead of throwing.
    /// </summary>
    [Test]
    public async Task TruncatedSetPriceRequestIsIgnoredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var handler = new PlayerShopSetItemPriceExtendedPacketHandlerPlugIn();
        var packet = new byte[] { 0xC3, 8, 0x3F, 0x09, 0, 0, 0, 0 };

        await handler.HandlePacketAsync(player, packet).ConfigureAwait(false);

        Moq.Mock.Get(player.ViewPlugIns.GetPlugIn<IItemPriceSetResponsePlugIn>()!)
            .Verify(v => v.ItemPriceSetResponseAsync(Moq.It.IsAny<byte>(), Moq.It.IsAny<ItemPriceResult>()), Moq.Times.Never);
    }

    /// <summary>
    /// Tests that the prices are decoded like they're encoded, and that unknown price items are not decoded.
    /// </summary>
    [Test]
    public void PriceEncodingRoundTrip()
    {
        var configuration = new Persistence.BasicModel.GameConfiguration();
        configuration.Items.Add(JewelOfBless);
        ShopPrice[] prices = [new(1000), new(12, JewelOfBless), new(500, CoinType: CashShopCoinType.WCoinP)];

        foreach (var price in prices)
        {
            var (moneyPrice, priceItemType, requiredItemAmount) = PlayerShopPriceEncoding.Encode(price);
            Assert.That(PlayerShopPriceEncoding.Decode(moneyPrice, priceItemType, requiredItemAmount, configuration), Is.EqualTo(price));
        }

        Assert.That(PlayerShopPriceEncoding.Decode(0, 0xE00E, 1, configuration), Is.Null, "the jewel of soul is not defined");
        Assert.That(PlayerShopPriceEncoding.Decode(uint.MaxValue, 0, 0, configuration)!.Value.Amount, Is.Negative);
    }

    /// <summary>
    /// Tests that the prices in jewels, account coins and zen are written into the fields of the extended item list,
    /// and that items with prices in disabled currencies are hidden.
    /// </summary>
    [Test]
    public async Task ItemListWithCurrenciesAsync()
    {
        var (viewer, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        viewer.GameContext.PlugInManager.RegisterPlugIn<IViewPlugIn, ItemSerializer>();
        viewer.ClientVersion = new ClientVersion(106, 3, ClientLanguage.English);
        var configuration = new PlayerShopCurrenciesConfiguration { IsGoblinPointsAllowed = true };
        configuration.PriceItems.Add(JewelOfBless);
        viewer.GameContext.FeaturePlugIns.AddPlugIn(new PlayerShopCurrenciesFeaturePlugIn { Configuration = configuration }, true);
        var seller = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        seller.SelectedCharacter!.Name = "Seller";
        seller.SelectedCharacter.StoreName = "Store";
        var definition = new ItemDefinition { Number = 1, Group = 0, Width = 1, Height = 1, Durability = 20 };
        await AddItemAsync(seller, 204, definition, new ShopPrice(12, JewelOfBless)).ConfigureAwait(false);
        await AddItemAsync(seller, 205, definition, new ShopPrice(500, CoinType: CashShopCoinType.GoblinPoints)).ConfigureAwait(false);
        await AddItemAsync(seller, 206, definition, new ShopPrice(1000)).ConfigureAwait(false);
        await AddItemAsync(seller, 207, definition, new ShopPrice(7, CoinType: CashShopCoinType.WCoinC)).ConfigureAwait(false);
        var view = new ShowShopItemListExtendedPlugIn(viewer);

        await view.ShowShopItemListAsync(seller, false).ConfigureAwait(false);

        var data = output.ToArray();
        var list = new PlayerShopItemListExtended(data);
        Assert.That(list.ItemCount, Is.EqualTo(3), "the item with the price in WCoin (C) is hidden, because it's not allowed");
        var itemSize = (data.Length - PlayerShopItemListExtended.GetRequiredSize(0, 0)) / 3;
        var items = Enumerable.Range(0, 3)
            .Select(i => new PlayerShopItemExtended(data.AsMemory(PlayerShopItemListExtended.GetRequiredSize(0, 0) + (i * itemSize), itemSize)))
            .ToDictionary(i => i.ItemSlot);
        Assert.That((items[204].MoneyPrice, items[204].PriceItemType, items[204].RequiredItemAmount), Is.EqualTo((0u, (ushort)0xE00D, (ushort)12)));
        Assert.That((items[205].MoneyPrice, items[205].PriceItemType, items[205].RequiredItemAmount), Is.EqualTo((500u, (ushort)0xFFF2, (ushort)0)));
        Assert.That((items[206].MoneyPrice, items[206].PriceItemType, items[206].RequiredItemAmount), Is.EqualTo((1000u, (ushort)0, (ushort)0)));
    }

    private static async ValueTask AddItemAsync(GameLogic.Player seller, byte slot, ItemDefinition definition, ShopPrice price)
    {
        var item = new Persistence.BasicModel.Item { Definition = definition, ItemSlot = slot, Durability = 20 };
        item.SetStorePrice(price);
        await seller.ShopStorage!.AddItemAsync(slot, item).ConfigureAwait(false);
    }
}
