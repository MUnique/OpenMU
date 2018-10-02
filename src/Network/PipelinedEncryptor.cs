// <copyright file="PipelinedEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Linq;

    /// <summary>
    /// Basic implementation of a <see cref="IPipelinedEncryptor"/> which uses the default <see cref="Encryptor"/>.
    /// </summary>
    /// <remarks>
    /// This is not yet optimized as the <see cref="Encryptor"/> isn't working on pipes but on manually created byte arrays.
    /// </remarks>
    public class PipelinedEncryptor : PacketPipeReaderBase, IPipelinedEncryptor
    {
        private readonly PipeWriter target; // pipewriter of the SocketConnection
        private readonly Pipe readPipe = new Pipe();
        private readonly Encryptor encryptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public PipelinedEncryptor(PipeWriter target)
        {
            this.target = target;
            this.encryptor = new Encryptor();
            this.Source = this.readPipe.Reader;
            this.ReadSource().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public PipeWriter Writer => this.readPipe.Writer;

        /// <summary>
        /// Reads the mu online packet.
        /// Encrypts the data and writes it to the target.
        /// </summary>
        /// <param name="packet">The mu online packet</param>
        protected override void ReadPacket(ReadOnlySequence<byte> packet)
        {
            var decrypted = packet.ToArray();
            var encrypted = this.encryptor.Encrypt(decrypted);
            this.target.Write(decrypted);
        }
    }
}
