// <copyright file="LiveCapturedConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.ComponentModel;

/// <summary>
/// Captures the traffic of a connection which is currently handled by one of our servers.
/// </summary>
public sealed class LiveCapturedConnection : ILiveCapturedConnection, IPacketCaptureSink
{
    /// <summary>
    /// The default maximum number of packets which are kept in memory.
    /// </summary>
    public const int DefaultMaximumPacketCount = 5000;

    private readonly object _syncRoot = new();

    private readonly int _maximumPacketCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveCapturedConnection"/> class.
    /// </summary>
    /// <param name="connectionInfo">The information about the captured connection.</param>
    /// <param name="maximumPacketCount">The maximum number of packets which are kept in
    /// memory. When more packets arrive, the oldest ones are dropped.</param>
    public LiveCapturedConnection(ICapturedConnectionInfo connectionInfo, int maximumPacketCount = DefaultMaximumPacketCount)
    {
        this.ConnectionInfo = connectionInfo;
        this._maximumPacketCount = Math.Max(1, maximumPacketCount);
        this.Name = connectionInfo.DisplayName;
    }

    /// <inheritdoc />
    public event EventHandler? PacketsChanged;

    /// <inheritdoc />
    public ICapturedConnectionInfo ConnectionInfo { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public BindingList<Packet> PacketList { get; } = new();

    /// <inheritdoc />
    public DateTime StartTimestamp { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public IReadOnlyList<Packet> GetPackets()
    {
        lock (this._syncRoot)
        {
            return this.PacketList.ToList();
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (this._syncRoot)
        {
            this.PacketList.Clear();
        }

        this.PacketsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void PacketCaptured(ReadOnlySpan<byte> packet, bool sent)
    {
        // A packet which was sent to the remote endpoint of a server connection is a packet
        // which goes to the client; a received one goes to the server.
        var capturedPacket = new Packet(DateTime.UtcNow - this.StartTimestamp, packet.ToArray(), !sent);

        lock (this._syncRoot)
        {
            while (this.PacketList.Count >= this._maximumPacketCount)
            {
                this.PacketList.RemoveAt(0);
            }

            this.PacketList.Add(capturedPacket);
        }

        this.PacketsChanged?.Invoke(this, EventArgs.Empty);
    }
}
