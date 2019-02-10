// <copyright file="Connection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Net;
    using System.Threading.Tasks;
    using Interfaces;
    using log4net;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A connection which works on <see cref="IDuplexPipe"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
    public sealed class Connection : PacketPipeReaderBase, IConnection
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Connection));
        private readonly IPipelinedDecryptor decryptionPipe;
        private readonly IPipelinedEncryptor encryptionPipe;
        private readonly EndPoint remoteEndPoint;
        private IDuplexPipe duplexPipe;
        private bool disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="duplexPipe">The duplex pipe of the (socket) connection.</param>
        /// <param name="decryptionPipe">The decryption pipe.</param>
        /// <param name="encryptionPipe">The encryption pipe.</param>
        public Connection(IDuplexPipe duplexPipe, IPipelinedDecryptor decryptionPipe, IPipelinedEncryptor encryptionPipe)
        {
            this.duplexPipe = duplexPipe;
            this.decryptionPipe = decryptionPipe;
            this.encryptionPipe = encryptionPipe;
            this.Source = this.decryptionPipe?.Reader ?? this.duplexPipe.Input;
            this.remoteEndPoint = this.SocketConnection?.Socket.RemoteEndPoint;
            this.Source.OnWriterCompleted((e, state) => this.OnComplete(e), null);
            this.Output.OnReaderCompleted((e, state) => this.OnComplete(e), null);
        }

        /// <inheritdoc />
        public event PipedPacketReceivedHandler PacketReceived;

        /// <inheritdoc />
        public event DisconnectedHandler Disconnected;

        /// <inheritdoc />
        public bool Connected => !this.disconnected || (this.SocketConnection?.Socket.Connected ?? false);

        /// <inheritdoc />
        public PipeWriter Output => this.encryptionPipe?.Writer ?? this.duplexPipe.Output;

        /// <summary>
        /// Gets the socket connection, if the <see cref="duplexPipe"/> is an instance of <see cref="SocketConnection"/>. Otherwise, it returns null.
        /// </summary>
        private SocketConnection SocketConnection => this.duplexPipe as SocketConnection;

        /// <inheritdoc/>
        public override string ToString() => this.remoteEndPoint?.ToString() ?? $"{base.ToString()} {this.GetHashCode()}";

        /// <inheritdoc/>
        public async Task BeginReceive()
        {
            try
            {
                await this.ReadSource().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                this.OnComplete(e);
            }
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            using (ThreadContext.Stacks["Connection"].Push(this.ToString()))
            {
                if (this.disconnected)
                {
                    this.log.Debug("Connection already disconnected.");
                    return;
                }

                this.log.Debug("Disconnecting...");
                if (this.duplexPipe != null)
                {
                    this.Source.Complete();
                    (this.duplexPipe as IDisposable)?.Dispose();
                    this.duplexPipe = null;
                }

                this.log.Debug("Disconnected");
                this.disconnected = true;

                this.Disconnected?.Invoke(this, System.EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Disconnect();
            this.PacketReceived = null;
            this.Disconnected = null;
        }

        /// <inheritdoc />
        protected override void OnComplete(Exception exception)
        {
            using (ThreadContext.Stacks["Connection"].Push(this.ToString()))
            {
                if (exception != null)
                {
                    this.log.Error("Connection will be disconnected, because of an exception", exception);
                }

                this.Disconnect();
            }
        }

        /// <summary>
        /// Reads the mu online packet by raising <see cref="PacketReceived" />.
        /// </summary>
        /// <param name="packet">The mu online packet.</param>
        /// <returns>The async task.</returns>
        protected override Task ReadPacket(ReadOnlySequence<byte> packet)
        {
            this.PacketReceived?.Invoke(this, packet);
            return Task.CompletedTask;
        }
    }
}
