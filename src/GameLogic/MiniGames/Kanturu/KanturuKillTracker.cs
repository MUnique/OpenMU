// <copyright file="KanturuKillTracker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Thread-safe kill counting for the current Kanturu phase.
/// </summary>
internal sealed class KanturuKillTracker
{
    private int _killCount;
    private TaskCompletionSource _phaseComplete = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Gets the current phase, if any.
    /// </summary>
    public KanturuPhaseDefinition? CurrentPhase { get; private set; }

    /// <summary>
    /// Gets the kill count of the current phase.
    /// </summary>
    public int KillCount => Volatile.Read(ref this._killCount);

    /// <summary>
    /// Gets a task which completes when the kill target is reached.
    /// </summary>
    public Task PhaseCompleted => this._phaseComplete.Task;

    /// <summary>
    /// Starts tracking a new phase.
    /// </summary>
    /// <param name="phase">The phase to track.</param>
    public void BeginPhase(KanturuPhaseDefinition phase)
    {
        Interlocked.Exchange(ref this._killCount, 0);
        this._phaseComplete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        this.CurrentPhase = phase;
    }

    /// <summary>
    /// Clears the current phase, e.g. for transition phases which count no kills.
    /// </summary>
    public void ClearPhase()
    {
        this.CurrentPhase = null;
    }

    /// <summary>
    /// Registers a monster kill.
    /// </summary>
    /// <param name="killed">The definition of the killed monster.</param>
    /// <returns>The outcome.</returns>
    public KanturuKillResult RegisterKill(MonsterDefinition? killed)
    {
        var phase = this.CurrentPhase;
        if (!KanturuMonsterComparer.IsCountedMonster(killed, phase) || phase is null)
        {
            return new KanturuKillResult(false, this.KillCount, false, false, phase?.Kind == KanturuPhaseKind.Nightmare);
        }

        var killCount = Interlocked.Increment(ref this._killCount);
        var phaseComplete = killCount >= phase.KillTarget;
        if (phaseComplete)
        {
            this._phaseComplete.TrySetResult();
        }

        var bossKilled = phase.Kind == KanturuPhaseKind.Nightmare
            && KanturuMonsterComparer.IsSameMonster(phase.Nightmare?.Monster, killed);

        return new KanturuKillResult(true, killCount, phaseComplete, bossKilled, phase.Kind == KanturuPhaseKind.Nightmare);
    }
}
