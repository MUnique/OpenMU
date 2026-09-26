// <copyright file="KanturuContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The context of a Kanturu Refinery Tower event game.
/// </summary>
/// <remarks>
/// The run of the event is described by a <see cref="KanturuEventDefinition"/>, which is
/// configured at the <see cref="KanturuStartPlugIn"/>. This context just executes its
/// <see cref="KanturuEventDefinition.Phases"/> one after another, so the event can be adapted
/// without code changes. <see cref="KanturuEventDefinition.CreateDefault"/> describes the
/// season 6 event: three waves of monsters which each end with a fight against the hands of
/// Maya, then the transition into the Nightmare zone and the boss fight, and finally the Tower
/// of Refinement.
/// Players who die are respawned at Kanturu Relics, which is handled by the safezone map of
/// the event map.
/// A game created while the tower window is still open skips the phases and only hosts
/// the Tower of Refinement (see <see cref="TowerMode"/>).
/// </remarks>
public sealed class KanturuContext : MiniGameContext
{
    /// <summary>
    /// The detail state which makes the clients hide the in-map HUD. It's the "none" value of
    /// all of the detail state enums. Shared with the gateway dialog, which shows the
    /// standby state instead while refills are accepted.
    /// </summary>
    internal const byte HudHiddenDetailState = 0;

    /// <summary>
    /// The map center, from which the range of the alive-monster query covers the whole map.
    /// Shared with the Kanturu collaborators which query all monsters of the map.
    /// </summary>
    internal static readonly Point MapCenter = new(127, 127);

    /// <summary>
    /// How long a tower without any visitor stays alive before it ends itself.
    /// </summary>
    private static readonly TimeSpan TowerIdleGracePeriod = TimeSpan.FromMinutes(5);

    private readonly IMapInitializer _mapInitializer;
    private readonly IGameContext _gameContext;
    private readonly KanturuEventDefinition _definition;
    private readonly MonsterDefinition? _nightmareMonsterDefinition;
    private readonly KanturuKillTracker _killTracker = new();
    private readonly KanturuWaveTimer _waveTimer = new();
    private readonly KanturuMayaWideAttacker _mayaAttacker;
    private readonly Dictionary<KanturuPhaseKind, IKanturuPhaseRunner> _phaseRunners;
    private readonly TimeSpan _towerOpenDuration;

    // Interlocked flags (0 = false, 1 = true) - avoids volatile by using explicit atomic reads/writes.
    private int _isVictory;
    private int _barrierOpened;
    private int _mayaAttacksPaused;
    private int _inStandby;
    private int _everEntered;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public KanturuContext(
        MiniGameMapKey key,
        MiniGameDefinition definition,
        IGameContext gameContext,
        IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._mapInitializer = mapInitializer;
        this._gameContext = gameContext;

        // The definition is resolved once, so that a configuration change doesn't affect a
        // running event.
        this._definition = GetEventDefinition(gameContext, out this._towerOpenDuration);
        this.TowerEntryPoint = KanturuTowerEntry.GetTowerEntryPoint(this._definition);
        if (KanturuTowerWindow.GetOpenUntilUtc(gameContext) is { } openUntil && openUntil > DateTime.UtcNow)
        {
            // A game created while the tower window is open hosts the tower: the map
            // must replicate the victory state (open barrier) from the start, because
            // players warp in before the game starts. The state starts as Tower, so
            // no Maya battle transition is ever observable on a fresh tower map.
            this.TowerMode = true;
            this.ApplyBarrierTerrain();
            this.CurrentKanturuState = KanturuState.Tower;
            this.CurrentKanturuDetailState = (byte)KanturuTowerDetailState.Revitalization;
            this.Logger.LogInformation("Kanturu: hosting tower until {TowerOpenUntilUtc}.", openUntil);
        }

        this._nightmareMonsterDefinition = this._definition.Phases
            .FirstOrDefault(phase => phase.Kind == KanturuPhaseKind.Nightmare)?.Nightmare?.Monster;

        // One runner per phase kind; the game loop dispatches through this map.
        this._mayaAttacker = new KanturuMayaWideAttacker(this.Map, action => this.ForEachPlayerAsync(action), this.Logger);
        var waveRunner = new KanturuMonsterWaveRunner(
            this.BeginPhaseAsync,
            this.AnnouncePhaseAsync,
            this.WaitForPhaseEndAsync,
            this.RunStandbyAsync);
        var transitionRunner = new KanturuTransitionRunner(
            this.ShowKanturuStateAsync,
            action => this.ForEachPlayerAsync(action),
            this._killTracker.ClearPhase);
        var nightmareRunner = new KanturuNightmareRunner(
            this.BeginPhaseAsync,
            this.ShowKanturuStateAsync,
            this.ShowGoldenMessageIfConfiguredAsync,
            this.ShowLiveMinionCountAsync,
            this.WaitForPhaseEndAsync,
            this.RunStandbyAsync,
            this.WaitForNightmareSpawnAsync,
            action => this.ForEachPlayerAsync(action),
            this.SpawnWaveAsync,
            this.Logger);
        this._phaseRunners = new Dictionary<KanturuPhaseKind, IKanturuPhaseRunner>
        {
            [waveRunner.Kind] = waveRunner,
            [transitionRunner.Kind] = transitionRunner,
            [nightmareRunner.Kind] = nightmareRunner,
        };
    }

    /// <summary>
    /// Gets the current Kanturu main state code (the last state sent via 0xD1/0x03).
    /// The Gateway NPC plugin reads this to populate the 0xD1/0x00 StateInfo dialog
    /// while the event is in progress.
    /// </summary>
    public KanturuState CurrentKanturuState { get; private set; } = KanturuState.MayaBattle;

    /// <summary>
    /// Gets the current Kanturu detail state code (the last detailState sent via 0xD1/0x03).
    /// </summary>
    public byte CurrentKanturuDetailState { get; private set; }

    /// <summary>
    /// Gets where tower entrants arrive: the Nightmare zone entry. It's <c>null</c>
    /// when the event definition configures no transition.
    /// </summary>
    public Point? TowerEntryPoint { get; }

    /// <summary>
    /// Gets a value indicating whether this game only hosts the Tower of
    /// Refinement within an already open tower window, without running the event
    /// phases. It's determined at creation from the open tower window.
    /// </summary>
    public bool TowerMode { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the map entry requirements are skipped.
    /// Tower visitors don't need the event entry requirements anymore.
    /// </summary>
    internal override bool SkipMapEntryRequirements => this.TowerMode || this.CurrentKanturuState == KanturuState.Tower;

    /// <summary>
    /// Gets a value indicating whether players may rejoin the open Tower of Refinement
    /// or refill the event during an inter-wave standby. Fights can never be joined
    /// mid-event.
    /// </summary>
    protected override bool AllowEnterWhilePlaying => this.CurrentKanturuState == KanturuState.Tower
        || (this.CurrentKanturuState == KanturuState.MayaBattle && Volatile.Read(ref this._inStandby) != 0);

    /// <summary>
    /// Gets the countdown duration after the entrance closed; the entrance closes one
    /// minute before the game starts. Tower games start immediately.
    /// </summary>
    protected override TimeSpan CountdownDuration => this.TowerMode ? TimeSpan.Zero : TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets the minimum duration of the entrance phase; tower games don't need a lobby.
    /// </summary>
    protected override TimeSpan MinimumEnterDuration => this.TowerMode ? TimeSpan.Zero : base.MinimumEnterDuration;

    /// <summary>
    /// Gets the minimum player count to start the game. A reopened tower starts empty;
    /// visitors join an already running tower.
    /// </summary>
    protected override int MinimumPlayerCount => this.TowerMode ? 0 : 1;

    /// <summary>
    /// Tower entrants spawn at the tower entry instead of the event start.
    /// </summary>
    /// <param name="player">The player which enters.</param>
    /// <returns>The tower entry point, or <c>null</c> to keep the warp target.</returns>
    internal override Point? GetEntrySpawnPosition(Player player)
    {
        if (this.TowerMode || this.CurrentKanturuState == KanturuState.Tower)
        {
            return this.TowerEntryPoint;
        }

        return null;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        _ = Task.Run(() => this.RunRequiredItemWearAsync(this.GameEndedToken), this.GameEndedToken);

        // The flag alone is not enough: it may have been set on a fresh event lobby
        // by an enter racing a scheduler start, which clears the window first.
        if (this.TowerMode
            && KanturuTowerWindow.GetOpenUntilUtc(this._gameContext) is { } until
            && until > DateTime.UtcNow)
        {
            Interlocked.Exchange(ref this._isVictory, 1);
            await this.AnnounceTowerMapAsync().ConfigureAwait(false);
            _ = Task.Run(() => this.RunTowerModeAsync(this.GameEndedToken), this.GameEndedToken);
            return;
        }

        // Maya rises from the depths when the battle begins.
        if (this._definition.IntroSpawnWaveNumber is { } introWave)
        {
            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, introWave).ConfigureAwait(false);
        }

        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.IntroMessageKey).ConfigureAwait(false);

        _ = Task.Run(() => this.RunKanturuGameLoopAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc/>
    protected override void OnMonsterDied(object? sender, DeathInformation e)
    {
        try
        {
            base.OnMonsterDied(sender, e);

            if (sender is not Monster monster)
            {
                return;
            }

            var killedDefinition = monster.Definition;
            var result = this._killTracker.RegisterKill(killedDefinition);
            if (this.IsUnexpectedNightmareDeath(killedDefinition, result))
            {
                this.Logger.LogWarning(
                    "Kanturu: Nightmare died during phase {Phase}, where it isn't expected. The barrier is NOT opened.",
                    result.Phase?.Name ?? "<none>");
            }

            if (!result.Counted && !result.IsNightmarePhase)
            {
                return;
            }

            // The handler itself stays synchronous, like the invasion death broadcast:
            // the client notifications run fire-and-forget on the thread pool instead
            // of making this an async void method.
            _ = Task.Run(() => this.HandleMonsterDiedAsync(result.Phase, result));
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in OnMonsterDied.");
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnObjectAddedToMapAsync((GameMap Map, ILocateable Object) args)
    {
        await base.OnObjectAddedToMapAsync(args).ConfigureAwait(false);

        if (args.Object is Player)
        {
            Interlocked.Exchange(ref this._everEntered, 1);
        }

        // Nothing else broadcasts during the tower, so players joining it late would
        // miss the tower state. Both broadcasts are idempotent for the other players.
        // Their position is already correct: tower entrants spawn at the tower entry
        // through GetEntrySpawnPosition.
        if (args.Object is Player && this.CurrentKanturuState == KanturuState.Tower)
        {
            await this.ShowKanturuStateAsync(this.CurrentKanturuState, this.CurrentKanturuDetailState).ConfigureAwait(false);
            await this.ShowLiveMinionCountAsync().ConfigureAwait(false);
            await this.SendBarrierAttributesAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        var isVictory = Volatile.Read(ref this._isVictory) != 0;

        await this.ShowGoldenMessageIfConfiguredAsync(isVictory
            ? this._definition.VictoryMessageKey
            : this._definition.DefeatMessageKey).ConfigureAwait(false);

        // On defeat show the Failure_kantru.tga overlay. On victory the Success_kantru.tga and
        // the tower state are sent from OpenElphisBarrierAsync.
        if (!isVictory)
        {
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                    p.ShowBattleResultAsync(false)).AsTask()).ConfigureAwait(false);
        }

        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    private static KanturuEventDefinition GetEventDefinition(IGameContext gameContext, out TimeSpan towerOpenDuration)
    {
        var startPlugIn = gameContext.PlugInManager
            .GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.Kanturu);
        var configuration = (startPlugIn as ISupportCustomConfiguration<KanturuStartConfiguration>)?.Configuration;
        var definition = configuration?.EventDefinition ?? KanturuEventDefinition.CreateDefault(gameContext.Configuration);
        towerOpenDuration = configuration?.TowerOpenDuration ?? definition.TowerOfRefinementDuration;
        return definition;
    }

    private async Task RunKanturuGameLoopAsync(CancellationToken ct)
    {
        try
        {
            // The intro cinematic pans the camera to Maya and lets her body rise from below.
            // It must be sent first, so the client camera is in position before the first wave.
            await this.ShowKanturuStateAsync(this._definition.IntroState, this._definition.IntroDetailState).ConfigureAwait(false);
            await this.DelayAsync(this._definition.IntroDuration, ct).ConfigureAwait(false);

            // The wide area attacks of Maya are shown until the players leave her battlefield.
            using var mayaAttackCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            if (this._definition.MayaAttackInterval > TimeSpan.Zero)
            {
                _ = Task.Run(
                    () => this._mayaAttacker.RunAsync(
                        this._definition.MayaAttackInterval,
                        () => Volatile.Read(ref this._mayaAttacksPaused) != 0,
                        mayaAttackCts.Token),
                    mayaAttackCts.Token);
            }

            foreach (var phase in this._definition.Phases)
            {
                if (phase.Kind == KanturuPhaseKind.Transition && !mayaAttackCts.IsCancellationRequested)
                {
                    // Maya explodes during the transition, so her attack visuals stop here.
                    await mayaAttackCts.CancelAsync().ConfigureAwait(false);
                }

                this.Logger.LogDebug("Kanturu: starting phase {Phase}.", phase.Name);
                if (!await this.RunPhaseAsync(phase, ct).ConfigureAwait(false))
                {
                    // The wave failed (its time limit expired) - the event is lost.
                    this.FinishEvent();
                    return;
                }
            }

            Interlocked.Exchange(ref this._isVictory, 1);
            this._killTracker.ClearPhase();

            // The fire-and-forget call from OnMonsterDied already opened the barrier; this is
            // a fallback for the case that no boss death was registered. The Interlocked guard
            // in OpenElphisBarrierAsync makes sure that it only executes once.
            await this.OpenElphisBarrierAsync().ConfigureAwait(false);

            await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerConqueredMessageKey).ConfigureAwait(false);
            await this.RunTowerOfRefinementAsync(this._towerOpenDuration, this._definition.TowerClosingWarningOffset, ct).ConfigureAwait(false);

            this.FinishEvent();
        }
        catch (OperationCanceledException)
        {
            // Game ended by timeout or external cancellation - treated as defeat.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in Kanturu game loop.");
        }
    }

    private Task<bool> RunPhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        if (this._phaseRunners.TryGetValue(phase.Kind, out var runner))
        {
            return runner.RunAsync(phase, ct);
        }

        this.Logger.LogWarning("Kanturu: no runner for phase kind {PhaseKind}, running it as a monster wave.", phase.Kind);
        return this._phaseRunners[KanturuPhaseKind.MonsterWave].RunAsync(phase, ct);
    }

    /// <summary>
    /// Waits for the Nightmare boss to spawn, by capturing it from <see cref="GameMap.ObjectAdded"/>.
    /// Infrastructure for <see cref="KanturuNightmareRunner"/>; returns <c>null</c> on timeout.
    /// </summary>
    private async Task<Monster?> WaitForNightmareSpawnAsync(KanturuNightmareDefinition nightmare, CancellationToken ct)
    {
        var nightmareFound = new TaskCompletionSource<Monster>(TaskCreationOptions.RunContinuationsAsynchronously);

        ValueTask OnObjectAddedAsync((GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Monster monster && KanturuMonsterComparer.IsSameMonster(nightmare.Monster, monster.Definition))
            {
                nightmareFound.TrySetResult(monster);
            }

            return ValueTask.CompletedTask;
        }

        this.Map.ObjectAdded += OnObjectAddedAsync;
        try
        {
            return await nightmareFound.Task.WaitAsync(nightmare.SpawnTimeout, ct).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            return null;
        }
        finally
        {
            this.Map.ObjectAdded -= OnObjectAddedAsync;
        }
    }

    private async Task BeginPhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        this._killTracker.BeginPhase(phase);

        await this.ShowKanturuStateAsync(phase.State, phase.DetailState).ConfigureAwait(false);

        // A following phase of a wave shows the remaining shared time instead of a fresh timer.
        // An already expired remainder is not shown; the wait below fails the wave at once.
        if (this._waveTimer.GetEffectiveLimit(phase) is { } timeLimit
            && (timeLimit > TimeSpan.Zero || phase.TimeLimitGroup is null))
        {
            await this.ShowTimeLimitToAllAsync(timeLimit).ConfigureAwait(false);
        }

        await this.DelayAsync(phase.StartDelay, ct).ConfigureAwait(false);

        if (phase.SpawnWaveNumber is { } waveNumber)
        {
            await this.SpawnWaveAsync(waveNumber, ct).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawns a configured monster wave on the event map, e.g. a phase wave or the
    /// Nightmare summons.
    /// </summary>
    /// <param name="waveNumber">The number of the started spawn wave.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task SpawnWaveAsync(byte waveNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, waveNumber).ConfigureAwait(false);
    }

    private async Task AnnouncePhaseAsync(KanturuPhaseDefinition phase)
    {
        // Broadcast the initial monster count, so the HUD shows the correct number from the start.
        await this.ShowMonsterUserCountAsync(phase.KillTarget, this.PlayerCount).ConfigureAwait(false);
        await this.ShowGoldenMessageIfConfiguredAsync(phase.StartMessageKey).ConfigureAwait(false);
    }

    private async Task<bool> WaitForPhaseEndAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        if (phase.Duration is { } duration)
        {
            await this.DelayAsync(duration, ct).ConfigureAwait(false);
            return true;
        }

        var killWait = this._killTracker.PhaseCompleted.WaitAsync(ct);
        var timeLimit = this._waveTimer.GetEffectiveLimit(phase);
        if (timeLimit is not { } limit || (limit <= TimeSpan.Zero && phase.TimeLimitGroup is null))
        {
            await killWait.ConfigureAwait(false);
            return true;
        }

        if (limit <= TimeSpan.Zero)
        {
            // The shared wave clock already expired during an earlier phase of the group.
            return false;
        }

        // A wave fails when its time limit expires before the kill target is reached.
        // A skipped wait (game master) passes the wave instead. When both finish at
        // once, the kills win: failing an actually completed wave would be unfair.
        // Both racers are observed: the loser would otherwise surface an unobserved
        // OperationCanceledException when the game ends.
        var timeoutWait = this.DelayWithSkipAsync(limit, ct);
        Observe(killWait);
        Observe(timeoutWait);
        var winner = await Task.WhenAny(killWait, timeoutWait).ConfigureAwait(false);
        if (winner == killWait)
        {
            await killWait.ConfigureAwait(false);
            return true;
        }

        return await timeoutWait.ConfigureAwait(false) || killWait.IsCompletedSuccessfully;

        static void Observe(Task task) => _ = task.ContinueWith(
            static faulted => _ = faulted.Exception,
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Runs the standby time after a phase, during which the in-map HUD is hidden and the wide
    /// area attacks of Maya are paused, so that she stays visually idle.
    /// </summary>
    private async Task RunStandbyAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(phase.CompletedMessageKey) && phase.StandbyDuration <= TimeSpan.Zero)
        {
            return;
        }

        Interlocked.Exchange(ref this._mayaAttacksPaused, 1);
        Interlocked.Exchange(ref this._inStandby, 1);
        try
        {
            await this.ShowKanturuStateAsync(phase.State, HudHiddenDetailState).ConfigureAwait(false);
            await this.ShowGoldenMessageIfConfiguredAsync(phase.CompletedMessageKey).ConfigureAwait(false);
            await this.DelayAsync(phase.StandbyDuration, ct).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref this._mayaAttacksPaused, 0);
            Interlocked.Exchange(ref this._inStandby, 0);
        }
    }

    /// <summary>
    /// Broadcasts the kill count change of a monster death and opens the Elphis barrier
    /// when the Nightmare boss died. Runs fire-and-forget from <see cref="OnMonsterDied"/>.
    /// </summary>
    /// <param name="phase">The phase which was current when the monster died.</param>
    /// <param name="result">The outcome of the kill registration.</param>
    private async Task HandleMonsterDiedAsync(KanturuPhaseDefinition? phase, KanturuKillResult result)
    {
        try
        {
            if (result.IsNightmarePhase)
            {
                await this.ShowLiveMinionCountAsync().ConfigureAwait(false);
            }
            else if (result.Counted && phase is not null)
            {
                await this.ShowMonsterUserCountAsync(Math.Max(0, phase.KillTarget - result.KillCount), this.PlayerCount).ConfigureAwait(false);
            }
            else
            {
                // Uncounted kills outside the Nightmare phase change nothing.
            }

            if (result.NightmareBossKilled)
            {
                // Open the barrier immediately from the death event. Don't wait for the game
                // loop - it may be interrupted by a cancellation of the GameEndedToken before
                // it reaches OpenElphisBarrierAsync.
                await this.OpenElphisBarrierAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in OnMonsterDied.");
        }
    }

    private bool IsUnexpectedNightmareDeath(MonsterDefinition? killedDefinition, KanturuKillResult result)
    {
        return !result.Counted && KanturuMonsterComparer.IsSameMonster(this._nightmareMonsterDefinition, killedDefinition);
    }

    /// <summary>
    /// Opens the barrier to the Elphis area by removing the
    /// <see cref="TerrainAttributeType.NoGround"/> attribute of the configured areas, both on
    /// the server walk map and at the clients.
    /// </summary>
    /// <remarks>
    /// It's guarded by <see cref="_barrierOpened"/>, so it executes at most once per game, even
    /// when it's called concurrently from <see cref="OnMonsterDied"/> and the game loop.
    /// </remarks>
    private async ValueTask OpenElphisBarrierAsync()
    {
        if (Interlocked.CompareExchange(ref this._barrierOpened, 1, 0) != 0)
        {
            return;
        }

        this.Logger.LogInformation("Kanturu: opening the barrier to the Elphis area.");

        // Persist the open window first, so it survives a server restart even if the
        // game ends before the tower closes.
        var towerOpenUntil = DateTime.UtcNow + this._towerOpenDuration;
        await KanturuTowerWindow.SetOpenUntilUtcAsync(this._gameContext, towerOpenUntil, this.Logger).ConfigureAwait(false);
        this.Logger.LogInformation("Kanturu: tower open until {TowerOpenUntilUtc}.", towerOpenUntil);

        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.BarrierOpeningMessageKey).ConfigureAwait(false);
        await this.ShowMonsterUserCountAsync(0, this.PlayerCount).ConfigureAwait(false);

        // Victory cinematic which moves the camera out of the Nightmare zone.
        await this.ShowKanturuStateAsync(KanturuState.NightmareBattle, (byte)KanturuNightmareDetailState.End).ConfigureAwait(false);
        await Task.Delay(this._definition.VictoryCinematicDuration).ConfigureAwait(false);

        // The success overlay requires the clients to still be in the Nightmare state, so it's
        // sent before the state changes to the tower.
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowBattleResultAsync(true)).AsTask()).ConfigureAwait(false);

        // The tower state makes the clients load the terrain file of the opened barrier,
        // switch to the tower music and play the success sound.
        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Revitalization).ConfigureAwait(false);

        // Update the server walk map, so the path finder and the movement checks treat the
        // formerly blocked cells as passable. The barrier rect mixes walls and holes,
        // so it explicitly opts into opening the whole tile instead of only removing
        // the configured attribute.
        this.ApplyBarrierTerrain();

        await this.SendBarrierAttributesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the terrain attribute change as a fallback: if the terrain file of
    /// the opened barrier is missing at a client, this packet still clears the attribute.
    /// </summary>
    private async ValueTask SendBarrierAttributesAsync()
    {
        var areas = this._definition.BarrierAreas
            .Select(area => (area.StartX, area.StartY, area.EndX, area.EndY))
            .ToList();
        if (areas.Count > 0)
        {
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p =>
                    p.ChangeAttributesAsync(TerrainAttributeType.NoGround, setAttribute: false, areas))
                .AsTask()).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the barrier attribute from the server walk map, so the path finder and
    /// the movement checks treat the formerly blocked cells as passable.
    /// </summary>
    private void ApplyBarrierTerrain()
    {
        var terrain = this.Map.Terrain;
        foreach (var (x, y) in KanturuBarrierAreaHelper.EnumerateCells(this._definition.BarrierAreas))
        {
            terrain.ApplyTerrainAttribute(x, y, TerrainAttributeType.NoGround, false, openArea: true);
        }
    }

    /// <summary>
    /// Announces an already-open tower on a fresh map, without battle overlay or cinematic.
    /// The barrier itself is opened at creation, so players warping in before the game
    /// starts never face a closed barrier.
    /// </summary>
    private async ValueTask AnnounceTowerMapAsync()
    {
        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Revitalization).ConfigureAwait(false);
        await this.SendBarrierAttributesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Hosts the tower for the remaining open window, then finishes the event.
    /// A tower nobody enters is ended after a grace period instead of idling the
    /// whole window: re-entry recreates it while the window lasts.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    private async Task RunTowerModeAsync(CancellationToken ct)
    {
        try
        {
            if (Volatile.Read(ref this._everEntered) == 0)
            {
                await this.DelayTowerPhaseAsync(TowerIdleGracePeriod, ct).ConfigureAwait(false);
                if (Volatile.Read(ref this._everEntered) == 0)
                {
                    this.FinishEvent();
                    return;
                }
            }

            var remaining = TimeSpan.Zero;
            if (KanturuTowerWindow.GetOpenUntilUtc(this._gameContext) is { } until)
            {
                remaining = until - DateTime.UtcNow;
                if (remaining < TimeSpan.Zero)
                {
                    remaining = TimeSpan.Zero;
                }
            }

            await this.RunTowerOfRefinementAsync(remaining, this._definition.TowerClosingWarningOffset, ct).ConfigureAwait(false);

            this.FinishEvent();
        }
        catch (OperationCanceledException)
        {
            // Game ended externally - treated as closed tower.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in Kanturu tower mode.");
        }
    }

    /// <summary>
    /// Keeps the map open as the Tower of Refinement after the boss has been defeated.
    /// </summary>
    /// <param name="duration">How long the tower stays open.</param>
    /// <param name="warningOffset">How long before the end the closing warning is shown.</param>
    /// <param name="ct">The cancellation token.</param>
    private async Task RunTowerOfRefinementAsync(TimeSpan duration, TimeSpan warningOffset, CancellationToken ct)
    {
        // An empty tower ends with the game: re-entry recreates it while the window lasts.
        if (duration > warningOffset)
        {
            await this.DelayTowerPhaseAsync(duration - warningOffset, ct).ConfigureAwait(false);
            await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerClosingWarningMessageKey).ConfigureAwait(false);
            await this.DelayTowerPhaseAsync(warningOffset, ct).ConfigureAwait(false);
        }
        else
        {
            await this.DelayTowerPhaseAsync(duration, ct).ConfigureAwait(false);
        }

        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Notify).ConfigureAwait(false);
        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerClosedMessageKey).ConfigureAwait(false);
        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Close).ConfigureAwait(false);

        // The window is consumed; a new one starts with the next victory.
        await KanturuTowerWindow.SetOpenUntilUtcAsync(this._gameContext, null, this.Logger).ConfigureAwait(false);
    }

    private async Task DelayTowerPhaseAsync(TimeSpan duration, CancellationToken ct)
    {
        if (duration <= TimeSpan.Zero)
        {
            return;
        }

        await Task.Delay(duration, ct).ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Wears down the items which are required by the event map, for example the Moonstone
    /// Pendant, and moves the players out of the event when their item is destroyed.
    /// </summary>
    private async Task RunRequiredItemWearAsync(CancellationToken ct)
    {
        if (this._definition.RequiredItemDurabilityLossInterval <= TimeSpan.Zero
            || this._definition.RequiredItemDurabilityLoss <= 0)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(this._definition.RequiredItemDurabilityLossInterval, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                await this.WearRequiredItemsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Unexpected error when wearing down the required items.");
            }
        }
    }

    private async Task WearRequiredItemsAsync()
    {
        if (this.Map.Definition.MapRequirements is not { Count: > 0 } requirements)
        {
            return;
        }

        // The players whose item got destroyed can't be moved out inside ForEachPlayerAsync:
        // it holds a reader lock which the removal from the map would wait for as a writer.
        var destroyedItems = new ConcurrentBag<(Player Player, Item Item)>();

        await this.ForEachPlayerAsync(player => this.WearPlayerRequiredItemsAsync(player, requirements, destroyedItems)).ConfigureAwait(false);

        await this.RemovePlayersWithDestroyedItemsAsync(destroyedItems).ConfigureAwait(false);
    }

    private async Task WearPlayerRequiredItemsAsync(Player player, ICollection<AttributeRequirement> requirements, ConcurrentBag<(Player Player, Item Item)> destroyedItems)
    {
        foreach (var item in KanturuRequiredItemHelper.GetRequiredItems(player, requirements))
        {
            if (item.DecreaseDurability(this._definition.RequiredItemDurabilityLoss))
            {
                await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p =>
                    p.ItemDurabilityChangedAsync(item, false)).ConfigureAwait(false);
            }

            if (item.Durability <= 0)
            {
                destroyedItems.Add((player, item));
            }
        }
    }

    private async Task RemovePlayersWithDestroyedItemsAsync(ConcurrentBag<(Player Player, Item Item)> destroyedItems)
    {
        foreach (var (player, item) in destroyedItems)
        {
            try
            {
                this.Logger.LogInformation(
                    "Kanturu: the {Item} of {Player} has been destroyed, so it leaves the event.",
                    item.Definition?.Name,
                    player);

                await player.DestroyInventoryItemAsync(item).ConfigureAwait(false);
                if (this._definition.RequiredItemDestroyedMessageKey is { Length: > 0 } messageKey)
                {
                    await player.ShowLocalizedBlueMessageAsync(messageKey, item.Definition?.Name).ConfigureAwait(false);
                }

                if (player.IsActive() && player.CurrentMap is not null)
                {
                    // Moving the player off the map also removes it from this mini game.
                    await player.WarpToSafezoneAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Unexpected error when moving {Player} out of the event.", player);
            }
        }
    }

    private Task DelayAsync(TimeSpan duration, CancellationToken ct)
    {
        return this.DelayWithSkipAsync(duration, ct);
    }

    private async ValueTask ShowGoldenMessageIfConfiguredAsync(string? messageKey)
    {
        if (!string.IsNullOrEmpty(messageKey))
        {
            await this.ShowGoldenMessageAsync(messageKey).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Broadcasts the state change packet to all players of the map. It also updates
    /// <see cref="CurrentKanturuState"/> and <see cref="CurrentKanturuDetailState"/>, so the
    /// gateway NPC plug-in can report the current phase of the event.
    /// </summary>
    private ValueTask ShowKanturuStateAsync(KanturuState state, byte detailState)
    {
        this.CurrentKanturuState = state;
        this.CurrentKanturuDetailState = detailState;
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowStateChangeAsync(state, detailState)).AsTask());
    }

    private ValueTask ShowMonsterUserCountAsync(int monsterCount, int userCount)
    {
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowMonsterUserCountAsync(monsterCount, userCount)).AsTask());
    }

    /// <summary>
    /// Broadcasts the currently alive minion count during the Nightmare fight
    /// instead of counting down a kill target. The Nightmare boss
    /// itself is not counted: when all minions are dead while it is still alive, the
    /// HUD shows 0.
    /// </summary>
    private async ValueTask ShowLiveMinionCountAsync()
    {
        // The range covers the whole map from its center.
        var aliveCount = this.Map.GetAttackablesInRange(MapCenter, byte.MaxValue)
            .OfType<Monster>()
            .Count(monster => monster.IsAlive && !this.IsNightmare(monster));
        await this.ShowMonsterUserCountAsync(aliveCount, this.PlayerCount).ConfigureAwait(false);
    }

    private bool IsNightmare(Monster monster)
    {
        return KanturuMonsterComparer.IsSameMonster(this._nightmareMonsterDefinition, monster.Definition);
    }

    private ValueTask ShowTimeLimitToAllAsync(TimeSpan timeLimit)
    {
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowTimeLimitAsync(timeLimit)).AsTask());
    }
}
