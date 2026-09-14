// <copyright file="NetworkObservationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// Tests for the archiving of the traffic of observed accounts.
/// </summary>
[TestFixture]
public class NetworkObservationTests
{
    private static readonly byte[] TestPacket = [0xC1, 0x06, 0xF1, 0x01, 0x01, 0x02];

    private string _archivePath = null!;

    /// <summary>
    /// Creates the archive directory of the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._archivePath = Path.Combine(Path.GetTempPath(), "openmu-observation-test-" + Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Removes the archive directory of the test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(this._archivePath))
        {
            Directory.Delete(this._archivePath, true);
        }
    }

    /// <summary>
    /// Tests if the traffic of a player is archived when the observation of its account is
    /// active.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task TrafficOfAnObservedAccountIsArchivedAsync()
    {
        var archive = this.CreateArchive();
        var handler = CreateHandler(archive);
        var (player, connection) = CreateRemotePlayer();
        handler.Watch(player);

        await player.SetAccountAsync(CreateAccount(isObserved: true)).ConfigureAwait(false);

        Assert.That(connection.Sinks, Has.Count.EqualTo(1), "The archive should be a sink of the connection.");
        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions, Has.Count.EqualTo(1));
        Assert.That(sessions[0].Metadata.AccountName, Is.EqualTo("ObservedAccount"));
        Assert.That(sessions[0].IsRunning, Is.True);

        connection.Sinks[0].PacketCaptured(TestPacket, false);
        var session = await this.WaitForPacketsAsync(archive, 1).ConfigureAwait(false);
        Assert.That(session.PacketList[0].PacketData, Is.EqualTo("C1 06 F1 01 01 02"));
    }

    /// <summary>
    /// Tests if the traffic of a player is not archived when the observation of its account
    /// is not active - which is the case for every account by default.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task TrafficOfAnUnobservedAccountIsNotArchivedAsync()
    {
        var archive = this.CreateArchive();
        var handler = CreateHandler(archive);
        var (player, connection) = CreateRemotePlayer();
        handler.Watch(player);

        await player.SetAccountAsync(CreateAccount(isObserved: false)).ConfigureAwait(false);

        Assert.That(connection.Sinks, Is.Empty, "Nothing should be captured.");
        Assert.That(await archive.GetSessionsAsync().ConfigureAwait(false), Is.Empty);
    }

    /// <summary>
    /// Tests if the archived session is finished when the player disconnects.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SessionIsFinishedWhenThePlayerDisconnectsAsync()
    {
        var archive = this.CreateArchive();
        var handler = CreateHandler(archive);
        var (player, connection) = CreateRemotePlayer();
        handler.Watch(player);

        // That's the state a player is in while it's at the login screen of the client.
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.SetAccountAsync(CreateAccount(isObserved: true)).ConfigureAwait(false);

        await player.DisconnectAsync().ConfigureAwait(false);

        Assert.That(connection.Sinks, Is.Empty, "The archive should not be a sink anymore.");
        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions, Has.Count.EqualTo(1));
        Assert.That(sessions[0].IsRunning, Is.False);
        Assert.That(sessions[0].Metadata.EndTimestamp, Is.Not.Null);
    }

    /// <summary>
    /// Tests if the options of the observation are taken from the system configuration.
    /// </summary>
    [Test]
    public void OptionsAreTakenFromTheSystemConfiguration()
    {
        var configuration = new SystemConfiguration
        {
            NetworkObservationArchivePath = "/tmp/openmu-captures",
            NetworkObservationMaxSessionSizeMb = 5,
            NetworkObservationMaxTotalSizeMb = 100,
            NetworkObservationRetentionDays = 7,
        };

        var options = NetworkObservationExtensions.CreateOptions(configuration);

        Assert.That(options.ArchivePath, Is.EqualTo("/tmp/openmu-captures"));
        Assert.That(options.MaximumSessionSizeMb, Is.EqualTo(5));
        Assert.That(options.MaximumTotalSizeMb, Is.EqualTo(100));
        Assert.That(options.RetentionDays, Is.EqualTo(7));
    }

    /// <summary>
    /// Tests if the defaults are used for the values which are not configured - an existing
    /// database has no values for them until they are saved once.
    /// </summary>
    [Test]
    public void UnconfiguredOptionsFallBackToTheDefaults()
    {
        var configuration = new SystemConfiguration
        {
            NetworkObservationArchivePath = null,
            NetworkObservationMaxSessionSizeMb = 0,
            NetworkObservationMaxTotalSizeMb = 0,
            NetworkObservationRetentionDays = 0,
        };

        var options = NetworkObservationExtensions.CreateOptions(configuration);

        Assert.That(options.ArchivePath, Is.EqualTo(NetworkObservationOptions.DefaultArchivePath));
        Assert.That(options.MaximumSessionSizeMb, Is.EqualTo(NetworkObservationOptions.DefaultMaximumSessionSizeMb));
        Assert.That(options.MaximumTotalSizeMb, Is.EqualTo(NetworkObservationOptions.DefaultMaximumTotalSizeMb));
        Assert.That(options.RetentionDays, Is.EqualTo(NetworkObservationOptions.DefaultRetentionDays));
    }

    private static NetworkObservationHandler CreateHandler(IPacketArchive archive)
    {
        return new NetworkObservationHandler(archive, 1, "Test Server", new NullLogger<NetworkObservationHandler>());
    }

    private static Account CreateAccount(bool isObserved)
    {
        return new Account
        {
            LoginName = isObserved ? "ObservedAccount" : "NormalAccount",
            IsNetworkObservationActive = isObserved,
        };
    }

    private static (RemotePlayer Player, TestConnection Connection) CreateRemotePlayer()
    {
        var connection = new TestConnection();
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(new Mock<IPersistenceContextProvider>().Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(manager);
        gameContext.Setup(c => c.LoggerFactory).Returns(new NullLoggerFactory());
        return (new RemotePlayer(gameContext.Object, connection, default), connection);
    }

    private PacketArchive CreateArchive()
    {
        var options = new NetworkObservationOptions { ArchivePath = this._archivePath };
        return new PacketArchive(options, new NullLogger<PacketArchive>());
    }

    private async ValueTask<ArchivedSession> WaitForPacketsAsync(PacketArchive archive, int packetCount)
    {
        // The packets are written by another task, so the file needs a moment to catch up.
        var watch = Stopwatch.StartNew();
        ArchivedSession? session = null;
        while (watch.Elapsed < TimeSpan.FromSeconds(10))
        {
            var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
            session = await ArchivedSession.LoadAsync(sessions[0], 0).ConfigureAwait(false);
            if (session.PacketList.Count >= packetCount)
            {
                return session;
            }

            await Task.Delay(50).ConfigureAwait(false);
        }

        return session!;
    }

    /// <summary>
    /// A connection which just keeps the registered capture sinks.
    /// </summary>
    private sealed class TestConnection : IConnection
    {
        private readonly MemoryStream _output = new();

        public TestConnection()
        {
            this.Output = PipeWriter.Create(this._output, new StreamPipeWriterOptions(leaveOpen: true));
        }

        /// <summary>
        /// Occurs when a packet got received. It's never raised by this test double.
        /// </summary>
        public event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived
        {
            add { /* not raised */ }
            remove { /* not raised */ }
        }

        /// <summary>
        /// Occurs when the client disconnected. It's never raised by this test double.
        /// </summary>
        public event AsyncEventHandler? Disconnected
        {
            add { /* not raised */ }
            remove { /* not raised */ }
        }

        public IList<IPacketCaptureSink> Sinks { get; } = new List<IPacketCaptureSink>();

        public Guid Id { get; } = Guid.NewGuid();

        public bool Connected => true;

        public EndPoint? EndPoint => null;

        public EndPoint? LocalEndPoint => null;

        public PipeWriter Output { get; }

        public AsyncLock OutputLock { get; } = new();

        public void AddCaptureSink(IPacketCaptureSink sink) => this.Sinks.Add(sink);

        public void RemoveCaptureSink(IPacketCaptureSink sink) => this.Sinks.Remove(sink);

        public Task BeginReceiveAsync() => Task.CompletedTask;

        public ValueTask DisconnectAsync() => ValueTask.CompletedTask;

        public void Dispose()
        {
            this._output.Dispose();
        }
    }
}
