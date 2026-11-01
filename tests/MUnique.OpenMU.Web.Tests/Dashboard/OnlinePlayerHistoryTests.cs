// <copyright file="OnlinePlayerHistoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="OnlinePlayerHistory"/>.
/// </summary>
[TestFixture]
public class OnlinePlayerHistoryTests
{
    /// <summary>
    /// A sample contains the number of players on the game servers at the time it was taken.
    /// </summary>
    [Test]
    public void SampleContainsOnlinePlayers()
    {
        var timeProvider = new ManualTimeProvider();
        var serverProvider = TestServers.CreateProvider(
            TestServers.Create(ServerType.GameServer, ServerState.Started, 42),
            TestServers.Create(ServerType.ConnectServer, ServerState.Started, 100));
        using var history = new OnlinePlayerHistory(serverProvider, timeProvider, NullLogger<OnlinePlayerHistory>.Instance);

        history.TakeSample();

        Assert.That(history.GetSamples(), Is.EqualTo(new[] { new OnlinePlayerSample(timeProvider.UtcNow.UtcDateTime, 42) }));
    }

    /// <summary>
    /// Samples which are older than the window are removed.
    /// </summary>
    [Test]
    public void SamplesOutsideOfWindowAreRemoved()
    {
        var timeProvider = new ManualTimeProvider();
        var start = timeProvider.UtcNow;
        using var history = new OnlinePlayerHistory(TestServers.CreateProvider(), timeProvider, NullLogger<OnlinePlayerHistory>.Instance);

        history.TakeSample();
        timeProvider.UtcNow = start + OnlinePlayerHistory.Window;
        history.TakeSample();
        timeProvider.UtcNow = start + OnlinePlayerHistory.Window + OnlinePlayerHistory.SampleInterval;
        history.TakeSample();

        Assert.That(
            history.GetSamples().Select(s => s.Timestamp),
            Is.EqualTo(new[] { (start + OnlinePlayerHistory.Window).UtcDateTime, timeProvider.UtcNow.UtcDateTime }));
    }

    /// <summary>
    /// Adding a sample raises the event, so that the dashboard can update its chart.
    /// </summary>
    [Test]
    public void SampleAddedIsRaised()
    {
        using var history = new OnlinePlayerHistory(TestServers.CreateProvider(), new ManualTimeProvider(), NullLogger<OnlinePlayerHistory>.Instance);
        var raised = 0;
        history.SampleAdded += (_, _) => raised++;

        history.TakeSample();

        Assert.That(raised, Is.EqualTo(1));
    }
}
