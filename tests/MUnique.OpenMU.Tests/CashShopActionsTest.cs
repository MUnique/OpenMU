// <copyright file="CashShopActionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="CashShopActions"/> and the <see cref="CashShopVersionPlugIn"/>.
/// </summary>
[TestFixture]
public class CashShopActionsTest
{
    private const int OptionsPackage = 100;
    private const int BundlePackage = 200;
    private const int TimeLimitedPackage = 300;
    private const int NotForSalePackage = 400;
    private const int UseTestsPackage = 500;
    private const string SenderName = "Sender";
    private const string RecipientName = "Recipient";

    private readonly CashShopActions _actions = new();

    /// <summary>
    /// Tests that the cash shop is opened, when the player stands in the safezone.
    /// </summary>
    [Test]
    public async Task OpenInSafezoneAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);

        await this._actions.OpenAsync(player).ConfigureAwait(false);

        Assert.That(player.IsCashShopOpen, Is.True);
        GetView(player).Verify(v => v.ShowOpenResultAsync(true), Times.Once);
    }

    /// <summary>
    /// Tests that the cash shop can't be opened outside of the safezone.
    /// </summary>
    [Test]
    public async Task OpenOutsideOfSafezoneIsDeniedAsync()
    {
        var player = await CreatePlayerAsync(isAtSafezone: false).ConfigureAwait(false);

        await this._actions.OpenAsync(player).ConfigureAwait(false);

        Assert.That(player.IsCashShopOpen, Is.False);
        GetView(player).Verify(v => v.ShowOpenResultAsync(false), Times.Once);
    }

    /// <summary>
    /// Tests that the cash shop can't be opened, when it's not configured.
    /// </summary>
    [Test]
    public async Task OpenWithoutConfigurationIsDeniedAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.CashShopConfiguration = null;

        await this._actions.OpenAsync(player).ConfigureAwait(false);

        Assert.That(player.IsCashShopOpen, Is.False);
        GetView(player).Verify(v => v.ShowOpenResultAsync(false), Times.Once);
    }

    /// <summary>
    /// Tests that the points and the storage are only shown, while the cash shop is open.
    /// </summary>
    [Test]
    public async Task PointsAndStorageRequireOpenedCashShopAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var view = GetView(player);

        await this._actions.ShowPointsAsync(player).ConfigureAwait(false);
        await this._actions.ShowStorageAsync(player, 1, false).ConfigureAwait(false);
        view.Verify(v => v.ShowPointsAsync(It.IsAny<Account>()), Times.Never);
        view.Verify(v => v.ShowStorageAsync(It.IsAny<IReadOnlyList<CashShopStorageItem>>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);

        await this._actions.OpenAsync(player).ConfigureAwait(false);
        await this._actions.ShowPointsAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowPointsAsync(player.Account!), Times.Once);

        this._actions.Close(player);
        await this._actions.ShowPointsAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowPointsAsync(player.Account!), Times.Once);
    }

    /// <summary>
    /// Tests that the storage shows the items of the account, ordered by the time they were added.
    /// </summary>
    [Test]
    public async Task StorageShowsItemsOfAccountAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var accountId = player.Account!.GetId();
        var first = await AddStorageItemAsync(player, accountId, 10, isGift: false, DateTime.UtcNow.AddMinutes(-2)).ConfigureAwait(false);
        var second = await AddStorageItemAsync(player, accountId, 11, isGift: false, DateTime.UtcNow.AddMinutes(-1)).ConfigureAwait(false);
        var gift = await AddStorageItemAsync(player, accountId, 12, isGift: true, DateTime.UtcNow).ConfigureAwait(false);
        await AddStorageItemAsync(player, Guid.NewGuid(), 13, isGift: false, DateTime.UtcNow).ConfigureAwait(false);
        await this._actions.OpenAsync(player).ConfigureAwait(false);

        await this._actions.ShowStorageAsync(player, 1, false).ConfigureAwait(false);
        await this._actions.ShowStorageAsync(player, 2, true).ConfigureAwait(false);

        var view = GetView(player);
        view.Verify(v => v.ShowStorageAsync(It.Is<IReadOnlyList<CashShopStorageItem>>(items => items.SequenceEqual(new[] { first, second, gift })), 1, false), Times.Once);
        view.Verify(v => v.ShowStorageAsync(It.Is<IReadOnlyList<CashShopStorageItem>>(items => items.SequenceEqual(new[] { first, second, gift })), 2, true), Times.Once);
    }

    /// <summary>
    /// Tests that the versions of the cash shop are shown, when the player enters the game.
    /// </summary>
    [Test]
    public async Task VersionsAreShownWhenEnteringTheGameAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new CashShopVersionPlugIn();
        var configuration = player.GameContext.Configuration.CashShopConfiguration!;

        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);
        await plugIn.PlayerStateChangedAsync(player, PlayerState.EnteredWorld, PlayerState.Dead).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowVersionsAsync(configuration), Times.Once);
    }

    /// <summary>
    /// Tests that buying a price option of a package takes the coins and adds the product to the storage.
    /// </summary>
    [Test]
    public async Task BuyPriceOptionAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.Success), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(50));
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(storage.Select(item => (item.ProductSequence, item.PriceSequence, item.IsGift)), Is.EqualTo(new[] { (1, 11, false) }));
    }

    /// <summary>
    /// Tests that buying a bundle takes the price of the package and adds all of its products to the storage.
    /// </summary>
    [Test]
    public async Task BuyBundleAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.GoblinPoints = 80;

        await this._actions.BuyAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.Success), Times.Once);
        Assert.That(player.Account.GoblinPoints, Is.Zero);
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(storage.Select(item => item.PriceSequence), Is.EquivalentTo(new[] { 21, 22 }));
    }

    /// <summary>
    /// Tests the reasons why a package can't be bought. Nothing is taken or added then.
    /// </summary>
    /// <param name="packageSequence">The package sequence number.</param>
    /// <param name="priceSequence">The price sequence number.</param>
    /// <param name="coinType">The coin type.</param>
    /// <param name="expectedResult">The expected result.</param>
    [TestCase(OptionsPackage, 12, CashShopCoinType.WCoinC, CashShopBuyResult.NotEnoughCoins)]
    [TestCase(OptionsPackage, 11, CashShopCoinType.WCoinP, CashShopBuyResult.WrongCoinType)]
    [TestCase(OptionsPackage, 11, null, CashShopBuyResult.WrongCoinType)]
    [TestCase(OptionsPackage, 0, CashShopCoinType.WCoinC, CashShopBuyResult.InvalidPriceOption)]
    [TestCase(OptionsPackage, 21, CashShopCoinType.WCoinC, CashShopBuyResult.InvalidPriceOption)]
    [TestCase(999, 11, CashShopCoinType.WCoinC, CashShopBuyResult.PackageNotFound)]
    [TestCase(TimeLimitedPackage, 31, CashShopCoinType.WCoinP, CashShopBuyResult.NotForSale)]
    [TestCase(NotForSalePackage, 41, CashShopCoinType.WCoinC, CashShopBuyResult.NotForSale)]
    public async Task BuyIsRefusedAsync(int packageSequence, int priceSequence, CashShopCoinType? coinType, CashShopBuyResult expectedResult)
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        player.Account.WCoinP = 100;

        await this._actions.BuyAsync(player, packageSequence, priceSequence, coinType).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(expectedResult), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        Assert.That(player.Account.WCoinP, Is.EqualTo(100));
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(storage, Is.Empty);
    }

    /// <summary>
    /// Tests that nothing can be bought while the cash shop is closed.
    /// </summary>
    [Test]
    public async Task BuyRequiresOpenedCashShopAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(It.IsAny<CashShopBuyResult>()), Times.Never);
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that using an item of the storage adds its item to the inventory and removes it from the storage.
    /// </summary>
    [Test]
    public async Task UseStorageItemAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, 11).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.Success), Times.Once);
        var item = player.Inventory!.Items.Single();
        Assert.That(item.Definition!.Name.ToString(), Is.EqualTo("Option item"));
        Assert.That(item.Level, Is.EqualTo(3));
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(storage, Is.Empty);
    }

    /// <summary>
    /// Tests that an item isn't used, when the storage index doesn't refer to an item with the price sequence number.
    /// </summary>
    /// <param name="storageIndex">The storage index.</param>
    /// <param name="priceSequence">The price sequence number.</param>
    [TestCase(0, 12)]
    [TestCase(1, 11)]
    [TestCase(-1, 11)]
    public async Task UseOfMissingStorageItemIsRefusedAsync(int storageIndex, int priceSequence)
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, player.Account!.GetId(), 11, isGift: false, DateTime.UtcNow, productSequence: 1).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, storageIndex, priceSequence).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.ItemNotFound), Times.Once);
        Assert.That(player.Inventory!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that an item can't be used, when its product doesn't exist in the configuration anymore.
    /// </summary>
    [Test]
    public async Task UseOfUnknownProductIsRefusedAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, player.Account!.GetId(), 999, isGift: false, DateTime.UtcNow).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, 999).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.CannotUse), Times.Once);
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account!.GetId()).ConfigureAwait(false);
        Assert.That(storage, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that no item of a product is added, when the inventory doesn't have space for all of them.
    /// The product has three items of which two fill the whole inventory, so the third one doesn't fit
    /// and the first two are removed again.
    /// </summary>
    [Test]
    public async Task UseWithFullInventoryIsRefusedAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, player.Account!.GetId(), 51, isGift: false, DateTime.UtcNow, productSequence: 6).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, 51).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.InventoryFull), Times.Once);
        Assert.That(player.Inventory!.Items, Is.Empty);
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account!.GetId()).ConfigureAwait(false);
        Assert.That(storage, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that the items of a stackable product are stacked up to the maximum durability of the item.
    /// </summary>
    [Test]
    public async Task UseOfStackableProductAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, player.Account!.GetId(), 61, isGift: false, DateTime.UtcNow, productSequence: 7).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, 61).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.Success), Times.Once);
        Assert.That(player.Inventory!.Items.Select(item => item.Durability), Is.EquivalentTo(new double[] { 10, 5 }));
    }

    /// <summary>
    /// Tests that a gift takes the coins of the sender and adds the products to the gift storage
    /// of the account of the recipient, with the sender and the message.
    /// </summary>
    [Test]
    public async Task GiftAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.GoblinPoints = 100;
        var recipientAccountId = await CreateRecipientAsync(player).ConfigureAwait(false);

        await this._actions.GiftAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, "Have fun").ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowGiftResultAsync(CashShopGiftResult.Success), Times.Once);
        Assert.That(player.Account.GoblinPoints, Is.EqualTo(20));
        var senderStorage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(senderStorage, Is.Empty);
        var recipientStorage = await player.PersistenceContext.GetCashShopStorageItemsAsync(recipientAccountId).ConfigureAwait(false);
        Assert.That(recipientStorage.Select(item => item.PriceSequence), Is.EquivalentTo(new[] { 21, 22 }));
        Assert.That(recipientStorage, Has.All.Matches<CashShopStorageItem>(item => item.IsGift && item.GiftSenderName == SenderName && item.GiftMessage == "Have fun"));
    }

    /// <summary>
    /// Tests the reasons why a gift can't be sent. The coins of the sender aren't taken then.
    /// </summary>
    /// <param name="packageSequence">The package sequence number.</param>
    /// <param name="priceSequence">The price sequence number.</param>
    /// <param name="coinType">The coin type.</param>
    /// <param name="recipientName">The name of the recipient.</param>
    /// <param name="expectedResult">The expected result.</param>
    [TestCase(OptionsPackage, 11, CashShopCoinType.WCoinC, RecipientName, CashShopGiftResult.NotGiftable)]
    [TestCase(BundlePackage, 0, CashShopCoinType.GoblinPoints, "Unknown", CashShopGiftResult.RecipientNotFound)]
    [TestCase(BundlePackage, 0, CashShopCoinType.GoblinPoints, "", CashShopGiftResult.RecipientNotFound)]
    [TestCase(BundlePackage, 0, CashShopCoinType.WCoinC, RecipientName, CashShopGiftResult.WrongCoinType)]
    public async Task GiftIsRefusedAsync(int packageSequence, int priceSequence, CashShopCoinType coinType, string recipientName, CashShopGiftResult expectedResult)
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        player.Account.GoblinPoints = 100;
        var recipientAccountId = await CreateRecipientAsync(player).ConfigureAwait(false);

        await this._actions.GiftAsync(player, packageSequence, priceSequence, coinType, recipientName, string.Empty).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowGiftResultAsync(expectedResult), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        Assert.That(player.Account.GoblinPoints, Is.EqualTo(100));
        var recipientStorage = await player.PersistenceContext.GetCashShopStorageItemsAsync(recipientAccountId).ConfigureAwait(false);
        Assert.That(recipientStorage, Is.Empty);
    }

    private static async ValueTask<Guid> CreateRecipientAsync(Player player)
    {
        player.SelectedCharacter!.Name = SenderName;
        var account = player.PersistenceContext.CreateNew<Account>();
        account.LoginName = "recipient";
        var character = player.PersistenceContext.CreateNew<Character>();
        character.Name = RecipientName;
        account.Characters.Add(character);
        await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        return account.GetId();
    }

    /// <summary>
    /// Tests that the pending coin grants of the account are applied once, when the points are shown.
    /// Negative grants don't take the balance below zero, and grants of other accounts aren't applied.
    /// </summary>
    [Test]
    public async Task PendingCoinGrantsAreAppliedOnceAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        var accountId = player.Account!.GetId();
        player.Account!.WCoinP = 10;
        var grants = new[]
        {
            await AddCoinGrantAsync(player, accountId, CashShopCoinType.WCoinC, 100).ConfigureAwait(false),
            await AddCoinGrantAsync(player, accountId, CashShopCoinType.WCoinC, 20).ConfigureAwait(false),
            await AddCoinGrantAsync(player, accountId, CashShopCoinType.WCoinP, -50).ConfigureAwait(false),
        };
        var otherGrant = await AddCoinGrantAsync(player, Guid.NewGuid(), CashShopCoinType.GoblinPoints, 5).ConfigureAwait(false);

        await this._actions.ShowPointsAsync(player).ConfigureAwait(false);
        await this._actions.ShowPointsAsync(player).ConfigureAwait(false);

        Assert.That(player.Account!.WCoinC, Is.EqualTo(120));
        Assert.That(player.Account.WCoinP, Is.Zero);
        Assert.That(player.Account.GoblinPoints, Is.Zero);
        Assert.That(grants, Has.All.Matches<CashShopCoinGrant>(grant => grant.AppliedAt is not null));
        Assert.That(otherGrant.AppliedAt, Is.Null);
        var pending = await player.PersistenceContext.GetPendingCashShopCoinGrantsAsync(accountId).ConfigureAwait(false);
        Assert.That(pending, Is.Empty);
    }

    /// <summary>
    /// Tests that the pending coin grants are applied before a purchase, so that they can be spent right away.
    /// </summary>
    [Test]
    public async Task PendingCoinGrantsAreAppliedBeforePurchaseAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddCoinGrantAsync(player, player.Account!.GetId(), CashShopCoinType.WCoinC, 100).ConfigureAwait(false);

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.Success), Times.Once);
        Assert.That(player.Account!.WCoinC, Is.EqualTo(50));
    }

    private static async ValueTask<CashShopCoinGrant> AddCoinGrantAsync(Player player, Guid accountId, CashShopCoinType coinType, int amount)
    {
        var grant = player.PersistenceContext.CreateNew<CashShopCoinGrant>();
        grant.AccountId = accountId;
        grant.CoinType = coinType;
        grant.Amount = amount;
        grant.Reason = "Test";
        grant.CreatedAt = DateTime.UtcNow;
        await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        return grant;
    }

    /// <summary>
    /// Tests that the cash shop can't be opened and its versions aren't sent, when the feature plugin is deactivated.
    /// </summary>
    [Test]
    public async Task DeactivatedFeatureClosesCashShopAsync()
    {
        var player = await CreatePlayerAsync(isFeatureActive: false).ConfigureAwait(false);

        await this._actions.OpenAsync(player).ConfigureAwait(false);
        await new CashShopVersionPlugIn().PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.IsCashShopOpen, Is.False);
        GetView(player).Verify(v => v.ShowOpenResultAsync(false), Times.Once);
        GetView(player).Verify(v => v.ShowVersionsAsync(It.IsAny<CashShopConfiguration>()), Times.Never);
    }

    /// <summary>
    /// Tests that the maximum number of storage items applies to purchases and gifts.
    /// </summary>
    [Test]
    public async Task MaximumStorageItemsAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync(new CashShopSettings { MaximumStorageItems = 2 }).ConfigureAwait(false);
        player.Account!.WCoinC = 1000;
        player.Account.GoblinPoints = 1000;
        await CreateRecipientAsync(player).ConfigureAwait(false);

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);
        await this._actions.BuyAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints).ConfigureAwait(false);
        await this._actions.GiftAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, string.Empty).ConfigureAwait(false);
        await this._actions.GiftAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, string.Empty).ConfigureAwait(false);

        var view = GetView(player);
        view.Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.Success), Times.Once);
        view.Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.StorageFull), Times.Once, "the bundle has two products, but only one is free");
        view.Verify(v => v.ShowGiftResultAsync(CashShopGiftResult.Success), Times.Once);
        view.Verify(v => v.ShowGiftResultAsync(CashShopGiftResult.RecipientStorageFull), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(950));
        Assert.That(player.Account.GoblinPoints, Is.EqualTo(920));
    }

    /// <summary>
    /// Tests that gifts can be deactivated, and that gifts to the own account can be refused.
    /// </summary>
    [Test]
    public async Task GiftSettingsAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync(new CashShopSettings { IsGiftingEnabled = false }).ConfigureAwait(false);
        player.Account!.GoblinPoints = 100;
        await CreateRecipientAsync(player).ConfigureAwait(false);
        await this._actions.GiftAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, string.Empty).ConfigureAwait(false);
        GetView(player).Verify(v => v.ShowGiftResultAsync(CashShopGiftResult.NotGiftable), Times.Once);

        var ownAccountPlayer = await this.CreateOpenedCashShopPlayerAsync(new CashShopSettings { CanGiftToOwnAccount = false }).ConfigureAwait(false);
        var ownAccountId = await CreateRecipientAsync(ownAccountPlayer).ConfigureAwait(false);
        var ownAccount = (await ownAccountPlayer.PersistenceContext.GetAsync<Account>().ConfigureAwait(false)).First(a => a.GetId() == ownAccountId);
        ownAccount.GoblinPoints = 100;
        ownAccountPlayer.Account = ownAccount;
        await this._actions.GiftAsync(ownAccountPlayer, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, string.Empty).ConfigureAwait(false);
        GetView(ownAccountPlayer).Verify(v => v.ShowGiftResultAsync(CashShopGiftResult.RecipientIsOwnAccount), Times.Once);
        Assert.That(ownAccount.GoblinPoints, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that a <see cref="ICashShopPackageBuyingPlugIn"/> can refuse a purchase, before the coins are taken.
    /// </summary>
    [Test]
    public async Task BuyingPlugInCanRefusePurchaseAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        var buyingPlugIn = new Mock<ICashShopPackageBuyingPlugIn>();
        buyingPlugIn
            .Setup(p => p.PackageBuyingAsync(It.IsAny<Player>(), It.IsAny<CashShopPackage>(), It.IsAny<IReadOnlyCollection<CashShopProduct>>(), It.IsAny<string?>(), It.IsAny<CancelEventArgs>()))
            .Callback<Player, CashShopPackage, IReadOnlyCollection<CashShopProduct>, string?, CancelEventArgs>((_, _, _, _, args) => args.Cancel = true)
            .Returns(ValueTask.CompletedTask);
        var boughtPlugIn = new Mock<ICashShopPackageBoughtPlugIn>();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(buyingPlugIn.Object);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(boughtPlugIn.Object);

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.RefusedByPlugIn), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        boughtPlugIn.Verify(p => p.PackageBoughtAsync(It.IsAny<Player>(), It.IsAny<CashShopPackage>(), It.IsAny<IReadOnlyCollection<CashShopProduct>>(), It.IsAny<int>(), It.IsAny<string?>()), Times.Never);
    }

    /// <summary>
    /// Tests that a <see cref="ICashShopPackageBoughtPlugIn"/> is called after a purchase and a gift.
    /// </summary>
    [Test]
    public async Task BoughtPlugInIsCalledAfterPurchaseAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        player.Account.GoblinPoints = 100;
        await CreateRecipientAsync(player).ConfigureAwait(false);
        var boughtPlugIn = new Mock<ICashShopPackageBoughtPlugIn>();
        boughtPlugIn
            .Setup(p => p.PackageBoughtAsync(It.IsAny<Player>(), It.IsAny<CashShopPackage>(), It.IsAny<IReadOnlyCollection<CashShopProduct>>(), It.IsAny<int>(), It.IsAny<string?>()))
            .Returns(ValueTask.CompletedTask);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(boughtPlugIn.Object);

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);
        await this._actions.GiftAsync(player, BundlePackage, 0, CashShopCoinType.GoblinPoints, RecipientName, string.Empty).ConfigureAwait(false);

        boughtPlugIn.Verify(
            p => p.PackageBoughtAsync(player, It.Is<CashShopPackage>(package => package.PackageSequence == OptionsPackage), It.Is<IReadOnlyCollection<CashShopProduct>>(products => products.Single().PriceSequence == 11), 50, null),
            Times.Once);
        boughtPlugIn.Verify(
            p => p.PackageBoughtAsync(player, It.Is<CashShopPackage>(package => package.PackageSequence == BundlePackage), It.Is<IReadOnlyCollection<CashShopProduct>>(products => products.Count == 2), 80, RecipientName),
            Times.Once);
    }

    /// <summary>
    /// Tests that a purchase is rolled back when its save fails, which throws: the coins are restored
    /// and no storage item is left in the context, which the next save would commit otherwise.
    /// </summary>
    [Test]
    public async Task PurchaseIsRolledBackWhenTheSaveFailsAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        var saveFails = true;
        SaveFailingContext.Install(player, () => saveFails);

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);
        saveFails = false;
        await player.SaveProgressAsync().ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.SaveFailed), Times.Once);
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        var storage = await player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account.GetId()).ConfigureAwait(false);
        Assert.That(storage, Is.Empty);
    }

    /// <summary>
    /// Tests that a storage item can't be used twice when the save after its use fails. The database still
    /// contains it until the next save succeeds.
    /// </summary>
    [Test]
    public async Task UsedItemIsSkippedWhenTheSaveFailsAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, player.Account!.GetId(), 11, isGift: false, DateTime.UtcNow, productSequence: 1).ConfigureAwait(false);
        SaveFailingContext.Install(player, () => true);

        await this._actions.UseStorageItemAsync(player, 0, 11).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, 11).ConfigureAwait(false);

        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.Success), Times.Once);
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.ItemNotFound), Times.Once);
        Assert.That(player.Inventory!.Items.Count(), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that an opened cash shop can't be used anymore when the player left the safezone,
    /// e.g. to keep it from being used while trading.
    /// </summary>
    [Test]
    public async Task ActionsAreRefusedWhenNotAllowedAnymoreAsync()
    {
        var player = await this.CreateOpenedCashShopPlayerAsync().ConfigureAwait(false);
        player.Account!.WCoinC = 100;
        await AddStorageItemAsync(player, player.Account.GetId(), 11, isGift: false, DateTime.UtcNow, productSequence: 1).ConfigureAwait(false);
        player.CurrentMap!.Terrain.SafezoneMap[player.Position.X, player.Position.Y] = false;

        await this._actions.BuyAsync(player, OptionsPackage, 11, CashShopCoinType.WCoinC).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, 11).ConfigureAwait(false);

        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        Assert.That(player.Inventory!.Items, Is.Empty);
        GetView(player).Verify(v => v.ShowBuyResultAsync(It.IsAny<CashShopBuyResult>()), Times.Never);
        GetView(player).Verify(v => v.ShowUseResultAsync(It.IsAny<CashShopUseResult>()), Times.Never);
    }

    private static Mock<ICashShopViewPlugIn> GetView(Player player)
    {
        return Mock.Get(player.ViewPlugIns.GetPlugIn<ICashShopViewPlugIn>()!);
    }

    private static async ValueTask<CashShopStorageItem> AddStorageItemAsync(Player player, Guid accountId, int priceSequence, bool isGift, DateTime addedAt, int? productSequence = null)
    {
        var item = player.PersistenceContext.CreateNew<CashShopStorageItem>();
        item.AccountId = accountId;
        item.ProductSequence = productSequence ?? priceSequence;
        item.PriceSequence = priceSequence;
        item.IsGift = isGift;
        item.AddedAt = addedAt;
        await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        return item;
    }

    private async ValueTask<Player> CreateOpenedCashShopPlayerAsync(CashShopSettings? settings = null)
    {
        var player = await CreatePlayerAsync(settings: settings).ConfigureAwait(false);
        await this._actions.OpenAsync(player).ConfigureAwait(false);
        return player;
    }

    private static async ValueTask<Player> CreatePlayerAsync(bool isAtSafezone = true, CashShopSettings? settings = null, bool isFeatureActive = true)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        if (isFeatureActive)
        {
            player.GameContext.FeaturePlugIns.AddPlugIn(new CashShopFeaturePlugIn { Configuration = settings ?? new CashShopSettings() }, true);
        }

        player.GameContext.Configuration.CashShopConfiguration = CreateConfiguration();
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.CurrentMap!.Terrain.SafezoneMap[player.Position.X, player.Position.Y] = isAtSafezone;
        return player;
    }

    private static CashShopConfiguration CreateConfiguration()
    {
        var configuration = new BasicModel.CashShopConfiguration();
        configuration.Packages.Add(CreatePackage(OptionsPackage, CashShopCoinType.WCoinC, 0, isBundle: false, (1, 11, 50, TimeSpan.Zero), (1, 12, 120, TimeSpan.Zero)));
        var bundle = CreatePackage(BundlePackage, CashShopCoinType.GoblinPoints, 80, isBundle: true, (2, 21, 0, TimeSpan.Zero), (3, 22, 30, TimeSpan.Zero));
        bundle.IsGiftable = true;
        configuration.Packages.Add(bundle);
        configuration.Packages.Add(CreatePackage(TimeLimitedPackage, CashShopCoinType.WCoinP, 0, isBundle: false, (4, 31, 10, TimeSpan.FromDays(1))));
        var notForSale = CreatePackage(NotForSalePackage, CashShopCoinType.WCoinC, 0, isBundle: false, (5, 41, 10, TimeSpan.Zero));
        notForSale.IsForSale = false;
        configuration.Packages.Add(notForSale);

        var useTests = CreatePackage(UseTestsPackage, CashShopCoinType.WCoinC, 0, isBundle: false);
        useTests.Products.Add(new BasicModel.CashShopProduct
        {
            ProductSequence = 6,
            PriceSequence = 51,
            Quantity = 3,
            ItemDefinition = new ItemDefinition { Width = 8, Height = 4, Durability = 1 },
        });
        useTests.Products.Add(new BasicModel.CashShopProduct
        {
            ProductSequence = 7,
            PriceSequence = 61,
            Quantity = 15,
            ItemDefinition = new ItemDefinition { Width = 1, Height = 1, Durability = 10 },
        });
        configuration.Packages.Add(useTests);
        return configuration;
    }

    private static CashShopPackage CreatePackage(int sequence, CashShopCoinType coinType, int price, bool isBundle, params (int ProductSequence, int PriceSequence, int Price, TimeSpan Duration)[] products)
    {
        var package = new BasicModel.CashShopPackage
        {
            PackageSequence = sequence,
            CoinType = coinType,
            Price = price,
            IsBundle = isBundle,
            IsForSale = true,
        };
        foreach (var (productSequence, priceSequence, productPrice, duration) in products)
        {
            package.Products.Add(new BasicModel.CashShopProduct
            {
                ProductSequence = productSequence,
                PriceSequence = priceSequence,
                Price = productPrice,
                Duration = duration,
                ItemDefinition = new ItemDefinition { Name = new("Option item"), Width = 1, Height = 1, Durability = 1 },
                ItemLevel = 3,
            });
        }

        return package;
    }

    /// <summary>
    /// A player context whose save fails on demand, like the database context, which throws then.
    /// </summary>
    public class SaveFailingContext : DispatchProxy
    {
        private IPlayerContext _inner = null!;

        private Func<bool> _saveFails = null!;

        /// <summary>
        /// Replaces the persistence context of the player by one whose save fails on demand.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="saveFails">Determines whether a save fails.</param>
        public static void Install(Player player, Func<bool> saveFails)
        {
            var proxy = Create<IPlayerContext, SaveFailingContext>();
            var context = (SaveFailingContext)(object)proxy;
            context._inner = player.PersistenceContext;
            context._saveFails = saveFails;
            typeof(Player).GetField($"<{nameof(Player.PersistenceContext)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(player, proxy);
        }

        /// <inheritdoc />
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod!.Name == nameof(IContext.SaveChangesAsync) && this._saveFails())
            {
                throw new InvalidOperationException("The save failed.");
            }

            try
            {
                return targetMethod.Invoke(this._inner, args);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is { } inner)
            {
                ExceptionDispatchInfo.Throw(inner);
                throw;
            }
        }
    }
}
