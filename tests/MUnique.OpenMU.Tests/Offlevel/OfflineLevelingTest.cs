// <copyright file="OfflineLevelingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Item = MUnique.OpenMU.Persistence.BasicModel.Item;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemSlotType = MUnique.OpenMU.Persistence.BasicModel.ItemSlotType;

/// <summary>
/// Tests for offline leveling handlers and intelligence.
/// </summary>
[TestFixture]
public class OfflineLevelingTest
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = this.CreateGameContext();
    }

    // -------------------------------------------------------------------------
    // RepairHandler
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that auto-repair restores item durability when the player has enough Zen.
    /// </summary>
    [Test]
    public async ValueTask RepairHandler_RepairsItemWhenSufficientZen()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that auto-repair does nothing when disabled in the configuration.
    /// </summary>
    [Test]
    public async ValueTask RepairHandler_DoesNothingWhenDisabled()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var config = new MuHelperSettings { RepairItem = false };
        var handler = new RepairHandler(player, config);
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that auto-repair does not repair when the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask RepairHandler_DoesNotRepairWhenInsufficientZen()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that fully durable items are skipped by the repair handler.
    /// </summary>
    [Test]
    public async ValueTask RepairHandler_SkipsFullyDurableItems()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 100);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        var initialMoney = 1_000_000;
        player.TryAddMoney(initialMoney);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(initialMoney));
    }

    // -------------------------------------------------------------------------
    // ItemPickupHandler
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that pickup does nothing when all pickup options are disabled.
    /// </summary>
    [Test]
    public async ValueTask ItemPickupHandler_DoesNothingWhenAllDisabled()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            PickAllItems = false,
            PickJewel = false,
            PickAncient = false,
            PickZen = false,
            PickExcellent = false,
        };

        var handler = new ItemPickupHandler(player, config);
        Assert.DoesNotThrowAsync(async () =>
            await handler.PickupItemsAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// Tests that pickup does nothing when config is null.
    /// </summary>
    [Test]
    public async ValueTask ItemPickupHandler_DoesNothingWhenConfigNull()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new ItemPickupHandler(player, null);

        Assert.DoesNotThrowAsync(async () =>
            await handler.PickupItemsAsync().ConfigureAwait(false));
    }

    // -------------------------------------------------------------------------
    // CombatHandler
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that <see cref="HealingHandler.PerformHealthRecoveryAsync"/> does nothing
    /// when config is null.
    /// </summary>
    [Test]
    public async ValueTask HealingHandler_DoesNothingWhenConfigNull()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new HealingHandler(player, null);

        Assert.DoesNotThrowAsync(async () =>
            await handler.PerformHealthRecoveryAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// Tests that <see cref="HealingHandler.PerformHealthRecoveryAsync"/> does not consume
    /// a potion when the player's HP is above the threshold.
    /// </summary>
    [Test]
    public async ValueTask HealingHandler_DoesNotUsePotionAboveThreshold()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var maxHp = player.Attributes![Stats.MaximumHealth];
        player.Attributes[Stats.CurrentHealth] = maxHp * 0.9f;

        var potion = this.CreateHealthPotion();
        await player.Inventory!.AddItemAsync((byte)(InventoryConstants.FirstEquippableItemSlotIndex + 12), potion).ConfigureAwait(false);

        var config = new MuHelperSettings
        {
            UseHealPotion = true,
            PotionThresholdPercent = 50,
        };

        var handler = new HealingHandler(player, config);
        await handler.PerformHealthRecoveryAsync().ConfigureAwait(false);

        Assert.That(player.Inventory?.GetItem(potion.ItemSlot), Is.Not.Null);
    }

    // -------------------------------------------------------------------------
    // BuffHandler
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true immediately
    /// when no buff skills are configured.
    /// </summary>
    [Test]
    public async ValueTask BuffHandler_ReturnsTrue_WhenNoBuffsConfigured()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            BuffSkill0Id = 0,
            BuffSkill1Id = 0,
            BuffSkill2Id = 0,
        };

        var handler = new BuffHandler(player, config);
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true when config is null.
    /// </summary>
    [Test]
    public async ValueTask BuffHandler_ReturnsTrue_WhenConfigNull()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new BuffHandler(player, null);
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        Assert.That(result, Is.True);
    }

    // -------------------------------------------------------------------------
    // OfflineLevelingIntelligence
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests that the intelligence can be created and started without throwing.
    /// </summary>
    [Test]
    public async ValueTask Intelligence_StartsWithoutException()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);

        OfflineLevelingIntelligence? intelligence = null;
        Assert.DoesNotThrow(() =>
        {
            intelligence = new OfflineLevelingIntelligence(player);
            intelligence.Start();
        });

        intelligence?.Dispose();
    }

    /// <summary>
    /// Tests that disposing the intelligence twice does not throw.
    /// </summary>
    [Test]
    public async ValueTask Intelligence_DisposeTwice_DoesNotThrow()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var intelligence = new OfflineLevelingIntelligence(player);
        intelligence.Start();

        intelligence.Dispose();
        Assert.DoesNotThrow(() => intelligence.Dispose());
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await TestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

    private Item CreateDamagedItem(byte maxDurability, byte currentDurability)
    {
        return new Item
        {
            Definition = new ItemDefinition
            {
                Durability = maxDurability,
                ItemSlot = new ItemSlotType(),
                Width = 1,
                Height = 1,
                Value = 1000,
            },
            Durability = currentDurability,
        };
    }

    private MUnique.OpenMU.DataModel.Entities.Item CreateHealthPotion()
    {
        var definition = new Mock<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definition.Object.Number = ItemConstants.SmallHealingPotion.Number!.Value;
        definition.Object.Group = ItemConstants.SmallHealingPotion.Group;

        var item = new Mock<MUnique.OpenMU.DataModel.Entities.Item>();
        item.SetupAllProperties();
        item.Setup(i => i.Definition).Returns(definition.Object);
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Durability = 1;

        return item.Object;
    }

    private IGameContext CreateGameContext()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfig = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
        gameConfig.Maps.Add(contextProvider.CreateNewContext().CreateNew<GameMapDefinition>());
        gameConfig.MaximumPartySize = 5;
        gameConfig.RecoveryInterval = int.MaxValue;
        gameConfig.MaximumInventoryMoney = int.MaxValue;

        var mapInitializer = new MapInitializer(gameConfig, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(gameConfig, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        return gameContext;
    }
}