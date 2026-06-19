// <copyright file="BuyNpcItemActionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="BuyNpcItemAction"/>.
/// </summary>
[TestFixture]
public class BuyNpcItemActionTests
{
    /// <summary>
    /// Tests that buying a stackable item which completes an existing stack invokes the
    /// <see cref="IItemStackedPlugIn"/> plugin point, just like picking up or dragging items does.
    /// This is required so that e.g. a Symbol of Kundun stack completed via an NPC purchase
    /// gets converted into a Lost Map.
    /// </summary>
    [Test]
    public async ValueTask BuyingItemCompletingStackInvokesItemStackedPlugInAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var stackedPlugIn = new RecordingItemStackedPlugIn();
        player.GameContext.PlugInManager!.RegisterPlugInAtPlugInPoint<IItemStackedPlugIn>(stackedPlugIn);

        var definition = CreateDefinition(durability: 5);
        var inventoryStack = CreateItem(definition, 4);
        await player.Inventory!.AddItemAsync(20, inventoryStack).ConfigureAwait(false);

        var storeItem = CreateItem(definition, 1);
        storeItem.ItemSlot = 0;
        player.OpenedNpc = CreateMerchant(storeItem);
        player.Money = 10_000_000;

        var action = new BuyNpcItemAction();
        await action.BuyItemAsync(player, 0).ConfigureAwait(false);

        Assert.That(inventoryStack.Durability, Is.EqualTo(5));
        Assert.That(stackedPlugIn.LastTarget, Is.SameAs(inventoryStack));
        Assert.That(stackedPlugIn.LastSource, Is.SameAs(storeItem));
    }

    private static NonPlayerCharacter CreateMerchant(params Item[] storeItems)
    {
        var storeMock = new Mock<ItemStorage>();
        storeMock.Setup(s => s.Items).Returns(storeItems.ToList());
        var definition = new MonsterDefinition { NpcWindow = NpcWindow.Merchant, MerchantStore = storeMock.Object };
        return new NonPlayerCharacter(null!, definition, null!);
    }

    private static ItemDefinition CreateDefinition(byte width = 1, byte height = 1, byte durability = 1)
    {
        return new ItemDefinition
        {
            Width = width,
            Height = height,
            Durability = durability,
        };
    }

    private static Item CreateItem(ItemDefinition definition, double durability, byte level = 0)
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        item.Object.Durability = durability;
        item.Object.Level = level;
        return item.Object;
    }

    private static async ValueTask<Player> CreateTestPlayerAsync()
    {
        var gameConfig = new Mock<GameConfiguration>();
        gameConfig.SetupAllProperties();
        gameConfig.Setup(c => c.Maps).Returns(new List<GameMapDefinition>());
        gameConfig.Setup(c => c.Items).Returns(new List<ItemDefinition>());
        gameConfig.Setup(c => c.Skills).Returns(new List<Skill>());
        gameConfig.Setup(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration>());
        gameConfig.Setup(c => c.CharacterClasses).Returns(new List<CharacterClass>());
        gameConfig.Setup(c => c.Attributes).Returns(new List<AttributeDefinition>());
        gameConfig.Setup(c => c.GlobalAttributeCombinations).Returns(new List<AttributeRelationship>());
        gameConfig.Setup(c => c.GlobalBaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new(1, Stats.MoneyAmountRate),
        });
        var map = new Mock<GameMapDefinition>();
        map.SetupAllProperties();
        map.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        map.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        map.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Object.RecoveryInterval = int.MaxValue;
        gameConfig.Object.Maps.Add(map.Object);

        var mapInitializer = new MapInitializer(gameConfig.Object, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(
            gameConfig.Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(null, new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        return await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
    }

    [Guid("A1B2C3D4-1111-2222-3333-444455556666")]
    private sealed class RecordingItemStackedPlugIn : IItemStackedPlugIn
    {
        public Item? LastSource { get; private set; }

        public Item? LastTarget { get; private set; }

        public ValueTask ItemStackedAsync(Player player, Item sourceItem, Item targetItem)
        {
            this.LastSource = sourceItem;
            this.LastTarget = targetItem;
            return ValueTask.CompletedTask;
        }
    }
}
