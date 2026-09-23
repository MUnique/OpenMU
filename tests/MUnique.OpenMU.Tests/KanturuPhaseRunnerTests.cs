// <copyright file="KanturuPhaseRunnerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Tests for the Kanturu phase runners.
/// </summary>
[TestFixture]
public class KanturuPhaseRunnerTests
{
    /// <summary>
    /// Tests that the wave runner executes its steps in order.
    /// </summary>
    [Test]
    public async Task MonsterWaveRunner_RunsBeginAnnounceWaitStandby_InOrder()
    {
        var order = new List<string>();
        var runner = new KanturuMonsterWaveRunner(
            (_, _) =>
            {
                order.Add("begin");
                return Task.CompletedTask;
            },
            _ =>
            {
                order.Add("announce");
                return Task.CompletedTask;
            },
            (_, _) =>
            {
                order.Add("wait");
                return Task.FromResult(true);
            },
            (_, _) =>
            {
                order.Add("standby");
                return Task.CompletedTask;
            });

        Assert.That(runner.Kind, Is.EqualTo(KanturuPhaseKind.MonsterWave));
        var completed = await runner.RunAsync(new KanturuPhaseDefinition(), CancellationToken.None).ConfigureAwait(false);

        Assert.That(completed, Is.True);
        Assert.That(order, Is.EqualTo(new[] { "begin", "announce", "wait", "standby" }));
    }

    /// <summary>
    /// Tests that the wave runner reports failure without standby when the wait fails.
    /// </summary>
    [Test]
    public async Task MonsterWaveRunner_ReportsFailure_WithoutStandby()
    {
        var order = new List<string>();
        var runner = new KanturuMonsterWaveRunner(
            (_, _) =>
            {
                order.Add("begin");
                return Task.CompletedTask;
            },
            _ =>
            {
                order.Add("announce");
                return Task.CompletedTask;
            },
            (_, _) =>
            {
                order.Add("wait");
                return Task.FromResult(false);
            },
            (_, _) =>
            {
                order.Add("standby");
                return Task.CompletedTask;
            });

        var completed = await runner.RunAsync(new KanturuPhaseDefinition(), CancellationToken.None).ConfigureAwait(false);

        Assert.That(completed, Is.False);
        Assert.That(order, Is.EqualTo(new[] { "begin", "announce", "wait" }));
    }

    /// <summary>
    /// Tests that the transition runner clears the phase and shows the state.
    /// </summary>
    [Test]
    public async Task TransitionRunner_ClearsPhase_AndShowsState()
    {
        KanturuState? shownState = null;
        byte shownDetail = 0;
        var cleared = false;
        var playerCalls = 0;
        var runner = new KanturuTransitionRunner(
            (state, detail) =>
            {
                shownState = state;
                shownDetail = detail;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                playerCalls++;
                return ValueTask.CompletedTask;
            },
            () => cleared = true);

        Assert.That(runner.Kind, Is.EqualTo(KanturuPhaseKind.Transition));
        var phase = new KanturuPhaseDefinition
        {
            State = KanturuState.MayaBattle,
            DetailState = 7,
            Transition = new KanturuTransitionDefinition { CinematicDuration = TimeSpan.Zero, WarpAnimationDelay = TimeSpan.Zero },
        };
        await runner.RunAsync(phase, CancellationToken.None).ConfigureAwait(false);

        Assert.That(cleared, Is.True);
        Assert.That(shownState, Is.EqualTo(KanturuState.MayaBattle));
        Assert.That(shownDetail, Is.EqualTo(7));
        Assert.That(playerCalls, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the nightmare runner arms the spawn capture before beginning the phase.
    /// The boss spawns synchronously inside begin, so subscribing afterwards would miss it
    /// and silently disable the health phases and special attacks.
    /// </summary>
    [Test]
    public async Task NightmareRunner_ArmsSpawnCapture_BeforeBeginningPhase()
    {
        var order = new List<string>();

        // Records "subscribed" synchronously when the wait task is created,
        // like the real waiter subscribes before its first await.
        Task<Monster?> WaitForSpawn(KanturuNightmareDefinition _, CancellationToken __)
        {
            order.Add("subscribed");
            return Task.FromResult<Monster?>(null);
        }

        Task Begin(KanturuPhaseDefinition _, CancellationToken __)
        {
            order.Add("begin");
            return Task.CompletedTask;
        }

        var runner = new KanturuNightmareRunner(
            Begin,
            (_, _) => ValueTask.CompletedTask,
            _ => ValueTask.CompletedTask,
            () => ValueTask.CompletedTask,
            (_, _) => Task.FromResult(true),
            (_, _) => Task.CompletedTask,
            WaitForSpawn,
            _ => ValueTask.CompletedTask,
            NullLogger.Instance);

        Assert.That(runner.Kind, Is.EqualTo(KanturuPhaseKind.Nightmare));
        var phase = new KanturuPhaseDefinition
        {
            Nightmare = new KanturuNightmareDefinition
            {
                HpPhases = new List<KanturuNightmareHpPhase>(),
                HealthCheckInterval = TimeSpan.Zero,
                SpecialAttackInterval = TimeSpan.Zero,
            },
        };
        await runner.RunAsync(phase, CancellationToken.None).ConfigureAwait(false);

        Assert.That(order, Is.EqualTo(new[] { "subscribed", "begin" }));
    }
}
