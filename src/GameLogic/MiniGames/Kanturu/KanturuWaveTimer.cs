// <copyright file="KanturuWaveTimer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tracks the shared countdown of a Kanturu wave. All phases of one wave (e.g. its
/// monsters and its boss) share a single clock: the first phase of the group which
/// carries a <see cref="KanturuPhaseDefinition.TimeLimit"/> starts it, and every later
/// phase of the group inherits the remaining time instead of getting a fresh timer.
/// </summary>
/// <remarks>
/// Only the game loop thread calls this; no locking is needed.
/// </remarks>
internal sealed class KanturuWaveTimer
{
    private readonly Dictionary<KanturuWaveGroup, DateTime> _deadlines = new();
    private readonly Func<DateTime> _utcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuWaveTimer"/> class.
    /// </summary>
    /// <param name="utcNow">The clock, for tests. Defaults to <see cref="DateTime.UtcNow"/>.</param>
    public KanturuWaveTimer(Func<DateTime>? utcNow = null)
    {
        this._utcNow = utcNow ?? (() => DateTime.UtcNow);
    }

    /// <summary>
    /// Gets the effective time limit of the phase: its own <see cref="KanturuPhaseDefinition.TimeLimit"/>,
    /// or the remaining shared time when it follows a wave clock. The first phase of the wave which
    /// carries a limit starts the clock; a later phase of the wave without a recorded clock falls back
    /// to its own limit (usually <c>null</c>, meaning no limit).
    /// </summary>
    /// <param name="phase">The phase which is about to run.</param>
    /// <returns>The effective limit, or <c>null</c> when the phase has no countdown.</returns>
    public TimeSpan? GetEffectiveLimit(KanturuPhaseDefinition phase)
    {
        if (phase.TimeLimitGroup is not { } group)
        {
            return phase.TimeLimit;
        }

        if (this._deadlines.TryGetValue(group, out var deadline))
        {
            return deadline - this._utcNow();
        }

        if (phase.TimeLimit is not { } limit)
        {
            return null;
        }

        this._deadlines[group] = this._utcNow() + limit;
        return limit;
    }
}
