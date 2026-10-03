// <copyright file="ItemPriceDefinitions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Creates the <see cref="ItemPriceDefinition"/>s and assigns them to the items which don't have one yet.
/// It's used by the data initialization of every version and by the update which adds them to existing databases.
/// </summary>
/// <remarks>
/// The prices are the ones which the item price calculator had hard-coded before they became configuration data,
/// so the decisions here follow its logic. Items which are priced like usual equipment get no definition.
/// The prices have to match the ones which the game client calculates and shows.
/// </remarks>
internal class ItemPriceDefinitions : InitializerBase
{
    private const string FixedValueName = "Fixed value";
    private const string PotionsName = "Potions";
    private const string ValueBasedName = "Value based";
    private const string AccessoriesName = "Accessories";
    private const string DarkRavenName = "Dark Raven";
    private const string DarkHorseName = "Dark Horse";
    private const string OneHandedName = "One-handed weapons and shields";
    private const string OneHandedWithoutSkillValueName = "One-handed weapons with a skill without value";
    private const string WithoutSkillValueName = "Equipment with a skill without value";
    private const string DiscountedFormula = "floor(automaticPrice * 80 / 100)";

    /// <summary>
    /// The skills which don't increase the price of an item: Force Wave of the scepters, and the skills of the summoner books.
    /// </summary>
    private static readonly short[] SkillsWithoutValue = [66, 223, 224, 225];

    private static readonly IReadOnlyList<SpecialPrice> SpecialPrices =
    [
        new("Arrows", "70", Scaling: ItemPriceQuantityScaling.ByFillRatio, Rows: [(1, 1200), (2, 2000), (3, 2800)], Items: [(4, 15)]),
        new("Bolts", "100", Scaling: ItemPriceQuantityScaling.ByFillRatio, Rows: [(1, 1400), (2, 2200), (3, 3000)], Items: [(4, 7)]),
        new("Jewel of Bless", "9000000", CraftingReferencePrice: 100_000, Items: [(14, 13)]),
        new("Jewel of Soul", "6000000", CraftingReferencePrice: 70_000, Items: [(14, 14)]),
        new("Jewel of Chaos", "810000", CraftingReferencePrice: 40_000, Items: [(12, 15)]),
        new("Jewel of Life", "45000000", CraftingReferencePrice: 450_000, Items: [(14, 16)]),
        new("Jewel of Creation", "36000000", CraftingReferencePrice: 450_000, Items: [(14, 22)]),
        new("Jewel of Guardian", "60000000", Items: [(14, 31)]),
        new("Gemstone, Jewel of Harmony and Refine Stones", "18600", Items: [(14, 41), (14, 42), (14, 43), (14, 44)]),
        new("Packed Jewel of Bless", "(level + 1) * 9000000 * 10", Items: [(12, 30)]),
        new("Packed Jewel of Soul", "(level + 1) * 6000000 * 10", Items: [(12, 31)]),
        new("Packed Jewel of Chaos", "(level + 1) * 810000 * 10", Items: [(12, 141)]),
        new("Packed Jewel of Life", "(level + 1) * 45000000 * 10", Items: [(12, 136)]),
        new("Packed Jewel of Creation", "(level + 1) * 36000000 * 10", Items: [(12, 137)]),
        new("Packed Jewel of Guardian", "(level + 1) * 60000000 * 10", Items: [(12, 138)]),
        new("Packed Gemstone, Jewel of Harmony and Refine Stones", "(level + 1) * 18600 * 10", Items: [(12, 139), (12, 140), (12, 142), (12, 143)]),
        new("Fruits", "33000000", Items: [(13, 15)]),
        new("Loch's Feather", "180000", Rows: [(1, 7_500_000)], Items: [(13, 14)]),
        new("Large Healing and Mana Potion", "1500 * (level + 1)", Scaling: ItemPriceQuantityScaling.PerPiece, Rounding: ItemPriceRounding.Tens, Items: [(14, 3), (14, 6)]),
        new("Siege Potion", "450000", Scaling: ItemPriceQuantityScaling.PerPiece, Rounding: ItemPriceRounding.Tens, Rows: [(0, 900_000)], Items: [(14, 7)]),
        new("Order (Guardian/Life Stone)", "1000000", Rows: [(1, 2_400_000)], Items: [(13, 11)]),
        new("Contract (Summon)", "0", Rows: [(0, 1_500_000), (1, 1_200_000)], Items: [(13, 7)]),
        new("Splinter of Armor", "150", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(13, 32)]),
        new("Bless of Guardian", "300", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(13, 33)]),
        new("Claw of Beast", "3000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(13, 34)]),
        new("Fragment of Horn", "30000", Items: [(13, 35)]),
        new("Broken Horn", "90000", Items: [(13, 36)]),
        new("Horn of Fenrir", "150000", Items: [(13, 37)]),
        new("Small Shield Potion", "2000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 35)]),
        new("Shield Potion", "4000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 36)]),
        new("Large Shield Potion", "6000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 37)]),
        new("Small Complex Potion", "2500", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 38)]),
        new("Complex Potion", "5000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 39)]),
        new("Large Complex Potion", "7500", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 40)]),
        new("Horn of Dinorant", "960000 + 300000 * optionCount", Items: [(13, 3)]),
        new("Devil's Eye", "10000", Rows: [(1, 10_000), (2, 50_000), (3, 100_000), (4, 300_000), (5, 500_000), (6, 800_000), (7, 1_000_000)], Items: [(14, 17)]),
        new("Devil's Key", "15000", Rows: [(1, 15_000), (2, 75_000), (3, 150_000), (4, 450_000), (5, 750_000), (6, 1_200_000), (7, 1_500_000)], Items: [(14, 18)]),
        new("Devil's Invitation", "(level - 1) * 60000", Rows: [(1, 60_000), (2, 84_000)], Items: [(14, 19)]),
        new("Remedy of Love", "900", Items: [(14, 20)]),
        new("Rena", "if(level = 3, durability * 3900, 9000)", Items: [(14, 21)]),
        new("Ale", "750", Items: [(14, 9)]),
        new("Invisibility Cloak", "600000 + (level - 1) * 60000", Rows: [(1, 150_000)], Items: [(13, 18)]),
        new("Scroll of Archangel", "10000", Rows: [(1, 10_000), (2, 50_000), (3, 100_000), (4, 300_000), (5, 500_000), (6, 800_000), (7, 1_000_000), (8, 1_200_000)], Items: [(13, 16)]),
        new("Blood Bone", "10000", Rows: [(1, 10_000), (2, 50_000), (3, 100_000), (4, 300_000), (5, 500_000), (6, 800_000), (7, 1_000_000), (8, 1_200_000)], Items: [(13, 17)]),
        new("Illusion Temple tickets", "(level + 1) * 200000", Rows: [(1, 500_000)], Items: [(13, 49), (13, 50), (13, 51)]),
        new("Flame and Feather of Condor", "3000000", Items: [(13, 52), (13, 53)]),
        new("Armor of Guardsman", "5000", Items: [(13, 29)]),
        new("Wizard's Ring", "0", Rows: [(0, 30_000)], Items: [(13, 20)]),
        new("Spirit", "0", Rows: [(0, 30_000_000), (1, 15_000_000)], Items: [(13, 31)]),
        new("Lost Map", "600000", Items: [(14, 28)]),
        new("Symbol of Kundun", "30000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 29)]),
        new("Halloween items", "150", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 45), (14, 46), (14, 47), (14, 48), (14, 49), (14, 50)]),
        new("Gem of Secret", "0", Rows: [(0, 60_000)], Items: [(12, 26)]),
        new("Imperial Guardian items", "30000", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 101), (14, 102), (14, 103), (14, 104), (14, 105), (14, 106), (14, 107), (14, 108), (14, 109)]),
        new("Christmas Star and Firecracker", "200000", Items: [(14, 51), (14, 63)]),
        new("Cherry Blossom items", "300", Scaling: ItemPriceQuantityScaling.PerPiece, Items: [(14, 85), (14, 86), (14, 87), (14, 90)]),
    ];

    private static readonly IReadOnlyList<SpecialPrice> GeneralPrices =
    [
        new(FixedValueName, "value"),
        new(PotionsName, "floor(floor(value^2 * 10 / 12) * 2^level / 10) * 10", Scaling: ItemPriceQuantityScaling.PerPiece, Rounding: ItemPriceRounding.Tens),
        new(ValueBasedName, "floor(value^2 * 10 / 12)"),
        new(AccessoriesName, "(dropLevel^3 + 100) * (1 + healthRecoveryOptionLevel)"),
        new(DarkRavenName, "level * 1000000"),
        new(DarkHorseName, "level * 2000000"),
        new(OneHandedName, DiscountedFormula, Modifiers: ItemPriceModifiers.All),
        new(OneHandedWithoutSkillValueName, DiscountedFormula, Modifiers: ItemPriceModifiers.All & ~ItemPriceModifiers.Skill),
        new(WithoutSkillValueName, null, Modifiers: ItemPriceModifiers.All & ~ItemPriceModifiers.Skill),
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPriceDefinitions"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ItemPriceDefinitions(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Assigns the price definitions to all items which don't have one yet.
    /// Existing price definitions with the same name are reused, so it can be applied more than once.
    /// </summary>
    public override void Initialize()
    {
        var specialPrices = SpecialPrices
            .SelectMany(price => price.Items.Select(item => (Key: GetKey(item.Group, item.Number), Price: price)))
            .ToDictionary(entry => entry.Key, entry => entry.Price);
        foreach (var item in this.GameConfiguration.Items.Where(item => item.PriceDefinition is null))
        {
            if (this.GetPrice(item, specialPrices) is { } price)
            {
                item.PriceDefinition = this.GetOrCreateDefinition(price);
            }
        }
    }

    private static int GetKey(byte group, short number) => (number << 8) + group;

    private static bool IsPotion(ItemDefinition item) => item.Group == 14 && item.Number <= 8;

    private static bool IsAccessory(ItemDefinition item)
    {
        return (item.Group == 12 && ((item.Number > 6 && item.Number < 36) || (item.Number > 43 && item.Number != 50)))
               || item.Group == 13
               || item.Group == 15;
    }

    private SpecialPrice? GetPrice(ItemDefinition item, Dictionary<int, SpecialPrice> specialPrices)
    {
        if (item.Value > 0 && (item.Group == 15 || item.Group == 12))
        {
            return GetGeneralPrice(FixedValueName);
        }

        if (item.IsTrainablePet())
        {
            return GetGeneralPrice(item is { Group: 13, Number: 5 } ? DarkRavenName : DarkHorseName);
        }

        if (specialPrices.TryGetValue(GetKey(item.Group, item.Number), out var specialPrice))
        {
            return specialPrice;
        }

        if (item.Value > 0)
        {
            return GetGeneralPrice(IsPotion(item) ? PotionsName : ValueBasedName);
        }

        if (IsAccessory(item))
        {
            return GetGeneralPrice(AccessoriesName);
        }

        var isDiscounted = (item.Group < 6 && item.Width < 2) || item.Group == 6;
        var hasSkillWithoutValue = item.Skill is { } skill && SkillsWithoutValue.Contains(skill.Number);
        return (isDiscounted, hasSkillWithoutValue) switch
        {
            (true, true) => GetGeneralPrice(OneHandedWithoutSkillValueName),
            (true, false) => GetGeneralPrice(OneHandedName),
            (false, true) => GetGeneralPrice(WithoutSkillValueName),
            _ => null, // automatic equipment price
        };
    }

    private static SpecialPrice GetGeneralPrice(string name) => GeneralPrices.First(price => price.Name == name);

    private ItemPriceDefinition GetOrCreateDefinition(SpecialPrice price)
    {
        if (this.GameConfiguration.ItemPriceDefinitions.FirstOrDefault(definition => definition.Name.ValueInNeutralLanguage == price.Name) is { } existingDefinition)
        {
            return existingDefinition;
        }

        var number = (short)(SpecialPrices.Concat(GeneralPrices).ToList().IndexOf(price) + 1);
        var definition = this.Context.CreateNew<ItemPriceDefinition>();
        definition.SetGuid(number);
        definition.Name = price.Name;
        definition.BasePriceFormula = price.Formula;
        definition.QuantityScaling = price.Scaling;
        definition.Modifiers = price.Modifiers;
        definition.SellingPriceRounding = price.Rounding;
        definition.CraftingReferencePrice = price.CraftingReferencePrice;
        foreach (var (level, levelPrice) in price.Rows ?? [])
        {
            var row = this.Context.CreateNew<ItemLevelPrice>();
            row.SetGuid(number, (short)level);
            row.Level = level;
            row.Price = levelPrice;
            definition.PricePerLevel.Add(row);
        }

        this.GameConfiguration.ItemPriceDefinitions.Add(definition);
        return definition;
    }

    /// <summary>
    /// The price of an item, from which an <see cref="ItemPriceDefinition"/> is created.
    /// </summary>
    /// <param name="Name">The name of the price definition.</param>
    /// <param name="Formula">The base price formula.</param>
    /// <param name="Scaling">The quantity scaling.</param>
    /// <param name="Rounding">The selling price rounding.</param>
    /// <param name="Modifiers">The applicable price modifiers.</param>
    /// <param name="CraftingReferencePrice">The crafting reference price.</param>
    /// <param name="Rows">The fixed prices per item level.</param>
    /// <param name="Items">The group and number of the items which have this price.</param>
    private sealed record SpecialPrice(
        string Name,
        string? Formula,
        ItemPriceQuantityScaling Scaling = ItemPriceQuantityScaling.None,
        ItemPriceRounding Rounding = ItemPriceRounding.Default,
        ItemPriceModifiers Modifiers = ItemPriceModifiers.None,
        long? CraftingReferencePrice = null,
        (int Level, long Price)[]? Rows = null,
        (byte Group, short Number)[]? Items = null)
    {
        /// <summary>
        /// Gets the group and number of the items which have this price.
        /// </summary>
        public (byte Group, short Number)[] Items { get; init; } = Items ?? [];
    }
}
