// <copyright file="PerServerScheduleTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Tests the <see cref="PerServerSchedule"/>, which lets a periodic task run on every game server of a process,
/// although they share the plugin instance.
/// </summary>
[TestFixture]
public class PerServerScheduleTest
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    /// <summary>
    /// A run on one game server doesn't take the run of another one, which calls right after it.
    /// </summary>
    [Test]
    public void EveryServerGetsItsRun()
    {
        var (schedule, _) = CreateSchedule();
        var first = GameContextTestHelper.CreateGameContext();
        var second = GameContextTestHelper.CreateGameContext();

        Assert.That(schedule.TryStartRun(first, Interval), Is.True);
        Assert.That(schedule.TryStartRun(second, Interval), Is.True);
    }

    /// <summary>
    /// A server doesn't run again before its interval passed, and does after it.
    /// </summary>
    [Test]
    public void ServerRunsAgainAfterTheInterval()
    {
        var (schedule, timeProvider) = CreateSchedule();
        var gameContext = GameContextTestHelper.CreateGameContext();

        Assert.That(schedule.TryStartRun(gameContext, Interval), Is.True);
        timeProvider.Advance(TimeSpan.FromSeconds(59));
        Assert.That(schedule.TryStartRun(gameContext, Interval), Is.False);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        Assert.That(schedule.TryStartRun(gameContext, Interval), Is.True);
    }

    /// <summary>
    /// The servers keep their own times, also when they started at different times.
    /// </summary>
    [Test]
    public void ServersKeepTheirOwnTimes()
    {
        var (schedule, timeProvider) = CreateSchedule();
        var first = GameContextTestHelper.CreateGameContext();
        var second = GameContextTestHelper.CreateGameContext();

        schedule.TryStartRun(first, Interval);
        timeProvider.Advance(TimeSpan.FromSeconds(30));
        schedule.TryStartRun(second, Interval);
        timeProvider.Advance(TimeSpan.FromSeconds(30));

        Assert.That(schedule.TryStartRun(first, Interval), Is.True);
        Assert.That(schedule.TryStartRun(second, Interval), Is.False);
    }

    /// <summary>
    /// <see cref="PerServerSchedule.ScheduleNextRun"/> sets the next run from now on.
    /// </summary>
    [Test]
    public void ScheduledRunIsDueAfterTheDelay()
    {
        var (schedule, timeProvider) = CreateSchedule();
        var gameContext = GameContextTestHelper.CreateGameContext();

        schedule.ScheduleNextRun(gameContext, TimeSpan.FromSeconds(10));
        Assert.That(schedule.TryStartRun(gameContext, Interval), Is.False);
        timeProvider.Advance(TimeSpan.FromSeconds(10));
        Assert.That(schedule.TryStartRun(gameContext, Interval), Is.True);
    }

    /// <summary>
    /// <see cref="PerServerSchedule.ForceStart"/> makes the next run due right away, on all servers.
    /// </summary>
    [Test]
    public void ForceStartMakesAllServersDue()
    {
        var (schedule, _) = CreateSchedule();
        var first = GameContextTestHelper.CreateGameContext();
        var second = GameContextTestHelper.CreateGameContext();
        schedule.TryStartRun(first, Interval);
        schedule.TryStartRun(second, Interval);

        schedule.ForceStart();

        Assert.That(schedule.TryStartRun(first, Interval), Is.True);
        Assert.That(schedule.TryStartRun(second, Interval), Is.True);
    }

    /// <summary>
    /// Of two overlapping calls of the same server, only one gets the run.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task OverlappingCallsGetOneRunAsync()
    {
        var (schedule, _) = CreateSchedule();
        var gameContext = GameContextTestHelper.CreateGameContext();

        var results = await Task.WhenAll(Enumerable.Range(0, 32).Select(_ => Task.Run(() => schedule.TryStartRun(gameContext, Interval)))).ConfigureAwait(false);

        Assert.That(results.Count(isStarted => isStarted), Is.EqualTo(1));
    }

    private static (PerServerSchedule Schedule, ManualTimeProvider TimeProvider) CreateSchedule()
    {
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));
        return (new PerServerSchedule(timeProvider), timeProvider);
    }

    private sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow() => this._utcNow;

        public void Advance(TimeSpan timeSpan)
        {
            this._utcNow += timeSpan;
        }
    }
}
