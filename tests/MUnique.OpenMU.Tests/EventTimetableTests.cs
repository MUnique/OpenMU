// <copyright file="EventTimetableTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Tests the default event timetables. The recurring events share one rotation in which
/// no two entrances open at the same time, and no event's task duration reaches past its
/// own next start, which would silently swallow that start.
/// </summary>
[TestFixture]
public class EventTimetableTests
{
    /// <summary>
    /// Tests that the default mini game timetables are the staggered four-hour rotation.
    /// </summary>
    [Test]
    public void MiniGameTimetablesMatchTheRotation()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DevilSquareStartConfiguration.Default.Timetable, Is.EqualTo(TimesAt(0, 4, 8, 12, 16, 20)));
            Assert.That(BloodCastleStartConfiguration.Default.Timetable, Is.EqualTo(TimesAt(1, 5, 9, 13, 17, 21)));
            Assert.That(ChaosCastleStartConfiguration.Default.Timetable, Is.EqualTo(TimesAt(2, 6, 10, 14, 18, 22)));
            Assert.That(KanturuStartConfiguration.Default.Timetable, Is.EqualTo(TimesAt(3, 11, 19)));
        });
    }

    /// <summary>
    /// Tests that the default invasion timetables are the staggered four-hour rotation,
    /// offset by half an hour so that they never collide with the mini games.
    /// </summary>
    [Test]
    public void InvasionTimetablesMatchTheRotation()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Golden.Timetable, Is.EqualTo(TimesAtHalfPast(0, 4, 8, 12, 16, 20)));
            Assert.That(RedDragon.Timetable, Is.EqualTo(TimesAtHalfPast(1, 5, 9, 13, 17, 21)));
            Assert.That(WhiteWizard.Timetable, Is.EqualTo(TimesAtHalfPast(2, 6, 10, 14, 18, 22)));
        });
    }

    /// <summary>
    /// Tests that no two events open their entrance in the same minute.
    /// </summary>
    [Test]
    public void NoTwoEventsOpenTheirEntranceAtTheSameTime()
    {
        var collidingTimes = AllDefaults()
            .SelectMany(entry => entry.Configuration.Timetable.Select(time => (Time: time, entry.Name)))
            .GroupBy(entry => entry.Time)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key}: {string.Join(", ", group.Select(entry => entry.Name))}")
            .ToList();

        Assert.That(collidingTimes, Is.Empty);
    }

    /// <summary>
    /// Tests that each event's task duration fits between its own consecutive starts.
    /// The task duration puts the plug-in to sleep for its full length, so a duration
    /// longer than the gap makes it skip the next scheduled start.
    /// </summary>
    [Test]
    public void TaskDurationFitsBetweenConsecutiveStarts()
    {
        Assert.Multiple(() =>
        {
            foreach (var (name, configuration) in AllDefaults())
            {
                Assert.That(
                    configuration.TaskDuration,
                    Is.LessThanOrEqualTo(SmallestGap(configuration.Timetable)),
                    $"{name} would skip starts: its task duration exceeds the gap between two of its own starts.");
            }
        });
    }

    private static PeriodicTaskConfiguration Golden
        => (PeriodicTaskConfiguration)new GoldenInvasionPlugIn().CreateDefaultConfig();

    private static PeriodicTaskConfiguration RedDragon
        => (PeriodicTaskConfiguration)new RedDragonInvasionPlugIn().CreateDefaultConfig();

    private static PeriodicTaskConfiguration WhiteWizard
        => (PeriodicTaskConfiguration)new WhiteWizardInvasionPlugIn().CreateDefaultConfig();

    private static IEnumerable<(string Name, PeriodicTaskConfiguration Configuration)> AllDefaults()
    {
        return MiniGameDefaults().Concat(InvasionDefaults());
    }

    private static IEnumerable<(string Name, PeriodicTaskConfiguration Configuration)> MiniGameDefaults()
    {
        yield return (nameof(DevilSquareStartConfiguration), DevilSquareStartConfiguration.Default);
        yield return (nameof(BloodCastleStartConfiguration), BloodCastleStartConfiguration.Default);
        yield return (nameof(ChaosCastleStartConfiguration), ChaosCastleStartConfiguration.Default);
        yield return (nameof(KanturuStartConfiguration), KanturuStartConfiguration.Default);
    }

    private static IEnumerable<(string Name, PeriodicTaskConfiguration Configuration)> InvasionDefaults()
    {
        yield return (nameof(GoldenInvasionPlugIn), Golden);
        yield return (nameof(RedDragonInvasionPlugIn), RedDragon);
        yield return (nameof(WhiteWizardInvasionPlugIn), WhiteWizard);
    }

    private static List<TimeOnly> TimesAt(params int[] hours)
    {
        return hours.Select(hour => new TimeOnly(hour, 0)).ToList();
    }

    private static List<TimeOnly> TimesAtHalfPast(params int[] hours)
    {
        return hours.Select(hour => new TimeOnly(hour, 30)).ToList();
    }

    /// <summary>
    /// Gets the smallest gap between two consecutive starts, considering the daily wrap-around.
    /// </summary>
    /// <param name="timetable">The timetable.</param>
    /// <returns>The smallest gap.</returns>
    private static TimeSpan SmallestGap(IList<TimeOnly> timetable)
    {
        var sorted = timetable.Order().ToList();
        var smallest = TimeSpan.MaxValue;
        for (var i = 0; i < sorted.Count; i++)
        {
            var next = sorted[(i + 1) % sorted.Count];
            var gap = next > sorted[i]
                ? next - sorted[i]
                : next.Add(TimeSpan.FromDays(1)) - sorted[i];
            smallest = gap < smallest ? gap : smallest;
        }

        return smallest;
    }
}
