// <copyright file="KanturuKillTracker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Thread-safe kill counting for the current Kanturu phase.
/// </summary>
/// <remarks>
/// All mutable per-phase state (phase, kill count, completion source) travels in one
/// atomically swapped <see cref="PhaseState"/> generation: a kill landing concurrently
/// with <see cref="BeginPhase"/> counts towards the generation it observed, and can
/// neither pollute nor complete the next one.
/// </remarks>
internal sealed class KanturuKillTracker
{
    private PhaseState? _state;

    /// <summary>
    /// Gets the current phase, if any.
    /// </summary>
    public KanturuPhaseDefinition? CurrentPhase => Volatile.Read(ref this._state)?.Phase;

    /// <summary>
    /// Gets the kill count of the current phase.
    /// </summary>
    public int KillCount => Volatile.Read(ref this._state)?.KillCount ?? 0;

    /// <summary>
    /// Gets a task which completes when the kill target is reached.
    /// </summary>
    public Task PhaseCompleted => Volatile.Read(ref this._state)?.Completion.Task ?? Task.CompletedTask;

    /// <summary>
    /// Starts tracking a new phase.
    /// </summary>
    /// <param name="phase">The phase to track.</param>
    public void BeginPhase(KanturuPhaseDefinition phase)
    {
        Volatile.Write(ref this._state, new PhaseState(phase));
    }

    /// <summary>
    /// Clears the current phase, e.g. for transition phases which count no kills.
    /// </summary>
    public void ClearPhase()
    {
        Volatile.Write(ref this._state, null);
    }

    /// <summary>
    /// Registers a monster kill.
    /// </summary>
    /// <param name="killed">The definition of the killed monster.</param>
    /// <returns>The outcome, carrying the phase it was counted for.</returns>
    public KanturuKillResult RegisterKill(MonsterDefinition? killed)
    {
        var state = Volatile.Read(ref this._state);
        var phase = state?.Phase;
        if (!KanturuMonsterComparer.IsCountedMonster(killed, phase) || phase is null || state is null)
        {
            return new KanturuKillResult(false, this.KillCount, false, false, phase?.Kind == KanturuPhaseKind.Nightmare, phase);
        }

        var killCount = state.RegisterKill();
        var phaseComplete = killCount >= phase.KillTarget;
        if (phaseComplete)
        {
            state.Completion.TrySetResult();
        }

        var bossKilled = phase.Kind == KanturuPhaseKind.Nightmare
            && KanturuMonsterComparer.IsSameMonster(phase.Nightmare?.Monster, killed);

        return new KanturuKillResult(true, killCount, phaseComplete, bossKilled, phase.Kind == KanturuPhaseKind.Nightmare, phase);
    }

    /// <summary>
    /// One generation of kill counting. Swapped atomically, never mutated in place
    /// except for its own counter.
    /// </summary>
    private sealed class PhaseState
    {
        private int _killCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhaseState"/> class.
        /// </summary>
        /// <param name="phase">The phase to track.</param>
        public PhaseState(KanturuPhaseDefinition? phase)
        {
            this.Phase = phase;
            this.Completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Gets the tracked phase.
        /// </summary>
        public KanturuPhaseDefinition? Phase { get; }

        /// <summary>
        /// Gets the completion source of the kill target.
        /// </summary>
        public TaskCompletionSource Completion { get; }

        /// <summary>
        /// Gets the kill count.
        /// </summary>
        public int KillCount => this._killCount;

        /// <summary>
        /// Registers a kill on this generation.
        /// </summary>
        /// <returns>The kill count after registration.</returns>
        public int RegisterKill() => Interlocked.Increment(ref this._killCount);
    }
}
