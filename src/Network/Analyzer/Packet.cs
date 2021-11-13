// <copyright file="Packet.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// A captured data packet.
/// </summary>
public class Packet
{
    private readonly byte[] _innerData;

    private string? _dataAsString;

    /// <summary>
    /// Initializes a new instance of the <see cref="Packet"/> class.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="data">The data.</param>
    /// <param name="toServer">If set to <c>true</c>, the packet was sent to server; Otherwise it was sent to the client.</param>
    public Packet(TimeSpan timestamp, byte[] data, bool toServer)
    {
        this._innerData = data;
        this.Timestamp = timestamp;
        this.ToServer = toServer;
        this.Direction = this.ToServer ? "C->S" : "S->C";
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public TimeSpan Timestamp { get; }

    /// <summary>
    /// Gets the raw data of the packet.
    /// </summary>
    public byte[] Data => this._innerData;

    /// <summary>
    /// Gets the type of the packet, which is the first byte of the packet byte array.
    /// </summary>
    public byte Type => this._innerData[0];

    /// <summary>
    /// Gets the code of the packet, which specifies the kind of the message.
    /// </summary>
    public byte Code => this._innerData.AsSpan().GetPacketType();

    /// <summary>
    /// Gets the sub code of the packet, which further specifies the kind of the message, if specified.
    /// </summary>
    public byte? SubCode => this._innerData.AsSpan().GetPacketSubType();

    /// <summary>
    /// Gets the direction as string.
    /// </summary>
    public string Direction { get; }

    /// <summary>
    /// Gets a value indicating whether the packet was sent to the server; Otherwise it was sent to the client.
    /// </summary>
    public bool ToServer { get; }

    /// <summary>
    /// Gets the size of the packet byte array.
    /// </summary>
    public int Size => this._innerData.Length;

    /// <summary>
    /// Gets the packet data as binary string.
    /// </summary>
    public string PacketData => this._dataAsString ??= this._innerData.AsString();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Direction}: {this.PacketData}";
    }
}