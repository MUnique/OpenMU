// <copyright file="KanturuContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The context of a Kanturu Refinery Tower event game.
/// </summary>
/// <remarks>
/// The Kanturu event progresses through sequential phases:
/// Phase 1: Kill 30 Blade Hunters + 10 Dreadfear → Maya (Left Hand) spawns → kill her.
/// Phase 2: Kill 30 Blade Hunters + 10 Dreadfear → Maya (Right Hand) spawns → kill her.
/// Phase 3: Kill 10 Dreadfear + 10 Twin Tale → Both Maya hands spawn → kill both.
/// Nightmare Prep: Kill 15 Genocider + 15 Dreadfear + 15 Persona → Nightmare spawns.
/// Nightmare: At 75/50/25% HP Nightmare teleports and regains full HP.
/// Victory: Kill Nightmare → Elphis barrier opens → Tower of Refinement.
/// Between phases there is a 2-minute standby period.
/// Players who die are respawned at Kanturu Relics (handled by map's SafezoneMap setting).
/// </remarks>
public sealed class KanturuContext : MiniGameContext
{
    // Monster definition numbers
    private const short MayaBodyNumber = 364;
    private const short MayaLeftHandNumber = 362;
    private const short MayaRightHandNumber = 363;
    private const short NightmareNumber = 361;
    private const short BladeHunterNumber = 354;
    private const short DreadfearNumber = 360;
    private const short TwinTaleNumber = 359;
    private const short GenociderNumber = 357;
    private const short PersonaNumber = 358;

    // Wave numbers matching MonsterSpawnArea.WaveNumber in KanturuEvent.cs
    private const byte WaveMayaAppear = 0;   // Maya body rises at battle start
    private const byte WavePhase1Monsters = 1;
    private const byte WavePhase1Boss = 2;
    private const byte WavePhase2Monsters = 3;
    private const byte WavePhase2Boss = 4;
    private const byte WavePhase3Monsters = 5;
    private const byte WavePhase3Bosses = 6;
    private const byte WaveNightmarePrep = 7;
    private const byte WaveNightmare = 8;

    // Client detail code for Maya "notify" cinematic (camera pan + Maya rise animation).
    // This is KANTURU_MAYA_DIRECTION_NOTIFY=2 on the client side.
    private const byte MayaNotifyDetail = 2;

    // Skill numbers for Nightmare special attacks.
    // These map directly to client-side AT_SKILL_* enum values and drive the animation selection
    // on the Nightmare model (MODEL_DARK_SKULL_SOLDIER_5) in GM_Kanturu_3rd.cpp.
    //   Inferno (#14, AT_SKILL_INFERNO): ATTACK4 — Inferno explosion + 2×MODEL_CIRCLE effects.
    private const short NightmareInfernoskillNumber = 14;

    // Nightmare phase teleport positions within the Nightmare Zone (X:75-88, Y:97-143)
    // Phase 1 = initial spawn at (78, 143) defined in KanturuEvent.cs
    private static readonly Point NightmarePhase2Pos = new(82, 130);
    private static readonly Point NightmarePhase3Pos = new(76, 115);
    private static readonly Point NightmarePhase4Pos = new(85, 100);

    // Elphis barrier area — cells that are NoGround (value 8) in the .att file and block
    // the path from the Nightmare zone to the Elpis NPC area (Y~177).
    // Confirmed via Terrain39.att analysis: entire X=73-90, Y=144-180 column is NoGround.
    // OpenMU reads WalkMap[x,y] = false for these cells by default; we override to true
    // after Nightmare is defeated so players can walk to Elpis.
    private static readonly (byte StartX, byte StartY, byte EndX, byte EndY)[] ElphisBarrierAreas =
    [
        (73, 144, 90, 195),
    ];

    private readonly IMapInitializer _mapInitializer;
    private readonly TimeSpan _towerOfRefinementDuration;

    private volatile KanturuPhase _phase = KanturuPhase.Open;
    private int _waveKillCount;
    private int _waveKillTarget;
    private TaskCompletionSource _phaseComplete = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _isVictory;

    // Interlocked flag: 0 = not yet opened, 1 = opened (or in progress).
    // Ensures the barrier is opened exactly once regardless of whether the trigger
    // comes from OnMonsterDied (fire-and-forget) or the game loop.
    private int _barrierOpened;

    // Nightmare HP-phase tracking
    private Monster? _nightmareMonster;
    private int _nightmarePhase;

    // True while ExecuteNightmareTeleportAsync is in progress.
    // Prevents MonitorNightmareHpAsync from re-triggering a teleport during the animation window.
    private volatile bool _nightmareTeleporting;

    // True while inter-phase standby is running — suppresses Maya wide-area attacks so
    // Maya stays visually idle (no storm/stone-rain effects) during the break between phases.
    private volatile bool _mayaAttacksPaused;

    /// <summary>
    /// Gets the current Kanturu main state code (the last state sent via 0xD1/0x03).
    /// The Gateway NPC plugin reads this to populate the 0xD1/0x00 StateInfo dialog
    /// while the event is in progress.
    /// </summary>
    public byte CurrentKanturuState { get; private set; } = KanturuStateCode.MayaBattle;

    /// <summary>
    /// Gets the current Kanturu detail state code (the last detailState sent via 0xD1/0x03).
    /// </summary>
    public byte CurrentKanturuDetailState { get; private set; }

    private enum KanturuPhase
    {
        Open,
        Phase1Monsters,
        Phase1Boss,
        Phase2Monsters,
        Phase2Boss,
        Phase3Monsters,
        Phase3Bosses,
        NightmarePrep,
        NightmareActive,
        Ended,
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    /// <param name="towerOfRefinementDuration">
    /// How long the Tower of Refinement stays open after Nightmare is defeated.
    /// Defaults to 1 hour if not specified.
    /// </param>
    public KanturuContext(
        MiniGameMapKey key,
        MiniGameDefinition definition,
        IGameContext gameContext,
        IMapInitializer mapInitializer,
        TimeSpan towerOfRefinementDuration = default)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._mapInitializer = mapInitializer;
        this._towerOfRefinementDuration = towerOfRefinementDuration == default
            ? TimeSpan.FromHours(1)
            : towerOfRefinementDuration;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        // Maya rises from the depths when the battle begins.
        await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, WaveMayaAppear).ConfigureAwait(false);

        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync("Maya rises from the depths of the Refinery Tower!", MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);

        _ = Task.Run(() => this.RunKanturuGameLoopAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc/>
    protected override void OnMonsterDied(object? sender, DeathInformation e)
    {
        base.OnMonsterDied(sender, e);

        if (sender is not Monster monster)
        {
            return;
        }

        var num = (short)monster.Definition.Number;
        var phase = this._phase;
        bool complete;

        switch (phase)
        {
            case KanturuPhase.Phase1Monsters when num is BladeHunterNumber or DreadfearNumber:
            {
                var killed = Interlocked.Increment(ref this._waveKillCount);
                complete = killed == this._waveKillTarget;
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                break;
            }

            case KanturuPhase.Phase1Boss when num == MayaLeftHandNumber:
            {
                complete = true;
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(0, pc);
                break;
            }

            case KanturuPhase.Phase2Monsters when num is BladeHunterNumber or DreadfearNumber:
            {
                var killed = Interlocked.Increment(ref this._waveKillCount);
                complete = killed == this._waveKillTarget;
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                break;
            }

            case KanturuPhase.Phase2Boss when num == MayaRightHandNumber:
            {
                complete = true;
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(0, pc);
                break;
            }

            case KanturuPhase.Phase3Monsters when num is DreadfearNumber or TwinTaleNumber:
            {
                var killed = Interlocked.Increment(ref this._waveKillCount);
                complete = killed == this._waveKillTarget;
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                break;
            }

            case KanturuPhase.Phase3Bosses when num is MayaLeftHandNumber or MayaRightHandNumber:
            {
                var killed = Interlocked.Increment(ref this._waveKillCount);
                complete = killed == this._waveKillTarget;
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                break;
            }

            case KanturuPhase.NightmarePrep when num is GenociderNumber or DreadfearNumber or PersonaNumber:
            {
                var killed = Interlocked.Increment(ref this._waveKillCount);
                complete = killed == this._waveKillTarget;
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                break;
            }

            case KanturuPhase.NightmareActive when num is GenociderNumber or DreadfearNumber or PersonaNumber:
            {
                // A guardian died while Nightmare is alive — update the HUD counter but do NOT
                // advance the phase. _waveKillTarget=46 (45 guardians + 1 Nightmare), so
                // remaining = 46 − killed still accounts for Nightmare being alive.
                var killed = Interlocked.Increment(ref this._waveKillCount);
                var remaining = (byte)Math.Max(0, this._waveKillTarget - killed);
                var pc = (byte)Math.Min(255, this.PlayerCount);
                this.FireAndForgetMonsterCountAsync(remaining, pc);
                complete = false;
                break;
            }

            case KanturuPhase.NightmareActive when num == NightmareNumber:
                complete = true;
                // Fire barrier opening immediately from the death event.
                // Do NOT wait for the game loop — it may be interrupted by
                // GameEndedToken cancellation before reaching OpenElphisBarrierAsync.
                _ = Task.Run(() => this.OpenElphisBarrierAsync().AsTask());
                break;

            default:
                // Diagnostic: if Nightmare dies outside the expected phase, warn.
                if (num == NightmareNumber)
                {
                    this.Logger.LogWarning(
                        "Kanturu: Nightmare died but _phase={Phase} (expected NightmareActive). Barrier NOT opened.",
                        phase);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await this.ForEachPlayerAsync(p =>
                                p.InvokeViewPlugInAsync<IShowMessagePlugIn>(v =>
                                    v.ShowMessageAsync(
                                        $"[Kanturu] Nightmare died out of phase! Phase={phase}",
                                        MessageType.BlueNormal)).AsTask()).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.LogWarning(ex, "{context}: Error broadcasting out-of-phase diagnostic message.", this);
                        }
                    });
                }

                complete = false;
                break;
        }

        if (complete)
        {
            this._phaseComplete.TrySetResult();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        var message = this._isVictory
            ? "Nightmare has been defeated! The Kanturu Refinery Tower is yours!"
            : "The Kanturu Event has ended. Better luck next time!";

        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync(message, MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);

        // On defeat show the Failure_kantru.tga overlay.
        // On victory the Success_kantru.tga and Tower state are sent from OpenElphisBarrierAsync.
        if (!this._isVictory)
        {
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                    p.SendBattleResultAsync(0)).AsTask()).ConfigureAwait(false);
        }

        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    private async Task RunKanturuGameLoopAsync(CancellationToken ct)
    {
        try
        {
            // Maya "notify" cinematic — camera pans to Maya, Maya body rises from below.
            // Must be sent first so the client camera is in position before the first wave.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, MayaNotifyDetail).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(3), ct).ConfigureAwait(false);

            // Start the Maya wide-area attack visual loop for the duration of all Maya phases.
            // The loop broadcasts 0xD1/0x06 every 15 s, alternating storm (0) and stone-rain (1).
            using var mayaAttackCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _ = Task.Run(() => this.RunMayaWideAreaAttacksAsync(mayaAttackCts.Token), mayaAttackCts.Token);

            // Phase 1: wave of monsters — 10-minute timer covers wave + boss.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Monster1).ConfigureAwait(false);
            await this.SendTimeLimitToAllAsync((int)TimeSpan.FromMinutes(10).TotalMilliseconds).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase1Monsters, WavePhase1Monsters, 40,
                "Phase 1: Defeat the monsters to unseal Maya's power!", ct).ConfigureAwait(false);

            // Phase 1: Maya Left Hand boss
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Maya1).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase1Boss, WavePhase1Boss, 1,
                "Maya (Left Hand) has appeared! Destroy her!", ct).ConfigureAwait(false);

            // Standby between phases
            await this.SendStandbyMessageAsync("Phase 1 cleared! Phase 2 begins in 2 minutes...", ct).ConfigureAwait(false);

            // Phase 2: wave of monsters — fresh 10-minute timer.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Monster2).ConfigureAwait(false);
            await this.SendTimeLimitToAllAsync((int)TimeSpan.FromMinutes(10).TotalMilliseconds).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase2Monsters, WavePhase2Monsters, 40,
                "Phase 2: More of Maya's minions have arrived!", ct).ConfigureAwait(false);

            // Phase 2: Maya Right Hand boss
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Maya2).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase2Boss, WavePhase2Boss, 1,
                "Maya (Right Hand) has appeared! Destroy her!", ct).ConfigureAwait(false);

            // Standby between phases
            await this.SendStandbyMessageAsync("Phase 2 cleared! Phase 3 begins in 2 minutes...", ct).ConfigureAwait(false);

            // Phase 3: wave of monsters — fresh 10-minute timer.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Monster3).ConfigureAwait(false);
            await this.SendTimeLimitToAllAsync((int)TimeSpan.FromMinutes(10).TotalMilliseconds).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase3Monsters, WavePhase3Monsters, 20,
                "Phase 3: The final wave approaches!", ct).ConfigureAwait(false);

            // Phase 3: Both Maya bosses simultaneously
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.Maya3).ConfigureAwait(false);
            await this.AdvancePhaseAsync(KanturuPhase.Phase3Bosses, WavePhase3Bosses, 2,
                "Both hands of Maya have appeared! Defeat them both!", ct).ConfigureAwait(false);

            // Hide HUD during the loot window — same reason as inter-phase standby.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.None).ConfigureAwait(false);

            // 10-second loot window: players pick up drops from both Maya hands
            // before the Nightmare transition cinematic begins.
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                    p.ShowMessageAsync(
                        "Maya's hands have fallen! Collect your loot — the Nightmare awakens in 10 seconds...",
                        MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(10), ct).ConfigureAwait(false);

            // Stop the Maya wide-area attack visuals before the Nightmare transition cinematic.
            await mayaAttackCts.CancelAsync().ConfigureAwait(false);

            // Transition to Nightmare room:
            //   1. TeleportToNightmareRoomAsync sends 0xD1/0x03 ENDCYCLE_MAYA3(16) which triggers
            //      the full cinematic (camera pan → Maya explosion → player falls through floor),
            //      waits ~10 s for it to complete, then MoveAsync teleports players to (79, 98).
            //   2. NightmareBattle/Idle is sent AFTER teleport so the HUD change applies in
            //      the Nightmare zone, not the Maya battlefield.
            await this.TeleportToNightmareRoomAsync(ct).ConfigureAwait(false);
            await this.SendKanturuStateAsync(KanturuStateCode.NightmareBattle, KanturuNightmareDetail.Idle).ConfigureAwait(false);

            // Nightmare Prep: spawn guardians immediately, then spawn Nightmare after 3 seconds.
            // No kill requirement — guardians fight alongside Nightmare.
            await this.SendTimeLimitToAllAsync((int)TimeSpan.FromMinutes(30).TotalMilliseconds).ConfigureAwait(false);
            Interlocked.Exchange(ref this._waveKillCount, 0);
            this._waveKillTarget = 45;
            this._phaseComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            this._phase = KanturuPhase.NightmarePrep;

            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, WaveNightmarePrep).ConfigureAwait(false);
            await this.SendMonsterUserCountAsync(45, (byte)Math.Min(255, this.PlayerCount)).ConfigureAwait(false);
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                    p.ShowMessageAsync("Nightmare's guardians have appeared! NIGHTMARE awakens in 3 seconds!", MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromSeconds(3), ct).ConfigureAwait(false);

            // Nightmare boss with HP-phase teleports
            await this.RunNightmarePhaseAsync(ct).ConfigureAwait(false);

            // Victory
            this._isVictory = true;
            this._phase = KanturuPhase.Ended;

            // Open the Elphis barrier (also sends the Success screen + Tower state).
            // The fire-and-forget from OnMonsterDied already called this, but we call again
            // as a fallback — the Interlocked guard ensures it only executes once.
            await this.OpenElphisBarrierAsync().ConfigureAwait(false);

            // Tower of Refinement: keep the map open for a configurable period
            await this.RunTowerOfRefinementAsync(ct).ConfigureAwait(false);

            this.FinishEvent();
        }
        catch (OperationCanceledException)
        {
            // Game ended by timeout or external cancellation — treated as defeat.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error in Kanturu game loop.");
        }
    }

    /// <summary>
    /// Runs the Nightmare phase. Nightmare teleports and recovers full HP at 75%, 50%, and 25% HP.
    /// </summary>
    private async Task RunNightmarePhaseAsync(CancellationToken ct)
    {
        this._nightmarePhase = 1;
        this._nightmareMonster = null;

        // Do NOT reset _waveKillCount — it holds guardian kills from the 3-second prep window.
        // _waveKillTarget = 46 (45 guardians + 1 Nightmare) so the HUD counter correctly
        // reflects every remaining mob in the Nightmare zone, not just the boss.
        this._waveKillTarget = 46;
        this._phaseComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        this._phase = KanturuPhase.NightmareActive;

        // Nightmare intro cinematic — camera moves to Nightmare zone and summons Nightmare.
        // This uses detail=NightmareIntro(2) = KANTURU_NIGHTMARE_DIRECTION_NIGHTMARE on the client.
        await this.SendKanturuStateAsync(KanturuStateCode.NightmareBattle, KanturuNightmareDetail.NightmareIntro).ConfigureAwait(false);
        await Task.Delay(TimeSpan.FromSeconds(3), ct).ConfigureAwait(false);

        // Subscribe to ObjectAdded to capture the Nightmare reference as soon as it spawns.
        var nightmareFound = new TaskCompletionSource<Monster>(TaskCreationOptions.RunContinuationsAsynchronously);

        async ValueTask OnObjectAdded((GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Monster m && (short)m.Definition.Number == NightmareNumber)
            {
                nightmareFound.TrySetResult(m);
            }
        }

        this.Map.ObjectAdded += OnObjectAdded;
        try
        {
            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, WaveNightmare)
                .ConfigureAwait(false);

            // Wait up to 5 seconds for the spawn to register.
            this._nightmareMonster = await nightmareFound.Task
                .WaitAsync(TimeSpan.FromSeconds(5), ct)
                .ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            this.Logger.LogWarning("Nightmare monster did not spawn within 5 seconds — HP phases disabled.");
        }
        finally
        {
            this.Map.ObjectAdded -= OnObjectAdded;
        }

        // Switch to active battle state — shows Nightmare HUD on client (INTERFACE_KANTURU_INFO).
        await this.SendKanturuStateAsync(KanturuStateCode.NightmareBattle, KanturuNightmareDetail.Battle).ConfigureAwait(false);

        // Show total remaining: alive guardians (45 − killed during prep) + 1 Nightmare.
        var totalRemaining = (byte)Math.Min(255, Math.Max(1, this._waveKillTarget - this._waveKillCount));
        await this.SendMonsterUserCountAsync(totalRemaining, (byte)Math.Min(255, this.PlayerCount)).ConfigureAwait(false);

        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync("NIGHTMARE has appeared! Defeat him to claim victory!", MessageType.GoldenCenter))
            .AsTask()).ConfigureAwait(false);

        // Start HP monitor and special-attack loop — both linked to the same CTS so they
        // stop together the moment Nightmare dies or the game is cancelled.
        using var hpCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var hpMonitor = Task.Run(
            () => this.MonitorNightmareHpAsync(hpCts.Token),
            hpCts.Token);
        var specialAttacks = Task.Run(
            () => this.RunNightmareSpecialAttacksAsync(hpCts.Token),
            hpCts.Token);

        await this._phaseComplete.Task.WaitAsync(ct).ConfigureAwait(false);

        await hpCts.CancelAsync().ConfigureAwait(false);
        try { await hpMonitor.ConfigureAwait(false); }
        catch (OperationCanceledException) { /* expected on cancel */ }
        try { await specialAttacks.ConfigureAwait(false); }
        catch (OperationCanceledException) { /* expected on cancel */ }
    }

    /// <summary>
    /// Polls Nightmare's HP every second and triggers a phase teleport at 75/50/25%.
    /// </summary>
    private async Task MonitorNightmareHpAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), ct).ConfigureAwait(false);

            // Do NOT check HP while a teleport is already in progress.
            // ExecuteNightmareTeleportAsync restores Nightmare's HP as part of the sequence;
            // reading HP mid-teleport would give a stale (low) value and re-trigger.
            if (this._nightmareTeleporting)
            {
                continue;
            }

            if (this._nightmareMonster is { IsAlive: true } nm)
            {
                var maxHp   = nm.Attributes[Stats.MaximumHealth];
                var hpRatio = maxHp > 0 ? (float)nm.Health / maxHp : 1f;

                var targetPhase = hpRatio switch
                {
                    < 0.25f => 4,
                    < 0.50f => 3,
                    < 0.75f => 2,
                    _       => 1,
                };

                if (targetPhase > this._nightmarePhase)
                {
                    this._nightmarePhase = targetPhase;
                    await this.ExecuteNightmareTeleportAsync(nm, targetPhase, ct)
                        .ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Teleports Nightmare to a new phase position and restores his HP to full.
    /// Uses a <see cref="_nightmareTeleporting"/> guard so the HP monitor cannot
    /// re-trigger while the teleport sequence is in progress.
    /// </summary>
    private async Task ExecuteNightmareTeleportAsync(Monster nightmare, int phase, CancellationToken ct)
    {
        // Nightmare may have died between the HP check and this call.
        if (!nightmare.IsAlive)
        {
            return;
        }

        this._nightmareTeleporting = true;
        try
        {
            var newPos = phase switch
            {
                2 => NightmarePhase2Pos,
                3 => NightmarePhase3Pos,
                4 => NightmarePhase4Pos,
                _ => NightmarePhase2Pos,
            };

            var msg = phase switch
            {
                2 => "Nightmare has teleported! He recovers his full strength!",
                3 => "Nightmare teleports again! He is more powerful than ever!",
                4 => "Nightmare is at his last stand! Finish him!",
                _ => string.Empty,
            };

            // Restore HP FIRST — any damage during the brief animation window will not kill Nightmare.
            // This prevents the race condition where a simultaneous player hit (e.g. Cyclone)
            // drops Nightmare's HP to 0 before the HP restore line, causing an incorrect death event.
            nightmare.Health = (int)nightmare.Attributes[Stats.MaximumHealth];

            // Short pause so clients can process the HP restore notification before the teleport.
            await Task.Delay(TimeSpan.FromMilliseconds(500), CancellationToken.None).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            // Teleport Nightmare to the phase-specific position.
            await nightmare.MoveAsync(newPos).ConfigureAwait(false);

            // Restore HP a second time as a safety net — covers any hits landing in the 500 ms window.
            nightmare.Health = (int)nightmare.Attributes[Stats.MaximumHealth];

            if (msg.Length > 0)
            {
                await this.ForEachPlayerAsync(player =>
                    player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                        p.ShowMessageAsync(msg, MessageType.GoldenCenter))
                    .AsTask()).ConfigureAwait(false);
            }
        }
        finally
        {
            this._nightmareTeleporting = false;
        }
    }

    /// <summary>
    /// Opens the Elphis barrier by removing the NoGround terrain attribute from the
    /// barrier area — both on the server walkmap and on every connected client.
    /// The .att file stores value 8 (NoGround) for this zone, so the client shows it
    /// as a visual void barrier. We must remove NoGround (not Blocked) to make it disappear.
    /// </summary>
    /// <remarks>
    /// This method is guarded by <see cref="_barrierOpened"/> so it executes at most once
    /// per game instance even when called concurrently from both <see cref="OnMonsterDied"/>
    /// and the game loop.
    /// </remarks>
    private async ValueTask OpenElphisBarrierAsync()
    {
        // Ensure we run only once — called both from OnMonsterDied (fire-and-forget)
        // and from the game loop as a fallback.
        if (Interlocked.CompareExchange(ref this._barrierOpened, 1, 0) != 0)
        {
            return;
        }

        try
        {
        this.Logger.LogInformation("Kanturu: opening Elphis barrier.");

        // Diagnostic confirmation message — visible in the game client.
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync("Kanturu: Barrier opening! The tower awaits...", MessageType.GoldenCenter)).AsTask())
            .ConfigureAwait(false);

        // Monster count = 0 (Nightmare defeated).
        await this.SendMonsterUserCountAsync(0, (byte)Math.Min(255, this.PlayerCount)).ConfigureAwait(false);

        // Victory camera-out cinematic (KANTURU_NIGHTMARE_DIRECTION_END = 4).
        await this.SendKanturuStateAsync(KanturuStateCode.NightmareBattle, KanturuNightmareDetail.End).ConfigureAwait(false);
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

        // 1. Send SUCCESS result overlay (Success_kantru.tga).
        //    The client requires state=NightmareBattle to show it, which was set when
        //    Nightmare spawned. Sending this before the Tower state change ensures
        //    the overlay renders while the client is still in the Nightmare state.
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.SendBattleResultAsync(1))
            .AsTask()).ConfigureAwait(false);

        // 2. Send Tower state → client reloads EncTerrain(n)01.att (barrier-open terrain),
        //    switches to Tower music, and plays the success sound.
        await this.SendKanturuStateAsync(KanturuStateCode.Tower, KanturuTowerDetail.Revitalization).ConfigureAwait(false);

        // 3. Update server-side walkmap so the AI pathfinder and movement checks
        //    treat the formerly-blocked cells as passable.
        var terrain = this.Map.Terrain;
        foreach (var (startX, startY, endX, endY) in ElphisBarrierAreas)
        {
            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    terrain.WalkMap[x, y] = true;
                    terrain.UpdateAiGridValue(x, y);
                }
            }
        }

        // 4. Also send the legacy 0x46 ChangeTerrainAttributes packet as a backup.
        //    If EncTerrain(n)01.att is missing on the client, this packet still clears
        //    the NoGround flag on the existing terrain so the visual barrier disappears.
        var areas = (IReadOnlyCollection<(byte, byte, byte, byte)>)ElphisBarrierAreas;
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IChangeTerrainAttributesViewPlugin>(p =>
                p.ChangeAttributesAsync(TerrainAttributeType.NoGround, setAttribute: false, areas))
            .AsTask()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error while opening Elphis barrier.", this);
        }
    }

    /// <summary>
    /// Keeps the map open as the Tower of Refinement after Nightmare is defeated.
    /// Sends a closing warning 5 minutes before the end of the configured duration.
    /// </summary>
    private async Task RunTowerOfRefinementAsync(CancellationToken ct)
    {
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync(
                    "The Kanturu Refinery Tower is conquered! The tower is now open.",
                    MessageType.GoldenCenter))
            .AsTask()).ConfigureAwait(false);

        var duration = this._towerOfRefinementDuration;
        var warningOffset = TimeSpan.FromMinutes(5);

        if (duration > warningOffset)
        {
            // Wait for most of the duration — use None so the delay isn't cancelled
            // if all current players leave while new ones might still arrive.
            await Task.Delay(duration - warningOffset, CancellationToken.None).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                    p.ShowMessageAsync(
                        "The Kanturu Tower closes in 5 minutes!",
                        MessageType.GoldenCenter))
                .AsTask()).ConfigureAwait(false);

            await Task.Delay(warningOffset, CancellationToken.None).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
        }
        else
        {
            await Task.Delay(duration, CancellationToken.None).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
        }

        // Tower closing notification
        await this.SendKanturuStateAsync(KanturuStateCode.Tower, KanturuTowerDetail.Notify).ConfigureAwait(false);

        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync("The Kanturu Tower has closed.", MessageType.GoldenCenter))
            .AsTask()).ConfigureAwait(false);

        await this.SendKanturuStateAsync(KanturuStateCode.Tower, KanturuTowerDetail.Close).ConfigureAwait(false);
    }

    private async Task TeleportToNightmareRoomAsync(CancellationToken ct)
    {
        var nightmareEntry = new Point(79, 98);

        // Step 1: Send 0xD1/0x03 with state=MayaBattle, detailState=EndCycleMaya3 (16).
        // This matches KANTURU_MAYA_DIRECTION_ENDCYCLE_MAYA3 on the client, which triggers
        // the full Maya→Nightmare transition cinematic via CKanturuDirection::Move2ndDirection():
        //   Stage 0 — camera flies to the Maya room (196, 85).
        //   Stage 1 — m_bMayaDie=true: Maya body plays its explosion animation; waits for it to finish.
        //   Stage 2 — m_bDownHero=true: the hero "falls through the floor" into the Nightmare zone.
        // NOTE: 0xD1/0x04 result=1 (Success_kantru.tga) must NOT be sent here — that overlay
        // is only correct after Nightmare is defeated and is already sent in OpenElphisBarrierAsync.
        await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.EndCycleMaya3).ConfigureAwait(false);

        // Step 2: Wait for the client cinematic to complete before teleporting.
        // The three stages take roughly: camera pan ~3 s + explosion ~4 s + fall ~3 s ≈ 10 s.
        // Use CancellationToken.None so the animation is never skipped mid-sequence.
        await Task.Delay(TimeSpan.FromSeconds(10), CancellationToken.None).ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();

        // Step 3: Teleport all players to the Nightmare zone entry point.
        // Same map (39), so MoveAsync is correct (sends MoveType.Instant / packet 0x15).
        // Mirrors C++ MoveAllUser(gate 134) → destination (79, 98).
        await this.ForEachPlayerAsync(player =>
            player.MoveAsync(nightmareEntry).AsTask()).ConfigureAwait(false);

        // Step 4: Play the warp arrival animation at the Nightmare zone entry point.
        // MapChangeFailedAsync (0xC2/0x1C) must be sent AFTER MoveAsync so the warp bubble
        // renders at (79, 98) and not at the Maya area. A brief pause ensures the client
        // has processed the position update before the animation triggers.
        // The fall cinematic (EndCycleMaya3 / m_bDownHero) is complete at this point
        // — 10 s have elapsed — so this packet does NOT interfere with it.
        // The bubble also briefly locks player input, preventing movement/attacks
        // during the visual scene transition.
        await Task.Delay(TimeSpan.FromMilliseconds(200), CancellationToken.None).ConfigureAwait(false);
        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IMapChangePlugIn>(p =>
                p.MapChangeFailedAsync()).AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Periodically broadcasts the Maya wide-area attack visual (0xD1/0x06) during the Maya battle.
    /// Alternates between storm (type 0) and stone-rain (type 1) every 15 seconds.
    /// This triggers the <c>MayaAction</c> animation on the Maya body object in the client,
    /// which is a purely cosmetic effect — actual damage is handled by the monsters' AttackSkill.
    /// </summary>
    private async Task RunMayaWideAreaAttacksAsync(CancellationToken ct)
    {
        byte attackType = 0;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(15), ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            // Skip the broadcast during inter-phase standby — Maya stays idle, no visual attacks.
            if (!this._mayaAttacksPaused)
            {
                try
                {
                    await this.ForEachPlayerAsync(player =>
                        player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                            p.SendMayaWideAreaAttackAsync(attackType)).AsTask()).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogWarning(ex, "{context}: Error sending Maya wide-area attack broadcast.", this);
                }
            }

            attackType = (byte)(1 - attackType); // alternate 0 → 1 → 0 → …
        }
    }

    /// <summary>
    /// Periodically broadcasts Nightmare's special explosion attack to all map players.
    /// Every 20 seconds this sends an Inferno skill animation (skill #14) from Nightmare,
    /// which the client maps to the ATTACK4 animation on MODEL_DARK_SKULL_SOLDIER_5:
    /// an Inferno explosion + 2×MODEL_CIRCLE visual effects — the iconic "explosion" attack
    /// seen in official MU Online Season 6 Kanturu Nightmare battles.
    /// </summary>
    /// <remarks>
    /// This is a pure visual broadcast. Nightmare's actual AoE damage (Decay poison) is
    /// already handled server-side by <see cref="MonsterDefinition.AttackSkill"/> (#38).
    /// </remarks>
    private async Task RunNightmareSpecialAttacksAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(20), ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (this._nightmareMonster is not { IsAlive: true } nm)
            {
                break;
            }

            // Skip during a teleport sequence to avoid animation conflicts.
            if (this._nightmareTeleporting)
            {
                continue;
            }

            // Broadcast Inferno skill animation (#14) from Nightmare to all players on the map.
            // Triggers ATTACK4 (frame 9): Inferno explosion + 2×MODEL_CIRCLE visual at Nightmare's position.
            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowSkillAnimationPlugIn>(p =>
                    p.ShowSkillAnimationAsync(nm, null, NightmareInfernoskillNumber, true)).AsTask())
                .ConfigureAwait(false);
        }
    }

    private async Task SendStandbyMessageAsync(string message, CancellationToken ct)
    {
        // Pause Maya wide-area attacks for the full duration of the standby so the
        // body stays visually idle (no storm/stone-rain effects between phases).
        this._mayaAttacksPaused = true;
        try
        {
            // Hide the in-map HUD (INTERFACE_KANTURU_INFO) during the inter-phase standby.
            // Sending detailState=None(0) while in MayaBattle state makes the client hide
            // the monster count / user count / timer panel until the next phase begins.
            // Maya body (#364) stays at its spawn position — it is ~27 tiles from the fight
            // room and well outside its ViewRange=9, so it naturally idles without attacking.
            await this.SendKanturuStateAsync(KanturuStateCode.MayaBattle, KanturuMayaDetail.None).ConfigureAwait(false);

            await this.ForEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                    p.ShowMessageAsync(message, MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromMinutes(2), ct).ConfigureAwait(false);
        }
        finally
        {
            this._mayaAttacksPaused = false;
        }
    }

    private async Task AdvancePhaseAsync(KanturuPhase phase, byte waveNumber, int killTarget, string message, CancellationToken ct)
    {
        Interlocked.Exchange(ref this._waveKillCount, 0);
        this._waveKillTarget = killTarget;
        this._phaseComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        this._phase = phase;

        await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this.Map, this, waveNumber).ConfigureAwait(false);

        // Broadcast the initial monster count so the HUD shows the correct number from the start.
        await this.SendMonsterUserCountAsync((byte)Math.Min(255, killTarget), (byte)Math.Min(255, this.PlayerCount)).ConfigureAwait(false);

        await this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p =>
                p.ShowMessageAsync(message, MessageType.GoldenCenter)).AsTask()).ConfigureAwait(false);

        await this._phaseComplete.Task.WaitAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Broadcasts packet 0xD1/0x03 (Kanturu state change) to all players currently on the map.
    /// Also updates <see cref="CurrentKanturuState"/> and <see cref="CurrentKanturuDetailState"/>
    /// so the Gateway NPC plugin can report the current event phase in the 0xD1/0x00 dialog.
    /// </summary>
    private ValueTask SendKanturuStateAsync(byte state, byte detailState)
    {
        this.CurrentKanturuState = state;
        this.CurrentKanturuDetailState = detailState;
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.SendStateChangeAsync(state, detailState)).AsTask());
    }

    /// <summary>
    /// Fires a monster/user-count broadcast without blocking the caller.
    /// Exceptions are caught and logged so that an unreachable player cannot
    /// propagate an unhandled exception through the fire-and-forget task.
    /// </summary>
    private void FireAndForgetMonsterCountAsync(byte remaining, byte pc) =>
        _ = Task.Run(async () =>
        {
            try
            {
                await this.SendMonsterUserCountAsync(remaining, pc).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "{context}: Error sending monster/user count update.", this);
            }
        });

    /// <summary>
    /// Broadcasts packet 0xD1/0x07 (Kanturu monster/user count) to all players currently on the map.
    /// </summary>
    private ValueTask SendMonsterUserCountAsync(byte monsterCount, byte userCount)
    {
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.SendMonsterUserCountAsync(monsterCount, userCount)).AsTask());
    }

    /// <summary>
    /// Broadcasts packet 0xD1/0x05 (Kanturu time limit) to all players currently on the map.
    /// </summary>
    /// <param name="milliseconds">Remaining time in milliseconds.</param>
    private ValueTask SendTimeLimitToAllAsync(int milliseconds)
    {
        return this.ForEachPlayerAsync(player =>
            player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
                p.SendTimeLimitAsync(milliseconds)).AsTask());
    }
}
