// <copyright file="PacketPipeReaderBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for all classes which read mu online data packets from a <see cref="PipeReader"/>.
    /// </summary>
    public abstract class PacketPipeReaderBase
    {
        private readonly byte[] headerBuffer = new byte[3];

        /// <summary>
        /// Gets or sets the <see cref="PipeReader"/> from which the packets can be read from at <see cref="ReadSource"/>.
        /// </summary>
        protected PipeReader Source { get; set; } = null!; // will be set in derived classes

        /// <summary>
        /// Reads the mu online packet.
        /// </summary>
        /// <param name="packet">The mu online packet.</param>
        /// <returns>The async task.</returns>
        protected abstract Task ReadPacket(ReadOnlySequence<byte> packet);

        /// <summary>
        /// Called when the <see cref="Source"/> completed.
        /// </summary>
        /// <param name="exception">The exception, if any occured; Otherwise, <c>null</c>.</param>
        protected abstract void OnComplete(Exception? exception);

        /// <summary>
        /// Reads from the <see cref="Source"/> until it's completed or cancelled.
        /// </summary>
        /// <returns>The task.</returns>
        protected async Task ReadSource()
        {
            if (this.Source is null)
            {
                throw new InvalidOperationException("Source must be set before.");
            }

            try
            {
                while (true)
                {
                    var completed = await this.ReadBuffer().ConfigureAwait(false);

                    // Stop reading if there's no more data coming
                    if (completed)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                this.OnComplete(e);
                throw;
            }

            this.OnComplete(null);
        }

        private async Task<bool> ReadBuffer()
        {
            ReadResult result = await this.Source.ReadAsync().ConfigureAwait(false);
            ReadOnlySequence<byte> buffer = result.Buffer;
            int? length = null;
            do
            {
                if (buffer.Length > 2)
                {
                    // peek the length of the next packet
                    buffer.Slice(0, 3).CopyTo(this.headerBuffer);
                    length = this.headerBuffer.AsSpan().GetPacketSize();
                    if (length == 0)
                    {
                        var exception = new InvalidPacketHeaderException(this.headerBuffer, result.Buffer, buffer.Start);

                        // Notify our source, that we don't intend to read anymore.
                        await this.Source.CompleteAsync(exception).ConfigureAwait(false);

                        this.OnComplete(exception);
                        throw exception;
                    }
                }

                if (length is > 0 && buffer.Length >= length)
                {
                    var packet = buffer.Slice(0, length.Value);
                    await this.ReadPacket(packet).ConfigureAwait(false);

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
                this.OnComplete(null);
            }
            else
            {
                // Tell the PipeReader how much of the buffer we have consumed
                this.Source.AdvanceTo(buffer.Start);
            }

            return result.IsCompleted;
        }
    }
}
