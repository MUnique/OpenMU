// <copyright file="KanturuKillTrackerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for <see cref="KanturuKillTracker"/>.
/// </summary>
[TestFixture]
public class KanturuKillTrackerTests
{
    private static KanturuPhaseDefinition CreateWavePhase(int killTarget = 2)
    {
        return new KanturuPhaseDefinition
        {
            Name = "wave",
            Kind = KanturuPhaseKind.MonsterWave,
            KillTarget = killTarget,
            CountedMonsters = new List<MonsterDefinition> { new() { Number = 354 } },
        };
    }

    /// <summary>
    /// Tests that kills without a current phase are ignored.
    /// </summary>
    [Test]
    public void RegisterKill_WithoutPhase_IsIgnored()
    {
        var tracker = new KanturuKillTracker();

        var result = tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        Assert.That(result.Counted, Is.False);
        Assert.That(result.PhaseComplete, Is.False);
    }

    /// <summary>
    /// Tests that the phase completes once the kill target is reached.
    /// </summary>
    [Test]
    public void RegisterKill_CountsUntilTarget_ThenCompletes()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase());

        var first = tracker.RegisterKill(new MonsterDefinition { Number = 354 });
        var second = tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        Assert.That(first.Counted, Is.True);
        Assert.That(first.PhaseComplete, Is.False);
        Assert.That(second.Counted, Is.True);
        Assert.That(second.PhaseComplete, Is.True);
        Assert.That(tracker.PhaseCompleted.IsCompleted, Is.True);
    }

    /// <summary>
    /// Tests that kills of other monsters don't change the kill count.
    /// </summary>
    [Test]
    public void RegisterKill_OtherMonster_IsIgnored()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase());

        var result = tracker.RegisterKill(new MonsterDefinition { Number = 999 });

        Assert.That(result.Counted, Is.False);
        Assert.That(tracker.KillCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that killing the Nightmare boss sets the boss flag.
    /// </summary>
    [Test]
    public void RegisterKill_NightmareBoss_SetsBossFlag()
    {
        var boss = new MonsterDefinition { Number = 361 };
        var phase = new KanturuPhaseDefinition
        {
            Name = "nightmare",
            Kind = KanturuPhaseKind.Nightmare,
            KillTarget = 1,
            CountedMonsters = new List<MonsterDefinition> { boss },
            Nightmare = new KanturuNightmareDefinition { Monster = new MonsterDefinition { Number = 361 } },
        };
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(phase);

        var result = tracker.RegisterKill(new MonsterDefinition { Number = 361 });

        Assert.That(result.Counted, Is.True);
        Assert.That(result.NightmareBossKilled, Is.True);
        Assert.That(result.IsNightmarePhase, Is.True);
        Assert.That(result.PhaseComplete, Is.True);
        Assert.That(result.Phase, Is.SameAs(phase));
    }

    /// <summary>
    /// Tests that starting a new phase resets the count and the completion.
    /// </summary>
    [Test]
    public void BeginPhase_ResetsCount_AndClearsCompletion()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase(killTarget: 1));
        tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        tracker.BeginPhase(CreateWavePhase(killTarget: 5));

        Assert.That(tracker.KillCount, Is.EqualTo(0));
        Assert.That(tracker.PhaseCompleted.IsCompleted, Is.False);
    }

    /// <summary>
    /// Tests that kills are ignored after the phase has been cleared.
    /// </summary>
    [Test]
    public void ClearPhase_KillsAreIgnored()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase());
        tracker.ClearPhase();

        var result = tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        Assert.That(result.Counted, Is.False);
        Assert.That(result.Phase, Is.Null);
    }

    /// <summary>
    /// Tests that concurrent kills count exactly once each and complete the phase.
    /// </summary>
    [Test]
    public async Task RegisterKill_FromMultipleThreads_CountsExactlyOnce()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase(killTarget: 2000));
        var monster = new MonsterDefinition { Number = 354 };

        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            for (var i = 0; i < 250; i++)
            {
                tracker.RegisterKill(monster);
            }
        }))).ConfigureAwait(false);

        Assert.That(tracker.KillCount, Is.EqualTo(2000));
        Assert.That(tracker.PhaseCompleted.IsCompleted, Is.True);
    }

    /// <summary>
    /// Tests that a completed phase's kills don't leak into the next generation.
    /// </summary>
    [Test]
    public void BeginPhase_AfterCompletedPhase_StartsIsolatedGeneration()
    {
        var tracker = new KanturuKillTracker();
        tracker.BeginPhase(CreateWavePhase(killTarget: 1));
        tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        tracker.BeginPhase(CreateWavePhase(killTarget: 2));

        Assert.That(tracker.KillCount, Is.EqualTo(0));
        Assert.That(tracker.PhaseCompleted.IsCompleted, Is.False);

        var result = tracker.RegisterKill(new MonsterDefinition { Number = 354 });

        Assert.That(result.Counted, Is.True);
        Assert.That(result.KillCount, Is.EqualTo(1));
        Assert.That(result.PhaseComplete, Is.False);
    }
}
