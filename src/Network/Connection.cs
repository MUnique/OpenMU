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
    using Microsoft.Extensions.Logging;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A connection which works on <see cref="IDuplexPipe"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
    public sealed class Connection : PacketPipeReaderBase, IConnection
    {
        private readonly IPipelinedEncryptor? encryptionPipe;
        private readonly ILogger<Connection> logger;
        private readonly EndPoint? remoteEndPoint;
        private IDuplexPipe? duplexPipe;
        private bool disconnected;
        private PipeWriter? outputWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection" /> class.
        /// </summary>
        /// <param name="duplexPipe">The duplex pipe of the (socket) connection.</param>
        /// <param name="decryptionPipe">The decryption pipe.</param>
        /// <param name="encryptionPipe">The encryption pipe.</param>
        /// <param name="logger">The logger.</param>
        public Connection(IDuplexPipe duplexPipe, IPipelinedDecryptor? decryptionPipe, IPipelinedEncryptor? encryptionPipe, ILogger<Connection> logger)
        {
            this.duplexPipe = duplexPipe;
            this.encryptionPipe = encryptionPipe;
            this.logger = logger;
            this.Source = decryptionPipe?.Reader ?? this.duplexPipe!.Input;
            this.remoteEndPoint = this.SocketConnection?.Socket.RemoteEndPoint;
        }

        /// <inheritdoc />
        public event PipedPacketReceivedHandler? PacketReceived;

        /// <inheritdoc />
        public event DisconnectedHandler? Disconnected;

        /// <inheritdoc />
        public bool Connected => this.SocketConnection != null ? this.SocketConnection.ShutdownKind == PipeShutdownKind.None && !this.disconnected : !this.disconnected;

        /// <inheritdoc />
        public PipeWriter Output => this.outputWriter ??= this.encryptionPipe?.Writer ?? this.duplexPipe!.Output;

        /// <summary>
        /// Gets the socket connection, if the <see cref="duplexPipe"/> is an instance of <see cref="SocketConnection"/>. Otherwise, it returns null.
        /// </summary>
        private SocketConnection? SocketConnection => this.duplexPipe as SocketConnection;

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
                return;
            }

            this.OnComplete(null);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            using var scope = this.logger.BeginScope(this.remoteEndPoint);
            if (this.disconnected)
            {
                this.logger.LogDebug("Connection already disconnected.");
                return;
            }

            this.logger.LogDebug("Disconnecting...");
            if (this.duplexPipe is not null)
            {
                this.Source.Complete();
                this.Output.Complete();
                (this.duplexPipe as IDisposable)?.Dispose();
                this.duplexPipe = null;
            }

            this.logger.LogDebug("Disconnected");
            this.disconnected = true;

            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Disconnect();
            this.PacketReceived = null;
            this.Disconnected = null;
        }

        /// <inheritdoc />
        protected override void OnComplete(Exception? exception)
        {
            using var scope = this.logger.BeginScope(this.remoteEndPoint);
            if (exception != null)
            {
                this.logger.LogError(exception, "Connection will be disconnected, because of an exception");
            }

            this.Output.Complete(exception);
            this.Disconnect();
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
