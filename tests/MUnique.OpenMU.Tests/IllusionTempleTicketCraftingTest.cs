// <copyright file="IllusionTempleTicketCraftingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the chaos goblin combination of the illusion temple ticket with the season 6 configuration:
/// an Old Scroll (13/49) and an Illusion Sorcerer Covenant (13/50) of the same level give the Scroll of Blood
/// (13/51) of this level, which the game client requires to enter the temple.
/// </summary>
[TestFixture]
public class IllusionTempleTicketCraftingTest
{
    private const byte EventItemGroup = 13;

    private const short OldScrollNumber = 49;

    private const short CovenantNumber = 50;

    private const short ScrollOfBloodNumber = 51;

    /// <summary>
    /// An item which is no ingredient of the combination, as a ticket which the server owner selected.
    /// </summary>
    private const short ArmorOfGuardsmanNumber = 29;

    private GameConfiguration _gameConfiguration = null!;

    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up the test environment by initializing the season 6 configuration data.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(0, false).ConfigureAwait(false);
        this._gameConfiguration = (await contextProvider.CreateNewConfigurationContext().GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        var mapInitializer = new MapInitializer(this._gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(this._gameConfiguration, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        this._gameContext = gameContext;
    }

    /// <summary>
    /// Tests that an Old Scroll and an Illusion Sorcerer Covenant give the Scroll of Blood of their level.
    /// </summary>
    /// <param name="level">The level of the items, which is the level of the temple.</param>
    [TestCase(1)]
    [TestCase(6)]
    public async ValueTask OldScrollAndCovenantGiveScrollOfBloodAsync(byte level)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, level).ConfigureAwait(false);
        var crafting = new TestableIllusionTempleTicketCrafting();

        Assert.That(crafting.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
        var result = await crafting.CreateResultItemsAsync(requiredItems, player).ConfigureAwait(false);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Definition, Is.SameAs(this.GetItem(EventItemGroup, ScrollOfBloodNumber)));
        Assert.That(result[0].Level, Is.EqualTo(level));
    }

    /// <summary>
    /// Tests that ingredients of a level which no temple has, e.g. +0 ones of a game master, are refused: they would
    /// give a ticket which no temple accepts.
    /// </summary>
    /// <param name="level">The level of the ingredients.</param>
    [TestCase(0)]
    [TestCase(7)]
    public async ValueTask IngredientsOfALevelWithoutTempleAreRefusedAsync(byte level)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, level).ConfigureAwait(false);

        Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.InvalidItemLevel));
    }

    /// <summary>
    /// Tests that ingredients of a level which the event doesn't have are refused by the Blood Castle and the Devil
    /// Square combinations, which share the event ticket crafting.
    /// </summary>
    [Test]
    public async ValueTask IngredientsOfALevelWithoutEventAreRefusedByTheOtherCraftingsAsync()
    {
        var bloodCastlePlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddByNameAsync(bloodCastlePlayer, "Scroll of Archangel", 0).ConfigureAwait(false);
        await this.AddByNameAsync(bloodCastlePlayer, "Blood Bone", 0).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(bloodCastlePlayer).ConfigureAwait(false);
        var devilSquarePlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddByNameAsync(devilSquarePlayer, "Devil's Eye", 0).ConfigureAwait(false);
        await this.AddByNameAsync(devilSquarePlayer, "Devil's Key", 0).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(devilSquarePlayer).ConfigureAwait(false);

        Assert.That(new BloodCastleTicketCrafting().TryGetRequiredItems(bloodCastlePlayer, out _, out _), Is.EqualTo(CraftingResult.InvalidItemLevel));
        Assert.That(new DevilSquareTicketCrafting().TryGetRequiredItems(devilSquarePlayer, out _, out _), Is.EqualTo(CraftingResult.InvalidItemLevel));
    }

    /// <summary>
    /// Tests that a pair of ingredients of a level without temple, e.g. +0 ones of a game master, doesn't hide a
    /// valid pair behind it: an Old Scroll+0 and a Covenant+0 in the first slots, an Old Scroll+3 and a Covenant+3
    /// after them.
    /// </summary>
    [Test]
    public async ValueTask ValidIngredientsAreFoundBehindIngredientsOfALevelWithoutTempleAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 0, slot: 0).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 0, slot: 1).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 3, slot: 2).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 3, slot: 3).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

        Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
        Assert.That(requiredItems.SelectMany(link => link.Items).Select(item => (int)item.Level), Is.EqualTo(new[] { 3, 3, 0 }));
    }

    /// <summary>
    /// Tests that the ingredients of the same level are combined when there's another ingredient of another level in
    /// the chaos machine before them: an Old Scroll+3, an Illusion Sorcerer Covenant+2 and a Covenant+3.
    /// </summary>
    [Test]
    public async ValueTask IngredientsOfTheSameLevelAreFoundBesidesOthersAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 3).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 2).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 3).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);
        var crafting = new TestableIllusionTempleTicketCrafting();

        Assert.That(crafting.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
        Assert.That(requiredItems.SelectMany(link => link.Items).Select(item => (int)item.Level), Is.EqualTo(new[] { 3, 3, 0 }));
        var result = await crafting.CreateResultItemsAsync(requiredItems, player).ConfigureAwait(false);

        Assert.That(result.Single().Level, Is.EqualTo(3));
    }

    /// <summary>
    /// Tests that the combination gives the ticket which the temple of the level accepts, also when the server
    /// owner selected another ticket item, so the crafting and the temples stay in step.
    /// </summary>
    [Test]
    public async ValueTask CombinationGivesTheTicketOfTheTempleAsync()
    {
        const byte level = 3;
        var temple = this.GetTemple(level);
        var originalTicket = temple.TicketItem;
        var selectedTicket = this.GetItem(EventItemGroup, ArmorOfGuardsmanNumber);
        temple.TicketItem = selectedTicket;
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            await this.AddIngredientsAsync(player, level).ConfigureAwait(false);
            var crafting = new TestableIllusionTempleTicketCrafting();

            Assert.That(crafting.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
            var result = await crafting.CreateResultItemsAsync(requiredItems, player).ConfigureAwait(false);

            Assert.That(result.Single().Definition, Is.SameAs(selectedTicket));
        }
        finally
        {
            temple.TicketItem = originalTicket;
        }
    }

    /// <summary>
    /// Tests that the combination gives the ticket of a temple which isn't available, e.g. deactivated by the server
    /// owner, when it's the only temple of the level: it's the ticket which the temple accepts when it's active again.
    /// </summary>
    [Test]
    public async ValueTask CombinationGivesTheTicketOfAnInactiveTempleAsync()
    {
        const byte level = 3;
        var temple = this.GetTemple(level);
        var (originalTicket, originalIsActive) = (temple.TicketItem, temple.IsActive);
        var selectedTicket = this.GetItem(EventItemGroup, ArmorOfGuardsmanNumber);
        temple.TicketItem = selectedTicket;
        temple.IsActive = false;
        try
        {
            var result = await this.CombineAsync(level).ConfigureAwait(false);

            Assert.That(result.Definition, Is.SameAs(selectedTicket));
        }
        finally
        {
            (temple.TicketItem, temple.IsActive) = (originalTicket, originalIsActive);
        }
    }

    /// <summary>
    /// Tests that the combination prefers the ticket of an available temple over the ticket of an inactive temple of
    /// the same level.
    /// </summary>
    [Test]
    public async ValueTask CombinationPrefersTheTicketOfAnAvailableTempleAsync()
    {
        const byte level = 3;
        var inactiveTemple = this.GetTemple(level);
        var (originalTicket, originalIsActive) = (inactiveTemple.TicketItem, inactiveTemple.IsActive);
        inactiveTemple.TicketItem = this.GetItem(EventItemGroup, ArmorOfGuardsmanNumber);
        inactiveTemple.IsActive = false;
        var availableTemple = this.GetTemple(level + 1);
        availableTemple.TicketItemLevel = level;
        try
        {
            var result = await this.CombineAsync(level).ConfigureAwait(false);

            Assert.That(result.Definition, Is.SameAs(this.GetItem(EventItemGroup, ScrollOfBloodNumber)));
        }
        finally
        {
            (inactiveTemple.TicketItem, inactiveTemple.IsActive) = (originalTicket, originalIsActive);
            availableTemple.TicketItemLevel = level + 1;
        }
    }

    /// <summary>
    /// Tests that the combination gives the item named "Scroll of Blood" when the temple of the level has no ticket.
    /// </summary>
    [Test]
    public async ValueTask CombinationGivesTheScrollOfBloodWithoutTicketOfTheTempleAsync()
    {
        const byte level = 2;
        var temple = this.GetTemple(level);
        var originalTicket = temple.TicketItem;
        temple.TicketItem = null;
        try
        {
            var result = await this.CombineAsync(level).ConfigureAwait(false);

            Assert.That(result.Definition, Is.SameAs(this.GetItem(EventItemGroup, ScrollOfBloodNumber)));
        }
        finally
        {
            temple.TicketItem = originalTicket;
        }
    }

    /// <summary>
    /// Tests that the Scroll of Blood is no ingredient: it is the result, so an Old Scroll and a Scroll of Blood
    /// are refused.
    /// </summary>
    [Test]
    public async ValueTask OldScrollAndScrollOfBloodAreRefusedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 1).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, ScrollOfBloodNumber, 1).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

        Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.IncorrectMixItems));
    }

    /// <summary>
    /// Tests that the ticket is refused also when its item definition has the name of the Illusion Sorcerer
    /// Covenant, like in a configuration whose data of 13/50 and 13/51 is only partly swapped, which the illusion
    /// temple update leaves as it is: otherwise the combination would take the ticket and give it back.
    /// </summary>
    [Test]
    public async ValueTask TicketWithTheNameOfTheCovenantIsRefusedAsync()
    {
        var ticket = this.GetItem(EventItemGroup, ScrollOfBloodNumber);
        var covenant = this.GetItem(EventItemGroup, CovenantNumber);
        var (ticketName, covenantName) = (ticket.Name, covenant.Name);
        (ticket.Name, covenant.Name) = (covenantName, "Renamed by the owner");
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 1).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, ScrollOfBloodNumber, 1).ConfigureAwait(false);
            await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

            Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.IncorrectMixItems));
        }
        finally
        {
            (ticket.Name, covenant.Name) = (ticketName, covenantName);
        }
    }

    /// <summary>
    /// Tests that a pair with the ticket as an ingredient, as it got the name of the covenant, doesn't hide a valid
    /// pair behind it: the Old Scroll is combined with the real covenant.
    /// </summary>
    [Test]
    public async ValueTask ValidIngredientsAreFoundBehindTheTicketWithTheNameOfTheCovenantAsync()
    {
        var ticket = this.GetItem(EventItemGroup, ScrollOfBloodNumber);
        var covenant = this.GetItem(EventItemGroup, CovenantNumber);
        var ticketName = ticket.Name;
        ticket.Name = covenant.Name;
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 1, slot: 0).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, ScrollOfBloodNumber, 1, slot: 1).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 1, slot: 3).ConfigureAwait(false);
            await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

            Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
            Assert.That(requiredItems.SelectMany(link => link.Items).Select(item => item.Definition), Does.Contain(covenant).And.Not.Contain(ticket));
        }
        finally
        {
            ticket.Name = ticketName;
        }
    }

    /// <summary>
    /// Tests that the reason of the first pair which gives no ticket is the answer when no pair gives one: a pair of
    /// a level without temple and a pair with the ticket as an ingredient, in both orders.
    /// </summary>
    /// <param name="levelWithoutTempleFirst">Whether the pair of the level without temple is the first one.</param>
    /// <param name="expectedResult">The expected result.</param>
    [TestCase(true, CraftingResult.InvalidItemLevel)]
    [TestCase(false, CraftingResult.IncorrectMixItems)]
    public async ValueTask TheReasonOfTheFirstPairIsTheAnswerAsync(bool levelWithoutTempleFirst, CraftingResult expectedResult)
    {
        var ticket = this.GetItem(EventItemGroup, ScrollOfBloodNumber);
        var covenant = this.GetItem(EventItemGroup, CovenantNumber);
        var ticketName = ticket.Name;
        ticket.Name = covenant.Name;
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            var (withoutTemple, withTicket) = levelWithoutTempleFirst ? ((byte)0, (byte)3) : ((byte)3, (byte)0);
            await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 0, slot: withoutTemple).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 0, slot: (byte)(withoutTemple + 1)).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 1, slot: withTicket).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, ScrollOfBloodNumber, 1, slot: (byte)(withTicket + 1)).ConfigureAwait(false);
            await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

            Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(expectedResult));
        }
        finally
        {
            ticket.Name = ticketName;
        }
    }

    /// <summary>
    /// Tests that an event ticket crafting of a plug-in, which has no event type, combines ingredients of every level,
    /// also of one which no temple has.
    /// </summary>
    /// <param name="level">The level of the ingredients.</param>
    [TestCase(0)]
    [TestCase(1)]
    public async ValueTask CraftingWithoutEventTypeCombinesEveryLevelAsync(byte level)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, level).ConfigureAwait(false);

        Assert.That(new PlugInTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.Null);
    }

    /// <summary>
    /// Tests that an event ticket crafting whose result item doesn't exist, e.g. because the server owner renamed
    /// it, refuses the items instead of throwing: it's asked for every item which is put into the chaos machine.
    /// </summary>
    [Test]
    public async ValueTask CraftingWithoutResultItemRefusesAsync()
    {
        var cloak = this.GetItemByName("Invisibility Cloak");
        var cloakName = cloak.Name;
        cloak.Name = "Renamed by the owner";
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            await this.AddByNameAsync(player, "Scroll of Archangel", 1).ConfigureAwait(false);
            await this.AddByNameAsync(player, "Blood Bone", 1).ConfigureAwait(false);
            await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);

            Assert.That(new BloodCastleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.IncorrectBloodCastleItems));
        }
        finally
        {
            cloak.Name = cloakName;
        }
    }

    /// <summary>
    /// Tests that other items besides the ingredients are accepted while the result, the 2x2 Scroll of Blood, finds
    /// a place after the smaller ingredients are removed.
    /// </summary>
    /// <param name="otherItemName">The name of the other item, an unrelated item or a second ingredient.</param>
    [TestCase("Jewel of Bless")]
    [TestCase("Illusion Sorcerer Covenant")]
    public async ValueTask OtherItemsInTheChaosMachineAreAcceptedAsync(string otherItemName)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, 1).ConfigureAwait(false);
        await this.AddByNameAsync(player, otherItemName, 2).ConfigureAwait(false);

        Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.Null);
    }

    /// <summary>
    /// Tests that other items besides the ingredients are refused when they leave the result, the 2x2 Scroll of
    /// Blood, no place after the smaller ingredients are removed: the created ticket would be in no storage. The
    /// ingredients are right, so the answer is that there are too many items.
    /// </summary>
    [Test]
    public async ValueTask OtherItemsWhichLeaveNoPlaceAreRefusedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, 1).ConfigureAwait(false);
        await this.FillChaosMachineAsync(player).ConfigureAwait(false);

        Assert.That(new IllusionTempleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.TooManyItems));
    }

    /// <summary>
    /// Tests that the result, the 2x2 Scroll of Blood, is placed into the chaos machine when its only place is the
    /// one of the ingredients: the combination removes them before it creates the result. The real combination runs;
    /// a crafting bonus of 30 percentage points raises its success rate of 70 % to 100 %.
    /// </summary>
    [Test]
    public async ValueTask ResultIsPlacedWhereTheIngredientsWereAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player.TryAddMoney(10_000_000);
        player.Attributes![Stats.CraftingBonusPercentage] = 30;
        var oldScroll = await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, 1, slot: 0).ConfigureAwait(false);
        var covenant = await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, 1, slot: 1).ConfigureAwait(false);
        var chaos = await this.AddByNameAsync(player, "Jewel of Chaos", 0, slot: 8).ConfigureAwait(false);
        await this.FillChaosMachineAsync(player).ConfigureAwait(false);

        var (result, item) = await new TestableIllusionTempleTicketCrafting().DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Success));
        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Definition, Is.SameAs(this.GetItem(EventItemGroup, ScrollOfBloodNumber)));
        Assert.That(item.ItemSlot, Is.EqualTo(0));
        Assert.That(player.TemporaryStorage!.Items, Does.Contain(item));
        Assert.That(player.TemporaryStorage.Items, Does.Not.Contain(oldScroll).And.Not.Contain(covenant).And.Not.Contain(chaos));
    }

    /// <summary>
    /// Tests that the Blood Castle combination, which shares the event ticket crafting, accepts other items besides
    /// its ingredients while its result, the 2x2 Invisibility Cloak, finds a place after they're removed, and refuses
    /// them otherwise.
    /// </summary>
    [Test]
    public async ValueTask OtherItemsInTheBloodCastleCraftingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Scroll of Archangel", 1, slot: 0).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Blood Bone", 1, slot: 2).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Jewel of Chaos", 0, slot: 4).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Jewel of Bless", 0).ConfigureAwait(false);
        Assert.That(new BloodCastleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.Null);

        // The free cells after the removal of the 1x2 ingredients and the jewel are in no 2x2 square.
        await this.FillChaosMachineAsync(player).ConfigureAwait(false);

        Assert.That(new BloodCastleTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.EqualTo(CraftingResult.TooManyItems));
    }

    /// <summary>
    /// Tests that the Devil Square combination accepts other items besides its ingredients: its result, the 1x1
    /// Devil's Invitation, always fits into the cells of an ingredient.
    /// </summary>
    [Test]
    public async ValueTask OtherItemsAreAcceptedByTheDevilSquareCraftingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Devil's Eye", 1).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Devil's Key", 1).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);
        await this.AddByNameAsync(player, "Jewel of Bless", 0).ConfigureAwait(false);

        Assert.That(new DevilSquareTicketCrafting().TryGetRequiredItems(player, out _, out _), Is.Null);
    }

    private MiniGameDefinition GetTemple(int ticketLevel) => this._gameConfiguration.MiniGameDefinitions.Single(definition => definition.Type == MiniGameType.IllusionTemple && definition.TicketItemLevel == ticketLevel);

    private ItemDefinition GetItemByName(string name) => this._gameConfiguration.Items.Single(item => item.Name.ValueInNeutralLanguage == name);

    private async ValueTask<Item> CombineAsync(byte level)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await this.AddIngredientsAsync(player, level).ConfigureAwait(false);
        var crafting = new TestableIllusionTempleTicketCrafting();

        Assert.That(crafting.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
        var result = await crafting.CreateResultItemsAsync(requiredItems, player).ConfigureAwait(false);
        Assert.That(result, Has.Count.EqualTo(1));
        return result[0];
    }

    private async ValueTask AddJewelOfChaosAsync(Player player) => await this.AddByNameAsync(player, "Jewel of Chaos", 0).ConfigureAwait(false);

    private async ValueTask AddIngredientsAsync(Player player, byte level)
    {
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, level).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, level).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);
    }

    private ValueTask<Item> AddByNameAsync(Player player, string name, byte level, byte? slot = null)
    {
        var definition = this.GetItemByName(name);
        return this.AddToChaosMachineAsync(player, definition.Group, definition.Number, level, slot);
    }

    private async ValueTask<Item> AddToChaosMachineAsync(Player player, byte group, short number, byte level, byte? slot = null)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = this.GetItem(group, number);
        item.Level = level;
        item.Durability = 1;
        var added = slot is { } targetSlot
            ? await player.TemporaryStorage!.AddItemAsync(targetSlot, item).ConfigureAwait(false)
            : await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false);
        Assert.That(added, Is.True);
        return item;
    }

    /// <summary>
    /// Fills the free cells of the chaos machine with jewels.
    /// </summary>
    private async ValueTask FillChaosMachineAsync(Player player)
    {
        var jewel = this.GetItemByName("Jewel of Bless");
        foreach (var slot in player.TemporaryStorage!.FreeSlots.ToList())
        {
            await this.AddToChaosMachineAsync(player, jewel.Group, jewel.Number, 0, slot).ConfigureAwait(false);
        }
    }

    private ItemDefinition GetItem(byte group, short number) => this._gameConfiguration.Items.Single(item => item.Group == group && item.Number == number);

    /// <summary>
    /// An event ticket crafting like one of a plug-in assembly, which doesn't know the event type.
    /// </summary>
    private sealed class PlugInTicketCrafting : BaseEventTicketCrafting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlugInTicketCrafting"/> class.
        /// </summary>
        public PlugInTicketCrafting()
            : base("Scroll of Blood", "Old Scroll", "Illusion Sorcerer Covenant")
        {
        }

        /// <inheritdoc />
        protected override int GetPrice(int eventLevel) => 1_000_000 * (eventLevel + 1);

        /// <inheritdoc />
        protected override byte GetSuccessRate(int eventLevel) => (byte)(50 + eventLevel);
    }

    /// <summary>
    /// Makes the creation of the result accessible, because the combination itself succeeds only by chance.
    /// </summary>
    private sealed class TestableIllusionTempleTicketCrafting : IllusionTempleTicketCrafting
    {
        /// <summary>
        /// Creates the result items of the combination.
        /// </summary>
        /// <param name="requiredItems">The required items.</param>
        /// <param name="player">The player.</param>
        /// <returns>The result items.</returns>
        public ValueTask<List<Item>> CreateResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player)
        {
            return this.CreateOrModifyResultItemsAsync(requiredItems, player, 0, 100);
        }
    }
}
