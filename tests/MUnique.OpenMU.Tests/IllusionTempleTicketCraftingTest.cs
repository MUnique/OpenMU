// <copyright file="IllusionTempleTicketCraftingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
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
        await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, level).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, level).ConfigureAwait(false);
        await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);
        var crafting = new TestableIllusionTempleTicketCrafting();

        Assert.That(crafting.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null);
        var result = await crafting.CreateResultItemsAsync(requiredItems, player).ConfigureAwait(false);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Definition, Is.SameAs(this.GetItem(EventItemGroup, ScrollOfBloodNumber)));
        Assert.That(result[0].Level, Is.EqualTo(level));
    }

    /// <summary>
    /// Tests that the combination gives the ticket which the temple of the level accepts, also when the server
    /// owner selected another ticket item, so the crafting and the temples stay in step.
    /// </summary>
    [Test]
    public async ValueTask CombinationGivesTheTicketOfTheTempleAsync()
    {
        const byte level = 3;
        var temple = this._gameConfiguration.MiniGameDefinitions.Single(definition => definition.Type == MiniGameType.IllusionTemple && definition.TicketItemLevel == level);
        var originalTicket = temple.TicketItem;
        var selectedTicket = this.GetItem(EventItemGroup, OldScrollNumber);
        temple.TicketItem = selectedTicket;
        try
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, OldScrollNumber, level).ConfigureAwait(false);
            await this.AddToChaosMachineAsync(player, EventItemGroup, CovenantNumber, level).ConfigureAwait(false);
            await this.AddJewelOfChaosAsync(player).ConfigureAwait(false);
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

    private ValueTask AddJewelOfChaosAsync(Player player)
    {
        var jewelOfChaos = this._gameConfiguration.Items.Single(item => item.Name.ValueInNeutralLanguage == "Jewel of Chaos");
        return this.AddToChaosMachineAsync(player, jewelOfChaos.Group, jewelOfChaos.Number, 0);
    }

    private async ValueTask AddToChaosMachineAsync(Player player, byte group, short number, byte level)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = this.GetItem(group, number);
        item.Level = level;
        item.Durability = 1;
        Assert.That(await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false), Is.True);
    }

    private ItemDefinition GetItem(byte group, short number) => this._gameConfiguration.Items.Single(item => item.Group == group && item.Number == number);

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
