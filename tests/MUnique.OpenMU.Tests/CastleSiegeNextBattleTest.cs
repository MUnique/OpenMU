// <copyright file="CastleSiegeNextBattleTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Tests for <see cref="CastleSiegeSchedule.GetNextPeriod"/>, which tells when the next battle takes place.
/// </summary>
[TestFixture]
public class CastleSiegeNextBattleTest
{
    private static readonly CastleSiegeSchedule Schedule = new(new[]
    {
        new CastleSiegeStateScheduleEntry { State = CastleSiegeState.RegisterGuild, DayOfWeek = DayOfWeek.Monday, Hour = 0 },
        new CastleSiegeStateScheduleEntry { State = CastleSiegeState.Start, DayOfWeek = DayOfWeek.Saturday, Hour = 20 },
        new CastleSiegeStateScheduleEntry { State = CastleSiegeState.End, DayOfWeek = DayOfWeek.Saturday, Hour = 22 },
    });

    /// <summary>
    /// Tests that the battle of this week is the next one before it starts.
    /// </summary>
    [Test]
    public void BattleOfThisWeekIsNext()
    {
        // Wednesday, 2026-10-07
        var period = Schedule.GetNextPeriod(CastleSiegeState.Start, new DateTime(2026, 10, 7, 12, 0, 0, DateTimeKind.Utc));

        Assert.That(period?.StartUtc, Is.EqualTo(new DateTime(2026, 10, 10, 20, 0, 0, DateTimeKind.Utc)));
        Assert.That(period?.EndUtc, Is.EqualTo(new DateTime(2026, 10, 10, 22, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the battle of the next week is the next one, when the battle of this week already started.
    /// </summary>
    [Test]
    public void BattleOfNextWeekIsNextAfterTheStart()
    {
        var period = Schedule.GetNextPeriod(CastleSiegeState.Start, new DateTime(2026, 10, 10, 21, 0, 0, DateTimeKind.Utc));

        Assert.That(period?.StartUtc, Is.EqualTo(new DateTime(2026, 10, 17, 20, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that there is no next period of a state which isn't scheduled.
    /// </summary>
    [Test]
    public void UnscheduledStateHasNoPeriod()
    {
        Assert.That(Schedule.GetNextPeriod(CastleSiegeState.Notify, DateTime.UtcNow), Is.Null);
    }
}
