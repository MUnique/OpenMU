// <copyright file="KanturuTowerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the persistent Tower of Refinement window.
/// </summary>
[TestFixture]
public class KanturuTowerTests
{
    /// <summary>
    /// Tests that the tower window survives a plug-in configuration round trip.
    /// </summary>
    [Test]
    public void TowerConfiguration_RoundTripsThroughPlugInConfiguration()
    {
        var configuration = new PlugInConfiguration();
        var startConfiguration = new KanturuStartConfiguration
        {
            TowerOpenDuration = TimeSpan.FromHours(12),
            TowerOpenUntilUtc = new DateTime(2026, 9, 23, 12, 0, 0, DateTimeKind.Utc),
        };

        configuration.SetConfiguration(startConfiguration, null);
        var restored = configuration.GetConfiguration<KanturuStartConfiguration>(null);

        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.TowerOpenDuration, Is.EqualTo(TimeSpan.FromHours(12)));
        Assert.That(restored.TowerOpenUntilUtc, Is.EqualTo(startConfiguration.TowerOpenUntilUtc));
    }

    /// <summary>
    /// Tests that the tower definition keys identically to the event definition,
    /// but carries the remaining window as its game duration.
    /// </summary>
    [Test]
    public void CreateTowerDefinition_KeysLikeEventDefinition_WithTowerTimers()
    {
        var source = new MiniGameDefinition
        {
            Type = MiniGameType.Kanturu,
            Name = "Kanturu Refinery Tower",
            GameLevel = 1,
            MapCreationPolicy = MiniGameMapCreationPolicy.Shared,
            Entrance = new ExitGate { Map = new GameMapDefinition { Number = 39 } },
            MaximumPlayerCount = 10,
            AllowParty = true,
            GameDuration = TimeSpan.FromMinutes(135),
        };
        var remaining = TimeSpan.FromHours(11);

        var tower = KanturuTowerEntry.CreateTowerDefinition(source, remaining);

        Assert.That(MiniGameMapKey.Create(tower, null!), Is.EqualTo(MiniGameMapKey.Create(source, null!)));
        Assert.That(tower.GameDuration, Is.EqualTo(remaining));
        Assert.That(tower.EnterDuration, Is.EqualTo(TimeSpan.Zero));
        Assert.That(tower.ExitDuration, Is.EqualTo(TimeSpan.Zero));
        Assert.That(tower.Rewards, Is.Empty);
        Assert.That(tower.SpawnWaves, Is.Empty);
        Assert.That(tower.ChangeEvents, Is.Empty);
    }

    /// <summary>
    /// Tests that tower entry doesn't apply without a stored window.
    /// </summary>
    [Test]
    public void GetRemainingTowerWindow_WithoutStoredWindow_ReturnsNull()
    {
        var player = CreatePlayer();

        var remaining = KanturuTowerEntry.GetRemainingTowerWindow(player, new MiniGameDefinition());

        Assert.That(remaining, Is.Null);
    }

    /// <summary>
    /// Tests that no tower game is ensured without a stored window.
    /// </summary>
    [Test]
    public async Task EnsureTowerGameAsync_WithoutStoredWindow_ReturnsFalse()
    {
        var player = CreatePlayer();

        var ensured = await KanturuTowerEntry.EnsureTowerGameAsync(player, new MiniGameDefinition()).ConfigureAwait(false);

        Assert.That(ensured, Is.False);
    }

    /// <summary>
    /// Tests that the tower entry point comes from the configured transition.
    /// </summary>
    [Test]
    public void GetTowerEntryPoint_WithTransition_ReturnsEntryPoint()
    {
        var definition = new KanturuEventDefinition
        {
            Phases = new List<KanturuPhaseDefinition>
            {
                new()
                {
                    Kind = KanturuPhaseKind.Transition,
                    Transition = new KanturuTransitionDefinition { EntryPointX = 79, EntryPointY = 98 },
                },
            },
        };

        var entryPoint = KanturuTowerEntry.GetTowerEntryPoint(definition);

        Assert.That(entryPoint, Is.EqualTo(new Point(79, 98)));
    }

    /// <summary>
    /// Tests that no entry point is returned without a configured transition.
    /// </summary>
    [Test]
    public void GetTowerEntryPoint_WithoutTransition_ReturnsNull()
    {
        Assert.That(KanturuTowerEntry.GetTowerEntryPoint(new KanturuEventDefinition()), Is.Null);
    }

    /// <summary>
    /// Tests that disposing the running games (game master restart) clears the tower window,
    /// so the forced start below isn't blocked by it.
    /// </summary>
    [Test]
    public async Task DisposeRunningGamesAsync_ClearsTowerWindow()
    {
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var startPlugIn = new KanturuStartPlugIn
        {
            Configuration = new KanturuStartConfiguration { TowerOpenUntilUtc = DateTime.UtcNow.AddHours(1) },
        };
        manager.RegisterPlugInAtPlugInPoint<IPeriodicMiniGameStartPlugIn>(startPlugIn);

        var miniGamesMock = new Mock<IMiniGameManager>();
        miniGamesMock
            .Setup(m => m.GetRunningMiniGames(It.IsAny<MiniGameType>()))
            .Returns(new List<MiniGameContext>());

        var contextMock = new Mock<IGameContext>();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(new GameConfiguration());
        contextMock.SetupGet(c => c.PersistenceContextProvider).Returns(new InMemoryPersistenceContextProvider());
        contextMock.SetupGet(c => c.PlugInManager).Returns(manager);
        contextMock.SetupGet(c => c.MiniGames).Returns(miniGamesMock.Object);

        await startPlugIn.DisposeRunningGamesAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(startPlugIn.Configuration!.TowerOpenUntilUtc, Is.Null);
    }

    private static Player CreatePlayer()
    {
        var contextMock = new Mock<IGameContext>();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(new GameConfiguration());
        contextMock.SetupGet(c => c.PersistenceContextProvider).Returns(new InMemoryPersistenceContextProvider());
        contextMock.SetupGet(c => c.PlugInManager).Returns(new PlugInManager(null, NullLoggerFactory.Instance, null, null));
        return new Player(contextMock.Object);
    }
}
