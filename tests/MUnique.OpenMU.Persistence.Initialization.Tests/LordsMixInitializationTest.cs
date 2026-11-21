// <copyright file="LordsMixInitializationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the season 6 data of the Lord's Mix of the castle senior NPC.
/// </summary>
[TestFixture]
internal class LordsMixInitializationTest
{
    private const byte LordsMixNumber = 18;

    /// <summary>
    /// Tests that a new season 6 database has the Lord's Mix on the castle senior, limited to one mix per day.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasLordsMixAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertLordsMix(gameConfiguration);
        Assert.That(gameConfiguration.CastleSiegeConfiguration!.LordsMixLimit, Is.EqualTo(1));
        Assert.That(gameConfiguration.CastleSiegeConfiguration.LordsMixLimitPeriod, Is.EqualTo(LordsMixLimitPeriod.Day));
    }

    /// <summary>
    /// Tests that the update adds the Lord's Mix and its limit to an existing database, and that applying it twice
    /// changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateAddsLordsMixAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var senior = GetSenior(gameConfiguration);
        var crafting = senior.ItemCraftings.Single(c => c.Number == LordsMixNumber);
        senior.ItemCraftings.Remove(crafting);
        gameConfiguration.WeightedValueTables.Remove(crafting.SimpleCraftingSettings!.ResultItemExcellentOptionCountTable!);
        gameConfiguration.CastleSiegeConfiguration!.LordsMixLimit = 0;
        gameConfiguration.CastleSiegeConfiguration.LordsMixLimitPeriod = LordsMixLimitPeriod.SiegeCycle;
        var tableCountBefore = gameConfiguration.WeightedValueTables.Count;

        var update = new AddLordsMixUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLordsMix(gameConfiguration);
        Assert.That(gameConfiguration.WeightedValueTables, Has.Count.EqualTo(tableCountBefore + 1));
        Assert.That(gameConfiguration.CastleSiegeConfiguration.LordsMixLimit, Is.EqualTo(1));
        Assert.That(gameConfiguration.CastleSiegeConfiguration.LordsMixLimitPeriod, Is.EqualTo(LordsMixLimitPeriod.Day));
    }

    /// <summary>
    /// Tests that the update keeps a limit and its period which the castle siege configuration already has.
    /// </summary>
    [Test]
    public async Task UpdateKeepsExistingLimitAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.CastleSiegeConfiguration!.LordsMixLimit = 3;
        gameConfiguration.CastleSiegeConfiguration.LordsMixLimitPeriod = LordsMixLimitPeriod.SiegeCycle;

        await new AddLordsMixUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.CastleSiegeConfiguration.LordsMixLimit, Is.EqualTo(3));
        Assert.That(gameConfiguration.CastleSiegeConfiguration.LordsMixLimitPeriod, Is.EqualTo(LordsMixLimitPeriod.SiegeCycle));
    }

    private static void AssertLordsMix(GameConfiguration gameConfiguration)
    {
        var crafting = GetSenior(gameConfiguration).ItemCraftings.Single(c => c.Number == LordsMixNumber);
        var settings = crafting.SimpleCraftingSettings!;
        var excellentResults = settings.ResultItems.Where(r => r.ExcellentOptionChance == 100).Select(r => r.ItemDefinition!).ToList();
        var normalResults = settings.ResultItems.Where(r => r.ExcellentOptionChance == 0).Select(r => (r.ItemDefinition!.Group, r.ItemDefinition.Number)).ToList();
        (byte Group, short Number)[] expectedNormalResults = [(0, 20), (0, 21), (2, 12), (4, 20), (8, 21), (8, 22), (8, 23), (8, 24), (8, 28)];

        Assert.Multiple(() =>
        {
            Assert.That(crafting.Name.ValueInNeutralLanguage, Is.EqualTo("Lord's Mix"));
            Assert.That(crafting.ItemCraftingHandlerClassName, Is.EqualTo(typeof(LordsMixCrafting).FullName));
            Assert.That(settings.Money, Is.EqualTo(1_000_000_000));
            Assert.That(settings.SuccessPercent, Is.EqualTo(100));
            Assert.That(settings.ResultItemSelect, Is.EqualTo(ResultItemSelection.Any));
            Assert.That(settings.ResultItemLuckOptionChance, Is.EqualTo(20));
            Assert.That(settings.ResultItemSkillChance, Is.EqualTo(50), "like for dropped items");

            AssertRequiredItem(settings, 14, 31, 30, 0); // Jewel of Guardian
            AssertRequiredItem(settings, 12, 30, 1, 2); // Packed Jewel of Bless, 30 jewels
            AssertRequiredItem(settings, 12, 31, 1, 2); // Packed Jewel of Soul, 30 jewels
            Assert.That(settings.RequiredItems, Has.Count.EqualTo(3));

            Assert.That(settings.ResultItems, Has.Count.EqualTo(75));
            Assert.That(settings.ResultItems.Select(r => r.ExcellentOptionChance), Is.All.EqualTo(100).Or.EqualTo(0));
            Assert.That(excellentResults, Has.Count.EqualTo(66), "18 weapons and shields, 48 pieces of 10 armor sets");
            Assert.That(excellentResults, Is.Unique);
            Assert.That(excellentResults.Count(i => i.Group <= 6), Is.EqualTo(18));
            Assert.That(excellentResults.Where(i => i.Group > 6).Select(i => i.Number).Distinct(), Is.EquivalentTo(new short[] { 21, 22, 23, 24, 28, 34, 35, 36, 37, 38 }));
            Assert.That(normalResults, Is.EquivalentTo(expectedNormalResults));
            Assert.That(settings.ResultItems.Select(r => r.ItemDefinition!), Has.None.Matches<ItemDefinition>(i => i.MaximumSockets > 0));

            Assert.That(ToWeights(settings.ResultItemExcellentOptionCountTable), Is.EquivalentTo(new Dictionary<int, double> { { 2, 1 }, { 3, 1 }, { 4, 1 } }));
            Assert.That(settings.ResultItemSocketCountTable, Is.Null);
            Assert.That(gameConfiguration.WeightedValueTables, Does.Contain(settings.ResultItemExcellentOptionCountTable));
        });
    }

    private static void AssertRequiredItem(SimpleCraftingSettings settings, byte group, short number, byte amount, byte level)
    {
        var requiredItem = settings.RequiredItems.SingleOrDefault(r => r.PossibleItems.Any(i => i.Group == group && i.Number == number));
        Assert.That(requiredItem, Is.Not.Null, $"{group}/{number}");
        Assert.That(requiredItem!.MinimumAmount, Is.EqualTo(amount), $"{group}/{number}");
        Assert.That(requiredItem.MaximumAmount, Is.EqualTo(amount), $"{group}/{number}");
        Assert.That(requiredItem.MinimumItemLevel, Is.EqualTo(level), $"{group}/{number}");
        Assert.That(requiredItem.MaximumItemLevel, Is.EqualTo(level), $"{group}/{number}");
    }

    private static Dictionary<int, double> ToWeights(WeightedValueTable? table)
    {
        return table?.Values.ToDictionary(v => v.Value, v => v.Weight) ?? [];
    }

    private static MonsterDefinition GetSenior(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Monsters.Single(monster => monster.NpcWindow == NpcWindow.CastleSeniorNPC);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
