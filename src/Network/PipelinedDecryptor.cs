// <copyright file="PipelinedDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Linq;

    /// <summary>
    /// Basic implementation of a <see cref="IPipelinedDecryptor"/> which uses the default <see cref="Decryptor"/>.
    /// </summary>
    /// <remarks>
    /// This is not yet optimized as the <see cref="Decryptor"/> isn't working on pipes but on manually created byte arrays.
    /// </remarks>
    public class PipelinedDecryptor : PacketPipeReaderBase, IPipelinedDecryptor
    {
        private readonly Pipe pipe = new Pipe();
        private readonly Decryptor decryptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedDecryptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public PipelinedDecryptor(PipeReader source)
        {
            this.decryptor = new Decryptor();
            this.Source = source;
            this.ReadSource().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public PipeReader Reader => this.pipe.Reader;

        /// <summary>
        /// Reads the mu online packet.
        /// Decrypts the packet and writes it into our pipe.
        /// </summary>
        /// <param name="packet">The mu online packet</param>
        protected override void ReadPacket(ReadOnlySequence<byte> packet)
        {
            var encrypted = packet.ToArray();
            if (this.decryptor.Decrypt(ref encrypted))
            {
                this.pipe.Writer.Write(encrypted);
                this.pipe.Writer.FlushAsync();
            }
        }
    }
}
