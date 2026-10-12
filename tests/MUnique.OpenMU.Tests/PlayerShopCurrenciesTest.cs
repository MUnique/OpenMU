// <copyright file="PlayerShopCurrenciesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the prices of player shops in other currencies than zen.
/// </summary>
[TestFixture]
public class PlayerShopCurrenciesTest
{
    private const byte StoreSlot = 204;

    private readonly ItemDefinition _jewelOfBless = new Persistence.BasicModel.ItemDefinition { Number = 13, Group = 14, Width = 1, Height = 1, Durability = 1 };
    private readonly ItemDefinition _jewelOfBlessBundle = new Persistence.BasicModel.ItemDefinition { Number = 30, Group = 12, Width = 1, Height = 1, Durability = 1 };
    private readonly ItemDefinition _jewelOfSoul = new Persistence.BasicModel.ItemDefinition { Number = 14, Group = 14, Width = 1, Height = 1, Durability = 1 };
    private readonly ItemDefinition _soldItemDefinition = new Persistence.BasicModel.ItemDefinition { Number = 1, Group = 0, Width = 1, Height = 2, Durability = 20 };

    /// <summary>
    /// Tests that a price in jewels can't be set when the feature is deactivated.
    /// </summary>
    [Test]
    public async ValueTask JewelPriceIsRejectedWhenFeatureIsDeactivatedAsync()
    {
        var (seller, _) = await this.CreatePlayersAsync(isFeatureActive: false).ConfigureAwait(false);
        var item = await this.AddStoreItemAsync(seller).ConfigureAwait(false);

        await new SetItemPriceAction().SetPriceAsync(seller, StoreSlot, new ShopPrice(10, this._jewelOfBless)).ConfigureAwait(false);

        Assert.That(item.StorePrice, Is.Null);
        Mock.Get(seller.ViewPlugIns.GetPlugIn<IItemPriceSetResponsePlugIn>()!).Verify(v => v.ItemPriceSetResponseAsync(StoreSlot, ItemPriceResult.Failed), Times.Once);
    }

    /// <summary>
    /// Tests that a price in jewels is set when the feature is activated.
    /// </summary>
    [Test]
    public async ValueTask JewelPriceIsSetWhenFeatureIsActivatedAsync()
    {
        var (seller, _) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var item = await this.AddStoreItemAsync(seller).ConfigureAwait(false);

        await new SetItemPriceAction().SetPriceAsync(seller, StoreSlot, new ShopPrice(10, this._jewelOfBless)).ConfigureAwait(false);

        Assert.That(item.StorePrice, Is.EqualTo(10));
        Assert.That(item.StorePriceItemDefinition, Is.SameAs(this._jewelOfBless));
        Assert.That(item.StorePriceCoinType, Is.Null);
        Mock.Get(seller.ViewPlugIns.GetPlugIn<IItemPriceSetResponsePlugIn>()!).Verify(v => v.ItemPriceSetResponseAsync(StoreSlot, ItemPriceResult.Success), Times.Once);
    }

    /// <summary>
    /// Tests that the account coins are not allowed by default, that prices above the configured maximum and
    /// prices in items which are not configured are rejected.
    /// </summary>
    /// <param name="priceCase">The case of the price.</param>
    [TestCase("coin")]
    [TestCase("tooHigh")]
    [TestCase("notConfiguredItem")]
    public async ValueTask PriceIsRejectedAsync(string priceCase)
    {
        var (seller, _) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var item = await this.AddStoreItemAsync(seller).ConfigureAwait(false);
        var price = priceCase switch
        {
            "coin" => new ShopPrice(10, CoinType: CashShopCoinType.WCoinC),
            "tooHigh" => new ShopPrice(PlayerShopCurrenciesConfiguration.DefaultMaximumItemAmount + 1, this._jewelOfBless),
            _ => new ShopPrice(10, this._soldItemDefinition),
        };

        await new SetItemPriceAction().SetPriceAsync(seller, StoreSlot, price).ConfigureAwait(false);

        Assert.That(item.StorePrice, Is.Null);
        Mock.Get(seller.ViewPlugIns.GetPlugIn<IItemPriceSetResponsePlugIn>()!).Verify(v => v.ItemPriceSetResponseAsync(StoreSlot, ItemPriceResult.Failed), Times.Once);
    }

    /// <summary>
    /// Tests that a price of 0 removes the price and resets the currency.
    /// </summary>
    [Test]
    public async ValueTask PriceOfZeroRemovesPriceAsync()
    {
        var (seller, _) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var item = await this.AddStoreItemAsync(seller, new ShopPrice(10, this._jewelOfSoul)).ConfigureAwait(false);

        await new SetItemPriceAction().SetPriceAsync(seller, StoreSlot, new ShopPrice(0, this._jewelOfSoul)).ConfigureAwait(false);

        Assert.That(item.StorePrice, Is.Null);
        Assert.That(item.StorePriceItemDefinition, Is.Null);
    }

    /// <summary>
    /// Tests that the store can't be opened when it contains an item with a price in a currency which isn't enabled.
    /// </summary>
    [Test]
    public async ValueTask StoreWithDisabledCurrencyIsNotOpenedAsync()
    {
        var (seller, _) = await this.CreatePlayersAsync(isFeatureActive: false).ConfigureAwait(false);
        await this.AddStoreItemAsync(seller, new ShopPrice(10, this._jewelOfBless)).ConfigureAwait(false);

        await new OpenStoreAction().OpenStoreAsync(seller, "Test").ConfigureAwait(false);

        Assert.That(seller.ShopStorage!.StoreOpen, Is.False);
    }

    /// <summary>
    /// Tests that an item is bought with jewels: a bundle and single jewels which match the price exactly
    /// are moved from the buyer to the seller.
    /// </summary>
    [Test]
    public async ValueTask BuyWithJewelsMovesJewelsToSellerAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(13, this._jewelOfBless)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        var bundle = await this.AddInventoryItemAsync(buyer, 12, this._jewelOfBlessBundle).ConfigureAwait(false);
        var singles = new List<Item>();
        for (byte slot = 13; slot < 17; slot++)
        {
            singles.Add(await this.AddInventoryItemAsync(buyer, slot, this._jewelOfBless).ConfigureAwait(false));
        }

        var otherJewel = await this.AddInventoryItemAsync(buyer, 17, this._jewelOfSoul).ConfigureAwait(false);

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.Success, soldItem), Times.Once);
        Assert.That(buyer.Inventory!.Items, Does.Contain(soldItem));
        Assert.That(seller.Inventory!.Items, Does.Contain(bundle));
        Assert.That(seller.Inventory.Items.Count(i => i.Definition == this._jewelOfBless), Is.EqualTo(3));
        Assert.That(buyer.Inventory.Items.Count(i => i.Definition == this._jewelOfBless), Is.EqualTo(1));
        Assert.That(buyer.Inventory.Items, Does.Contain(otherJewel));
        Assert.That(soldItem.StorePrice, Is.Null);
        Assert.That(soldItem.StorePriceItemDefinition, Is.Null);
    }

    /// <summary>
    /// Tests that nothing is moved, when the jewels of the buyer can't pay the price exactly, because bundles are not split.
    /// </summary>
    [Test]
    public async ValueTask BuyWithJewelsFailsWithoutExactCombinationAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(5, this._jewelOfBless)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        var bundle = await this.AddInventoryItemAsync(buyer, 12, this._jewelOfBlessBundle).ConfigureAwait(false);

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.LackOfPriceCurrency, null), Times.Once);
        Assert.That(buyer.Inventory!.Items, Does.Contain(bundle));
        Assert.That(seller.ShopStorage.Items, Does.Contain(soldItem));
    }

    /// <summary>
    /// Tests that an item is bought with account coins.
    /// </summary>
    [Test]
    public async ValueTask BuyWithCoinsTransfersCoinsAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync(new PlayerShopCurrenciesConfiguration { IsWCoinCAllowed = true }).ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(30, CoinType: CashShopCoinType.WCoinC)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        buyer.Account!.WCoinC = 100;
        seller.Account!.WCoinC = 5;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.Success, soldItem), Times.Once);
        Assert.That(buyer.Account.WCoinC, Is.EqualTo(70));
        Assert.That(seller.Account.WCoinC, Is.EqualTo(35));
    }

    /// <summary>
    /// Tests that nothing is transferred, when the buyer doesn't have enough coins.
    /// </summary>
    [Test]
    public async ValueTask BuyWithCoinsFailsWithoutEnoughCoinsAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync(new PlayerShopCurrenciesConfiguration { IsWCoinCAllowed = true }).ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(30, CoinType: CashShopCoinType.WCoinC)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        buyer.Account!.WCoinC = 29;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.LackOfPriceCurrency, null), Times.Once);
        Assert.That(buyer.Account.WCoinC, Is.EqualTo(29));
        Assert.That(seller.ShopStorage.Items, Does.Contain(soldItem));
    }

    /// <summary>
    /// Tests that an item with a price in a currency which got disabled after the store was opened can't be bought.
    /// </summary>
    [Test]
    public async ValueTask BuyWithDisabledCurrencyFailsAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync(isFeatureActive: false).ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(1, this._jewelOfBless)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        await this.AddInventoryItemAsync(buyer, 12, this._jewelOfBless).ConfigureAwait(false);

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.ItemBlock, null), Times.Once);
        Assert.That(seller.ShopStorage.Items, Does.Contain(soldItem));
    }

    /// <summary>
    /// Tests that zen prices still work like before.
    /// </summary>
    [Test]
    public async ValueTask BuyWithZenTransfersMoneyAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync(isFeatureActive: false).ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(1000)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        buyer.Money = 1500;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.Success, soldItem), Times.Once);
        Assert.That(buyer.Money, Is.EqualTo(500));
        Assert.That(seller.Money, Is.EqualTo(1000));
    }

    /// <summary>
    /// Tests that an item with a stored price of 0, which can only come from a modified database,
    /// is not sold for nothing.
    /// </summary>
    [Test]
    public async ValueTask ItemWithZeroPriceIsNotSoldAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(1, this._jewelOfBless)).ConfigureAwait(false);
        soldItem.StorePrice = 0;
        seller.ShopStorage!.StoreOpen = true;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.NameMismatchOrPriceMissing, null), Times.Once);
        Assert.That(seller.ShopStorage.Items, Does.Contain(soldItem));
        Assert.That(soldItem.GetStorePrice(), Is.Null);
    }

    /// <summary>
    /// Tests that a bundle with a higher level than the jewel mix creates doesn't count as payment,
    /// because it couldn't be unstacked into its pieces.
    /// </summary>
    [Test]
    public async ValueTask BundleAboveMixLevelsIsNotAcceptedAsync()
    {
        var (seller, buyer) = await this.CreatePlayersAsync().ConfigureAwait(false);
        var soldItem = await this.AddStoreItemAsync(seller, new ShopPrice(40, this._jewelOfBless)).ConfigureAwait(false);
        seller.ShopStorage!.StoreOpen = true;
        var bundle = await this.AddInventoryItemAsync(buyer, 12, this._jewelOfBlessBundle, level: 3).ConfigureAwait(false);

        await new BuyRequestAction().BuyItemAsync(buyer, seller, StoreSlot).ConfigureAwait(false);

        Mock.Get(buyer.ViewPlugIns.GetPlugIn<IPlayerShopBuyRequestResultPlugIn>()!).Verify(v => v.ShowResultAsync(seller, ItemBuyResult.LackOfPriceCurrency, null), Times.Once);
        Assert.That(buyer.Inventory!.Items, Does.Contain(bundle));
        Assert.That(seller.ShopStorage.Items, Does.Contain(soldItem));
    }

    /// <summary>
    /// Tests that the admin panel shows readable captions for the account coin settings, which can't be derived
    /// from the property names.
    /// </summary>
    [Test]
    public void ConfigurationHasCaptions()
    {
        var english = System.Globalization.CultureInfo.GetCultureInfo("en");

        Assert.That(typeof(PlayerShopCurrenciesConfiguration).GetPropertyCaption(nameof(PlayerShopCurrenciesConfiguration.IsWCoinCAllowed), english), Is.EqualTo("Is WCoin (C) Allowed"));
        Assert.That(typeof(PlayerShopCurrenciesConfiguration).GetPropertyCaption(nameof(PlayerShopCurrenciesConfiguration.IsWCoinPAllowed), english), Is.EqualTo("Is WCoin (P) Allowed"));
    }

    /// <summary>
    /// Tests the selection of jewel items which pay a price exactly.
    /// </summary>
    /// <param name="pieces">The pieces of the available items.</param>
    /// <param name="price">The price.</param>
    /// <param name="expectedRows">The expected number of selected rows; <c>-1</c>, if no combination is expected.</param>
    [TestCase(new[] { 30, 20, 20 }, 40, 2)]
    [TestCase(new[] { 10, 1, 1, 1 }, 12, 3)]
    [TestCase(new[] { 10, 1, 1, 1 }, 11, 2)]
    [TestCase(new[] { 10, 10 }, 15, -1)]
    [TestCase(new[] { 1, 1 }, 3, -1)]
    [TestCase(new[] { 20, 10, 1 }, 21, 2)]
    public void SelectRowsMatchesThePriceExactly(int[] pieces, int price, int expectedRows)
    {
        var rows = pieces.Select(p => ((Item)new Persistence.BasicModel.Item(), p)).ToList();

        var selected = ItemShopCurrencyExchange.SelectRows(rows, price);

        if (expectedRows < 0)
        {
            Assert.That(selected, Is.Null);
        }
        else
        {
            Assert.That(selected, Is.Not.Null);
            Assert.That(selected!.Count, Is.EqualTo(expectedRows));
            Assert.That(selected.Sum(r => r.Pieces), Is.EqualTo(price));
            Assert.That(selected.Select(r => r.Item).Distinct(ReferenceEqualityComparer.Instance).Count(), Is.EqualTo(selected.Count));
        }
    }

    /// <summary>
    /// Tests the parsing of the currencies of the chat command.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="expectedKind">The expected kind of the price; <c>null</c>, if the text isn't a currency.</param>
    [TestCase("zen", ShopPriceKind.Zen)]
    [TestCase("BLESS", ShopPriceKind.Item)]
    [TestCase("jos", ShopPriceKind.Item)]
    [TestCase("14/13", ShopPriceKind.Item)]
    [TestCase("gp", ShopPriceKind.AccountCoin)]
    [TestCase("14/99", null)]
    [TestCase("gold", null)]
    public async ValueTask ParseCurrencyAsync(string text, ShopPriceKind? expectedKind)
    {
        var (seller, _) = await this.CreatePlayersAsync().ConfigureAwait(false);

        var success = ShopPrices.TryParse(seller.GameContext, text, 3, out var price);

        Assert.That(success, Is.EqualTo(expectedKind.HasValue));
        if (expectedKind is { } kind)
        {
            Assert.That(price.Kind, Is.EqualTo(kind));
            Assert.That(price.Amount, Is.EqualTo(3));
        }
    }

    /// <summary>
    /// Tests that the allowed prices follow the configuration.
    /// </summary>
    [Test]
    public async ValueTask AllowedPricesFollowConfigurationAsync()
    {
        var configuration = new PlayerShopCurrenciesConfiguration { IsGoblinPointsAllowed = true };
        configuration.PriceItems.Add(this._jewelOfSoul);
        var (seller, _) = await this.CreatePlayersAsync(configuration).ConfigureAwait(false);

        Assert.That(ShopPrices.GetAllowedPriceItems(seller.GameContext), Is.EquivalentTo(new[] { this._jewelOfSoul }));
        Assert.That(ShopPrices.GetAllowedCoinTypes(seller.GameContext), Is.EquivalentTo(new[] { CashShopCoinType.GoblinPoints }));
        Assert.That(ShopPrices.IsAllowed(seller.GameContext, new ShopPrice(1, this._jewelOfBless)), Is.False);
        Assert.That(ShopPrices.IsAllowed(seller.GameContext, new ShopPrice(1, this._jewelOfSoul)), Is.True);
    }

    private async ValueTask<(Player Seller, Player Buyer)> CreatePlayersAsync(PlayerShopCurrenciesConfiguration? configuration = null, bool isFeatureActive = true)
    {
        // All plugins are discovered and activated by the test helper, so the feature plugin is deactivated first,
        // and then activated with the configuration of the test.
        var gameContext = GameContextTestHelper.CreateGameContext(
        [
            new PlugInConfiguration { TypeId = typeof(PlayerShopCurrenciesFeaturePlugIn).GUID, IsActive = false },
        ]);
        if (isFeatureActive)
        {
            gameContext.FeaturePlugIns.AddPlugIn(new PlayerShopCurrenciesFeaturePlugIn { Configuration = configuration ?? this.CreateConfiguration() }, true);
        }

        gameContext.Configuration.Items.Add(this._jewelOfBless);
        gameContext.Configuration.Items.Add(this._jewelOfSoul);
        gameContext.Configuration.JewelMixes.Add(new Persistence.BasicModel.JewelMix { Number = 0, SingleJewel = this._jewelOfBless, MixedJewel = this._jewelOfBlessBundle });
        var seller = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var buyer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        seller.Attributes![Stats.Level] = 10;
        buyer.Attributes![Stats.Level] = 10;
        return (seller, buyer);
    }

    private PlayerShopCurrenciesConfiguration CreateConfiguration()
    {
        var configuration = new PlayerShopCurrenciesConfiguration();
        configuration.PriceItems.Add(this._jewelOfBless);
        configuration.PriceItems.Add(this._jewelOfSoul);
        return configuration;
    }

    private async ValueTask<Item> AddStoreItemAsync(Player seller, ShopPrice? price = null)
    {
        var item = new Persistence.BasicModel.Item { Definition = this._soldItemDefinition, ItemSlot = StoreSlot, Durability = 20 };
        item.SetStorePrice(price);
        await seller.ShopStorage!.AddItemAsync(StoreSlot, item).ConfigureAwait(false);
        return item;
    }

    private async ValueTask<Item> AddInventoryItemAsync(Player player, byte slot, ItemDefinition definition, byte level = 0)
    {
        var item = new Persistence.BasicModel.Item { Definition = definition, ItemSlot = slot, Durability = 1, Level = level };
        await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false);
        return item;
    }
}
