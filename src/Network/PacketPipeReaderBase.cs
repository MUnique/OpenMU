// <copyright file="PacketPipeReaderBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for all classes which read mu online data packets from a <see cref="PipeReader"/>.
    /// </summary>
    public abstract class PacketPipeReaderBase
    {
        /// <summary>
        /// Gets or sets the <see cref="PipeReader"/> from which the packets can be read from at <see cref="ReadSource"/>.
        /// </summary>
        protected PipeReader Source { get; set; }

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> for the reading task.
        /// </summary>
        protected CancellationTokenSource ReadCancellationTokenSource { get; } = new CancellationTokenSource();

        /// <summary>
        /// Reads the mu online packet.
        /// </summary>
        /// <param name="packet">The mu online packet.</param>
        protected abstract void ReadPacket(ReadOnlySequence<byte> packet);

        /// <summary>
        /// Reads from the <see cref="Source"/> until it's completed or cancelled.
        /// </summary>
        /// <returns>The task.</returns>
        protected async Task ReadSource()
        {
            var header = new byte[3];
            while (true)
            {
                ReadResult result = await this.Source.ReadAsync(this.ReadCancellationTokenSource.Token).ConfigureAwait(false);

                ReadOnlySequence<byte> buffer = result.Buffer;

                int? length = null;
                do
                {
                    if (length == null && buffer.Length > 2)
                    {
                        // peek the length of the next packet
                        buffer.Slice(0, 3).CopyTo(header);
                        length = header.GetPacketSize();
                    }

                    if (length != null && length > 0 && buffer.Length >= length)
                    {
                        var packet = buffer.Slice(0, length.Value);
                        this.ReadPacket(packet);
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

                // Tell the PipeReader how much of the buffer we have consumed
                this.Source.AdvanceTo(buffer.Start);

                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete
            this.Source.Complete();
        }
    }
}
