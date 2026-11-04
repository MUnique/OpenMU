// <copyright file="ResetProgressionCalculatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Tests for <see cref="ResetProgressionCalculator"/>.
/// </summary>
[TestFixture]
public class ResetProgressionCalculatorTest
{
    /// <summary>
    /// Verifies boundary behavior for point tiers.
    /// </summary>
    [TestCase(0, 800)]
    [TestCase(9, 800)]
    [TestCase(10, 600)]
    [TestCase(19, 600)]
    [TestCase(20, 400)]
    [TestCase(29, 400)]
    [TestCase(30, 200)]
    public void PointsTierBoundaries(int currentResetCount, int expectedPoints)
    {
        var configuration = this.CreateTieredConfiguration();

        var progression = ResetProgressionCalculator.Calculate(currentResetCount, 0, configuration);

        Assert.That(progression.PointsForReset, Is.EqualTo(expectedPoints));
    }

    /// <summary>
    /// Verifies cumulative totals for point tiers.
    /// </summary>
    [TestCase(0, 800)]
    [TestCase(1, 1600)]
    [TestCase(9, 8000)]
    [TestCase(10, 8600)]
    [TestCase(11, 9200)]
    [TestCase(20, 14400)]
    public void PointsTierTotals(int currentResetCount, int expectedTotalPoints)
    {
        var configuration = this.CreateTieredConfiguration();

        var progression = ResetProgressionCalculator.Calculate(currentResetCount, 0, configuration);

        Assert.That(progression.TotalPointsAfterReset, Is.EqualTo(expectedTotalPoints));
    }

    /// <summary>
    /// Verifies boundary behavior for item cost tiers with capped final tier.
    /// </summary>
    [TestCase(0, 2)]
    [TestCase(1, 4)]
    [TestCase(2, 8)]
    [TestCase(3, 16)]
    [TestCase(4, 32)]
    [TestCase(5, 64)]
    [TestCase(6, 64)]
    [TestCase(20, 64)]
    public void ItemCostTierBoundaries(int currentResetCount, int expectedItemAmount)
    {
        var configuration = this.CreateTieredConfiguration();

        var progression = ResetProgressionCalculator.Calculate(currentResetCount, 0, configuration);

        Assert.That(progression.RequiredItemAmount, Is.EqualTo(expectedItemAmount));
    }

    /// <summary>
    /// Verifies legacy fallback behavior when no tier collections are configured.
    /// </summary>
    [Test]
    public void LegacyFallbackWhenTiersAreEmpty()
    {
        var configuration = new ResetConfiguration
        {
            RequiredMoney = 2,
            MultiplyRequiredMoneyByResetCount = true,
            PointsPerReset = 300,
            MultiplyPointsByResetCount = true,
            PointsTiers = [],
            ItemCostTiers = [],
            RequiredResetItem = new ItemDefinition { Name = "Jewel of Creation" },
        };

        var progression = ResetProgressionCalculator.Calculate(2, 0, configuration);

        Assert.That(progression.NextResetCount, Is.EqualTo(3));
        Assert.That(progression.RequiredZen, Is.EqualTo(6));
        Assert.That(progression.PointsForReset, Is.EqualTo(900));
        Assert.That(progression.TotalPointsAfterReset, Is.EqualTo(900));
        Assert.That(progression.RequiredItemAmount, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that the per-character points override applies in legacy mode.
    /// </summary>
    [Test]
    public void PlayerSpecificPointsOverrideIsAppliedInLegacyMode()
    {
        var configuration = new ResetConfiguration
        {
            PointsPerReset = 300,
            MultiplyPointsByResetCount = true,
            PointsTiers = [],
        };

        var progression = ResetProgressionCalculator.Calculate(4, 120, configuration);

        Assert.That(progression.PointsForReset, Is.EqualTo(600));
        Assert.That(progression.TotalPointsAfterReset, Is.EqualTo(600));
    }

    private ResetConfiguration CreateTieredConfiguration()
    {
        return new ResetConfiguration
        {
            RequiredResetItem = new ItemDefinition { Name = "Jewel of Creation" },
            PointsTiers =
            [
                new() { MinimumResetCount = 1, PointsGranted = 800 },
                new() { MinimumResetCount = 11, PointsGranted = 600 },
                new() { MinimumResetCount = 21, PointsGranted = 400 },
                new() { MinimumResetCount = 31, PointsGranted = 200 },
            ],
            ItemCostTiers =
            [
                new() { MinimumResetCount = 1, RequiredItemAmount = 2 },
                new() { MinimumResetCount = 2, RequiredItemAmount = 4 },
                new() { MinimumResetCount = 3, RequiredItemAmount = 8 },
                new() { MinimumResetCount = 4, RequiredItemAmount = 16 },
                new() { MinimumResetCount = 5, RequiredItemAmount = 32 },
                new() { MinimumResetCount = 6, RequiredItemAmount = 64 },
                new() { MinimumResetCount = 7, RequiredItemAmount = 64 },
            ],
        };
    }
}
