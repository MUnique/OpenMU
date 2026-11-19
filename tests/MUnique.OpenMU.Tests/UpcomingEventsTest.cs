// <copyright file="UpcomingEventsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Administration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="PeriodicTaskConfiguration.GetNextStartUtc"/> and <see cref="UpcomingEvents"/>.
/// </summary>
[TestFixture]
public class UpcomingEventsTest
{
    private static readonly DateTime Now = new(2026, 10, 8, 12, 30, 0, DateTimeKind.Utc);

    /// <summary>
    /// Tests that the next start is the next time of the timetable today.
    /// </summary>
    [Test]
    public void NextStartIsLaterToday()
    {
        var configuration = CreateConfiguration(new TimeOnly(8, 0), new TimeOnly(14, 0), new TimeOnly(20, 0));

        Assert.That(configuration.GetNextStartUtc(Now, TimeZoneInfo.Utc), Is.EqualTo(new DateTime(2026, 10, 8, 14, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the next start is the first time of the timetable tomorrow, when all times of today passed.
    /// </summary>
    [Test]
    public void NextStartIsTomorrow()
    {
        var configuration = CreateConfiguration(new TimeOnly(8, 0), new TimeOnly(10, 0));

        Assert.That(configuration.GetNextStartUtc(Now, TimeZoneInfo.Utc), Is.EqualTo(new DateTime(2026, 10, 9, 8, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the times of the timetable are in the time zone of the server.
    /// </summary>
    [Test]
    public void NextStartRespectsTheServerTimeZone()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+2", TimeSpan.FromHours(2), "UTC+2", "UTC+2");
        var configuration = CreateConfiguration(new TimeOnly(16, 0));

        Assert.That(configuration.GetNextStartUtc(Now, timeZone), Is.EqualTo(new DateTime(2026, 10, 8, 14, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that there is no next start without timetable.
    /// </summary>
    [Test]
    public void NoNextStartWithoutTimetable()
    {
        Assert.That(CreateConfiguration().GetNextStartUtc(Now, TimeZoneInfo.Utc), Is.Null);
    }

    /// <summary>
    /// Tests that the upcoming events contain the active scheduled plugins with their names, ordered by their start.
    /// </summary>
    [Test]
    public void UpcomingEventsAreOrderedAndNamed()
    {
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new GoldenInvasionPlugIn { Configuration = CreateInvasionConfiguration(new TimeOnly(18, 0)) });
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new RedDragonInvasionPlugIn { Configuration = CreateInvasionConfiguration(new TimeOnly(13, 0)) });

        var events = UpcomingEvents.Get(plugInManager, Now, TimeZoneInfo.Utc, TimeSpan.FromHours(12), CultureInfo.InvariantCulture);

        Assert.That(events.Select(e => e.Name), Is.EqualTo(new[] { "Red Dragon Invasion", "Golden Invasion" }));
        Assert.That(events[0].StartsAtUtc, Is.EqualTo(new DateTime(2026, 10, 8, 13, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that events which start later than the requested time span aren't included.
    /// </summary>
    [Test]
    public void EventsAfterTheTimeSpanAreExcluded()
    {
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new GoldenInvasionPlugIn { Configuration = CreateInvasionConfiguration(new TimeOnly(18, 0)) });

        var events = UpcomingEvents.Get(plugInManager, Now, TimeZoneInfo.Utc, TimeSpan.FromHours(1), CultureInfo.InvariantCulture);

        Assert.That(events, Is.Empty);
    }

    /// <summary>
    /// Tests that the scheduled events of a game server are all active scheduled plugins which run on it,
    /// with their identifier and english name, ordered by their next start.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ScheduledEventsOfGameServerRespectTheServerScopeAsync()
    {
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var golden = CreateInvasionConfiguration(new TimeOnly(18, 0));
        golden.AllowedServerIds = [1];
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new GoldenInvasionPlugIn { Configuration = golden });
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new RedDragonInvasionPlugIn { Configuration = CreateInvasionConfiguration(new TimeOnly(13, 0)) });
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(new HappyHourPlugIn { Configuration = new HappyHourConfiguration { Timetable = [new TimeOnly(23, 0)] } });

        var otherServer = await new GameServerAdministration(CreateServerContext(plugInManager, 0)).GetScheduledEventsAsync(CancellationToken.None).ConfigureAwait(false);
        var goldenServer = await new GameServerAdministration(CreateServerContext(plugInManager, 1)).GetScheduledEventsAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.That(otherServer.Select(e => e.Name), Is.EquivalentTo(new[] { "Red Dragon Invasion", "Happy Hour" }));
        Assert.That(goldenServer.Select(e => e.PlugInId), Does.Contain(typeof(GoldenInvasionPlugIn).GUID));
        Assert.That(goldenServer.Select(e => e.NextStartUtc), Is.Ordered);
    }

    private static IGameServerContext CreateServerContext(PlugInManager plugInManager, byte serverId)
    {
        var context = new Mock<IGameServerContext>();
        context.Setup(c => c.Id).Returns(serverId);
        context.Setup(c => c.PlugInManager).Returns(plugInManager);
        context.Setup(c => c.ServerTimeZone).Returns(TimeZoneInfo.Utc);
        return context.Object;
    }

    private static PeriodicTaskConfiguration CreateConfiguration(params TimeOnly[] times)
    {
        return new PeriodicTaskConfiguration { Timetable = times.ToList() };
    }

    private static PeriodicInvasionConfiguration CreateInvasionConfiguration(params TimeOnly[] times)
    {
        return new PeriodicInvasionConfiguration { Timetable = times.ToList() };
    }
}
