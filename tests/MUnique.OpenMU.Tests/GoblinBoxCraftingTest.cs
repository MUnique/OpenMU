// <copyright file="GoblinBoxCraftingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the goblin combinations of the season 6 configuration, which create the Golden Box and the Silver Box.
/// The client sends the same mix id for both, so the server has to tell them apart by the items.
/// </summary>
[TestFixture]
public class GoblinBoxCraftingTest
{
    private const byte GoblinBoxMixId = 47;

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
    /// Tests that a key and its sealed box are combined into the opened box.
    /// </summary>
    /// <param name="keyNumber">The item number of the key.</param>
    /// <param name="sealedBoxNumber">The item number of the sealed box.</param>
    /// <param name="resultNumber">The item number of the opened box.</param>
    [TestCase(113, 121, 123)]
    [TestCase(112, 122, 124)]
    public async ValueTask KeyOpensItsSealedBoxAsync(short keyNumber, short sealedBoxNumber, short resultNumber)
    {
        var player = await this.CreatePlayerAtChaosGoblinAsync().ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, keyNumber).ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, sealedBoxNumber).ConfigureAwait(false);

        await new ItemCraftAction().MixItemsAsync(player, GoblinBoxMixId, 0).ConfigureAwait(false);

        var items = player.TemporaryStorage!.Items.ToList();
        Assert.That(items, Has.Count.EqualTo(1));
        Assert.That(items[0].Definition, Is.SameAs(this.GetItem(resultNumber)));
    }

    /// <summary>
    /// Tests that a key doesn't open the sealed box of the other key.
    /// </summary>
    [Test]
    public async ValueTask KeyDoesNotOpenOtherSealedBoxAsync()
    {
        var player = await this.CreatePlayerAtChaosGoblinAsync().ConfigureAwait(false);
        await this.AddToChaosMachineAsync(player, 112).ConfigureAwait(false); // Silver Key
        await this.AddToChaosMachineAsync(player, 121).ConfigureAwait(false); // Sealed Golden Box

        await new ItemCraftAction().MixItemsAsync(player, GoblinBoxMixId, 0).ConfigureAwait(false);

        Assert.That(player.TemporaryStorage!.Items.Select(item => item.Definition!.Number), Is.EquivalentTo(new short[] { 112, 121 }));
    }

    private async ValueTask<Player> CreatePlayerAtChaosGoblinAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var chaosGoblin = this._gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
        player.OpenedNpc = new NonPlayerCharacter(null!, chaosGoblin, null!);
        return player;
    }

    private async ValueTask AddToChaosMachineAsync(Player player, short number)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = this.GetItem(number);
        item.Durability = 1;
        Assert.That(await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false), Is.True);
    }

    private ItemDefinition GetItem(short number) => this._gameConfiguration.Items.Single(item => item.Group == 14 && item.Number == number);
}
