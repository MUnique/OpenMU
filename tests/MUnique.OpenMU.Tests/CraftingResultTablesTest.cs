// <copyright file="CraftingResultTablesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the result tables of the <see cref="SimpleCraftingSettings"/>: the number of excellent options
/// (<see cref="SimpleCraftingSettings.ResultItemExcellentOptionCountTable"/>) and of sockets
/// (<see cref="SimpleCraftingSettings.ResultItemSocketCountTable"/>) of a result item.
/// </summary>
[TestFixture]
public class CraftingResultTablesTest
{
    private GameConfiguration _gameConfiguration = null!;

    private IGameContext _gameContext = null!;

    private IContext _context = null!;

    /// <summary>
    /// Sets up the test environment by initializing the season 6 configuration data.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(0, false).ConfigureAwait(false);
        this._context = contextProvider.CreateNewConfigurationContext();
        this._gameConfiguration = (await this._context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        var mapInitializer = new MapInitializer(this._gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(this._gameConfiguration, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        this._gameContext = gameContext;
    }

    /// <summary>
    /// Tests that an excellent result item gets the number of excellent options of the table,
    /// also more than the maximum options per item of its excellent option definition.
    /// </summary>
    /// <param name="optionCount">The number of options of the table.</param>
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public async ValueTask ExcellentOptionCountIsSelectedFromTheTableAsync(int optionCount)
    {
        var excellentItem = this.GetExcellentWeapon();
        var settings = this.CreateSettings(excellentItem);
        settings.ResultItemExcellentOptionChance = 100;
        settings.ResultItemExcellentOptionCountTable = this.CreateTable(optionCount);

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.EqualTo(optionCount));
        Assert.That(item.HasSkill, Is.EqualTo(excellentItem.Skill is not null), "excellent items have the skill of their definition");
    }

    /// <summary>
    /// Tests that without a table, the excellent options of a result item stay limited by the maximum options per item.
    /// </summary>
    [Test]
    public async ValueTask ExcellentOptionsWithoutTableAreLimitedByTheDefinitionAsync()
    {
        var excellentItem = this.GetExcellentWeapon();
        var settings = this.CreateSettings(excellentItem);
        settings.ResultItemExcellentOptionChance = 100;
        var maximumOptions = excellentItem.PossibleItemOptions
            .First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)).MaximumOptionsPerItem;

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.EqualTo(maximumOptions));
    }

    /// <summary>
    /// Tests that the excellent option chance of a result item is used instead of the one of the crafting.
    /// </summary>
    /// <param name="craftingChance">The excellent option chance of the crafting.</param>
    /// <param name="resultItemChance">The excellent option chance of the result item.</param>
    /// <param name="expectedOptionCount">The expected number of excellent options.</param>
    [TestCase(100, 0, 0)]
    [TestCase(0, 100, 3)]
    public async ValueTask ExcellentOptionChanceOfTheResultItemIsUsedAsync(byte craftingChance, byte resultItemChance, int expectedOptionCount)
    {
        var settings = this.CreateSettings(this.GetExcellentWeapon());
        settings.ResultItemExcellentOptionChance = craftingChance;
        settings.ResultItemExcellentOptionCountTable = this.CreateTable(3);
        settings.ResultItems.Single().ExcellentOptionChance = resultItemChance;

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.EqualTo(expectedOptionCount));
    }

    /// <summary>
    /// Tests that a socket result item gets the number of sockets of the table.
    /// </summary>
    /// <param name="socketCount">The number of sockets of the table.</param>
    [TestCase(4)]
    [TestCase(5)]
    public async ValueTask SocketCountIsSelectedFromTheTableAsync(int socketCount)
    {
        var settings = this.CreateSettings(this.GetSocketItem());
        settings.ResultItemSocketCountTable = this.CreateTable(socketCount);

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.SocketCount, Is.EqualTo(socketCount));
    }

    /// <summary>
    /// Tests that without a socket count table, a crafting creates socket result items as before:
    /// without sockets, and with excellent options by the excellent option chance.
    /// </summary>
    [Test]
    public async ValueTask SocketResultItemWithoutTableIsUnchangedAsync()
    {
        var socketItem = this.GetSocketItemWithExcellentOptions();
        var settings = this.CreateSettings(socketItem);
        settings.ResultItemExcellentOptionChance = 100;

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.SocketCount, Is.Zero);
        Assert.That(item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that with a socket count table, a socket result item gets sockets and no excellent options, even when the
    /// crafting always adds them, because socket items can't be excellent.
    /// </summary>
    [Test]
    public async ValueTask SocketResultItemWithTableGetsNoExcellentOptionsAsync()
    {
        var settings = this.CreateSettings(this.GetSocketItemWithExcellentOptions());
        settings.ResultItemExcellentOptionChance = 100;
        settings.ResultItemExcellentOptionCountTable = this.CreateTable(4);
        settings.ResultItemSocketCountTable = this.CreateTable(4);

        var item = await this.MixAsync(settings).ConfigureAwait(false);

        Assert.That(item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.Zero);
        Assert.That(item.SocketCount, Is.EqualTo(4));
    }

    private ItemDefinition GetSocketItemWithExcellentOptions()
    {
        var socketItem = this.GetSocketItem();
        Assert.That(
            socketItem.PossibleItemOptions.Any(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)),
            Is.True,
            "the definition of the socket item offers excellent options");
        return socketItem;
    }

    private ItemDefinition GetExcellentWeapon()
    {
        return this._gameConfiguration.Items.First(item => item.Group == 0
                                                         && item.MaximumSockets == 0
                                                         && item.PossibleItemOptions.Any(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent)));
    }

    private ItemDefinition GetSocketItem() => this._gameConfiguration.Items.First(item => item.MaximumSockets >= 5);

    private WeightedValueTable CreateTable(int value)
    {
        var table = this._context.CreateNew<WeightedValueTable>();
        var weightedValue = this._context.CreateNew<WeightedValue>();
        weightedValue.Value = value;
        weightedValue.Weight = 1;
        table.Values.Add(weightedValue);
        return table;
    }

    /// <summary>
    /// Creates settings which turn one Jewel of Chaos into the <paramref name="resultItem"/>, always successful.
    /// </summary>
    private SimpleCraftingSettings CreateSettings(ItemDefinition resultItem)
    {
        var settings = this._context.CreateNew<SimpleCraftingSettings>();
        settings.SuccessPercent = 100;
        var requiredItem = this._context.CreateNew<ItemCraftingRequiredItem>();
        requiredItem.PossibleItems.Add(this._gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == 15));
        requiredItem.MinimumAmount = 1;
        requiredItem.MaximumAmount = 1;
        settings.RequiredItems.Add(requiredItem);
        var result = this._context.CreateNew<ItemCraftingResultItem>();
        result.ItemDefinition = resultItem;
        settings.ResultItems.Add(result);
        return settings;
    }

    private async ValueTask<Item> MixAsync(SimpleCraftingSettings settings)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var jewel = player.PersistenceContext.CreateNew<Item>();
        jewel.Definition = this._gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == 15);
        jewel.Durability = 1;
        await player.TemporaryStorage!.AddItemAsync(jewel).ConfigureAwait(false);

        var (result, item) = await new SimpleItemCraftingHandler(settings).DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Success));
        return item!;
    }
}
