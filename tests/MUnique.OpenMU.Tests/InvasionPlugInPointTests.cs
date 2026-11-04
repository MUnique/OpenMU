// <copyright file="InvasionPlugInPointTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="IInvasionStartedPlugIn"/> and <see cref="IInvasionEndedPlugIn"/>.
/// </summary>
[TestFixture]
public class InvasionPlugInPointTests
{
    /// <summary>
    /// Tests that the start and the end of an invasion are reported with the announced maps.
    /// </summary>
    [Test]
    public async Task StartAndEndAreReportedWithAnnouncedMapsAsync()
    {
        var lorencia = new GameMapDefinition { Number = 0 };
        var devias = new GameMapDefinition { Number = 2 };
        var noria = new GameMapDefinition { Number = 3 };
        var configuration = new Mock<GameConfiguration>();
        configuration.SetupGet(c => c.Maps).Returns(new List<GameMapDefinition> { lorencia, devias, noria });

        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var recorder = new RecordingPlugIn();
        plugInManager.RegisterPlugInAtPlugInPoint<IInvasionStartedPlugIn>(recorder);
        plugInManager.RegisterPlugInAtPlugInPoint<IInvasionEndedPlugIn>(recorder);

        var gameContext = new Mock<IGameContext>();
        gameContext.SetupGet(c => c.Configuration).Returns(configuration.Object);
        gameContext.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        gameContext.SetupGet(c => c.PlugInManager).Returns(plugInManager);

        var invasion = new TestInvasionPlugIn();
        var state = new InvasionGameServerState(gameContext.Object);
        state.SetAnnouncedMaps(new ushort[] { 0, 2 });

        await invasion.StartForTestAsync(state).ConfigureAwait(false);
        await invasion.FinishForTestAsync(state).ConfigureAwait(false);

        Assert.That(recorder.Calls, Is.EqualTo(new[] { "Started", "Ended" }));
        Assert.That(recorder.Invasions, Is.All.SameAs(invasion));
        Assert.That(recorder.Maps, Has.All.EquivalentTo(new[] { lorencia, devias }));
    }

    /// <summary>
    /// An invasion which exposes the lifecycle methods for the test.
    /// </summary>
    private sealed class TestInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>
    {
        public Task StartForTestAsync(InvasionGameServerState state) => this.OnStartedAsync(state).AsTask();

        public Task FinishForTestAsync(InvasionGameServerState state) => this.OnFinishedAsync(state).AsTask();
    }

    [Guid("041CD81F-79DC-4DD3-BEFA-154C7322AFD7")]
    private sealed class RecordingPlugIn : IInvasionStartedPlugIn, IInvasionEndedPlugIn
    {
        public List<string> Calls { get; } = new();

        public List<IPeriodicTaskPlugIn> Invasions { get; } = new();

        public List<IReadOnlyCollection<GameMapDefinition>> Maps { get; } = new();

        public ValueTask InvasionStartedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
        {
            this.Record("Started", invasion, maps);
            return ValueTask.CompletedTask;
        }

        public ValueTask InvasionEndedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
        {
            this.Record("Ended", invasion, maps);
            return ValueTask.CompletedTask;
        }

        private void Record(string call, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
        {
            this.Calls.Add(call);
            this.Invasions.Add(invasion);
            this.Maps.Add(maps);
        }
    }
}
