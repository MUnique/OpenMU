// <copyright file="ItemPriceCalculatorGoldenMasterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Compares the prices of the <see cref="ItemPriceCalculator"/> with the ones of the <see cref="LegacyItemPriceCalculator"/>,
/// for every item of the initialized configurations, every item level and the combinations of options which the item can have.
/// </summary>
/// <remarks>
/// This proves that moving the price rules from the code into the configuration didn't change any price.
/// </remarks>
[TestFixture]
public class ItemPriceCalculatorGoldenMasterTest
{
    private const int MaximumReportedDifferences = 50;

    private static readonly byte[] OptionLevels = [1, 2, 3, 4];

    /// <summary>
    /// Tests that all prices of all items of the specified version are the same as before.
    /// </summary>
    /// <param name="version">The version of the data initialization.</param>
    /// <returns>The task.</returns>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task PricesAreUnchangedAsync(string version)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        DataInitializationBase initialization = version switch
        {
            "075" => new MUnique.OpenMU.Persistence.Initialization.Version075.DataInitialization(contextProvider, NullLoggerFactory.Instance),
            "095d" => new MUnique.OpenMU.Persistence.Initialization.Version095d.DataInitialization(contextProvider, NullLoggerFactory.Instance),
            _ => new MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.DataInitialization(contextProvider, NullLoggerFactory.Instance),
        };
        await initialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();

        var calculator = new ItemPriceCalculator();
        var legacyCalculator = new LegacyItemPriceCalculator();
        var differences = new List<string>();
        var comparedItems = 0;
        foreach (var definition in gameConfiguration.Items.OrderBy(d => d.Group).ThenBy(d => d.Number))
        {
            foreach (var item in CreateItems(context, definition))
            {
                comparedItems++;
                Compare(item, "buying", calculator.CalculateFinalBuyingPrice, legacyCalculator.CalculateFinalBuyingPrice, differences);
                Compare(item, "old buying", calculator.CalculateFinalOldBuyingPrice, legacyCalculator.CalculateFinalOldBuyingPrice, differences);
                Compare(item, "selling", i => calculator.CalculateSellingPrice(i, i.Durability()), i => legacyCalculator.CalculateSellingPrice(i, i.Durability()), differences);
                Compare(item, "repair", i => calculator.CalculateRepairPrice(i, true), i => legacyCalculator.CalculateRepairPrice(i, true), differences);
                Compare(item, "repair without npc", i => calculator.CalculateRepairPrice(i, false), i => legacyCalculator.CalculateRepairPrice(i, false), differences);
            }
        }

        Assert.That(comparedItems, Is.GreaterThan(gameConfiguration.Items.Count));
        Assert.That(differences, Is.Empty, $"{differences.Count} differences, the first ones:{Environment.NewLine}{string.Join(Environment.NewLine, differences.Take(MaximumReportedDifferences))}");
    }

    private static void Compare(Item item, string priceName, Func<Item, long> calculate, Func<Item, long> calculateLegacy, List<string> differences)
    {
        var price = calculate(item);
        var legacyPrice = calculateLegacy(item);
        if (price != legacyPrice)
        {
            differences.Add($"{item.Definition} ({item.Definition!.Group}, {item.Definition.Number}) +{item.Level}, durability {item.Durability}, skill {item.HasSkill}, options [{string.Join(", ", item.ItemOptions.Select(o => $"{o.ItemOption?.OptionType?.Name} {o.ItemOption?.PowerUpDefinition?.TargetAttribute?.Designation} {o.Level}"))}]: {priceName} {price} instead of {legacyPrice}");
        }
    }

    private static IEnumerable<Item> CreateItems(IContext context, ItemDefinition definition)
    {
        var possibleOptions = definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions).ToList();
        var luck = possibleOptions.FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck);
        var normalOptions = possibleOptions.Where(o => o.OptionType == ItemOptionTypes.Option).ToList();
        var excellentOptions = possibleOptions.Where(o => o.OptionType == ItemOptionTypes.Excellent).Take(2).ToList();
        var wingOptions = possibleOptions.Where(o => o.OptionType == ItemOptionTypes.Wing).Take(2).ToList();
        var guardianOption = possibleOptions.FirstOrDefault(o => o.OptionType == ItemOptionTypes.GuardianOption);
        var hasSkill = definition.Skill is not null;

        var optionCombinations = new List<(bool Skill, List<(IncreasableItemOption Option, byte Level)> Options)>
        {
            (false, []),
        };

        if (hasSkill)
        {
            optionCombinations.Add((true, []));
        }

        if (luck is not null)
        {
            optionCombinations.Add((false, [(luck, 0)]));
        }

        foreach (var normalOption in normalOptions)
        {
            optionCombinations.AddRange(OptionLevels.Select(level => (false, new List<(IncreasableItemOption, byte)> { (normalOption, level) })));
        }

        for (int count = 1; count <= excellentOptions.Count; count++)
        {
            optionCombinations.Add((false, excellentOptions.Take(count).Select(o => (o, (byte)0)).ToList()));
        }

        for (int count = 1; count <= wingOptions.Count; count++)
        {
            optionCombinations.Add((false, wingOptions.Take(count).Select(o => (o, (byte)0)).ToList()));
        }

        if (guardianOption is not null)
        {
            optionCombinations.Add((false, [(guardianOption, 0)]));
        }

        var allOptions = new List<(IncreasableItemOption, byte)>();
        if (luck is not null)
        {
            allOptions.Add((luck, 0));
        }

        if (normalOptions.FirstOrDefault() is { } firstNormalOption)
        {
            allOptions.Add((firstNormalOption, OptionLevels[^1]));
        }

        allOptions.AddRange(excellentOptions.Select(o => (o, (byte)0)));
        allOptions.AddRange(wingOptions.Select(o => (o, (byte)0)));
        if (guardianOption is not null)
        {
            allOptions.Add((guardianOption, 0));
        }

        optionCombinations.Add((hasSkill, allOptions));

        for (byte level = 0; level <= definition.MaximumItemLevel; level++)
        {
            foreach (var (skill, options) in optionCombinations)
            {
                var item = CreateItem(context, definition, level, skill, options);
                var maximumDurability = Math.Max(item.GetMaximumDurabilityOfOnePiece(), definition.Durability);
                foreach (var durability in new[] { maximumDurability, maximumDurability / 2, 1, 0 }.Distinct())
                {
                    var itemWithDurability = CreateItem(context, definition, level, skill, options);
                    itemWithDurability.Durability = durability;
                    yield return itemWithDurability;
                }
            }
        }
    }

    private static Item CreateItem(IContext context, ItemDefinition definition, byte level, bool skill, List<(IncreasableItemOption Option, byte Level)> options)
    {
        var item = context.CreateNew<Item>();
        item.Definition = definition;
        item.Level = level;
        item.HasSkill = skill;
        foreach (var (option, optionLevel) in options)
        {
            var link = context.CreateNew<ItemOptionLink>();
            link.ItemOption = option;
            link.Level = optionLevel;
            item.ItemOptions.Add(link);
        }

        return item;
    }
}
