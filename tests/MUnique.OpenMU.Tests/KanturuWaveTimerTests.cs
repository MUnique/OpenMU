// <copyright file="KanturuWaveTimerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for the <see cref="KanturuWaveTimer"/> shared wave countdown.
/// </summary>
[TestFixture]
public class KanturuWaveTimerTests
{
    /// <summary>
    /// Tests that a phase without a group simply keeps its own limit.
    /// </summary>
    [Test]
    public void UngroupedPhase_KeepsOwnLimit()
    {
        var timer = new KanturuWaveTimer(() => DateTime.UtcNow);
        var phase = new KanturuPhaseDefinition { TimeLimit = TimeSpan.FromMinutes(15) };

        Assert.That(timer.GetEffectiveLimit(phase), Is.EqualTo(TimeSpan.FromMinutes(15)));
    }

    /// <summary>
    /// Tests that a phase without a group and without a limit has no countdown.
    /// </summary>
    [Test]
    public void UngroupedPhaseWithoutLimit_HasNoCountdown()
    {
        var timer = new KanturuWaveTimer(() => DateTime.UtcNow);

        Assert.That(timer.GetEffectiveLimit(new KanturuPhaseDefinition()), Is.Null);
    }

    /// <summary>
    /// Tests that the first grouped phase starts the shared clock with its full limit,
    /// and a following phase of the group inherits the remainder.
    /// </summary>
    [Test]
    public void GroupedPhases_ShareOneClock()
    {
        var now = new DateTime(2026, 9, 26, 12, 0, 0, DateTimeKind.Utc);
        var timer = new KanturuWaveTimer(() => now);
        var monsters = new KanturuPhaseDefinition { TimeLimit = TimeSpan.FromMinutes(15), TimeLimitGroup = KanturuWaveGroup.MayaLeftHand };
        var boss = new KanturuPhaseDefinition { TimeLimitGroup = KanturuWaveGroup.MayaLeftHand };

        Assert.That(timer.GetEffectiveLimit(monsters), Is.EqualTo(TimeSpan.FromMinutes(15)));

        now += TimeSpan.FromMinutes(14);
        Assert.That(timer.GetEffectiveLimit(boss), Is.EqualTo(TimeSpan.FromMinutes(1)));
    }

    /// <summary>
    /// Tests that a following phase sees an expired remainder once the shared clock ran out.
    /// </summary>
    [Test]
    public void GroupedPhase_AfterExpiry_SeesExpiredRemainder()
    {
        var now = new DateTime(2026, 9, 26, 12, 0, 0, DateTimeKind.Utc);
        var timer = new KanturuWaveTimer(() => now);
        var monsters = new KanturuPhaseDefinition { TimeLimit = TimeSpan.FromMinutes(15), TimeLimitGroup = KanturuWaveGroup.MayaLeftHand };
        var boss = new KanturuPhaseDefinition { TimeLimitGroup = KanturuWaveGroup.MayaLeftHand };

        timer.GetEffectiveLimit(monsters);
        now += TimeSpan.FromMinutes(16);

        Assert.That(timer.GetEffectiveLimit(boss), Is.LessThanOrEqualTo(TimeSpan.Zero));
    }

    /// <summary>
    /// Tests that different groups run independent clocks.
    /// </summary>
    [Test]
    public void DifferentGroups_RunIndependentClocks()
    {
        var now = new DateTime(2026, 9, 26, 12, 0, 0, DateTimeKind.Utc);
        var timer = new KanturuWaveTimer(() => now);

        timer.GetEffectiveLimit(new KanturuPhaseDefinition { TimeLimit = TimeSpan.FromMinutes(15), TimeLimitGroup = KanturuWaveGroup.MayaLeftHand });
        now += TimeSpan.FromMinutes(14);

        var nextWave = new KanturuPhaseDefinition { TimeLimit = TimeSpan.FromMinutes(15), TimeLimitGroup = KanturuWaveGroup.MayaRightHand };
        Assert.That(timer.GetEffectiveLimit(nextWave), Is.EqualTo(TimeSpan.FromMinutes(15)));
    }

    /// <summary>
    /// Tests that a grouped phase without any clock started in its group has no countdown.
    /// </summary>
    [Test]
    public void GroupedPhaseWithoutStartedClock_HasNoCountdown()
    {
        var timer = new KanturuWaveTimer(() => DateTime.UtcNow);

        Assert.That(timer.GetEffectiveLimit(new KanturuPhaseDefinition { TimeLimitGroup = KanturuWaveGroup.MayaLeftHand }), Is.Null);
    }
}
