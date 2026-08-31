// <copyright file="KanturuContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views.World;
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
/// without code changes. <see cref="KanturuEventDefinition.Default"/> describes the original
/// season 6 event: three waves of monsters which each end with a fight against the hands of
/// Maya, then the transition into the Nightmare zone and the boss fight, and finally the Tower
/// of Refinement.
/// Players who die are respawned at Kanturu Relics, which is handled by the safezone map of
/// the event map.
/// </remarks>
public sealed class KanturuContext : MiniGameContext
{
    /// <summary>
    /// The detail state which makes the clients hide the in-map HUD. It's the "none" value of
    /// all of the detail state enums.
    /// </summary>
    private const byte HudHiddenDetailState = 0;

    private readonly IMapInitializer _mapInitializer;
    private readonly KanturuEventDefinition _definition;
    private readonly short? _nightmareMonsterNumber;

    private KanturuPhaseDefinition? _currentPhase;
    private int _waveKillCount;
    private TaskCompletionSource _phaseComplete = new(TaskCreationOptions.RunContinuationsAsynchronously);

    // Interlocked flags (0 = false, 1 = true) - avoids volatile by using explicit atomic reads/writes.
    private int _isVictory;
    private int _barrierOpened;
    private int _nightmareTeleporting;
    private int _mayaAttacksPaused;

    // Nightmare health phase tracking
    private Monster? _nightmareMonster;
    private int _nightmarePhaseIndex;

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

        // The definition is resolved once, so that a configuration change doesn't affect a
        // running event.
        this._definition = GetEventDefinition(gameContext);
        this._nightmareMonsterNumber = this._definition.Phases
            .FirstOrDefault(phase => phase.Kind == KanturuPhaseKind.Nightmare)?.Nightmare?.MonsterNumber;
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

    /// <inheritdoc/>
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        // Maya rises from the depths when the battle begins.
        if (this._definition.IntroSpawnWaveNumber is { } introWave)
        {
            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, introWave).ConfigureAwait(false);
        }

        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.IntroMessageKey).ConfigureAwait(false);

        _ = Task.Run(() => this.RunKanturuGameLoopAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc/>
#pragma warning disable VSTHRD100 // Avoid async void methods
    protected override async void OnMonsterDied(object? sender, DeathInformation e)
#pragma warning restore VSTHRD100
    {
        try
        {
            base.OnMonsterDied(sender, e);

            if (sender is not Monster monster)
            {
                return;
            }

            var monsterNumber = (short)monster.Definition.Number;
            var phase = this._currentPhase;
            if (phase is null || !phase.CountedMonsterNumbers.Contains(monsterNumber))
            {
                if (monsterNumber == this._nightmareMonsterNumber)
                {
                    this.Logger.LogWarning(
                        "Kanturu: Nightmare died during phase {Phase}, where it isn't expected. The barrier is NOT opened.",
                        phase?.Name ?? "<none>");
                }

                return;
            }

            var killed = Interlocked.Increment(ref this._waveKillCount);
            await this.ShowMonsterUserCountAsync(Math.Max(0, phase.KillTarget - killed), this.PlayerCount).ConfigureAwait(false);

            if (phase.Kind == KanturuPhaseKind.Nightmare && monsterNumber == phase.Nightmare?.MonsterNumber)
            {
                // Open the barrier immediately from the death event. Don't wait for the game
                // loop - it may be interrupted by a cancellation of the GameEndedToken before
                // it reaches OpenElphisBarrierAsync.
                await this.OpenElphisBarrierAsync().ConfigureAwait(false);
            }

            if (killed >= phase.KillTarget)
            {
                this._phaseComplete.TrySetResult();
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in OnMonsterDied.");
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

    private static KanturuEventDefinition GetEventDefinition(IGameContext gameContext)
    {
        var startPlugIn = gameContext.PlugInManager
            .GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.Kanturu);
        if (startPlugIn is ISupportCustomConfiguration<KanturuStartConfiguration> { Configuration.EventDefinition: { } definition })
        {
            return definition;
        }

        return KanturuEventDefinition.Default;
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
                _ = Task.Run(() => this.RunMayaWideAreaAttacksAsync(mayaAttackCts.Token), mayaAttackCts.Token);
            }

            foreach (var phase in this._definition.Phases)
            {
                if (phase.Kind == KanturuPhaseKind.Transition && !mayaAttackCts.IsCancellationRequested)
                {
                    // Maya explodes during the transition, so her attack visuals stop here.
                    await mayaAttackCts.CancelAsync().ConfigureAwait(false);
                }

                this.Logger.LogDebug("Kanturu: starting phase {Phase}.", phase.Name);
                await this.RunPhaseAsync(phase, ct).ConfigureAwait(false);
            }

            Interlocked.Exchange(ref this._isVictory, 1);
            this._currentPhase = null;

            // The fire-and-forget call from OnMonsterDied already opened the barrier; this is
            // a fallback for the case that no boss death was registered. The Interlocked guard
            // in OpenElphisBarrierAsync makes sure that it only executes once.
            await this.OpenElphisBarrierAsync().ConfigureAwait(false);

            await this.RunTowerOfRefinementAsync(ct).ConfigureAwait(false);

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

    private Task RunPhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        return phase.Kind switch
        {
            KanturuPhaseKind.Transition => this.RunTransitionPhaseAsync(phase, ct),
            KanturuPhaseKind.Nightmare => this.RunNightmarePhaseAsync(phase, ct),
            _ => this.RunMonsterWavePhaseAsync(phase, ct),
        };
    }

    private async Task RunMonsterWavePhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        await this.BeginPhaseAsync(phase, ct).ConfigureAwait(false);
        await this.AnnouncePhaseAsync(phase).ConfigureAwait(false);
        await this.WaitForPhaseEndAsync(phase, ct).ConfigureAwait(false);
        await this.RunStandbyAsync(phase, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs the transition into the Nightmare zone.
    /// </summary>
    /// <remarks>
    /// The detail state of the phase (<see cref="KanturuMayaDetailState.EndCycleMaya3"/>)
    /// triggers the full cinematic on the client: the camera flies to the Maya room, her body
    /// plays its explosion animation and then the hero falls through the floor. Only after
    /// that the players are moved, so the movement isn't visible during the animation.
    /// </remarks>
    private async Task RunTransitionPhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        var transition = phase.Transition ?? new KanturuTransitionDefinition();
        this._currentPhase = null;

        await this.ShowKanturuStateAsync(phase.State, phase.DetailState).ConfigureAwait(false);

        // The cinematic is never cancelled in the middle, so it uses no cancellation token.
        await Task.Delay(transition.CinematicDuration).ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();

        // It's the same map, so a move is sufficient.
        var entryPoint = new Point(transition.EntryPointX, transition.EntryPointY);
        await this.ForEachPlayerAsync(player => player.MoveAsync(entryPoint).AsTask()).ConfigureAwait(false);

        // The warp animation has to be played after the move, so it's rendered at the entry
        // point and not at the Maya battlefield. It also locks the player input briefly,
        // which prevents movement and attacks during the scene transition.
        await Task.Delay(transition.WarpAnimationDelay).ConfigureAwait(false);
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IMapChangePlugIn>(p =>
                p.MapChangeFailedAsync()).AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs the boss fight. The boss teleports and recovers its health at the configured
    /// <see cref="KanturuNightmareDefinition.HpPhases"/>.
    /// </summary>
    private async Task RunNightmarePhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        var nightmare = phase.Nightmare ?? new KanturuNightmareDefinition();
        this._nightmarePhaseIndex = 0;
        this._nightmareMonster = null;

        // Subscribe to ObjectAdded to capture the boss as soon as it spawns.
        var nightmareFound = new TaskCompletionSource<Monster>(TaskCreationOptions.RunContinuationsAsynchronously);

        ValueTask OnObjectAddedAsync((GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Monster monster && (short)monster.Definition.Number == nightmare.MonsterNumber)
            {
                nightmareFound.TrySetResult(monster);
            }

            return ValueTask.CompletedTask;
        }

        this.Map.ObjectAdded += OnObjectAddedAsync;
        try
        {
            await this.BeginPhaseAsync(phase, ct).ConfigureAwait(false);
            this._nightmareMonster = await nightmareFound.Task
                .WaitAsync(nightmare.SpawnTimeout, ct)
                .ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            this.Logger.LogWarning(
                "Kanturu: the Nightmare monster didn't spawn within {Timeout} - its health phases are disabled.",
                nightmare.SpawnTimeout);
        }
        finally
        {
            this.Map.ObjectAdded -= OnObjectAddedAsync;
        }

        // Switch to the battle state, so the clients show the boss HUD.
        await this.ShowKanturuStateAsync(phase.State, nightmare.BattleDetailState).ConfigureAwait(false);
        await this.AnnouncePhaseAsync(phase).ConfigureAwait(false);

        // Both loops are linked to the same token source, so they stop together as soon as the
        // boss died or the game was cancelled.
        using var bossCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var healthMonitor = Task.Run(() => this.MonitorNightmareHealthAsync(nightmare, bossCts.Token), bossCts.Token);
        var specialAttacks = Task.Run(() => this.RunNightmareSpecialAttacksAsync(nightmare, bossCts.Token), bossCts.Token);

        await this.WaitForPhaseEndAsync(phase, ct).ConfigureAwait(false);

        await bossCts.CancelAsync().ConfigureAwait(false);

        try
        {
            await healthMonitor.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Expected when the phase ends.
        }

        try
        {
            await specialAttacks.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Expected when the phase ends.
        }

        await this.RunStandbyAsync(phase, ct).ConfigureAwait(false);
    }

    private async Task BeginPhaseAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        Interlocked.Exchange(ref this._waveKillCount, 0);
        this._phaseComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        this._currentPhase = phase;

        await this.ShowKanturuStateAsync(phase.State, phase.DetailState).ConfigureAwait(false);

        if (phase.TimeLimit is { } timeLimit)
        {
            await this.ShowTimeLimitToAllAsync(timeLimit).ConfigureAwait(false);
        }

        await this.DelayAsync(phase.StartDelay, ct).ConfigureAwait(false);

        if (phase.SpawnWaveNumber is { } waveNumber)
        {
            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, waveNumber).ConfigureAwait(false);
        }
    }

    private async Task AnnouncePhaseAsync(KanturuPhaseDefinition phase)
    {
        // Broadcast the initial monster count, so the HUD shows the correct number from the start.
        await this.ShowMonsterUserCountAsync(phase.KillTarget, this.PlayerCount).ConfigureAwait(false);
        await this.ShowGoldenMessageIfConfiguredAsync(phase.StartMessageKey).ConfigureAwait(false);
    }

    private async Task WaitForPhaseEndAsync(KanturuPhaseDefinition phase, CancellationToken ct)
    {
        if (phase.Duration is { } duration)
        {
            await this.DelayAsync(duration, ct).ConfigureAwait(false);
        }
        else
        {
            await this._phaseComplete.Task.WaitAsync(ct).ConfigureAwait(false);
        }
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
        try
        {
            await this.ShowKanturuStateAsync(phase.State, HudHiddenDetailState).ConfigureAwait(false);
            await this.ShowGoldenMessageIfConfiguredAsync(phase.CompletedMessageKey).ConfigureAwait(false);
            await this.DelayAsync(phase.StandbyDuration, ct).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref this._mayaAttacksPaused, 0);
        }
    }

    /// <summary>
    /// Polls the health of the boss and triggers the teleport of the next health phase.
    /// </summary>
    private async Task MonitorNightmareHealthAsync(KanturuNightmareDefinition nightmare, CancellationToken ct)
    {
        if (nightmare.HpPhases.Count == 0 || nightmare.HealthCheckInterval <= TimeSpan.Zero)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(nightmare.HealthCheckInterval, ct).ConfigureAwait(false);

            // Don't check the health while a teleport is in progress: the teleport restores the
            // health itself, so reading it in the meantime would give a stale (low) value and
            // trigger the next phase too early.
            if (Volatile.Read(ref this._nightmareTeleporting) != 0)
            {
                continue;
            }

            if (this._nightmareMonster is not { IsAlive: true } monster)
            {
                continue;
            }

            var maximumHealth = monster.Attributes[Stats.MaximumHealth];
            var healthPercentage = maximumHealth > 0 ? monster.Health * 100f / maximumHealth : 100f;

            var targetPhaseIndex = 0;
            for (var i = 0; i < nightmare.HpPhases.Count; i++)
            {
                if (healthPercentage < nightmare.HpPhases[i].HealthPercentage)
                {
                    targetPhaseIndex = i + 1;
                }
            }

            if (targetPhaseIndex > this._nightmarePhaseIndex)
            {
                this._nightmarePhaseIndex = targetPhaseIndex;
                await this.ExecuteNightmareTeleportAsync(monster, nightmare, nightmare.HpPhases[targetPhaseIndex - 1], ct)
                    .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Teleports the boss to the position of the given health phase and restores its health.
    /// </summary>
    /// <remarks>
    /// The <see cref="_nightmareTeleporting"/> guard makes sure that the health monitor can't
    /// trigger the next phase while the teleport is running.
    /// </remarks>
    private async Task ExecuteNightmareTeleportAsync(Monster monster, KanturuNightmareDefinition nightmare, KanturuNightmareHpPhase hpPhase, CancellationToken ct)
    {
        // The boss may have died between the health check and this call.
        if (!monster.IsAlive)
        {
            return;
        }

        Interlocked.Exchange(ref this._nightmareTeleporting, 1);
        try
        {
            // Restore the health first, so that damage during the animation can't kill the boss.
            // Otherwise a simultaneous hit could drop its health to 0 and cause a death event.
            monster.Health = (int)monster.Attributes[Stats.MaximumHealth];

            // A short pause, so the clients can process the health update before the teleport.
            await Task.Delay(nightmare.TeleportDelay).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            await monster.MoveAsync(new Point(hpPhase.TeleportTargetX, hpPhase.TeleportTargetY)).ConfigureAwait(false);

            // Restore the health a second time, to cover the hits which landed in the meantime.
            monster.Health = (int)monster.Attributes[Stats.MaximumHealth];

            await this.ShowGoldenMessageIfConfiguredAsync(hpPhase.MessageKey).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref this._nightmareTeleporting, 0);
        }
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
        // formerly blocked cells as passable.
        var terrain = this.Map.Terrain;
        foreach (var area in this._definition.BarrierAreas)
        {
            for (int x = area.StartX; x <= area.EndX; x++)
            {
                for (int y = area.StartY; y <= area.EndY; y++)
                {
                    terrain.WalkMap[x, y] = true;
                    terrain.UpdateAiGridValue((byte)x, (byte)y);
                }
            }
        }

        // Additionally send the terrain attribute change as a fallback: if the terrain file of
        // the opened barrier is missing at a client, this packet still clears the attribute.
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
    /// Keeps the map open as the Tower of Refinement after the boss has been defeated.
    /// </summary>
    private async Task RunTowerOfRefinementAsync(CancellationToken ct)
    {
        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerConqueredMessageKey).ConfigureAwait(false);

        var duration = this._definition.TowerOfRefinementDuration;
        var warningOffset = this._definition.TowerClosingWarningOffset;

        // The delays don't use the token, so they aren't cancelled when all current players
        // leave while new ones might still arrive.
        if (duration > warningOffset)
        {
            await Task.Delay(duration - warningOffset).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerClosingWarningMessageKey).ConfigureAwait(false);

            await Task.Delay(warningOffset).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
        }
        else if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
        }

        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Notify).ConfigureAwait(false);
        await this.ShowGoldenMessageIfConfiguredAsync(this._definition.TowerClosedMessageKey).ConfigureAwait(false);
        await this.ShowKanturuStateAsync(KanturuState.Tower, (byte)KanturuTowerDetailState.Close).ConfigureAwait(false);
    }

    /// <summary>
    /// Periodically broadcasts the wide area attack of Maya, alternating between the storm and
    /// the stone rain animation.
    /// </summary>
    private async Task RunMayaWideAreaAttacksAsync(CancellationToken ct)
    {
        var isStorm = true;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(this._definition.MayaAttackInterval, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (Volatile.Read(ref this._mayaAttacksPaused) == 0)
            {
                var showStorm = isStorm;
                await this.ForEachPlayerAsync(player =>
                    player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                        p.ShowMayaWideAreaAttackAsync(showStorm)).AsTask()).ConfigureAwait(false);
            }

            isStorm = !isStorm;
        }
    }

    /// <summary>
    /// Periodically broadcasts the special attack animation of the boss to all players of the map.
    /// </summary>
    private async Task RunNightmareSpecialAttacksAsync(KanturuNightmareDefinition nightmare, CancellationToken ct)
    {
        if (nightmare.SpecialAttackInterval <= TimeSpan.Zero)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(nightmare.SpecialAttackInterval, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (this._nightmareMonster is not { IsAlive: true } monster)
            {
                break;
            }

            // Skip during a teleport, to avoid conflicting animations.
            if (Volatile.Read(ref this._nightmareTeleporting) != 0)
            {
                continue;
            }

            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowSkillAnimationPlugIn>(p =>
                    p.ShowSkillAnimationAsync(monster, null, nightmare.SpecialAttackSkillNumber, true)).AsTask())
                .ConfigureAwait(false);
        }
    }

    private async Task DelayAsync(TimeSpan duration, CancellationToken ct)
    {
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, ct).ConfigureAwait(false);
        }
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

    private ValueTask ShowTimeLimitToAllAsync(TimeSpan timeLimit)
    {
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.ShowTimeLimitAsync(timeLimit)).AsTask());
    }
}
