// <copyright file="PacketPipeReaderBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Buffers;
using System.IO.Pipelines;

/// <summary>
/// Base class for all classes which read mu online data packets from a <see cref="PipeReader"/>.
/// </summary>
public abstract class PacketPipeReaderBase
{
    private readonly byte[] _headerBuffer = new byte[3];

    /// <summary>
    /// Gets or sets the <see cref="PipeReader"/> from which the packets can be read from at <see cref="ReadSourceAsync"/>.
    /// </summary>
    protected PipeReader Source { get; set; } = null!; // will be set in derived classes

    /// <summary>
    /// Reads the mu online packet.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns>The async task.</returns>
    protected abstract ValueTask ReadPacketAsync(ReadOnlySequence<byte> packet);

    /// <summary>
    /// Called when the <see cref="Source"/> completed.
    /// </summary>
    /// <param name="exception">The exception, if any occurred; Otherwise, <c>null</c>.</param>
    protected abstract ValueTask OnCompleteAsync(Exception? exception);

    /// <summary>
    /// Reads from the <see cref="Source"/> until it's completed or cancelled.
    /// </summary>
    /// <returns>The task.</returns>
    protected async Task ReadSourceAsync()
    {
        if (this.Source is null)
        {
            throw new InvalidOperationException("Source must be set before.");
        }

        try
        {
            while (true)
            {
                var completed = await this.ReadBufferAsync().ConfigureAwait(false);

                // Stop reading if there's no more data coming
                if (completed)
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            await this.OnCompleteAsync(e).ConfigureAwait(false);
            throw;
        }

        await this.OnCompleteAsync(null).ConfigureAwait(false);
    }

    private async Task<bool> ReadBufferAsync()
    {
        ReadResult result = await this.Source.ReadAsync().ConfigureAwait(false);
        ReadOnlySequence<byte> buffer = result.Buffer;
        int? length = null;
        do
        {
            if (buffer.Length > 2)
            {
                // peek the length of the next packet
                buffer.Slice(0, 3).CopyTo(this._headerBuffer);
                length = this._headerBuffer.AsSpan().GetPacketSize();
                if (length == 0)
                {
                    var exception = new InvalidPacketHeaderException(this._headerBuffer, result.Buffer, buffer.Start);

                    // Notify our source, that we don't intend to read anymore.
                    await this.Source.CompleteAsync(exception).ConfigureAwait(false);

                    await this.OnCompleteAsync(exception);
                    throw exception;
                }
            }

            if (length is > 0 && buffer.Length >= length)
            {
                var packet = buffer.Slice(0, length.Value);
                await this.ReadPacketAsync(packet).ConfigureAwait(false);

                buffer = buffer.Slice(buffer.GetPosition(length.Value), buffer.End);
                length = null;
            }
            else
            {
                // read more
                break;
            }
        }
        while (buffer.Length > 2);

        if (result.IsCanceled || result.IsCompleted)
        {
            // Not possible to advance any further, e.g. because of a disconnected network connection.
            await this.OnCompleteAsync(null);
        }
        else
        {
            // Tell the PipeReader how much of the buffer we have consumed
            this.Source.AdvanceTo(buffer.Start);
        }

        return result.IsCompleted;
    }
}