// <copyright file="BotChaosMachineHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for <see cref="BotChaosMachineHandler"/>: which worn pieces a bot takes to the chaos machine, and
/// the mix itself through the regular player actions and crafting handler.
/// </summary>
[TestFixture]
public class BotChaosMachineHandlerTests
{
    private const byte ArmorGroup = 8;
    private const byte WingsGroup = 12;
    private const int MixPrice = 1000;

    private ItemDefinition _chaos = null!;
    private ItemDefinition _bless = null!;
    private ItemDefinition _soul = null!;

    /// <summary>
    /// Creates the jewel definitions.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._chaos = CreateDefinition(12, 15, slot: null);
        this._bless = CreateDefinition(14, 13, slot: null);
        this._soul = CreateDefinition(14, 14, slot: null);
    }

    /// <summary>
    /// Tests that a successful mix puts the piece back on, one level up, and uses up the jewels and the price.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask SuccessfulMixPutsTheUpgradedPieceBackOnAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var armor = await this.WearLuckyArmorAsync(player, 9).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice * 3;

        var mixed = await BotChaosMachineHandler.MixAtAsync(player, this.CreateGoblin(successPercent: 100)).ConfigureAwait(false);

        Assert.That(mixed, Is.True);
        Assert.That(player.Inventory!.GetItem(InventoryConstants.ArmorSlot), Is.SameAs(armor));
        Assert.That(armor.Level, Is.EqualTo(10));
        Assert.That(player.Inventory!.Items.Count(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount), Is.Zero, "the jewels are used up");
        Assert.That(player.Money, Is.EqualTo(MixPrice * 2));
        Assert.That(player.TemporaryStorage!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that a failed mix costs the piece, like it costs a player: the bot goes without it.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask FailedMixCostsThePieceAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var armor = await this.WearLuckyArmorAsync(player, 9).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice;

        var mixed = await BotChaosMachineHandler.MixAtAsync(player, this.CreateGoblin(successPercent: 0)).ConfigureAwait(false);

        Assert.That(mixed, Is.True);
        Assert.That(player.Inventory!.Items, Has.No.Member(armor));
        Assert.That(player.Inventory.GetItem(InventoryConstants.ArmorSlot), Is.Null);
        Assert.That(player.TemporaryStorage!.Items, Is.Empty);
    }

    /// <summary>
    /// Tests that a piece without luck never goes into the machine.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask PieceWithoutLuckIsNotMixedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var armor = await this.WearAsync(player, CreateDefinition(ArmorGroup, 1, InventoryConstants.ArmorSlot, player), 9, luck: false).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice;

        Assert.That(BotChaosMachineHandler.PlanNextMix(player, this.CreateGoblin(100).Definition), Is.Null);
        Assert.That(armor.Level, Is.EqualTo(9));
    }

    /// <summary>
    /// Tests that the bot doesn't go to the machine without all the jewels or without the price.
    /// </summary>
    /// <param name="withJewels">Whether the bot carries the jewels.</param>
    /// <param name="money">The money of the bot.</param>
    /// <returns>The task.</returns>
    [TestCase(false, MixPrice)]
    [TestCase(true, MixPrice - 1)]
    public async ValueTask MixNeedsTheJewelsAndThePriceAsync(bool withJewels, int money)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.WearLuckyArmorAsync(player, 9).ConfigureAwait(false);
        if (withJewels)
        {
            await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        }
        else
        {
            await player.Inventory!.AddItemAsync(CreateItem(this._chaos)).ConfigureAwait(false);
        }

        player.Money = money;

        Assert.That(BotChaosMachineHandler.PlanNextMix(player, this.CreateGoblin(100).Definition), Is.Null);
    }

    /// <summary>
    /// Tests that a piece below +9 has no upgrade crafting (the jewels take it there), and wings never
    /// go into the machine.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask OnlyGearFromPlusNineIsMixedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.WearLuckyArmorAsync(player, 8).ConfigureAwait(false);
        await this.WearAsync(player, CreateDefinition(WingsGroup, 3, InventoryConstants.WingsSlot, player), 9, luck: true).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice;

        Assert.That(BotChaosMachineHandler.PlanNextMix(player, this.CreateGoblin(100).Definition), Is.Null);
    }

    /// <summary>
    /// Tests that a mix the machine refuses gives everything back - the piece goes on again, the jewels
    /// return to the backpack - and is reported as not mixed, so the trips can back off.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask RefusedMixGivesEverythingBackAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var armor = await this.WearLuckyArmorAsync(player, 9).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice;

        var mixed = await BotChaosMachineHandler.MixAtAsync(player, this.CreateGoblin(100, ItemOptionTypes.Excellent)).ConfigureAwait(false);

        Assert.That(mixed, Is.False);
        Assert.That(player.Inventory!.GetItem(InventoryConstants.ArmorSlot), Is.SameAs(armor));
        Assert.That(armor.Level, Is.EqualTo(9));
        Assert.That(player.Inventory.Items.Count(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount), Is.EqualTo(3), "the jewels are back");
        Assert.That(player.TemporaryStorage!.Items, Is.Empty);
        Assert.That(player.Money, Is.EqualTo(MixPrice));
    }

    /// <summary>
    /// Tests that the bot doesn't mix without room for the piece in its backpack, where the machine
    /// returns it - otherwise the upgraded piece would stay stuck in the machine.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask NoRoomForThePieceMeansNoMixAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.WearLuckyArmorAsync(player, 9).ConfigureAwait(false);
        await this.AddJewelsAsync(player, 1).ConfigureAwait(false);
        player.Money = MixPrice;
        var filler = CreateDefinition(14, 100, slot: null);
        while (await player.Inventory!.AddItemAsync(CreateItem(filler)).ConfigureAwait(false))
        {
            // fill every free backpack slot
        }

        Assert.That(BotChaosMachineHandler.PlanNextMix(player, this.CreateGoblin(100).Definition), Is.Null);
    }

    /// <summary>
    /// Tests that only a bot with a worn lucky piece it can still mix keeps Jewels of Chaos; for all the
    /// others they would be blocked backpack slots.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ChaosIsKeptOnlyWithAMixablePieceAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await this.WearAsync(player, CreateDefinition(ArmorGroup, 1, InventoryConstants.ArmorSlot, player), 9, luck: false).ConfigureAwait(false);

        Assert.That(BotJewelHandler.GetStockLimit(player, ItemConstants.JewelOfChaos), Is.Zero);
        Assert.That(BotJewelHandler.GetStockLimit(player, ItemConstants.JewelOfBless), Is.EqualTo(BotJewelHandler.GetStockLimit(player)));

        await this.WearAsync(player, CreateDefinition(ArmorGroup + 1, 1, InventoryConstants.PantsSlot, player), 3, luck: true).ConfigureAwait(false);

        Assert.That(BotJewelHandler.GetStockLimit(player, ItemConstants.JewelOfChaos), Is.EqualTo(BotJewelHandler.GetStockLimit(player)));
    }

    /// <summary>
    /// Tests that the +10 ... +15 combinations of the Season 6 goblin are found for the levels 9 to 14,
    /// with one Chaos and as many Bless and Soul as levels above +9 - and nothing for +8 or +15.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task SeasonSixGoblinOffersTheUpgradesFromNineToFifteenAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new Persistence.Initialization.VersionSeasonSix.DataInitialization(contextProvider, NullLoggerFactory.Instance)
            .CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var goblin = BotChaosMachineHandler.GetChaosMachine(configuration)!;

        Assert.That(BotChaosMachineHandler.FindUpgradeCrafting(goblin, 8), Is.Null);
        Assert.That(BotChaosMachineHandler.FindUpgradeCrafting(goblin, 15), Is.Null);
        for (byte level = 9; level < 15; level++)
        {
            var settings = BotChaosMachineHandler.FindUpgradeCrafting(goblin, level)?.SimpleCraftingSettings;
            Assert.That(settings, Is.Not.Null, $"+{level}");
            int AmountOf(string jewel) => settings!.RequiredItems.Single(r => r.PossibleItems.Any(d => d.Name.ValueInNeutralLanguage == jewel)).MinimumAmount;
            Assert.That(AmountOf("Jewel of Chaos"), Is.EqualTo(1), $"+{level}");
            Assert.That(AmountOf("Jewel of Bless"), Is.EqualTo(level - 8), $"+{level}");
            Assert.That(AmountOf("Jewel of Soul"), Is.EqualTo(level - 8), $"+{level}");
        }
    }

    /// <summary>
    /// Tests that the bot finds a way to the goblin of Noria: the goblin stands on a blocked tile, so no
    /// route from the town gate ends on it, but one ends on the standing point within talk range.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task RouteToTheNoriaGoblinEndsNextToItAsync()
    {
        const int talkRange = 3;
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new Persistence.Initialization.VersionSeasonSix.DataInitialization(contextProvider, NullLoggerFactory.Instance)
            .CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var noria = configuration.Maps.Single(m => m.Number == 3);
        var spawn = noria.MonsterSpawns.Single(s => s.MonsterDefinition is { } monster && BotChaosMachineHandler.IsChaosMachine(monster));
        var goblin = new Point(spawn.X1, spawn.Y1);
        var terrain = new GameMapTerrain(noria);
        var start = terrain.GetWalkableCoordinate(noria.ExitGates.First(g => g.IsSpawnGate))!.Value;
        var finder = new PathFinder(new FullGridNetwork(true));

        var standingPoint = BotChaosMachineHandler.FindStandingPoint(terrain, goblin, talkRange);

        Assert.That(terrain.WalkMap[goblin.X, goblin.Y], Is.False, "the goblin's own tile is blocked");
        Assert.That(finder.FindPath(start, goblin, terrain.AIgrid, true), Is.Null.Or.Empty);
        Assert.That(standingPoint, Is.Not.Null);
        Assert.That(goblin.EuclideanDistanceTo(standingPoint!.Value), Is.LessThanOrEqualTo(talkRange));
        finder.ResetPathFinder();
        Assert.That(finder.FindPath(start, standingPoint.Value, terrain.AIgrid, true), Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Tests that the standing point is the NPC's own tile when it is walkable, the closest walkable tile
    /// otherwise, and none when everything within the talk range is blocked.
    /// </summary>
    [Test]
    public void StandingPointIsTheClosestWalkableTile()
    {
        var npc = new Point(100, 100);
        var terrain = new GameMapTerrain((byte[]?)null);
        Assert.That(BotChaosMachineHandler.FindStandingPoint(terrain, npc, 3), Is.EqualTo(npc));

        for (var x = 97; x <= 103; x++)
        {
            for (var y = 97; y <= 103; y++)
            {
                if (x != 102 || y != 100)
                {
                    terrain.ApplyTerrainAttribute((byte)x, (byte)y, TerrainAttributeType.Blocked, true);
                }
            }
        }

        Assert.That(BotChaosMachineHandler.FindStandingPoint(terrain, npc, 3), Is.EqualTo(new Point(102, 100)));
        terrain.ApplyTerrainAttribute(102, 100, TerrainAttributeType.Blocked, true);
        Assert.That(BotChaosMachineHandler.FindStandingPoint(terrain, npc, 3), Is.Null);
    }

    private static ItemDefinition CreateDefinition(byte group, short number, int? slot, Player? qualified = null)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.QualifiedCharacters).Returns(new List<CharacterClass>());
        definitionMock.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definitionMock.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        definitionMock.Setup(d => d.Requirements).Returns(new List<AttributeRequirement>());
        if (slot is { } targetSlot)
        {
            var slotTypeMock = new Mock<ItemSlotType>();
            slotTypeMock.Setup(s => s.ItemSlots).Returns(new List<int> { targetSlot });
            definitionMock.Setup(d => d.ItemSlot).Returns(slotTypeMock.Object);
        }

        var definition = definitionMock.Object;
        definition.Group = group;
        definition.Number = number;
        definition.Width = 1;
        definition.Height = 1;
        definition.Durability = slot is null ? (byte)1 : (byte)50;
        definition.MaximumItemLevel = 15;
        if (qualified?.SelectedCharacter?.CharacterClass is { } characterClass)
        {
            definition.QualifiedCharacters.Add(characterClass);
            qualified.GameContext.Configuration.Items.Add(definition);
        }

        return definition;
    }

    private static Item CreateItem(ItemDefinition definition, byte level = 0, bool luck = false)
    {
        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = definition;
        item.Durability = definition.Durability;
        item.Level = level;
        if (luck)
        {
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Luck } });
        }

        return item;
    }

    private async ValueTask<Item> WearLuckyArmorAsync(Player player, byte level)
        => await this.WearAsync(player, CreateDefinition(ArmorGroup, 1, InventoryConstants.ArmorSlot, player), level, luck: true).ConfigureAwait(false);

    private async ValueTask<Item> WearAsync(Player player, ItemDefinition definition, byte level, bool luck)
    {
        var item = CreateItem(definition, level, luck);
        await player.Inventory!.AddItemAsync((byte)definition.ItemSlot!.ItemSlots.First(), item).ConfigureAwait(false);
        return item;
    }

    private async ValueTask AddJewelsAsync(Player player, int amount)
    {
        foreach (var definition in new[] { this._chaos, this._bless, this._soul })
        {
            for (var i = 0; i < amount; i++)
            {
                Assert.That(await player.Inventory!.AddItemAsync(CreateItem(definition)).ConfigureAwait(false), Is.True);
            }
        }
    }

    /// <summary>
    /// Creates a goblin with one "+10 item combination" shaped like the Season 6 one: the +9 piece,
    /// one Chaos, one Bless and one Soul, adding one level, with a fixed success rate.
    /// </summary>
    private NonPlayerCharacter CreateGoblin(byte successPercent, ItemOptionType? requiredPieceOption = null)
    {
        var settings = new Mock<SimpleCraftingSettings>();
        settings.SetupAllProperties();
        settings.Setup(c => c.RequiredItems).Returns(new List<ItemCraftingRequiredItem>());
        settings.Setup(c => c.ResultItems).Returns(new List<ItemCraftingResultItem>());
        settings.Object.Money = MixPrice;
        settings.Object.SuccessPercent = successPercent;
        settings.Object.MaximumSuccessPercent = 100;

        var piece = CreateRequiredItem(null);
        piece.Reference = 1;
        piece.MinimumItemLevel = 9;
        piece.MaximumItemLevel = 9;
        piece.SuccessResult = MixResult.StaysAsIs;
        if (requiredPieceOption is { } option)
        {
            // An option the bot's piece lacks: the plan doesn't look at it, so the machine refuses the mix.
            piece.RequiredItemOptions.Add(option);
        }

        settings.Object.RequiredItems.Add(piece);
        foreach (var jewel in new[] { this._chaos, this._bless, this._soul })
        {
            settings.Object.RequiredItems.Add(CreateRequiredItem(jewel));
        }

        settings.Object.ResultItems.Add(new ItemCraftingResultItem { Reference = 1, AddLevel = 1 });

        var crafting = new ItemCrafting { Number = 3, SimpleCraftingSettings = settings.Object };
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<ItemCrafting> { crafting });
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        return new NonPlayerCharacter(null!, definition.Object, null!);
    }

    /// <summary>One required item of the crafting: the given jewel, or - without one - the piece to upgrade.</summary>
    private static ItemCraftingRequiredItem CreateRequiredItem(ItemDefinition? jewel)
    {
        var required = new Mock<ItemCraftingRequiredItem>();
        required.SetupAllProperties();
        required.Setup(r => r.PossibleItems).Returns(jewel is null ? new List<ItemDefinition>() : new List<ItemDefinition> { jewel });
        required.Setup(r => r.RequiredItemOptions).Returns(new List<ItemOptionType>());
        required.Object.MinimumAmount = 1;
        required.Object.MaximumAmount = 1;
        required.Object.SuccessResult = MixResult.Disappear;
        required.Object.FailResult = MixResult.Disappear;
        return required.Object;
    }
}
