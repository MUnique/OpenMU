// <copyright file="WeeklyPeriod.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// Calculates the weekly periods of the weekly quests.
/// </summary>
public static class WeeklyPeriod
{
    /// <summary>
    /// The period start of the quests which are done <see cref="QuestPeriod.Once"/>.
    /// It's a fixed date instead of <see cref="DateTime.MinValue"/>, which the database may store as "-infinity".
    /// </summary>
    public static readonly DateTime OncePeriodStartUtc = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the starts (UTC) of the periods which contain the specified point in time.
    /// </summary>
    /// <param name="utcNow">The point in time (UTC).</param>
    /// <param name="configuration">The configuration with the reset times.</param>
    /// <param name="timeZone">The time zone of the server.</param>
    /// <returns>The starts of the periods.</returns>
    public static QuestPeriodStarts GetPeriodStarts(DateTime utcNow, WeeklyQuestsConfiguration configuration, TimeZoneInfo timeZone)
    {
        return new QuestPeriodStarts(
            GetPeriodStartUtc(utcNow, configuration.ResetDay, configuration.ResetTime, timeZone),
            GetDailyPeriodStartUtc(utcNow, configuration.DailyResetTime, timeZone));
    }

    /// <summary>
    /// Gets the start (UTC) of the daily period which contains the specified point in time.
    /// </summary>
    /// <param name="utcNow">The point in time (UTC).</param>
    /// <param name="resetTime">The time of the day, in the <paramref name="timeZone"/>, on which a period starts.</param>
    /// <param name="timeZone">The time zone of the server.</param>
    /// <returns>The start of the period as UTC.</returns>
    public static DateTime GetDailyPeriodStartUtc(DateTime utcNow, TimeOnly resetTime, TimeZoneInfo timeZone)
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc), timeZone);
        var localStart = localNow.Date.Add(resetTime.ToTimeSpan());
        if (localStart > localNow)
        {
            localStart = localStart.AddDays(-1);
        }

        return ToUtc(localStart, timeZone);
    }

    /// <summary>
    /// Gets the start (UTC) of the daily period which follows the period with the specified start.
    /// </summary>
    /// <param name="periodStartUtc">The start of the current daily period (UTC).</param>
    /// <param name="timeZone">The time zone of the server.</param>
    /// <returns>The start of the next daily period as UTC.</returns>
    public static DateTime GetNextDailyPeriodStartUtc(DateTime periodStartUtc, TimeZoneInfo timeZone)
    {
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(periodStartUtc, DateTimeKind.Utc), timeZone);
        return ToUtc(localStart.AddDays(1), timeZone);
    }

    /// <summary>
    /// Gets the start (UTC) of the period which contains the specified point in time.
    /// </summary>
    /// <param name="utcNow">The point in time (UTC).</param>
    /// <param name="resetDay">The day of the week on which a period starts.</param>
    /// <param name="resetTime">The time of the day, in the <paramref name="timeZone"/>, on which a period starts.</param>
    /// <param name="timeZone">The time zone of the server.</param>
    /// <returns>The start of the period as UTC.</returns>
    public static DateTime GetPeriodStartUtc(DateTime utcNow, DayOfWeek resetDay, TimeOnly resetTime, TimeZoneInfo timeZone)
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc), timeZone);
        var daysSinceResetDay = ((int)localNow.DayOfWeek - (int)resetDay + 7) % 7;
        var localStart = localNow.Date.AddDays(-daysSinceResetDay).Add(resetTime.ToTimeSpan());
        if (localStart > localNow)
        {
            localStart = localStart.AddDays(-7);
        }

        return ToUtc(localStart, timeZone);
    }

    /// <summary>
    /// Gets the start (UTC) of the period which follows the period with the specified start.
    /// </summary>
    /// <param name="periodStartUtc">The start of the current period (UTC).</param>
    /// <param name="timeZone">The time zone of the server.</param>
    /// <returns>The start of the next period as UTC.</returns>
    public static DateTime GetNextPeriodStartUtc(DateTime periodStartUtc, TimeZoneInfo timeZone)
    {
        // Adding the week in local time keeps the configured time of day across daylight saving time changes.
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(periodStartUtc, DateTimeKind.Utc), timeZone);
        return ToUtc(localStart.AddDays(7), timeZone);
    }

    private static DateTime ToUtc(DateTime local, TimeZoneInfo timeZone)
    {
        local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

        // A time which is skipped by a daylight saving time change doesn't exist, so we take the next valid one.
        while (timeZone.IsInvalidTime(local))
        {
            local = local.AddMinutes(30);
        }

        return TimeZoneInfo.ConvertTimeToUtc(local, timeZone);
    }
}
