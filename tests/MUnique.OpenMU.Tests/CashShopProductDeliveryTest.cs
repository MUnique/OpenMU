// <copyright file="CashShopProductDeliveryTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Persistence;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the delivery of cash shop products by <see cref="ICashShopProductDeliveryPlugIn"/>s.
/// </summary>
[TestFixture]
public class CashShopProductDeliveryTest
{
    private const int VaultCertificatePrice = 10;
    private const int MagicBackpackPrice = 20;
    private const int RageFighterCardPrice = 30;
    private const int OtherGroupItemPrice = 40;
    private const byte RageFighterNumber = 24;

    private readonly CashShopActions _actions = new();

    /// <summary>
    /// Tests that using a Vault Expansion Certificate extends the vault instead of adding an item to the inventory,
    /// and that a second certificate can't be used then and stays in the storage.
    /// </summary>
    [Test]
    public async Task VaultCertificateExtendsVaultAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, VaultExtensionDeliveryPlugIn>();
        await AddStorageItemAsync(player, VaultCertificatePrice).ConfigureAwait(false);
        await AddStorageItemAsync(player, VaultCertificatePrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, VaultCertificatePrice).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, VaultCertificatePrice).ConfigureAwait(false);

        Assert.That(player.Account!.IsVaultExtended, Is.True);
        Assert.That(player.Inventory!.Items, Is.Empty);
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Has.Count.EqualTo(1));
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.Success), Times.Once);
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.CannotUse), Times.Once);
    }

    /// <summary>
    /// Tests that a product whose delivery plugin is inactive can't be used and stays in the storage,
    /// instead of being put into the inventory, and that it can't be bought.
    /// </summary>
    [Test]
    public async Task InactiveDeliveryPlugInKeepsProductInStorageAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, VaultExtensionDeliveryPlugIn>();
        player.GameContext.PlugInManager.DeactivatePlugIn<VaultExtensionDeliveryPlugIn>();
        player.Account!.WCoinC = 100;
        await AddStorageItemAsync(player, VaultCertificatePrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, VaultCertificatePrice).ConfigureAwait(false);
        await this._actions.BuyAsync(player, VaultCertificatePrice, 0, CashShopCoinType.WCoinC).ConfigureAwait(false);

        Assert.That(player.Account.IsVaultExtended, Is.False);
        Assert.That(player.Inventory!.Items, Is.Empty);
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Has.Count.EqualTo(1));
        Assert.That(player.Account.WCoinC, Is.EqualTo(100));
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.CannotUse), Times.Once);
        GetView(player).Verify(v => v.ShowBuyResultAsync(CashShopBuyResult.NotForSale), Times.Once);
    }

    /// <summary>
    /// Tests that a product without a known delivery plugin is put into the inventory.
    /// </summary>
    [Test]
    public async Task ProductWithoutDeliveryPlugInIsPutIntoInventoryAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        await AddStorageItemAsync(player, VaultCertificatePrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, VaultCertificatePrice).ConfigureAwait(false);

        Assert.That(player.Account!.IsVaultExtended, Is.False);
        Assert.That(player.Inventory!.Items.Count(), Is.EqualTo(1));
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// Tests that the strategy of an item takes precedence over the strategy of its item group,
    /// and that the strategy of the group delivers the other items of the group.
    /// </summary>
    [Test]
    public async Task ItemStrategyTakesPrecedenceOverGroupStrategyAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, VaultExtensionDeliveryPlugIn>();
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, GroupDeliveryPlugIn>();
        await AddStorageItemAsync(player, VaultCertificatePrice).ConfigureAwait(false);
        await AddStorageItemAsync(player, OtherGroupItemPrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, VaultCertificatePrice).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, OtherGroupItemPrice).ConfigureAwait(false);

        Assert.That(player.Account!.IsVaultExtended, Is.True);
        Assert.That(player.Account.GoblinPoints, Is.EqualTo(GroupDeliveryPlugIn.Points), "the group strategy delivered the other item once");
        Assert.That(player.Inventory!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that the Magic Backpack adds inventory extensions up to the configured maximum.
    /// </summary>
    [Test]
    public async Task MagicBackpackAddsInventoryExtensionsUpToMaximumAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, InventoryExtensionDeliveryPlugIn>();
        var plugIn = (InventoryExtensionDeliveryPlugIn)player.GameContext.PlugInManager.GetStrategy<ItemIdentifier, ICashShopProductDeliveryPlugIn>(InventoryExtensionDeliveryPlugIn.MagicBackpack)!;
        plugIn.Configuration = new InventoryExtensionDeliveryConfiguration { MaximumExtensions = 1 };
        await AddStorageItemAsync(player, MagicBackpackPrice).ConfigureAwait(false);
        await AddStorageItemAsync(player, MagicBackpackPrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, MagicBackpackPrice).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, MagicBackpackPrice).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.InventoryExtensions, Is.EqualTo(1));
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Has.Count.EqualTo(1));
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.CannotUse), Times.Once);
    }

    /// <summary>
    /// Tests that a character card unlocks its character class, and that a second card can't be used then.
    /// </summary>
    [Test]
    public async Task CharacterCardUnlocksCharacterClassAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        var rageFighter = new CharacterClass { Number = RageFighterNumber, Name = new("Rage Fighter") };
        player.GameContext.Configuration.CharacterClasses.Add(rageFighter);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, RageFighterCharacterCardDeliveryPlugIn>();
        await AddStorageItemAsync(player, RageFighterCardPrice).ConfigureAwait(false);
        await AddStorageItemAsync(player, RageFighterCardPrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, RageFighterCardPrice).ConfigureAwait(false);
        await this._actions.UseStorageItemAsync(player, 0, RageFighterCardPrice).ConfigureAwait(false);

        Assert.That(player.Account!.UnlockedCharacterClasses, Is.EquivalentTo(new[] { rageFighter }));
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that a character card can't be used, when its character class isn't configured.
    /// </summary>
    [Test]
    public async Task CharacterCardOfMissingClassCantBeUsedAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<ICashShopProductDeliveryPlugIn, RageFighterCharacterCardDeliveryPlugIn>();
        await AddStorageItemAsync(player, RageFighterCardPrice).ConfigureAwait(false);

        await this._actions.UseStorageItemAsync(player, 0, RageFighterCardPrice).ConfigureAwait(false);

        Assert.That(player.Account!.UnlockedCharacterClasses, Is.Empty);
        Assert.That(await GetStorageItemsAsync(player).ConfigureAwait(false), Has.Count.EqualTo(1));
        GetView(player).Verify(v => v.ShowUseResultAsync(CashShopUseResult.CannotUse), Times.Once);
    }

    private static Mock<ICashShopViewPlugIn> GetView(Player player)
    {
        return Mock.Get(player.ViewPlugIns.GetPlugIn<ICashShopViewPlugIn>()!);
    }

    private static async ValueTask AddStorageItemAsync(Player player, int priceSequence)
    {
        var item = player.PersistenceContext.CreateNew<CashShopStorageItem>();
        item.AccountId = player.Account!.GetId();
        item.ProductSequence = priceSequence;
        item.PriceSequence = priceSequence;
        item.AddedAt = DateTime.UtcNow;
        await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private static ValueTask<IReadOnlyList<CashShopStorageItem>> GetStorageItemsAsync(Player player)
    {
        return player.PersistenceContext.GetCashShopStorageItemsAsync(player.Account!.GetId());
    }

    private static CashShopConfiguration CreateConfiguration()
    {
        var configuration = new BasicModel.CashShopConfiguration();
        configuration.Packages.Add(CreatePackage(VaultCertificatePrice, VaultExtensionDeliveryPlugIn.VaultExpansionCertificate));
        configuration.Packages.Add(CreatePackage(MagicBackpackPrice, InventoryExtensionDeliveryPlugIn.MagicBackpack));
        configuration.Packages.Add(CreatePackage(RageFighterCardPrice, RageFighterCharacterCardDeliveryPlugIn.RageFighterCharacterCard));
        configuration.Packages.Add(CreatePackage(OtherGroupItemPrice, new ItemIdentifier(200, 14)));
        return configuration;
    }

    /// <summary>
    /// Creates a package with one product. To keep it simple, the sequence numbers and the price are the same.
    /// </summary>
    private static CashShopPackage CreatePackage(int sequence, ItemIdentifier item)
    {
        var package = new BasicModel.CashShopPackage
        {
            PackageSequence = sequence,
            CoinType = CashShopCoinType.WCoinC,
            IsForSale = true,
        };
        package.Products.Add(new BasicModel.CashShopProduct
        {
            ProductSequence = sequence,
            PriceSequence = sequence,
            Price = sequence,
            ItemDefinition = new ItemDefinition { Group = item.Group, Number = item.Number ?? 0, Width = 1, Height = 1, Durability = 1 },
        });
        return package;
    }

    private async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.AddPlugIn(new CashShopFeaturePlugIn(), true);
        player.GameContext.Configuration.CashShopConfiguration = CreateConfiguration();
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.CurrentMap!.Terrain.SafezoneMap[player.Position.X, player.Position.Y] = true;
        await this._actions.OpenAsync(player).ConfigureAwait(false);
        return player;
    }

    /// <summary>
    /// A delivery strategy for all items of group 14, which gives Goblin Points.
    /// </summary>
    [Guid("6E6A7852-C9A3-4F42-A48D-91835CCA3C88")]
    private sealed class GroupDeliveryPlugIn : ICashShopProductDeliveryPlugIn
    {
        /// <summary>
        /// The points which are given for each delivered product.
        /// </summary>
        public const int Points = 7;

        /// <inheritdoc />
        public ItemIdentifier Key => new(null, 14);

        /// <inheritdoc />
        public bool CanDeliver(CashShopProduct product) => true;

        /// <inheritdoc />
        public ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product)
        {
            player.Account!.GoblinPoints += Points;
            return ValueTask.FromResult(CashShopUseResult.Success);
        }
    }
}
