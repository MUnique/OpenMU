// <copyright file="PeriodicInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

/// <summary>
/// Abstract configuration for periodic invasions.
/// </summary>
public abstract class PeriodicInvasionConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether event is enabled/disabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a timetable for the event.
    /// </summary>
    public List<TimeOnly> Timetable { get; set; } = new(GenerateTimeSequence(TimeSpan.FromHours(4)));

    /// <summary>
    /// Gets or sets an event's duration.
    /// </summary>
    public TimeSpan EventDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets a time delay.
    /// </summary>
    public TimeSpan PreStartMessageDelay { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the text which prints as a golden message in the game.
    /// </summary>
    public string? Message { get; set; } = "Invasion's been started!";

    /// <summary>
    /// Generate a sequnce of time points like [00:00, 00:01, ...].
    /// </summary>
    /// <param name="duration">The duration.</param>
    public static IEnumerable<TimeOnly> GenerateTimeSequence(TimeSpan duration)
    {
        var limit = TimeSpan.FromDays(1);
        var current = TimeSpan.FromDays(0);

        while (current < limit)
        {
            yield return TimeOnly.FromTimeSpan(current);
            current = current.Add(duration);
        }
    }

    /// <summary>
    /// Check if current time is OK for starting an invasion.
    /// </summary>
    /// <returns>Returns true if the invasion can be started.</returns>
    public bool IsItTimeToStartInvasion()
    {
        if (this.Timetable.Count == 0)
        {
            return true;
        }

        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        var erlier = nowTime.Add(TimeSpan.FromSeconds(-5));

        // For example, p = 00:00. Check that time between 00:00:00 and 00:00:05
        return this.Timetable.Any(p => p.IsBetween(erlier, nowTime));
    }
}
