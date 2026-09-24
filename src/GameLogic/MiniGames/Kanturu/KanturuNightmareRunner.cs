// <copyright file="KanturuNightmareRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Runs the Nightmare boss fight: spawn wait, health-phase teleports, special attacks.
/// Owns the boss state; the spawn wait is provided by the caller.
/// </summary>
internal sealed class KanturuNightmareRunner : IKanturuPhaseRunner
{
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task> _beginAsync;
    private readonly Func<KanturuState, byte, ValueTask> _showStateAsync;
    private readonly Func<string?, ValueTask> _showGoldenAsync;
    private readonly Func<ValueTask> _showLiveCountAsync;
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task<bool>> _waitAsync;
    private readonly Func<KanturuPhaseDefinition, CancellationToken, Task> _standbyAsync;
    private readonly Func<KanturuNightmareDefinition, CancellationToken, Task<Monster?>> _waitForSpawnAsync;
    private readonly Func<Func<Player, Task>, ValueTask> _forEachPlayerAsync;
    private readonly ILogger _logger;

    private Monster? _nightmareMonster;
    private int _nightmarePhaseIndex;
    private int _nightmareTeleporting;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuNightmareRunner"/> class.
    /// </summary>
    /// <param name="beginAsync">Starts a phase.</param>
    /// <param name="showStateAsync">Broadcasts a state change to the clients.</param>
    /// <param name="showGoldenAsync">Shows a golden message, if a message key is configured.</param>
    /// <param name="showLiveCountAsync">Broadcasts the currently alive monster count.</param>
    /// <param name="waitAsync">Waits for a phase to end; reports whether it completed.</param>
    /// <param name="standbyAsync">Runs the standby time after a phase.</param>
    /// <param name="waitForSpawnAsync">Waits for the Nightmare boss to spawn.</param>
    /// <param name="forEachPlayerAsync">Executes an action for each player.</param>
    /// <param name="logger">The logger.</param>
    public KanturuNightmareRunner(
        Func<KanturuPhaseDefinition, CancellationToken, Task> beginAsync,
        Func<KanturuState, byte, ValueTask> showStateAsync,
        Func<string?, ValueTask> showGoldenAsync,
        Func<ValueTask> showLiveCountAsync,
        Func<KanturuPhaseDefinition, CancellationToken, Task<bool>> waitAsync,
        Func<KanturuPhaseDefinition, CancellationToken, Task> standbyAsync,
        Func<KanturuNightmareDefinition, CancellationToken, Task<Monster?>> waitForSpawnAsync,
        Func<Func<Player, Task>, ValueTask> forEachPlayerAsync,
        ILogger logger)
    {
        this._beginAsync = beginAsync;
        this._showStateAsync = showStateAsync;
        this._showGoldenAsync = showGoldenAsync;
        this._showLiveCountAsync = showLiveCountAsync;
        this._waitAsync = waitAsync;
        this._standbyAsync = standbyAsync;
        this._waitForSpawnAsync = waitForSpawnAsync;
        this._forEachPlayerAsync = forEachPlayerAsync;
        this._logger = logger;
    }

    /// <inheritdoc />
    public KanturuPhaseKind Kind => KanturuPhaseKind.Nightmare;

    /// <inheritdoc />
    public async Task<bool> RunAsync(KanturuPhaseDefinition phase, CancellationToken cancellationToken)
    {
        var nightmare = phase.Nightmare ?? new KanturuNightmareDefinition();
        this._nightmarePhaseIndex = 0;
        this._nightmareMonster = null;

        // Arm the spawn capture before beginning the phase: the boss spawns
        // synchronously inside BeginPhaseAsync (via GameMap.AddAsync raising
        // ObjectAdded), so subscribing afterwards would miss it. The waiter
        // subscribes synchronously up to its first await.
        var spawnTask = this._waitForSpawnAsync(nightmare, cancellationToken);
        await this._beginAsync(phase, cancellationToken).ConfigureAwait(false);
        this._nightmareMonster = await spawnTask.ConfigureAwait(false);
        if (this._nightmareMonster is null)
        {
            this._logger.LogWarning(
                "Kanturu: the Nightmare monster didn't spawn within {Timeout} - its health phases are disabled.",
                nightmare.SpawnTimeout);
        }

        await this._showStateAsync(phase.State, nightmare.BattleDetailState).ConfigureAwait(false);
        await this._showGoldenAsync(phase.StartMessageKey).ConfigureAwait(false);
        await this._showLiveCountAsync().ConfigureAwait(false);

        using var bossCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var healthMonitor = Task.Run(() => this.MonitorNightmareHealthAsync(nightmare, bossCts.Token), bossCts.Token);
        var specialAttacks = Task.Run(() => this.RunNightmareSpecialAttacksAsync(nightmare, bossCts.Token), bossCts.Token);

        var completed = await this._waitAsync(phase, cancellationToken).ConfigureAwait(false);

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

        if (!completed)
        {
            return false;
        }

        await this._standbyAsync(phase, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task MonitorNightmareHealthAsync(KanturuNightmareDefinition nightmare, CancellationToken ct)
    {
        if (nightmare.HpPhases.Count == 0 || nightmare.HealthCheckInterval <= TimeSpan.Zero)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(nightmare.HealthCheckInterval, ct).ConfigureAwait(false);

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

            var targetPhaseIndex = KanturuNightmarePhaseSelector.GetTargetPhaseIndex(nightmare.HpPhases, healthPercentage);
            if (KanturuNightmarePhaseSelector.ShouldAdvance(targetPhaseIndex, this._nightmarePhaseIndex))
            {
                this._nightmarePhaseIndex = targetPhaseIndex;
                await this.ExecuteNightmareTeleportAsync(monster, nightmare, nightmare.HpPhases[targetPhaseIndex - 1], ct)
                    .ConfigureAwait(false);
            }
        }
    }

    private async Task ExecuteNightmareTeleportAsync(Monster monster, KanturuNightmareDefinition nightmare, KanturuNightmareHpPhase hpPhase, CancellationToken ct)
    {
        if (!monster.IsAlive)
        {
            return;
        }

        Interlocked.Exchange(ref this._nightmareTeleporting, 1);
        try
        {
            monster.Health = (int)monster.Attributes[Stats.MaximumHealth];

            await Task.Delay(nightmare.TeleportDelay).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            // The boss may have died while teleporting; restoring its health then
            // would resurrect it after the death event already ran.
            if (!monster.IsAlive)
            {
                return;
            }

            await monster.MoveAsync(new Point(hpPhase.TeleportTargetX, hpPhase.TeleportTargetY)).ConfigureAwait(false);

            if (!monster.IsAlive)
            {
                return;
            }

            monster.Health = (int)monster.Attributes[Stats.MaximumHealth];

            await this._showGoldenAsync(hpPhase.MessageKey).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref this._nightmareTeleporting, 0);
        }
    }

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

            if (Volatile.Read(ref this._nightmareTeleporting) != 0)
            {
                continue;
            }

            await this._forEachPlayerAsync(player =>
                player.InvokeViewPlugInAsync<IShowSkillAnimationPlugIn>(p =>
                    p.ShowSkillAnimationAsync(monster, null, nightmare.SpecialAttackSkillNumber, true)).AsTask())
                .ConfigureAwait(false);
        }
    }
}
