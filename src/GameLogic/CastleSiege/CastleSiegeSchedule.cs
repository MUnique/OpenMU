// <copyright file="CastleSiegeSchedule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Provides calculations for the weekly Castle Siege state schedule.
/// </summary>
/// <remarks>
/// The configured day, hour and minute of each state are wall-clock times in the server time zone,
/// like the timetables of the periodic events. All inputs and results are UTC.
/// Because of daylight saving time, a local week is not always 168 hours long, so the
/// boundaries are always converted per concrete week.
/// </remarks>
internal sealed class CastleSiegeSchedule
{
    private static readonly TimeSpan WeekDuration = TimeSpan.FromDays(7);

    private readonly IReadOnlyList<ScheduledState> _states;
    private readonly TimeZoneInfo _serverTimeZone;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeSchedule"/> class.
    /// </summary>
    /// <param name="entries">The configured state schedule.</param>
    /// <param name="serverTimeZone">
    /// The server time zone used to interpret the configured times.
    /// Pass <see cref="TimeZoneInfo.Utc"/> to interpret them as UTC.
    /// </param>
    public CastleSiegeSchedule(IEnumerable<CastleSiegeStateScheduleEntry> entries, TimeZoneInfo serverTimeZone)
    {
        this._serverTimeZone = serverTimeZone;
        this._states = entries
            .Select(entry => new ScheduledState(entry.State, GetStartOfWeek(entry)))
            .OrderBy(entry => entry.StartOfWeek)
            .ToList();

        if (this._states.Count < 2)
        {
            throw new InvalidOperationException("The Castle Siege schedule must contain at least two states.");
        }

        if (this._states.Select(entry => entry.State).Distinct().Count() != this._states.Count)
        {
            throw new InvalidOperationException("The Castle Siege schedule must not contain a state more than once.");
        }

        if (this._states.Select(entry => entry.StartOfWeek).Distinct().Count() != this._states.Count)
        {
            throw new InvalidOperationException("The Castle Siege schedule must not contain multiple states with the same start time.");
        }
    }

    /// <summary>
    /// Gets the number of configured states.
    /// </summary>
    public int Count => this._states.Count;

    /// <summary>
    /// Calculates the state period which contains <paramref name="utcNow"/>.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>The current state period.</returns>
    public CastleSiegeStatePeriod GetCurrentPeriod(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow);
        var weekStart = this.GetLocalWeekStart(utcNow);
        for (var weekOffset = 0; weekOffset >= -1; weekOffset--)
        {
            var week = weekStart.AddDays(7 * weekOffset);
            for (var i = this._states.Count - 1; i >= 0; i--)
            {
                if (this.GetStartUtc(week, this._states[i]) <= utcNow)
                {
                    return this.CreateScheduledPeriod(week, i);
                }
            }
        }

        throw new InvalidOperationException("The current Castle Siege state could not be determined.");
    }

    /// <summary>
    /// Calculates the active event period, including the early registration phase after a completed cycle.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>The active Castle Siege period.</returns>
    public CastleSiegeStatePeriod GetCurrentEventPeriod(DateTime utcNow)
    {
        var scheduledPeriod = this.GetCurrentPeriod(utcNow);
        if (scheduledPeriod.State is not (CastleSiegeState.EndCycle or CastleSiegeState.Idle1 or CastleSiegeState.RegisterGuild))
        {
            return scheduledPeriod;
        }

        if (!this.ContainsState(CastleSiegeState.EndCycle)
            || !this.ContainsState(CastleSiegeState.RegisterGuild)
            || !this.ContainsState(this.GetNextState(CastleSiegeState.RegisterGuild)))
        {
            return scheduledPeriod;
        }

        var endCycleStart = this.GetPreviousOrCurrentStateStart(CastleSiegeState.EndCycle, utcNow);
        return this.CreateEarlyStartPeriod(CastleSiegeState.RegisterGuild, endCycleStart);
    }

    /// <summary>
    /// Creates a period which starts immediately and uses the configured duration of the state.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="startUtc">The UTC start time.</param>
    /// <returns>The created state period.</returns>
    public CastleSiegeStatePeriod CreatePeriod(CastleSiegeState state, DateTime startUtc)
    {
        startUtc = EnsureUtc(startUtc);
        var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, this._serverTimeZone);
        return new(state, startUtc, this.ToUtc(startLocal + this.GetDuration(state)));
    }

    /// <summary>
    /// Creates an early-start period which ends at the next scheduled successor state.
    /// </summary>
    /// <param name="state">The state which starts early.</param>
    /// <param name="startUtc">The UTC start time.</param>
    /// <returns>The created state period.</returns>
    public CastleSiegeStatePeriod CreateEarlyStartPeriod(CastleSiegeState state, DateTime startUtc)
    {
        startUtc = EnsureUtc(startUtc);
        var successor = this.GetScheduledState(this.GetNextState(state));
        var weekStart = this.GetLocalWeekStart(startUtc);
        var endUtc = this.GetStartUtc(weekStart, successor);
        if (endUtc <= startUtc)
        {
            endUtc = this.GetStartUtc(weekStart.AddDays(7), successor);
        }

        return new(state, startUtc, endUtc);
    }

    /// <summary>
    /// Gets the state after <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The current state.</param>
    /// <returns>The next configured state.</returns>
    public CastleSiegeState GetNextState(CastleSiegeState state)
    {
        var index = this.GetStateIndex(state);
        return this._states[(index + 1) % this._states.Count].State;
    }

    /// <summary>
    /// Gets the next scheduled start of a state after <paramref name="utcNow"/>.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="utcNow">The reference time.</param>
    /// <returns>The next state start, or <see langword="null"/> when the schedule does not contain the state.</returns>
    public DateTime? GetNextStateStartUtc(CastleSiegeState state, DateTime utcNow)
    {
        if (!this.ContainsState(state))
        {
            return null;
        }

        utcNow = EnsureUtc(utcNow);
        var scheduledState = this.GetScheduledState(state);
        var weekStart = this.GetLocalWeekStart(utcNow);
        var startUtc = this.GetStartUtc(weekStart, scheduledState);
        return startUtc > utcNow ? startUtc : this.GetStartUtc(weekStart.AddDays(7), scheduledState);
    }

    private static TimeSpan GetStartOfWeek(CastleSiegeStateScheduleEntry entry)
    {
        if (entry.Hour > 23)
        {
            throw new InvalidOperationException($"The Castle Siege schedule contains an invalid hour for state {entry.State}.");
        }

        if (entry.Minute > 59)
        {
            throw new InvalidOperationException($"The Castle Siege schedule contains an invalid minute for state {entry.State}.");
        }

        return TimeSpan.FromDays((int)entry.DayOfWeek)
               + TimeSpan.FromHours(entry.Hour)
               + TimeSpan.FromMinutes(entry.Minute);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
    }

    /// <summary>
    /// Gets the most recent start of a configured state.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="utcNow">The reference time.</param>
    /// <returns>The most recent state start, including <paramref name="utcNow"/>.</returns>
    private DateTime GetPreviousOrCurrentStateStart(CastleSiegeState state, DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow);
        var scheduledState = this.GetScheduledState(state);
        var weekStart = this.GetLocalWeekStart(utcNow);
        var startUtc = this.GetStartUtc(weekStart, scheduledState);
        return startUtc <= utcNow ? startUtc : this.GetStartUtc(weekStart.AddDays(-7), scheduledState);
    }

    /// <summary>
    /// Gets the configured wall-clock duration of a state, which is the distance to the next state in the schedule.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>The duration.</returns>
    private TimeSpan GetDuration(CastleSiegeState state)
    {
        var index = this.GetStateIndex(state);
        var start = this._states[index].StartOfWeek;
        var end = this._states[(index + 1) % this._states.Count].StartOfWeek;
        var duration = end - start;
        return duration > TimeSpan.Zero ? duration : duration + WeekDuration;
    }

    private CastleSiegeStatePeriod CreateScheduledPeriod(DateTime weekStart, int stateIndex)
    {
        var successorIndex = (stateIndex + 1) % this._states.Count;
        var successorWeekStart = successorIndex > stateIndex ? weekStart : weekStart.AddDays(7);
        return new(
            this._states[stateIndex].State,
            this.GetStartUtc(weekStart, this._states[stateIndex]),
            this.GetStartUtc(successorWeekStart, this._states[successorIndex]));
    }

    /// <summary>
    /// Gets the local start of the week (Sunday at midnight, unspecified kind) which contains the UTC time.
    /// </summary>
    private DateTime GetLocalWeekStart(DateTime utc)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utc, this._serverTimeZone);
        return DateTime.SpecifyKind(local.Date.AddDays(-(int)local.DayOfWeek), DateTimeKind.Unspecified);
    }

    private DateTime GetStartUtc(DateTime localWeekStart, ScheduledState state) => this.ToUtc(localWeekStart + state.StartOfWeek);

    /// <summary>
    /// Converts a wall-clock time of the server time zone to UTC.
    /// A time which does not exist because the clock was set forward is moved forward by the length of the gap,
    /// and an ambiguous time resolves to the standard time.
    /// </summary>
    private DateTime ToUtc(DateTime local)
    {
        local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        if (this._serverTimeZone.IsInvalidTime(local))
        {
            return DateTime.SpecifyKind(local - this._serverTimeZone.BaseUtcOffset, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeToUtc(local, this._serverTimeZone);
    }

    private int GetStateIndex(CastleSiegeState state)
    {
        for (var i = 0; i < this._states.Count; i++)
        {
            if (this._states[i].State == state)
            {
                return i;
            }
        }

        throw new InvalidOperationException($"The Castle Siege schedule does not contain state {state}.");
    }

    private bool ContainsState(CastleSiegeState state) => this._states.Any(entry => entry.State == state);

    private ScheduledState GetScheduledState(CastleSiegeState state) => this._states[this.GetStateIndex(state)];

    private readonly record struct ScheduledState(CastleSiegeState State, TimeSpan StartOfWeek);
}
