// <copyright file="BotMerchandiseTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests which loot a bot puts on its stall and what it costs (<see cref="BotMerchandise"/>), with the
/// default prices of <see cref="BotMerchantConfiguration"/>.
/// </summary>
[TestFixture]
public class BotMerchandiseTest
{
    private readonly BotMerchantConfiguration _configuration = new();

    /// <summary>
    /// The prices of typical goods.
    /// </summary>
    /// <param name="description">What the item is.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <param name="level">The level (+) of the item.</param>
    /// <param name="luck">Whether the item has luck.</param>
    /// <param name="optionLevel">The level of the additional option: 1 for +4, 2 for +8 and so on.</param>
    /// <param name="ancient">Whether the item is ancient.</param>
    /// <param name="excellentOptions">The excellent options: "DD", "DSR", "Reflect", "HP", "Mana", "EDR", "Speed".</param>
    /// <param name="expectedClass">The name of the expected price class (the enum is internal).</param>
    /// <param name="expectedCurrency">The expected currency.</param>
    /// <param name="expectedAmount">The expected amount.</param>
    [TestCase("normal Dragon Armor +7, luck, +4", 60, 7, true, 1, false, "", "Cheap", "zen", 2_500_000)]
    [TestCase("normal Legendary Helm +9, luck, +8", 70, 9, true, 2, false, "", "Cheap", "zen", 5_000_000)]
    [TestCase("exc Sphinx Mask +6 (HP), luck", 70, 6, true, 0, false, "HP", "Cheap", "zen", 2_000_000)]
    [TestCase("exc Brass Armor +5 (DD)", 30, 5, false, 0, false, "DD", "Medium", "bless", 2)]
    [TestCase("exc Dragon Gloves +7 (DSR), +8", 60, 7, false, 2, false, "DSR", "Medium", "bless", 3)]
    [TestCase("exc Brass Armor +7 (Reflect), luck, +4", 30, 7, true, 1, false, "Reflect", "Expensive", "soul", 3)]
    [TestCase("exc Lightning Sword +6 (EDR, 2%)", 60, 6, false, 0, false, "EDR,Speed", "Expensive", "soul", 5)]
    [TestCase("exc Dragon Armor +9 (DD, DSR), luck, +8", 80, 9, true, 2, false, "DD,DSR", "Expensive", "soul", 9)]
    [TestCase("exc staff +7 (EDR, speed, 2%), luck", 90, 7, true, 0, false, "EDR,Speed,DD", "Expensive", "soul", 10)]
    [TestCase("ancient Hyon Dragon Gloves +7", 60, 7, false, 0, true, "", "Expensive", "soul", 5)]
    [TestCase("ancient pants +9, luck, +12", 100, 9, true, 3, true, "", "Expensive", "soul", 11)]
    public void PricesOfTypicalGoods(string description, int dropLevel, int level, bool luck, int optionLevel, bool ancient, string excellentOptions, string expectedClass, string expectedCurrency, int expectedAmount)
    {
        var item = CreateItem(dropLevel, level, luck, optionLevel, ancient, excellentOptions);

        var priceClass = BotMerchandise.Classify(item, this._configuration);
        var price = BotMerchandise.CalculatePrice(item, priceClass, this._configuration);

        Assert.That(priceClass.ToString(), Is.EqualTo(expectedClass), description);
        Assert.That(price, Is.EqualTo((expectedCurrency, expectedAmount)), description);
    }

    /// <summary>
    /// A zen price is drawn within the spread and rounded the way a player would ask for it.
    /// </summary>
    /// <param name="amount">The calculated price.</param>
    /// <param name="spreadPercent">The spread in percent.</param>
    /// <param name="random">The random number: 0 for the lowest price, 1 for the highest.</param>
    /// <param name="expected">The expected price for the store.</param>
    [TestCase(810_000, 10, 0.0, 750_000)]
    [TestCase(810_000, 10, 0.5, 800_000)]
    [TestCase(810_000, 10, 1.0, 900_000)]
    [TestCase(6_000_000, 10, 0.0, 5_400_000)]
    [TestCase(6_000_000, 10, 0.3, 5_800_000)]
    [TestCase(45_000_000, 10, 0.75, 47_000_000)]
    [TestCase(45_000_000, 10, 0.0, 41_000_000)]
    [TestCase(30_000, 10, 0.0, 27_000)]
    [TestCase(500, 10, 1.0, 550)]
    [TestCase(2_000_000, 0, 0.9, 2_000_000)]
    [TestCase(810_123, 0, 0.0, 810_123)]
    [TestCase(int.MaxValue, 50, 1.0, int.MaxValue)]
    public void ZenPriceSpread(int amount, int spreadPercent, double random, int expected)
    {
        Assert.That(BotMerchandise.SpreadZenPrice(amount, spreadPercent, random), Is.EqualTo(expected));
    }

    /// <summary>
    /// The loot which isn't worth a stall goes to the NPC as before.
    /// </summary>
    /// <param name="description">What the item is.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <param name="level">The level (+) of the item.</param>
    /// <param name="luck">Whether the item has luck.</param>
    /// <param name="excellentOptions">The excellent options.</param>
    [TestCase("normal Dragon Armor +7 without luck", 60, 7, false, "")]
    [TestCase("normal Dragon Armor +6 with luck", 60, 6, true, "")]
    [TestCase("normal Leather Armor +9 with luck - a low set", 10, 9, true, "")]
    [TestCase("exc armor with only mana, without luck", 60, 5, false, "Mana")]
    public void LootWhichIsNotWorthAStall(string description, int dropLevel, int level, bool luck, string excellentOptions)
    {
        var item = CreateItem(dropLevel, level, luck, 0, false, excellentOptions);

        Assert.That(BotMerchandise.Classify(item, this._configuration), Is.EqualTo(BotMerchandise.PriceClass.None), description);
        Assert.That(BotMerchandise.CalculatePrice(item, BotMerchandise.PriceClass.None, this._configuration), Is.Null, description);
    }

    /// <summary>
    /// An item which the game doesn't let into a personal store stays out of it, whatever its options.
    /// </summary>
    [Test]
    public void ItemWhichCantBeSoldInAStoreIsNoGood()
    {
        var item = CreateItem(80, 9, true, 2, true, "DD,DSR");
        item.Definition!.IsPersonalStoreSellable = false;

        Assert.That(BotMerchandise.Classify(item, this._configuration), Is.EqualTo(BotMerchandise.PriceClass.None));
    }

    /// <summary>
    /// The game doesn't open a store with a harmony option item, so it's no good either.
    /// </summary>
    [Test]
    public void ItemWithHarmonyOptionIsNoGood()
    {
        var item = CreateItem(80, 9, true, 2, false, "DD,DSR");
        item.ItemOptions.Add(new ItemOptionLink { ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.HarmonyOption } });

        Assert.That(BotMerchandise.Classify(item, this._configuration), Is.EqualTo(BotMerchandise.PriceClass.None));
    }

    private static Item CreateItem(int dropLevel, int level, bool luck, int optionLevel, bool ancient, string excellentOptions)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definitionMock.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        var definition = definitionMock.Object;
        definition.Width = 2;
        definition.Height = 2;
        definition.DropLevel = (byte)dropLevel;
        definition.IsPersonalStoreSellable = true;

        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = definition;
        item.Level = (byte)level;
        if (luck)
        {
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Luck } });
        }

        if (optionLevel > 0)
        {
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Option }, Level = optionLevel });
        }

        if (ancient)
        {
            item.ItemSetGroups.Add(new ItemOfItemSet { AncientSetDiscriminator = 1 });
        }

        foreach (var option in excellentOptions.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            item.ItemOptions.Add(new ItemOptionLink
            {
                ItemOption = new IncreasableItemOption
                {
                    OptionType = ItemOptionTypes.Excellent,
                    PowerUpDefinition = new PowerUpDefinition { TargetAttribute = GetAttribute(option) },
                },
            });
        }

        return item;
    }

    private static AttributeDefinition GetAttribute(string option) => option switch
    {
        "DD" => Stats.ArmorDamageDecrease,
        "DSR" => Stats.DefenseRatePvm,
        "Reflect" => Stats.DamageReflection,
        "HP" => Stats.MaximumHealth,
        "Mana" => Stats.MaximumMana,
        "EDR" => Stats.ExcellentDamageChance,
        "Speed" => Stats.AttackSpeedAny,
        _ => throw new ArgumentException(option),
    };
}
