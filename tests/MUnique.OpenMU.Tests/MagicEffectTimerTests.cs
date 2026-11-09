// <copyright file="MagicEffectTimerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests duration management for active magic effects.
/// </summary>
[TestFixture]
public class MagicEffectTimerTests
{
    /// <summary>
    /// Decreasing the timer shortens its remaining time by the requested amount.
    /// </summary>
    [Test]
    public async Task DecreaseTimerShortensTimeLeftAsync()
    {
        var effect = new MagicEffect(TimeSpan.FromSeconds(20), new MagicEffectDefinition());
        try
        {
            var timeLeftBefore = effect.TimeLeft;

            effect.DecreaseTimer(5);

            Assert.That(effect.TimeLeft, Is.LessThanOrEqualTo(timeLeftBefore - TimeSpan.FromSeconds(5)));
            Assert.That(effect.TimeLeft, Is.GreaterThan(TimeSpan.Zero));
        }
        finally
        {
            await effect.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Decreasing the timer below zero clamps it to zero.
    /// </summary>
    [Test]
    public async Task DecreaseTimerClampsAtZeroAsync()
    {
        var effect = new MagicEffect(TimeSpan.FromSeconds(20), new MagicEffectDefinition());
        try
        {
            effect.DecreaseTimer(30);

            Assert.That(effect.TimeLeft, Is.EqualTo(TimeSpan.Zero));
        }
        finally
        {
            await effect.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A negative decrement is rejected.
    /// </summary>
    [Test]
    public async Task DecreaseTimerRejectsNegativeSecondsAsync()
    {
        var effect = new MagicEffect(TimeSpan.FromSeconds(20), new MagicEffectDefinition());
        try
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => effect.DecreaseTimer(-1));
        }
        finally
        {
            await effect.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Shortening effects by stat only changes effects which produce that stat.
    /// </summary>
    [Test]
    public async Task ShortenEffectsProducingSpecificStatOnlyChangesMatchingEffectsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var frozenEffect = CreateEffect(1, Stats.IsFrozen);
        var unrelatedEffect = CreateEffect(2, Stats.IsAsleep);
        var effects = player.MagicEffectList;

        try
        {
            await effects.AddEffectAsync(frozenEffect).ConfigureAwait(false);
            await effects.AddEffectAsync(unrelatedEffect).ConfigureAwait(false);
            var frozenTimeBefore = frozenEffect.TimeLeft;
            var unrelatedTimeBefore = unrelatedEffect.TimeLeft;

            await effects.ShortenEffectsProducingSpecificStatAsync(Stats.IsFrozen, 1).ConfigureAwait(false);

            Assert.That(frozenEffect.TimeLeft, Is.LessThanOrEqualTo(frozenTimeBefore - TimeSpan.FromSeconds(1)));
            Assert.That(unrelatedEffect.TimeLeft, Is.GreaterThan(unrelatedTimeBefore - TimeSpan.FromSeconds(1)));
        }
        finally
        {
            await effects.ClearAllEffectsAsync().ConfigureAwait(false);
        }
    }

    private static MagicEffect CreateEffect(short id, AttributeDefinition target)
    {
        return new MagicEffect(
            TimeSpan.FromMinutes(10),
            new MagicEffectDefinition { Number = id },
            new MagicEffect.ElementWithTarget(new SimpleElement(1, AggregateType.AddRaw), target));
    }
}
