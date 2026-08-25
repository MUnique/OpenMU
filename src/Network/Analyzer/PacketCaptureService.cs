// <copyright file="PacketCaptureService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The implementation of the <see cref="IPacketCaptureService"/>, which collects the
/// connections of all servers of this process which implement <see cref="IConnectionSource"/>.
/// </summary>
/// <remarks>
/// In a distributed deployment, the servers of other processes are just proxies which don't
/// implement <see cref="IConnectionSource"/> - their connections are simply not listed.
/// </remarks>
public sealed class PacketCaptureService : IPacketCaptureService
{
    private readonly IServerProvider _serverProvider;

    private readonly int _maximumPacketCount;

    private readonly ConcurrentDictionary<Guid, RunningCapture> _runningCaptures = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketCaptureService"/> class.
    /// </summary>
    /// <param name="serverProvider">The provider of the servers of this process.</param>
    /// <param name="maximumPacketCount">The maximum number of packets which are kept in
    /// memory per capture.</param>
    public PacketCaptureService(IServerProvider serverProvider, int maximumPacketCount = LiveCapturedConnection.DefaultMaximumPacketCount)
    {
        this._serverProvider = serverProvider;
        this._maximumPacketCount = maximumPacketCount;
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<ICapturedConnectionInfo>> GetConnectionsAsync()
    {
        var result = new List<ICapturedConnectionInfo>();
        foreach (var source in this._serverProvider.Servers.OfType<IConnectionSource>())
        {
            result.AddRange(await source.GetConnectionsAsync().ConfigureAwait(false));
        }

        return result;
    }

    /// <inheritdoc />
    public async ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(Guid connectionId)
    {
        var connections = await this.GetConnectionsAsync().ConfigureAwait(false);
        return connections.FirstOrDefault(connection => connection.Id == connectionId);
    }

    /// <inheritdoc />
    public async ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(int serverId, string accountOrCharacterName)
    {
        var connections = await this.GetConnectionsAsync().ConfigureAwait(false);
        return connections.FirstOrDefault(connection => connection.ServerId == serverId
                                                        && (string.Equals(connection.CharacterName, accountOrCharacterName, StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(connection.AccountName, accountOrCharacterName, StringComparison.OrdinalIgnoreCase)));
    }

    /// <inheritdoc />
    public async ValueTask<ILiveCapturedConnection?> StartCaptureAsync(Guid connectionId)
    {
        if (this._runningCaptures.TryGetValue(connectionId, out var running))
        {
            running.AddInterestedParty();
            return running.Capture;
        }

        if (await this.FindConnectionAsync(connectionId).ConfigureAwait(false) is not { } connectionInfo)
        {
            return null;
        }

        var capture = new LiveCapturedConnection(connectionInfo, this._maximumPacketCount);
        var newRunning = new RunningCapture(connectionInfo, capture);
        var current = this._runningCaptures.GetOrAdd(connectionId, newRunning);
        if (!ReferenceEquals(current, newRunning))
        {
            // Another caller was faster.
            current.AddInterestedParty();
            return current.Capture;
        }

        connectionInfo.AddCaptureSink(capture);
        return capture;
    }

    /// <inheritdoc />
    public void StopCapture(Guid connectionId)
    {
        if (!this._runningCaptures.TryGetValue(connectionId, out var running)
            || running.RemoveInterestedParty() > 0)
        {
            return;
        }

        if (this._runningCaptures.TryRemove(connectionId, out _))
        {
            running.ConnectionInfo.RemoveCaptureSink(running.Capture);
        }
    }

    /// <inheritdoc />
    public ILiveCapturedConnection? GetRunningCapture(Guid connectionId)
    {
        return this._runningCaptures.TryGetValue(connectionId, out var running) ? running.Capture : null;
    }

    private sealed class RunningCapture
    {
        private int _interestedParties = 1;

        public RunningCapture(ICapturedConnectionInfo connectionInfo, LiveCapturedConnection capture)
        {
            this.ConnectionInfo = connectionInfo;
            this.Capture = capture;
        }

        public ICapturedConnectionInfo ConnectionInfo { get; }

        public LiveCapturedConnection Capture { get; }

        public void AddInterestedParty()
        {
            Interlocked.Increment(ref this._interestedParties);
        }

        public int RemoveInterestedParty()
        {
            return Interlocked.Decrement(ref this._interestedParties);
        }
    }
}
