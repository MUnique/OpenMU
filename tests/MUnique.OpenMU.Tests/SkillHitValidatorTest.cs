// <copyright file="SkillHitValidatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the animation counter of the <see cref="SkillHitValidator"/>.
/// </summary>
[TestFixture]
public class SkillHitValidatorTest
{
    private const ushort SkillId = 8;
    private const byte MaximumCounterValue = 0x32;

    /// <summary>
    /// Tests that the first animation counter is accepted, because the client doesn't reset its counter,
    /// and that the following animations have to continue it.
    /// </summary>
    [Test]
    public void AnimationCounterContinuesFromFirstAnimation()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.That(validator.TryRegisterAnimation(SkillId, 10), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 11), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 13), Is.False);
    }

    /// <summary>
    /// Tests that the expected animation counter wraps from its maximum back to 1.
    /// </summary>
    [Test]
    public void AnimationCounterWrapsAtMaximum()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.That(validator.TryRegisterAnimation(SkillId, MaximumCounterValue), Is.True);
        Assert.That(validator.TryRegisterAnimation(SkillId, 1), Is.True);
    }

    /// <summary>
    /// Tests that animation counters above the maximum are rejected instead of causing an out-of-range access.
    /// </summary>
    [Test]
    public void AnimationCounterAboveMaximumIsRejected()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);
        var invalidCounter = (byte)(MaximumCounterValue + 1);

        Assert.Multiple(() =>
        {
            Assert.That(validator.TryRegisterAnimation(SkillId, invalidCounter), Is.False);
            Assert.That(validator.IsHitValid(SkillId, invalidCounter, 1).IsValid, Is.False);
            Assert.That(validator.IsHitCounterValid(SkillId, invalidCounter, 1), Is.False);
        });
    }

    /// <summary>
    /// Tests that an animation counter of 0 is only checked for a skill whose strategy counts every animation.
    /// </summary>
    /// <param name="withStrategy">If set to <c>true</c>, the <see cref="TwisterAnimationCounterStrategy"/> is registered.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void AnimationCounterOfZeroIsCheckedIfStrategyCountsEveryAnimation(bool withStrategy)
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        if (withStrategy)
        {
            plugInManager.RegisterPlugIn<ISkillAnimationCounterStrategy, TwisterAnimationCounterStrategy>();
        }

        var validator = new SkillHitValidator(NullLogger.Instance, plugInManager);
        validator.TryRegisterAnimation(SkillId, 10);

        Assert.That(validator.TryRegisterAnimation(SkillId, 0), Is.EqualTo(!withStrategy));
    }

    /// <summary>
    /// Tests that a hit is only valid, if the animation to which it refers was registered.
    /// </summary>
    [Test]
    public void HitIsOnlyValidForRegisteredAnimation()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.That(validator.IsHitValid(SkillId, 10, 11).IsValid, Is.False);

        validator.TryRegisterAnimation(SkillId, 10);

        Assert.That(validator.IsHitValid(SkillId, 10, 11).IsValid, Is.True);
        Assert.That(validator.IsHitValid(SkillId, 12, 11).IsValid, Is.False);
    }

    /// <summary>
    /// Tests that a hit cannot reference the animation of another skill.
    /// </summary>
    [Test]
    public void HitForAnimationOfAnotherSkillIsInvalid()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);
        validator.TryRegisterAnimation(SkillId, 10);

        Assert.That(validator.IsHitValid((ushort)(SkillId + 1), 10, 11).IsValid, Is.False);
    }

    /// <summary>
    /// Tests that an animation which is too old cannot be used for a hit.
    /// </summary>
    [Test]
    public void StaleAnimationIsInvalid()
    {
        var timeProvider = new ManualTimeProvider();
        var validator = new SkillHitValidator(NullLogger.Instance, timeProvider: timeProvider);
        validator.TryRegisterAnimation(SkillId, 10);

        timeProvider.Advance(TimeSpan.FromSeconds(11));

        Assert.That(validator.IsHitValid(SkillId, 10, 11).IsValid, Is.False);
    }

    /// <summary>
    /// Tests that the same hit counter cannot be reused for the same animation.
    /// </summary>
    [Test]
    public void RepeatedHitCounterIsInvalid()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);

        Assert.Multiple(() =>
        {
            Assert.That(validator.IsHitCounterValid(SkillId, 10, 5), Is.True);
            Assert.That(validator.IsHitCounterValid(SkillId, 10, 5), Is.False);
            Assert.That(validator.IsHitCounterValid(SkillId, 10, 6), Is.True);
            Assert.That(validator.IsHitCounterValid(SkillId, 11, 5), Is.True, "different animation");
            Assert.That(validator.IsHitCounterValid((ushort)(SkillId + 1), 10, 5), Is.True, "different skill");
        });
    }

    /// <summary>
    /// Tests that the hit counter check uses the animation counter which the strategy resolves for the hit.
    /// </summary>
    [Test]
    public void HitCounterCheckUsesAnimationCounterFromStrategy()
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugIn<ISkillAnimationCounterStrategy, TwisterAnimationCounterStrategy>();
        var validator = new SkillHitValidator(NullLogger.Instance, plugInManager);
        validator.TryRegisterAnimation(SkillId, 10);
        validator.TryRegisterAnimation(SkillId, 11);

        Assert.Multiple(() =>
        {
            Assert.That(validator.IsHitCounterValid(SkillId, 0, 5), Is.True);
            Assert.That(validator.IsHitCounterValid(SkillId, 0, 5), Is.False);
        });

        validator.TryRegisterAnimation(SkillId, 12);

        Assert.That(validator.IsHitCounterValid(SkillId, 0, 5), Is.True);
    }

    /// <summary>
    /// Tests that the same hit counter may be reused after the replay window, e.g. when the client counter wrapped.
    /// </summary>
    [Test]
    public void RepeatedHitCounterIsAllowedAfterReplayWindow()
    {
        var timeProvider = new ManualTimeProvider();
        var validator = new SkillHitValidator(NullLogger.Instance, timeProvider: timeProvider);

        Assert.That(validator.IsHitCounterValid(SkillId, 10, 5), Is.True);

        timeProvider.Advance(TimeSpan.FromSeconds(2));

        Assert.That(validator.IsHitCounterValid(SkillId, 10, 5), Is.True);
    }

    /// <summary>
    /// Tests that explicit hits require a preceding area skill attack and stay within its area.
    /// </summary>
    [Test]
    public void TargetOutsideRegisteredAreaIsInvalid()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);
        var skill = new Skill { Number = (short)SkillId, Range = 5, SkillType = SkillType.AreaSkillExplicitHits };
        var center = new Point(100, 100);

        Assert.That(validator.IsTargetInArea(SkillId, center), Is.False, "missing previous attack");

        validator.RegisterArea(SkillId, skill, center, center);

        Assert.Multiple(() =>
        {
            Assert.That(validator.IsTargetInArea(SkillId, new Point(107, 100)), Is.True, "inside range plus slack");
            Assert.That(validator.IsTargetInArea(SkillId, new Point(108, 100)), Is.False, "outside range plus slack");
        });
    }

    /// <summary>
    /// Tests that target area skills are checked against the aimed center.
    /// </summary>
    [Test]
    public void TargetAreaSkillUsesAimedCenter()
    {
        var validator = new SkillHitValidator(NullLogger.Instance);
        var skill = new Skill
        {
            Number = (short)SkillId,
            Range = 5,
            SkillType = SkillType.AreaSkillExplicitHits,
            AreaSkillSettings = new AreaSkillSettings { UseTargetAreaFilter = true, TargetAreaDiameter = 2f },
        };

        validator.RegisterArea(SkillId, skill, new Point(100, 100), new Point(110, 100));

        Assert.Multiple(() =>
        {
            Assert.That(validator.IsTargetInArea(SkillId, new Point(110, 100)), Is.True, "target at the aimed center");
            Assert.That(validator.IsTargetInArea(SkillId, new Point(111, 100)), Is.True, "target inside the target-area radius plus slack");
            Assert.That(validator.IsTargetInArea(SkillId, new Point(114, 100)), Is.False, "target outside the target-area radius plus slack");
        });
    }

    /// <summary>
    /// Tests that a hit without animation counter refers to the last animation of the skill,
    /// if its strategy says that the hits don't contain the animation counter.
    /// </summary>
    /// <param name="withStrategy">If set to <c>true</c>, the <see cref="TwisterAnimationCounterStrategy"/> is registered.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void HitWithoutAnimationCounterRefersToLastAnimationIfStrategySaysSo(bool withStrategy)
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        if (withStrategy)
        {
            plugInManager.RegisterPlugIn<ISkillAnimationCounterStrategy, TwisterAnimationCounterStrategy>();
        }

        var validator = new SkillHitValidator(NullLogger.Instance, plugInManager);
        validator.TryRegisterAnimation(SkillId, 10);

        Assert.That(validator.IsHitValid(SkillId, 0, 11).IsValid, Is.EqualTo(withStrategy));
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private long _timestamp;

        /// <inheritdoc />
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public void Advance(TimeSpan delta) => this._timestamp += delta.Ticks;

        /// <inheritdoc />
        public override long GetTimestamp() => this._timestamp;
    }
}
