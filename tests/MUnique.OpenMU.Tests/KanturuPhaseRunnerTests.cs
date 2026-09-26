// <copyright file="KanturuPhaseRunnerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.NPC;
using MonsterAttribute = MUnique.OpenMU.Persistence.BasicModel.MonsterAttribute;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;

/// <summary>
/// Tests for the Kanturu phase runners.
/// </summary>
[TestFixture]
public class KanturuPhaseRunnerTests
{
    private IGameContext _gameContext = null!;

    private GameMap _map = null!;

    /// <summary>
    /// Sets up a fresh game context and map before each test.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
        this._map = (await this._gameContext.GetMapAsync(0).ConfigureAwait(false))!;
    }

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
            (_, _) => Task.CompletedTask,
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

    /// <summary>
    /// Tests that the phase completes once the boss health drops below its threshold.
    /// </summary>
    [Test]
    public async Task NightmareRunner_CompletesPhase_WithSummon()
    {
        var result = await this.RunSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.Completed, Is.True);
    }

    /// <summary>
    /// Tests that the nightmare runner summons the configured wave once the
    /// boss health drops below its threshold.
    /// </summary>
    [Test]
    public async Task NightmareRunner_SummonsConfiguredWave_OnHealthPhaseThreshold()
    {
        var result = await this.RunSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.Waves, Is.EqualTo(new byte[] { 9 }));
    }

    /// <summary>
    /// Tests that the live monster count is refreshed after the summon: once at
    /// phase start, once after the summon.
    /// </summary>
    [Test]
    public async Task NightmareRunner_RefreshesLiveCount_AfterSummon()
    {
        var result = await this.RunSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.LiveCountShows, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the phase completes without a configured summon wave.
    /// </summary>
    [Test]
    public async Task NightmareRunner_CompletesPhase_WithoutSummonWave()
    {
        var result = await this.RunNoSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.Completed, Is.True);
    }

    /// <summary>
    /// Tests that the teleport still happened without a summon wave: start message
    /// plus teleport message prove it.
    /// </summary>
    [Test]
    public async Task NightmareRunner_Teleports_WithoutSummonWave()
    {
        var result = await this.RunNoSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.Messages, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that no wave spawns without a configured summon wave number.
    /// </summary>
    [Test]
    public async Task NightmareRunner_SpawnsNoWave_WithoutWaveNumber()
    {
        var result = await this.RunNoSummonScenarioAsync().ConfigureAwait(false);

        Assert.That(result.Waves, Is.Empty);
    }

    private async Task<(bool Completed, List<byte> Waves, int LiveCountShows)> RunSummonScenarioAsync()
    {
        var monster = CreateNightmare(this._map, this._gameContext);
        monster.Health = 700; // 70% of 1000, below the 75% threshold.

        var phase = new KanturuPhaseDefinition
        {
            Nightmare = new KanturuNightmareDefinition
            {
                HpPhases =
                [
                    new KanturuNightmareHpPhase
                    {
                        HealthPercentage = 75,
                        TeleportTargetX = 100,
                        TeleportTargetY = 100,
                        SummonWaveNumber = 9,
                    },
                ],
                HealthCheckInterval = TimeSpan.FromMilliseconds(10),
                TeleportDelay = TimeSpan.Zero,
                SpecialAttackInterval = TimeSpan.Zero,
            },
        };

        var waves = new List<byte>();
        var summoned = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task SpawnWave(byte wave, CancellationToken _)
        {
            waves.Add(wave);
            summoned.TrySetResult();
            return Task.CompletedTask;
        }

        var liveCountShows = 0;
        async Task<bool> WaitForPhaseEnd(KanturuPhaseDefinition _, CancellationToken ct)
        {
            await summoned.Task.WaitAsync(ct).ConfigureAwait(false);
            return true;
        }

        var runner = new KanturuNightmareRunner(
            (_, _) => Task.CompletedTask,
            (_, _) => ValueTask.CompletedTask,
            _ => ValueTask.CompletedTask,
            () =>
            {
                liveCountShows++;
                return ValueTask.CompletedTask;
            },
            WaitForPhaseEnd,
            (_, _) => Task.CompletedTask,
            (_, _) => Task.FromResult<Monster?>(monster),
            _ => ValueTask.CompletedTask,
            SpawnWave,
            NullLogger.Instance);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var completed = await runner.RunAsync(phase, timeout.Token).ConfigureAwait(false);
        return (completed, waves, liveCountShows);
    }

    private async Task<(bool Completed, int Messages, List<byte> Waves)> RunNoSummonScenarioAsync()
    {
        var monster = CreateNightmare(this._map, this._gameContext);
        monster.Health = 700; // 70% of 1000, below the 75% threshold.

        var phase = new KanturuPhaseDefinition
        {
            Nightmare = new KanturuNightmareDefinition
            {
                HpPhases =
                [
                    new KanturuNightmareHpPhase
                    {
                        HealthPercentage = 75,
                        TeleportTargetX = 100,
                        TeleportTargetY = 100,
                    },
                ],
                HealthCheckInterval = TimeSpan.FromMilliseconds(10),
                TeleportDelay = TimeSpan.Zero,
                SpecialAttackInterval = TimeSpan.Zero,
            },
        };

        var messages = 0;
        var waves = new List<byte>();
        async Task<bool> WaitForPhaseEnd(KanturuPhaseDefinition _, CancellationToken ct)
        {
            await Task.Delay(500, ct).ConfigureAwait(false);
            return true;
        }

        var runner = new KanturuNightmareRunner(
            (_, _) => Task.CompletedTask,
            (_, _) => ValueTask.CompletedTask,
            _ =>
            {
                messages++;
                return ValueTask.CompletedTask;
            },
            () => ValueTask.CompletedTask,
            WaitForPhaseEnd,
            (_, _) => Task.CompletedTask,
            (_, _) => Task.FromResult<Monster?>(monster),
            _ => ValueTask.CompletedTask,
            (byte wave, CancellationToken _) =>
            {
                waves.Add(wave);
                return Task.CompletedTask;
            },
            NullLogger.Instance);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var completed = await runner.RunAsync(phase, timeout.Token).ConfigureAwait(false);
        return (completed, messages, waves);
    }

    private static Monster CreateNightmare(GameMap map, IGameContext gameContext)
    {
        var definition = new MonsterDefinition { ObjectKind = NpcObjectKind.Monster };
        definition.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 1000 });
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };
        var monster = new Monster(
            spawnArea,
            definition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
        monster.Initialize();
        return monster;
    }
}
