// <copyright file="PerServerSchedule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// The time of the next run of a periodic task, kept for each game server on its own.
/// </summary>
/// <remarks>
/// A plugin instance is shared by all game servers of a process, while each of them calls
/// <see cref="IPeriodicTaskPlugIn.ExecuteTaskAsync"/> with its own timer. With a single time of the next run in the
/// plugin, the server whose timer fires first after it takes the run and moves the time on, so the others skip it:
/// with N game servers, each one ran the task only about every N-th interval, irregularly.
/// </remarks>
public sealed class PerServerSchedule
{
    private readonly ConditionalWeakTable<IGameContext, StrongBox<long>> _dueTicks = new();

    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerServerSchedule"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider; <see cref="TimeProvider.System"/> if not given.</param>
    public PerServerSchedule(TimeProvider? timeProvider = null)
    {
        this._timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Starts a run on the given game server, if it's due there. A server is due right away at its first call.
    /// </summary>
    /// <param name="gameContext">The game context of the server.</param>
    /// <param name="interval">The time until the next run on this server.</param>
    /// <returns><c>true</c>, if the run is due and the caller carries it out; otherwise, <c>false</c>.</returns>
    public bool TryStartRun(IGameContext gameContext, TimeSpan interval)
    {
        var dueTicks = this.GetDueTicks(gameContext);
        var due = Volatile.Read(ref dueTicks.Value);
        var now = this._timeProvider.GetUtcNow().UtcTicks;
        if (now < due)
        {
            return false;
        }

        // The timer of a server doesn't wait for its previous call, so calls may overlap: only one of them gets the run.
        return Interlocked.CompareExchange(ref dueTicks.Value, now + interval.Ticks, due) == due;
    }

    /// <summary>
    /// Sets the time of the next run on the given game server, counted from now.
    /// </summary>
    /// <param name="gameContext">The game context of the server.</param>
    /// <param name="delay">The time until the next run on this server.</param>
    public void ScheduleNextRun(IGameContext gameContext, TimeSpan delay)
    {
        Volatile.Write(ref this.GetDueTicks(gameContext).Value, this._timeProvider.GetUtcNow().UtcTicks + delay.Ticks);
    }

    /// <summary>
    /// Makes the next run due right away, on all game servers.
    /// </summary>
    public void ForceStart()
    {
        foreach (var entry in this._dueTicks)
        {
            Volatile.Write(ref entry.Value.Value, 0);
        }
    }

    private StrongBox<long> GetDueTicks(IGameContext gameContext)
        => this._dueTicks.GetValue(gameContext, _ => new StrongBox<long>(0));
}
