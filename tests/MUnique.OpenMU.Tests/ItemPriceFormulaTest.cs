// <copyright file="ItemPriceFormulaTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests the <see cref="ItemPriceFormula"/>.
/// </summary>
[TestFixture]
public class ItemPriceFormulaTest
{
    private static readonly ItemPriceFormula.Variables DefaultVariables = new(
        Level: 3,
        Durability: 10,
        MaxDurability: 20,
        Value: 5,
        DropLevel: 40,
        OptionLevel: 0,
        OptionCount: 0,
        HealthRecoveryOptionLevel: 0,
        AutomaticPrice: 1000);

    /// <summary>
    /// Tests that a valid formula is evaluated with the variables and truncated to an integer.
    /// </summary>
    /// <param name="formula">The formula.</param>
    /// <param name="expectedPrice">The expected price.</param>
    [TestCase("9000000", 9_000_000)]
    [TestCase("(level + 1) * 200000", 800_000)]
    [TestCase("if(level = 3, durability * 3900, 9000)", 39_000)]
    [TestCase("floor(automaticPrice * 80 / 100)", 800)]
    [TestCase("value / 2", 2)]
    [TestCase("(level - 4) * 60000", -60_000)]
    public void EvaluatesValidFormula(string formula, long expectedPrice)
    {
        Assert.That(ItemPriceFormula.Get(formula).Evaluate(DefaultVariables), Is.EqualTo(expectedPrice));
    }

    /// <summary>
    /// Tests that a formula which doesn't result in a valid price throws, instead of returning an arbitrary price.
    /// </summary>
    /// <param name="formula">The formula.</param>
    [TestCase("1 / 0")]
    [TestCase("1 / (level - 3)")]
    [TestCase("10^400")]
    [TestCase("-(10^400)")]
    [TestCase("10^19")]
    [TestCase("-(10^19)")]
    [TestCase("2^63")]
    [TestCase("unknownVariable * 2")]
    [TestCase("9000000 +")]
    public void ThrowsForInvalidResult(string formula)
    {
        Assert.Throws<InvalidOperationException>(() => ItemPriceFormula.Get(formula).Evaluate(DefaultVariables));
    }

    /// <summary>
    /// Tests that the largest results which fit into a <see cref="long"/> are accepted.
    /// </summary>
    [Test]
    public void AcceptsResultsInTheRangeOfLong()
    {
        Assert.That(ItemPriceFormula.Get("2^62").Evaluate(DefaultVariables), Is.EqualTo(1L << 62));
        Assert.That(ItemPriceFormula.Get("-(2^63)").Evaluate(DefaultVariables), Is.EqualTo(long.MinValue));
    }
}
