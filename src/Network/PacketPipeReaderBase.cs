// <copyright file="PacketPipeReaderBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Buffers;
using System.IO.Pipelines;
using Pipelines.Sockets.Unofficial;

/// <summary>
/// Base class for all classes which read mu online data packets from a <see cref="PipeReader"/>.
/// </summary>
/// <remarks>
/// Things to consider here:
///  * Do not call FlushAsync if the reader can't start until FlushAsync finishes, as that may cause a deadlock.
///  * Ensure that only one context "owns" a PipeReader or PipeWriter or accesses them. These types are not thread-safe.
/// </remarks>
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
    /// <returns><see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.</returns>
    protected abstract ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet);

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
        catch (ConnectionAbortedException)
        {
            // we can ignore that.
        }
        catch (Exception e)
        {
            await this.OnCompleteAsync(e).ConfigureAwait(false);
            return;
        }

        await this.OnCompleteAsync(null).ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to flush the writer.
    /// </summary>
    /// <param name="pipeWriter">The pipe writer.</param>
    /// <returns>
    ///   <see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.
    /// </returns>
    protected async ValueTask<bool> TryFlushWriterAsync(PipeWriter pipeWriter)
    {
        if (pipeWriter is { CanGetUnflushedBytes: true, UnflushedBytes: 0 })
        {
            // It was flushed already in the background.
            return true;
        }

        // todo: what happens if it was flushed in the background in the meantime? race-condition?
        var flushResult = await pipeWriter.FlushAsync().ConfigureAwait(false);
        return !flushResult.IsCompleted;
    }

    private async Task<bool> ReadBufferAsync()
    {
        ReadResult result = await this.Source.ReadAsync().ConfigureAwait(false);
        ReadOnlySequence<byte> buffer = result.Buffer;
        int? length = null;
        var readingCancelledOrCompleted = false;
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

                    await this.OnCompleteAsync(exception).ConfigureAwait(false);
                    throw exception;
                }
            }

            if (length is > 0 && buffer.Length >= length)
            {
                var packet = buffer.Slice(0, length.Value);
                if (!await this.ReadPacketAsync(packet).ConfigureAwait(false))
                {
                    readingCancelledOrCompleted = true;
                    break;
                }

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
            await this.OnCompleteAsync(null).ConfigureAwait(false);
        }
        else
        {
            // Tell the PipeReader how much of the buffer we have consumed
            this.Source.AdvanceTo(buffer.Start);
        }

        return result.IsCompleted || readingCancelledOrCompleted;
    }
}