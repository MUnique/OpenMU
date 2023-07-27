// <copyright file="PeriodicTaskConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Configuration for the <see cref="PeriodicTaskBasePlugIn{TConfiguration,TState}"/>.
/// </summary>
public class PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets or sets a timetable for the event.
    /// </summary>
    public IList<TimeOnly> Timetable { get; set; } = new List<TimeOnly>();

    /// <summary>
    /// Gets or sets a time delay.
    /// </summary>
    public TimeSpan PreStartMessageDelay { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the minimum time of the task duration.
    /// </summary>
    public TimeSpan TaskDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the text which prints as a golden message in the game.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Generate a sequence of time points like [00:00, 00:01, ...].
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <param name="startLimit">The start limit, inclusive.</param>
    /// <param name="endLimit">The end limit, exclusive.</param>
    public static IEnumerable<TimeOnly> GenerateTimeSequence(TimeSpan duration, TimeOnly? startLimit = null, TimeOnly? endLimit = null)
    {
        var limit = endLimit?.ToTimeSpan() ?? TimeSpan.FromDays(1);
        var current = startLimit?.ToTimeSpan() ?? TimeSpan.Zero;

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
    public virtual bool IsItTimeToStart()
    {
        if (this.Timetable.Count == 0)
        {
            return false;
        }

        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        var earlier = nowTime.Add(TimeSpan.FromSeconds(-5));

        // For example, p = 00:00. Check that time between 00:00:00 and 00:00:05
        return this.Timetable.Any(p => p.IsBetween(earlier, nowTime));
    }
}