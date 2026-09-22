// <copyright file="PacketCaptureServiceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="PacketCaptureService"/> and the <see cref="LiveCapturedConnection"/>.
/// </summary>
[TestFixture]
public class PacketCaptureServiceTest
{
    /// <summary>
    /// Tests if the connections of all servers which can provide them are collected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ConnectionsOfAllSourcesAreCollectedAsync()
    {
        var gameServerConnection = new TestConnectionInfo(ServerType.GameServer, 1) { CharacterName = "TestCharacter" };
        var connectServerConnection = new TestConnectionInfo(ServerType.ConnectServer, 100);
        var service = CreateService(
            new TestServer(ServerType.GameServer, gameServerConnection),
            new TestServer(ServerType.ConnectServer, connectServerConnection),
            new ServerWithoutConnections());

        var connections = await service.GetConnectionsAsync().ConfigureAwait(false);

        Assert.That(connections, Has.Count.EqualTo(2));
        Assert.That(connections, Does.Contain(gameServerConnection));
        Assert.That(connections, Does.Contain(connectServerConnection));
    }

    /// <summary>
    /// Tests if a connection is found by its identifier.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ConnectionIsFoundByIdAsync()
    {
        var connection = new TestConnectionInfo(ServerType.GameServer, 1);
        var service = CreateService(new TestServer(ServerType.GameServer, connection));

        Assert.That(await service.FindConnectionAsync(connection.Id).ConfigureAwait(false), Is.EqualTo(connection));
        Assert.That(await service.FindConnectionAsync(Guid.NewGuid()).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests if a connection is found by the name of its account or character.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ConnectionIsFoundByNameAsync()
    {
        var connection = new TestConnectionInfo(ServerType.GameServer, 1) { AccountName = "testAccount", CharacterName = "TestCharacter" };
        var service = CreateService(new TestServer(ServerType.GameServer, connection));

        Assert.That(await service.FindConnectionAsync(1, "TestCharacter").ConfigureAwait(false), Is.EqualTo(connection));
        Assert.That(await service.FindConnectionAsync(1, "testaccount").ConfigureAwait(false), Is.EqualTo(connection));
        Assert.That(await service.FindConnectionAsync(2, "TestCharacter").ConfigureAwait(false), Is.Null);
        Assert.That(await service.FindConnectionAsync(1, "Unknown").ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests if a started capture registers itself at the connection, and if it's removed
    /// again when the capture is stopped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task CaptureIsAttachedAndDetachedAsync()
    {
        var connection = new TestConnectionInfo(ServerType.GameServer, 1);
        var service = CreateService(new TestServer(ServerType.GameServer, connection));

        var capture = await service.StartCaptureAsync(connection.Id).ConfigureAwait(false);

        Assert.That(capture, Is.Not.Null);
        Assert.That(connection.Sinks, Has.Count.EqualTo(1));
        Assert.That(service.GetRunningCapture(connection.Id), Is.EqualTo(capture));

        service.StopCapture(connection.Id);

        Assert.That(connection.Sinks, Is.Empty);
        Assert.That(service.GetRunningCapture(connection.Id), Is.Null);
    }

    /// <summary>
    /// Tests if a second interested party gets the same capture, and that the capture is only
    /// stopped when the last one is gone.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task CaptureIsSharedByInterestedPartiesAsync()
    {
        var connection = new TestConnectionInfo(ServerType.GameServer, 1);
        var service = CreateService(new TestServer(ServerType.GameServer, connection));

        var first = await service.StartCaptureAsync(connection.Id).ConfigureAwait(false);
        var second = await service.StartCaptureAsync(connection.Id).ConfigureAwait(false);

        Assert.That(second, Is.EqualTo(first));
        Assert.That(connection.Sinks, Has.Count.EqualTo(1));

        service.StopCapture(connection.Id);
        Assert.That(connection.Sinks, Has.Count.EqualTo(1), "The capture is still watched.");

        service.StopCapture(connection.Id);
        Assert.That(connection.Sinks, Is.Empty);
    }

    /// <summary>
    /// Tests if starting a capture of an unknown connection returns null.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task CaptureOfUnknownConnectionIsNotStartedAsync()
    {
        var service = CreateService(new TestServer(ServerType.GameServer));

        Assert.That(await service.StartCaptureAsync(Guid.NewGuid()).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests if the captured packets are added with the correct direction.
    /// </summary>
    [Test]
    public void CapturedPacketsKeepTheirDirection()
    {
        var capture = new LiveCapturedConnection(new TestConnectionInfo(ServerType.GameServer, 1));

        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x00 }, false);
        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x01 }, true);

        var packets = capture.GetPackets();
        Assert.That(packets, Has.Count.EqualTo(2));
        Assert.That(packets[0].ToServer, Is.True, "A received packet goes to the server.");
        Assert.That(packets[1].ToServer, Is.False, "A sent packet goes to the client.");
    }

    /// <summary>
    /// Tests if the oldest packets are dropped when the maximum count is reached.
    /// </summary>
    [Test]
    public void OldestPacketsAreDroppedWhenBufferIsFull()
    {
        var capture = new LiveCapturedConnection(new TestConnectionInfo(ServerType.GameServer, 1), 3);

        for (byte i = 0; i < 5; i++)
        {
            capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, i }, false);
        }

        var packets = capture.GetPackets();
        Assert.That(packets, Has.Count.EqualTo(3));
        Assert.That(packets.Select(packet => packet.Data[3]), Is.EqualTo(new byte[] { 2, 3, 4 }));
    }

    private static PacketCaptureService CreateService(params IManageableServer[] servers)
    {
        return new PacketCaptureService(new TestServerProvider(servers));
    }

    private sealed class TestConnectionInfo : ICapturedConnectionInfo
    {
        public TestConnectionInfo(ServerType serverType, int serverId)
        {
            this.ServerType = serverType;
            this.ServerId = serverId;
        }

        public IList<IPacketCaptureSink> Sinks { get; } = new List<IPacketCaptureSink>();

        public Guid Id { get; } = Guid.NewGuid();

        public ServerType ServerType { get; }

        public int ServerId { get; }

        public string ServerDescription => $"Test {this.ServerType} {this.ServerId}";

        public string? AccountName { get; init; }

        public string? CharacterName { get; init; }

        public string? RemoteEndPoint => "127.0.0.1:1234";

        public ClientVersion ClientVersion => default;

        public PacketDefinitionSet DefinitionSet => PacketDefinitionSet.GameServer;

        public bool IsConnected => true;

        public string DisplayName => this.CharacterName ?? this.AccountName ?? this.RemoteEndPoint!;

        public void AddCaptureSink(IPacketCaptureSink sink) => this.Sinks.Add(sink);

        public void RemoveCaptureSink(IPacketCaptureSink sink) => this.Sinks.Remove(sink);

        public ValueTask DisconnectAsync() => ValueTask.CompletedTask;
    }

    private class ServerWithoutConnections : IManageableServer
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id => 0;

        public Guid ConfigurationId => Guid.Empty;

        public string Description => "Test";

        public ServerType Type => ServerType.GameServer;

        public ServerState ServerState => ServerState.Started;

        public int MaximumConnections => 100;

        public int CurrentConnections => 0;

        public ValueTask StartAsync() => ValueTask.CompletedTask;

        public ValueTask ShutdownAsync() => ValueTask.CompletedTask;

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private sealed class TestServer : ServerWithoutConnections, IConnectionSource
    {
        private readonly IReadOnlyList<ICapturedConnectionInfo> _connections;

        public TestServer(ServerType serverType, params ICapturedConnectionInfo[] connections)
        {
            this.ServerType = serverType;
            this._connections = connections;
        }

        public ServerType ServerType { get; }

        public ValueTask<IReadOnlyList<ICapturedConnectionInfo>> GetConnectionsAsync()
        {
            return ValueTask.FromResult(this._connections);
        }
    }

    private sealed class TestServerProvider : IServerProvider
    {
        public TestServerProvider(IEnumerable<IManageableServer> servers)
        {
            this.Servers = servers.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add { }
            remove { }
        }

        public IList<IManageableServer> Servers { get; }
    }
}
