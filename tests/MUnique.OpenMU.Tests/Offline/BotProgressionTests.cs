// <copyright file="BotProgressionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests for <see cref="BotProgression"/>: the point split with capacities and the per-bot rolls.
/// </summary>
[TestFixture]
public class BotProgressionTests
{
    /// <summary>
    /// Tests that the split assigns all points proportionally when nothing is capped.
    /// </summary>
    [Test]
    public void SplitPoints_AssignsAllPointsProportionally()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 35), (Stats.BaseVitality, 5) };

        var result = BotProgression.SplitPoints(1000, weights).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(1000));
        Assert.That(result[Stats.BaseAgility], Is.EqualTo(350));
        Assert.That(result[Stats.BaseVitality], Is.EqualTo(50));
        Assert.That(result[Stats.BaseStrength], Is.EqualTo(600));
    }

    /// <summary>
    /// Tests that a capped stat drops out of the split and its share flows to the remaining stats.
    /// </summary>
    [Test]
    public void SplitPoints_CappedStatOverflowsToOthers()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 35), (Stats.BaseVitality, 5) };
        long CapacityOf(AttributeDefinition stat) => stat == Stats.BaseVitality ? 10 : long.MaxValue;

        var result = BotProgression.SplitPoints(1000, weights, CapacityOf).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result[Stats.BaseVitality], Is.EqualTo(10));
        Assert.That(result.Values.Sum(), Is.EqualTo(1000));
        Assert.That(result[Stats.BaseStrength] + result[Stats.BaseAgility], Is.EqualTo(990));
    }

    /// <summary>
    /// Tests that points stay unassigned when every stat is at its capacity, like for a maxed character.
    /// </summary>
    [Test]
    public void SplitPoints_AllCapped_LeavesPointsUnassigned()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 40) };

        // Every stat is capped at the same value, so which one is asked for does not matter.
        Func<AttributeDefinition, long> capacityOf = _ => 25;

        var result = BotProgression.SplitPoints(1000, weights, capacityOf).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(50));
        Assert.That(result.Values, Is.All.EqualTo(25));
    }

    /// <summary>
    /// Tests that the vitality target roll stays within 100..500 and is stable for the same name.
    /// </summary>
    [Test]
    public void GetVitalityTarget_IsStableAndWithinRange()
    {
        foreach (var name in new[] { "Kaeoris", "Milynara", "Hallin", "Oriwen", "X" })
        {
            var target = BotProgression.GetVitalityTarget(name);
            Assert.That(target, Is.InRange(100, 500), name);
            Assert.That(BotProgression.GetVitalityTarget(name), Is.EqualTo(target), name);
        }
    }
}
