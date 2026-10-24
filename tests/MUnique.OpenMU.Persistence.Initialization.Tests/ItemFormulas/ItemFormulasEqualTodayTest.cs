// <copyright file="ItemFormulasEqualTodayTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests.ItemFormulas;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests that the item requirement, power-up and price formulas give for every item of every version's data
/// what the server computed at commit 90a571100, which the frozen copies in the <c>Legacy</c> folder keep.
/// </summary>
/// <remarks>
/// Each item is checked at every item level, without its option and at the option's first and highest level,
/// in every slot it fits, and at full, half and no durability: as a normal item, as an excellent item with all its
/// excellent options and with one of them, as an ancient item and as an excellent ancient item. A wearable item which
/// can't be excellent or ancient in the data gets a made-up excellent option or ancient set, so that every kind of item
/// runs through these formulas. The requirements are checked with the item's own requirements and with made-up ones of
/// each stat (0, 100 and 100 from item level 7); and, with the item's own and the made-up ones of 100, with each harmony
/// option which reduces a requirement (at its lowest and highest level) and, on a normal item without its option, with a
/// made-up reduction of each stat. All differences are listed in one report.
/// </remarks>
[TestFixture]
internal class ItemFormulasEqualTodayTest
{
    /// <summary>
    /// Tests the items of the 0.75 data.
    /// </summary>
    [Test]
    public async Task Version075Async()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create075Async().ConfigureAwait(false);
        using var _ = context;
        AssertFormulasEqualToday(context, gameConfiguration);
    }

    /// <summary>
    /// Tests the items of the 0.95d data.
    /// </summary>
    [Test]
    public async Task Version095dAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create095dAsync().ConfigureAwait(false);
        using var _ = context;
        AssertFormulasEqualToday(context, gameConfiguration);
    }

    /// <summary>
    /// Tests the items of the Season 6 data.
    /// </summary>
    [Test]
    public async Task Season6Async()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        AssertFormulasEqualToday(context, gameConfiguration);
    }

    private static void AssertFormulasEqualToday(IContext context, GameConfiguration gameConfiguration)
    {
        var comparison = new FormulaComparison(context);
        foreach (var definition in gameConfiguration.Items.OrderBy(d => d.Group).ThenBy(d => d.Number))
        {
            comparison.CompareItem(definition);
        }

        Assert.Multiple(() =>
        {
            Assert.That(comparison.RequirementCases, Is.GreaterThan(0));
            Assert.That(comparison.PowerUpCases, Is.GreaterThan(0));
            Assert.That(comparison.PriceCases, Is.GreaterThan(0));
            Assert.That(comparison.Differences, Is.Empty, comparison.GetReport());
        });
    }

    /// <summary>
    /// A quality of an item: which excellent options it has and to which ancient set it belongs.
    /// </summary>
    /// <param name="Name">The name in the report.</param>
    /// <param name="ExcellentOptions">The excellent options.</param>
    /// <param name="AncientSetItem">The ancient set item, if it's ancient.</param>
    private sealed record ItemQuality(string Name, IReadOnlyList<IncreasableItemOption> ExcellentOptions, ItemOfItemSet? AncientSetItem)
    {
        /// <summary>
        /// Gets a value indicating whether the item is neither excellent nor ancient.
        /// </summary>
        public bool IsNormal => this.ExcellentOptions.Count == 0 && this.AncientSetItem is null;
    }

    /// <summary>
    /// An option which reduces a requirement, at a level.
    /// </summary>
    /// <param name="Option">The option.</param>
    /// <param name="Level">The level of the option.</param>
    /// <param name="Name">The name in the report.</param>
    private sealed record ReductionOption(IncreasableItemOption Option, int Level, string Name);

    /// <summary>
    /// Compares the frozen formulas with the current ones and collects the differences.
    /// </summary>
    private sealed class FormulaComparison
    {
        private const int MaximumReportedDifferences = 50;

        private const int MadeUpReduction = 7;

        private static readonly IReadOnlyDictionary<AttributeDefinition, AttributeDefinition> RequirementReductions = new Dictionary<AttributeDefinition, AttributeDefinition>
        {
            { Stats.TotalStrengthRequirementValue, Stats.RequiredStrengthReduction },
            { Stats.TotalAgilityRequirementValue, Stats.RequiredAgilityReduction },
            { Stats.TotalEnergyRequirementValue, Stats.RequiredEnergyReduction },
            { Stats.TotalVitalityRequirementValue, Stats.RequiredVitalityReduction },
            { Stats.TotalLeadershipRequirementValue, Stats.RequiredLeadershipReduction },
        };

        private readonly Legacy.LegacyItemPowerUpFactory _legacyPowerUps = new(NullLogger<Legacy.LegacyItemPowerUpFactory>.Instance);

        private readonly ItemPowerUpFactory _powerUps = new(NullLogger<ItemPowerUpFactory>.Instance);

        private readonly Legacy.LegacyItemPriceCalculator _legacyPrices = new();

        private readonly ItemPriceCalculator _prices = new();

        private readonly IncreasableItemOption _madeUpExcellentOption;

        private readonly ItemOfItemSet _madeUpAncientSetItem;

        private readonly List<AttributeRequirement> _madeUpRequirements = [];

        private readonly List<AttributeRequirement> _madeUpReducedRequirements = [];

        private readonly List<ReductionOption> _madeUpReductions = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaComparison"/> class.
        /// </summary>
        /// <param name="context">The context which creates the made-up options, sets and requirements. They aren't saved.</param>
        public FormulaComparison(IContext context)
        {
            this._madeUpExcellentOption = context.CreateNew<IncreasableItemOption>();
            this._madeUpExcellentOption.OptionType = ItemOptionTypes.Excellent;

            this._madeUpAncientSetItem = context.CreateNew<ItemOfItemSet>();
            this._madeUpAncientSetItem.AncientSetDiscriminator = 1;

            foreach (var (requirementAttribute, reductionAttribute) in RequirementReductions)
            {
                var requirement = CreateRequirement(context, requirementAttribute, 100, null);
                this._madeUpReducedRequirements.Add(requirement);
                this._madeUpRequirements.Add(CreateRequirement(context, requirementAttribute, 0, null));
                this._madeUpRequirements.Add(requirement);
                this._madeUpRequirements.Add(CreateRequirement(context, requirementAttribute, 100, 7));

                var reductionOption = context.CreateNew<IncreasableItemOption>();
                reductionOption.OptionType = ItemOptionTypes.HarmonyOption;
                reductionOption.PowerUpDefinition = context.CreateNew<PowerUpDefinition>();
                reductionOption.PowerUpDefinition.TargetAttribute = reductionAttribute;
                reductionOption.PowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
                reductionOption.PowerUpDefinition.Boost.ConstantValue.Value = MadeUpReduction;
                this._madeUpReductions.Add(new ReductionOption(reductionOption, 0, $"made-up {reductionAttribute.Designation} {MadeUpReduction}"));
            }
        }

        /// <summary>
        /// Gets the differences, one text per difference.
        /// </summary>
        public List<string> Differences { get; } = [];

        /// <summary>
        /// Gets the number of differences per compared value, e.g. "requirements" or "buying price".
        /// </summary>
        public SortedDictionary<string, int> DifferencesByKind { get; } = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets the number of compared requirement cases.
        /// </summary>
        public int RequirementCases { get; private set; }

        /// <summary>
        /// Gets the number of compared power-up cases.
        /// </summary>
        public int PowerUpCases { get; private set; }

        /// <summary>
        /// Gets the number of compared price cases.
        /// </summary>
        public int PriceCases { get; private set; }

        /// <summary>
        /// Compares all formulas of the item in all its variants.
        /// </summary>
        /// <param name="definition">The item definition.</param>
        public void CompareItem(ItemDefinition definition)
        {
            var possibleOptions = definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions).ToList();
            var option = possibleOptions.FirstOrDefault(o => o.OptionType == ItemOptionTypes.Option);

            var harmonyReductions = new List<ReductionOption>();
            foreach (var harmonyOption in possibleOptions.Where(o => o.OptionType == ItemOptionTypes.HarmonyOption && ReducesRequirement(o)))
            {
                var levels = harmonyOption.LevelDependentOptions.Select(o => o.Level).DefaultIfEmpty(0).ToList();
                foreach (var harmonyLevel in new[] { levels.Min(), levels.Max() }.Distinct())
                {
                    harmonyReductions.Add(new ReductionOption(harmonyOption, harmonyLevel, $"harmony {harmonyOption.Number} level {harmonyLevel}"));
                }
            }

            var allReductions = harmonyReductions.Concat(this._madeUpReductions).ToList();

            // Level 1 of an option is its own power-up, the higher levels are in its level dependent options.
            // A fixed option (e.g. of the Dinorant) has only level 1. The option adds its level times a factor to the
            // strength requirement, so the first and the highest level show the factor.
            var optionLevels = new List<int> { 0 };
            if (option is not null)
            {
                optionLevels.AddRange(new[] { 1, option.LevelDependentOptions.Select(o => o.Level).DefaultIfEmpty(1).Max() }.Distinct());
            }

            foreach (var quality in this.GetQualities(definition, possibleOptions))
            {
                for (int level = 0; level <= definition.MaximumItemLevel; level++)
                {
                    foreach (var optionLevel in optionLevels)
                    {
                        var item = new TemporaryItem { Definition = definition, Level = (byte)level };
                        foreach (var excellentOption in quality.ExcellentOptions)
                        {
                            item.ItemOptions.Add(new ItemOptionLink { ItemOption = excellentOption });
                        }

                        if (quality.AncientSetItem is { } ancientSetItem)
                        {
                            AddAncientSet(item, ancientSetItem);
                        }

                        if (optionLevel > 0)
                        {
                            item.ItemOptions.Add(new ItemOptionLink { ItemOption = option, Level = optionLevel });
                        }

                        var itemCase = $"{definition.Group}/{definition.Number} {definition.Name.ValueInNeutralLanguage} +{level} {quality.Name}, option level {optionLevel}";
                        this.CompareRequirementsWithReductions(item, quality.IsNormal && optionLevel == 0 ? allReductions : harmonyReductions, itemCase);
                        this.ComparePowerUps(item, itemCase);
                        this.ComparePrices(item, itemCase);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the report of the differences.
        /// </summary>
        /// <returns>The report.</returns>
        public string GetReport()
        {
            var report = new StringBuilder();
            report.AppendLine(CultureInfo.InvariantCulture, $"{this.Differences.Count} differences in {this.RequirementCases} requirement, {this.PowerUpCases} power-up and {this.PriceCases} price cases");
            report.AppendLine(CultureInfo.InvariantCulture, $"({string.Join(", ", this.DifferencesByKind.Select(d => $"{d.Key}: {d.Value}"))}):");
            foreach (var difference in this.Differences.Take(MaximumReportedDifferences))
            {
                report.AppendLine(difference);
            }

            if (this.Differences.Count > MaximumReportedDifferences)
            {
                report.AppendLine(CultureInfo.InvariantCulture, $"... and {this.Differences.Count - MaximumReportedDifferences} more.");
            }

            return report.ToString();
        }

        private static AttributeRequirement CreateRequirement(IContext context, AttributeDefinition attribute, int value, byte? minimumItemLevel)
        {
            var requirement = context.CreateNew<AttributeRequirement>();
            requirement.Attribute = attribute;
            requirement.MinimumValue = value;
            requirement.MinimumItemLevel = minimumItemLevel;
            return requirement;
        }

        private static bool ReducesRequirement(IncreasableItemOption option)
        {
            return (option.PowerUpDefinition?.TargetAttribute is { } target && RequirementReductions.Values.Contains(target))
                || option.LevelDependentOptions.Any(l => l.PowerUpDefinition?.TargetAttribute is { } levelTarget && RequirementReductions.Values.Contains(levelTarget));
        }

        private static void AddAncientSet(Item item, ItemOfItemSet ancientSetItem)
        {
            item.ItemSetGroups.Add(ancientSetItem);
            if (ancientSetItem.BonusOption is { } bonusOption)
            {
                var bonusLevel = bonusOption.LevelDependentOptions.Select(o => o.Level).DefaultIfEmpty(0).Max();
                item.ItemOptions.Add(new ItemOptionLink { ItemOption = bonusOption, Level = bonusLevel });
            }
        }

        private static string Capture(Func<string> calculation)
        {
            try
            {
                return calculation();
            }
            catch (Exception ex)
            {
                return $"throws {ex.GetType().Name}";
            }
        }

        private static string DescribePowerUps(Item item, Func<Item, AttributeSystem, IEnumerable<PowerUpWrapper>> getPowerUps)
        {
            using var attributes = new AttributeSystem([], [], []);
            var powerUps = new List<PowerUpWrapper>();
            try
            {
                powerUps.AddRange(getPowerUps(item, attributes));

                // Each power-up added itself to its target attribute, which tells the attribute.
                var elements = attributes.OfType<ComposableAttribute>()
                    .SelectMany(attribute => attribute.Elements.Select(element =>
                        $"{attribute.Definition.Designation} {element.AggregateType} {element.Value.ToString("R", CultureInfo.InvariantCulture)}"))
                    .Order(StringComparer.Ordinal);
                return $"{powerUps.Count}: {string.Join("; ", elements)}";
            }
            finally
            {
                // Power-ups of shared option values subscribe to them, so they are removed again.
                powerUps.ForEach(powerUp => powerUp.Dispose());
            }
        }

        private static string DescribeRequirements(Item item, IEnumerable<AttributeRequirement> madeUpRequirements, Func<Item, AttributeRequirement, (AttributeDefinition Attr, int Value)> getRequirement)
        {
            string Describe(IEnumerable<AttributeRequirement> requirements) => string.Join("; ", requirements.Select(requirement =>
            {
                var (attribute, value) = getRequirement(item, requirement);
                return $"{attribute.Designation} {value}";
            }));

            return $"{Describe(item.Definition!.Requirements)}; made-up: {Describe(madeUpRequirements)}";
        }

        private IEnumerable<ItemQuality> GetQualities(ItemDefinition definition, List<IncreasableItemOption> possibleOptions)
        {
            var isWearable = definition.ItemSlot is not null;
            IReadOnlyList<IncreasableItemOption> excellentOptions = possibleOptions.Where(o => o.OptionType == ItemOptionTypes.Excellent).ToList();
            var excellent = "excellent";
            if (excellentOptions.Count == 0 && isWearable)
            {
                excellentOptions = [this._madeUpExcellentOption];
                excellent = "made-up excellent";
            }

            var ancientSetItem = definition.PossibleItemSetGroups
                .Where(g => g.Options?.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.AncientOption) ?? false)
                .SelectMany(g => g.Items)
                .FirstOrDefault(i => object.Equals(i.ItemDefinition, definition) && i.AncientSetDiscriminator > 0);
            var ancient = "ancient";
            if (ancientSetItem is null && isWearable)
            {
                ancientSetItem = this._madeUpAncientSetItem;
                ancient = "made-up ancient";
            }

            yield return new ItemQuality("normal", [], null);
            if (excellentOptions.Count > 0)
            {
                yield return new ItemQuality(excellent, excellentOptions, null);
            }

            if (excellentOptions.Count > 1)
            {
                // Not the damage increase option, which decides about a power-up of double wielded excellent weapons.
                var oneOption = excellentOptions.FirstOrDefault(o => o.PowerUpDefinition?.TargetAttribute != Stats.PhysicalBaseDmgIncrease) ?? excellentOptions[0];
                yield return new ItemQuality($"{excellent} with only option {oneOption.Number}", [oneOption], null);
            }

            if (ancientSetItem is not null)
            {
                yield return new ItemQuality(ancient, [], ancientSetItem);
                if (excellentOptions.Count > 0)
                {
                    yield return new ItemQuality($"{excellent} {ancient}", excellentOptions, ancientSetItem);
                }
            }
        }

        private void Compare(string itemCase, string what, string before, string now)
        {
            if (before != now)
            {
                this.Differences.Add($"{itemCase}, {what}: before [{before}], now [{now}]");
                this.DifferencesByKind[what] = this.DifferencesByKind.GetValueOrDefault(what) + 1;
            }
        }

        private void CompareRequirementsWithReductions(Item item, List<ReductionOption> reductions, string itemCase)
        {
            this.CompareRequirements(item, this._madeUpRequirements, itemCase);
            foreach (var reduction in reductions)
            {
                var reductionLink = new ItemOptionLink { ItemOption = reduction.Option, Level = reduction.Level };
                item.ItemOptions.Add(reductionLink);
                this.CompareRequirements(item, this._madeUpReducedRequirements, $"{itemCase}, {reduction.Name}");
                item.ItemOptions.Remove(reductionLink);
            }
        }

        private void CompareRequirements(Item item, List<AttributeRequirement> madeUpRequirements, string itemCase)
        {
            this.RequirementCases++;
            this.Compare(
                itemCase,
                "requirements",
                Capture(() => DescribeRequirements(item, madeUpRequirements, Legacy.LegacyItemExtensions.GetRequirement)),
                Capture(() => DescribeRequirements(item, madeUpRequirements, (i, r) => i.GetRequirement(r))));
        }

        private void ComparePowerUps(Item item, string itemCase)
        {
            if (item.Definition!.ItemSlot is not { } slotType)
            {
                return;
            }

            var fullDurability = item.GetMaximumDurabilityOfOnePiece();
            foreach (var slot in slotType.ItemSlots)
            {
                foreach (var durability in new[] { fullDurability, fullDurability / 2 }.Distinct())
                {
                    item.ItemSlot = (byte)slot;
                    item.Durability = durability;
                    this.PowerUpCases++;
                    this.Compare(
                        $"{itemCase}, slot {slot}, durability {durability}",
                        "power-ups",
                        Capture(() => DescribePowerUps(item, this._legacyPowerUps.GetPowerUps)),
                        Capture(() => DescribePowerUps(item, this._powerUps.GetPowerUps)));
                }
            }
        }

        private void ComparePrices(Item item, string itemCase)
        {
            item.ItemSlot = (byte)(item.Definition!.ItemSlot?.ItemSlots.FirstOrDefault() ?? 0);
            var fullDurability = item.GetMaximumDurabilityOfOnePiece();
            item.Durability = fullDurability;

            // The buying prices don't depend on the durability.
            this.Compare(itemCase, "buying price", Capture(() => $"{this._legacyPrices.CalculateFinalBuyingPrice(item)}"), Capture(() => $"{this._prices.CalculateFinalBuyingPrice(item)}"));
            this.Compare(itemCase, "old buying price", Capture(() => $"{this._legacyPrices.CalculateFinalOldBuyingPrice(item)}"), Capture(() => $"{this._prices.CalculateFinalOldBuyingPrice(item)}"));
            foreach (var durability in new[] { fullDurability, fullDurability / 2, 0 }.Distinct())
            {
                item.Durability = durability;
                this.PriceCases++;
                var priceCase = $"{itemCase}, durability {durability}";
                this.Compare(priceCase, "selling price", Capture(() => $"{this._legacyPrices.CalculateSellingPrice(item)}"), Capture(() => $"{this._prices.CalculateSellingPrice(item)}"));
                this.Compare(priceCase, "selling price at durability", Capture(() => $"{this._legacyPrices.CalculateSellingPrice(item, item.Durability())}"), Capture(() => $"{this._prices.CalculateSellingPrice(item, item.Durability())}"));
                this.Compare(priceCase, "repair price", Capture(() => $"{this._legacyPrices.CalculateRepairPrice(item, false)}"), Capture(() => $"{this._prices.CalculateRepairPrice(item, false)}"));
                this.Compare(priceCase, "repair price with discount", Capture(() => $"{this._legacyPrices.CalculateRepairPrice(item, true)}"), Capture(() => $"{this._prices.CalculateRepairPrice(item, true)}"));
            }
        }
    }
}
