// <copyright file="ItemPriceCalculatorOverflowTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the <see cref="ItemPriceCalculator"/> limits prices of configured formulas which are too large,
/// instead of overflowing, e.g. to a price of 0.
/// </summary>
[TestFixture]
public class ItemPriceCalculatorOverflowTest
{
    private const long MaximumPrice = 3_000_000_000;

    private readonly ItemPriceCalculator _calculator = new();

    /// <summary>
    /// Tests that multiplying a large price by the quantity doesn't overflow.
    /// 2^62 * 4 would be 0 with unchecked 64-bit arithmetic.
    /// </summary>
    /// <param name="formula">The base price formula.</param>
    /// <param name="durability">The durability, which is the quantity.</param>
    /// <param name="expectedPrice">The expected buying price.</param>
    [TestCase("2^62", 4, MaximumPrice)]
    [TestCase("2^62", 255, MaximumPrice)]
    [TestCase("2^63 - 1024", 2, MaximumPrice)]
    [TestCase("-(2^62)", 4, -MaximumPrice)]
    public void QuantityScalingDoesNotOverflow(string formula, byte durability, long expectedPrice)
    {
        var item = CreateItem(formula, ItemPriceQuantityScaling.PerPiece, ItemPriceModifiers.None, durability);

        Assert.That(this._calculator.CalculateFinalBuyingPrice(item), Is.EqualTo(expectedPrice));
    }

    /// <summary>
    /// Tests that the fill ratio scaling of a large price doesn't overflow.
    /// </summary>
    [Test]
    public void FillRatioScalingDoesNotOverflow()
    {
        var item = CreateItem("2^62", ItemPriceQuantityScaling.ByFillRatio, ItemPriceModifiers.None, durability: 200);

        Assert.That(this._calculator.CalculateFinalBuyingPrice(item), Is.EqualTo(MaximumPrice));
    }

    /// <summary>
    /// Tests that repeated doubling by excellent options doesn't overflow.
    /// 2^61 doubled six times would be 0 with unchecked 64-bit arithmetic.
    /// </summary>
    /// <param name="formula">The base price formula.</param>
    /// <param name="expectedPrice">The expected buying price.</param>
    [TestCase("2^61", MaximumPrice)]
    [TestCase("-(2^61)", -MaximumPrice)]
    [TestCase("1000", 64_000)]
    public void ExcellentOptionsDoNotOverflow(string formula, long expectedPrice)
    {
        var item = CreateItem(formula, ItemPriceQuantityScaling.None, ItemPriceModifiers.All, durability: 1);
        for (int i = 0; i < 6; i++)
        {
            item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Excellent } });
        }

        Assert.That(this._calculator.CalculateFinalBuyingPrice(item), Is.EqualTo(expectedPrice));
    }

    /// <summary>
    /// Tests that the other modifiers don't overflow, including the normal option with a level beyond the usual ones.
    /// </summary>
    [Test]
    public void OtherModifiersDoNotOverflow()
    {
        var item = CreateItem("2^62", ItemPriceQuantityScaling.None, ItemPriceModifiers.All, durability: 1);
        item.HasSkill = true;
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Luck } });
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Option }, Level = 100 });
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Wing } });
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.GuardianOption } });

        Assert.That(this._calculator.CalculateFinalBuyingPrice(item), Is.EqualTo(MaximumPrice));
        Assert.That(this._calculator.CalculateSellingPrice(item, 1), Is.EqualTo(MaximumPrice / 3));
    }

    private static Item CreateItem(string formula, ItemPriceQuantityScaling scaling, ItemPriceModifiers modifiers, byte durability)
    {
        var definition = new BasicModel.ItemDefinition
        {
            Group = 14,
            Number = 200,
            Durability = 255,
            PriceDefinition = new BasicModel.ItemPriceDefinition
            {
                Name = "Too expensive",
                BasePriceFormula = formula,
                QuantityScaling = scaling,
                Modifiers = modifiers,
            },
        };

        return new BasicModel.Item { Definition = definition, Durability = durability };
    }
}
