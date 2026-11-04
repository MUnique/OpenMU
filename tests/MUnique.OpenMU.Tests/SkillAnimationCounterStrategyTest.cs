// <copyright file="SkillAnimationCounterStrategyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

/// <summary>
/// Tests for the <see cref="ISkillAnimationCounterStrategy"/> implementations.
/// </summary>
[TestFixture]
public class SkillAnimationCounterStrategyTest
{
    private const byte LastAnimationCounter = 7;

    /// <summary>
    /// Tests whether the strategies count an animation with a counter of 0.
    /// </summary>
    /// <param name="strategyType">The type of the strategy.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(typeof(TwisterAnimationCounterStrategy), true)]
    [TestCase(typeof(EvilSpiritAnimationCounterStrategy), true)]
    [TestCase(typeof(MultiShotAnimationCounterStrategy), false)]
    public void AnimationWithCounterZeroIsCounted(Type strategyType, bool expected)
    {
        var strategy = (ISkillAnimationCounterStrategy)Activator.CreateInstance(strategyType)!;

        Assert.That(strategy.IsAnimationCounted(0), Is.EqualTo(expected));
        Assert.That(strategy.IsAnimationCounted(1), Is.True);
    }

    /// <summary>
    /// Tests to which animation a hit without animation counter refers.
    /// </summary>
    /// <param name="strategyType">The type of the strategy.</param>
    /// <param name="expected">The expected animation counter.</param>
    [TestCase(typeof(TwisterAnimationCounterStrategy), LastAnimationCounter)]
    [TestCase(typeof(EvilSpiritAnimationCounterStrategy), 0)]
    [TestCase(typeof(MultiShotAnimationCounterStrategy), LastAnimationCounter)]
    public void HitWithoutAnimationCounter(Type strategyType, byte expected)
    {
        var strategy = (ISkillAnimationCounterStrategy)Activator.CreateInstance(strategyType)!;

        Assert.That(strategy.GetAnimationCounterOfHit(0, LastAnimationCounter), Is.EqualTo(expected));
        Assert.That(strategy.GetAnimationCounterOfHit(3, LastAnimationCounter), Is.EqualTo(3));
    }
}
