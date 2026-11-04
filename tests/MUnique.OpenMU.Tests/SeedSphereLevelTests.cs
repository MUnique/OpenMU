// <copyright file="SeedSphereLevelTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the socket option level which results from mounting a seed sphere.
/// </summary>
[TestFixture]
public class SeedSphereLevelTests
{
    private const int SphereLevels = 5;

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
    /// Gets the combinations of the sphere levels (0 to 4) and the seed numbers (60 to 65).
    /// </summary>
    private static IEnumerable<TestCaseData> SphereLevelsAndSeeds =>
        from sphereLevel in Enumerable.Range(0, SphereLevels)
        from seedNumber in Enumerable.Range(60, SeedSphereCrafting.SeedTypes)
        select new TestCaseData(sphereLevel, seedNumber);

    /// <summary>
    /// Tests that the socket option level of each seed sphere is defined by its sphere,
    /// so that a Mono seed sphere results in level 1 and a fifth level seed sphere in level 5.
    /// </summary>
    [Test]
    public void SocketOptionLevelFollowsSphereOfSeedSphere()
    {
        var seedSpheres = this._gameConfiguration.Items
            .Where(i => i.Group == 12 && i.Number >= SeedSphereCrafting.SeedSphereNumberStart && i.Number < SeedSphereCrafting.SeedSphereNumberStart + (5 * SeedSphereCrafting.SeedTypes))
            .ToList();
        Assert.That(seedSpheres, Has.Count.EqualTo(5 * SeedSphereCrafting.SeedTypes));

        foreach (var seedSphere in seedSpheres)
        {
            // The sphere level of a seed sphere is initialized as its drop level, starting at 0.
            Assert.That(MountSeedSphereCrafting.GetSphereLevel(seedSphere), Is.EqualTo(seedSphere.DropLevel + 1), seedSphere.Name.ValueInNeutralLanguage);
        }
    }

    /// <summary>
    /// Tests the way of a socket option from the crafting of the seed sphere, over mounting it on a
    /// socket item, to sending the item to the client and reading it back, for every option of the seed.
    /// The option and its level, which is defined by the sphere, have to stay the same.
    /// </summary>
    /// <remarks>
    /// The craftings are executed without their success chance, which is random.
    /// </remarks>
    /// <param name="sphereLevel">The level of the sphere, starting at 0 for the Mono sphere.</param>
    /// <param name="seedNumber">The item number of the seed.</param>
    /// <returns>The task.</returns>
    [TestCaseSource(nameof(SphereLevelsAndSeeds))]
    public async ValueTask SocketOptionSurvivesCraftingMountingAndSerializationAsync(int sphereLevel, int seedNumber)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var seedDefinition = this.GetItemDefinition(12, seedNumber);
        var sphereDefinition = this.GetItemDefinition(12, 70 + sphereLevel);
        var seedSphereCrafting = new TestSeedSphereCrafting(this.GetCraftingSettings<SeedSphereCrafting>());
        var mountCrafting = new TestMountSeedSphereCrafting(this.GetCraftingSettings<MountSeedSphereCrafting>());
        IItemSerializer[] serializers = [new ItemSerializer(), new ItemSerializerExtended()];

        for (byte seedLevel = 0; seedLevel <= seedDefinition.MaximumItemLevel; seedLevel++)
        {
            await ClearAsync(player.TemporaryStorage!).ConfigureAwait(false);
            await this.AddItemAsync(player, seedDefinition, seedLevel).ConfigureAwait(false);
            await this.AddItemAsync(player, sphereDefinition).ConfigureAwait(false);
            await this.AddJewelsAsync(player).ConfigureAwait(false);
            var seedSphere = await seedSphereCrafting.CraftAsync(player).ConfigureAwait(false);

            await ClearAsync(player.TemporaryStorage!).ConfigureAwait(false);
            var seedOption = seedSphere.Definition!.PossibleItemOptions.Single();
            var socketItemDefinition = this._gameConfiguration.Items.First(i => i.MaximumSockets > 0 && i.PossibleItemOptions.Contains(seedOption));
            var socketItem = await this.AddItemAsync(player, socketItemDefinition).ConfigureAwait(false);
            socketItem.SocketCount = 1;
            await player.TemporaryStorage!.AddItemAsync(seedSphere).ConfigureAwait(false);
            await this.AddJewelsAsync(player).ConfigureAwait(false);
            var mountedItem = await mountCrafting.CraftAsync(player).ConfigureAwait(false);

            var expectedOption = seedOption.PossibleOptions.Single(o => o.OptionType == ItemOptionTypes.SocketOption && o.Number == seedLevel);
            var context = $"{seedDefinition.Name.ValueInNeutralLanguage} level {seedLevel}, {sphereDefinition.Name.ValueInNeutralLanguage}";
            foreach (var serializer in serializers)
            {
                var data = new byte[serializer.NeededSpace];
                serializer.SerializeItem(data, mountedItem);
                var deserializedItem = serializer.DeserializeItem(data, this._gameConfiguration, player.PersistenceContext);

                var socketOption = deserializedItem.ItemOptions.Single(link => link.ItemOption!.OptionType == ItemOptionTypes.SocketOption);
                Assert.That(socketOption.ItemOption, Is.SameAs(expectedOption), $"{context}, {serializer.GetType().Name}");
                Assert.That(socketOption.Level, Is.EqualTo(sphereLevel + 1), $"{context}, {serializer.GetType().Name}");
            }
        }
    }

    private static async ValueTask ClearAsync(IStorage storage)
    {
        foreach (var item in storage.Items.ToList())
        {
            await storage.RemoveItemAsync(item).ConfigureAwait(false);
        }
    }

    private ItemDefinition GetItemDefinition(byte group, int number)
    {
        return this._gameConfiguration.Items.Single(i => i.Group == group && i.Number == number);
    }

    private SimpleCraftingSettings GetCraftingSettings<T>()
        where T : SimpleItemCraftingHandler
    {
        return this._gameConfiguration.Monsters
            .SelectMany(m => m.ItemCraftings)
            .First(c => c.ItemCraftingHandlerClassName == typeof(T).FullName)
            .SimpleCraftingSettings!;
    }

    private async ValueTask<Item> AddItemAsync(Player player, ItemDefinition definition, byte level = 0)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = level;
        item.Durability = definition.Durability;
        Assert.That(await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        return item;
    }

    private async ValueTask AddJewelsAsync(Player player)
    {
        await this.AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Name.ValueInNeutralLanguage == "Jewel of Chaos")).ConfigureAwait(false);
        await this.AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Name.ValueInNeutralLanguage == "Jewel of Creation")).ConfigureAwait(false);
    }

    private static async ValueTask<Item> CraftWithoutSuccessChanceAsync(SimpleItemCraftingHandler handler, Player player, Func<IList<CraftingRequiredItemLink>, ValueTask<List<Item>>> createResult)
    {
        Assert.That(handler.TryGetRequiredItems(player, out var requiredItems, out _), Is.Null, handler.GetType().Name);
        return (await createResult(requiredItems).ConfigureAwait(false)).Single();
    }

    /// <summary>
    /// A <see cref="SeedSphereCrafting"/> which creates its result without the random success chance.
    /// </summary>
    private sealed class TestSeedSphereCrafting(SimpleCraftingSettings settings) : SeedSphereCrafting(settings)
    {
        public ValueTask<Item> CraftAsync(Player player) => CraftWithoutSuccessChanceAsync(this, player, items => this.CreateOrModifyResultItemsAsync(items, player, 0, 100));
    }

    /// <summary>
    /// A <see cref="MountSeedSphereCrafting"/> which mounts the seed sphere without the random success chance.
    /// </summary>
    private sealed class TestMountSeedSphereCrafting(SimpleCraftingSettings settings) : MountSeedSphereCrafting(settings)
    {
        public ValueTask<Item> CraftAsync(Player player) => CraftWithoutSuccessChanceAsync(this, player, items => this.CreateOrModifyResultItemsAsync(items, player, 0, 100));
    }
}
