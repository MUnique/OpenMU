// <copyright file="OutgoingPacketCollector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

/// <summary>
/// Handler for a collected data packet.
/// </summary>
/// <param name="packet">The complete data packet.</param>
internal delegate void PacketCollectedHandler(ReadOnlySpan<byte> packet);

/// <summary>
/// Collects the data which is written to an <see cref="ExtendedPipeWriter"/> and forwards
/// complete data packets to a handler.
/// </summary>
/// <remarks>
/// One write to a <see cref="System.IO.Pipelines.PipeWriter"/> is not necessarily one data
/// packet: a bigger message may be written in several chunks, and it's also possible that
/// more than one packet is written before the writer gets flushed. The written data is
/// therefore buffered and split into packets again, based on the packet header.
/// </remarks>
internal sealed class OutgoingPacketCollector
{
    private const int InitialBufferSize = 256;

    private readonly PacketCollectedHandler _packetCollected;

    private byte[] _buffer = new byte[InitialBufferSize];

    private int _bufferedLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutgoingPacketCollector"/> class.
    /// </summary>
    /// <param name="packetCollected">The handler which is called for each complete data packet.</param>
    public OutgoingPacketCollector(PacketCollectedHandler packetCollected)
    {
        this._packetCollected = packetCollected;
    }

    /// <summary>
    /// Adds the written data and forwards each complete data packet to the handler.
    /// </summary>
    /// <param name="data">The data which has been written to the pipe writer.</param>
    public void DataWritten(ReadOnlySpan<byte> data)
    {
        this.Append(data);

        var offset = 0;
        while (offset < this._bufferedLength)
        {
            var rest = this._buffer.AsSpan(offset, this._bufferedLength - offset);
            var headerSize = ArrayExtensions.GetPacketHeaderSize(rest[0]);
            if (headerSize == 0)
            {
                // It's not a packet we know, so we're not able to determine the packet
                // boundaries of the subsequent data anymore. We drop what we have instead
                // of reporting garbage.
                this._bufferedLength = 0;
                return;
            }

            if (rest.Length < headerSize)
            {
                break;
            }

            var packetSize = rest.GetPacketSize();
            if (packetSize < headerSize)
            {
                this._bufferedLength = 0;
                return;
            }

            if (rest.Length < packetSize)
            {
                break;
            }

            this._packetCollected(rest[..packetSize]);
            offset += packetSize;
        }

        this.RemoveFromBuffer(offset);
    }

    private void Append(ReadOnlySpan<byte> data)
    {
        var requiredLength = this._bufferedLength + data.Length;
        if (this._buffer.Length < requiredLength)
        {
            Array.Resize(ref this._buffer, Math.Max(requiredLength, this._buffer.Length * 2));
        }

        data.CopyTo(this._buffer.AsSpan(this._bufferedLength));
        this._bufferedLength = requiredLength;
    }

    private void RemoveFromBuffer(int count)
    {
        if (count == 0)
        {
            return;
        }

        var rest = this._bufferedLength - count;
        if (rest > 0)
        {
            Array.Copy(this._buffer, count, this._buffer, 0, rest);
        }

        this._bufferedLength = rest;
    }
}
