// <copyright file="PipelinedConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Net;
    using System.Threading.Tasks;
    using log4net;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// Similar to <see cref="Connection"/> but works on a <see cref="SocketConnection"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
    public sealed class PipelinedConnection : PacketPipeReaderBase, IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipelinedConnection));
        private readonly IPipelinedDecryptor decryptionPipe;
        private readonly IPipelinedEncryptor encryptionPipe;
        private readonly EndPoint remoteEndPoint;
        private SocketConnection socketConnection;
        private bool disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedConnection"/> class.
        /// </summary>
        /// <param name="socketConnection">The socket connection.</param>
        /// <param name="decryptionPipe">The decryption pipe.</param>
        /// <param name="encryptionPipe">The encryption pipe.</param>
        public PipelinedConnection(SocketConnection socketConnection, IPipelinedDecryptor decryptionPipe, IPipelinedEncryptor encryptionPipe)
        {
            this.socketConnection = socketConnection;
            this.remoteEndPoint = socketConnection.Socket.RemoteEndPoint;
            this.decryptionPipe = decryptionPipe;
            this.encryptionPipe = encryptionPipe;
            this.Source = this.decryptionPipe?.Reader ?? this.socketConnection.Input;
        }

        /// <summary>
        /// A delegate which is executed when a packet gets received from a connection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="packet">The packet.</param>
        public delegate void PacketReceivedHandler(object sender, ReadOnlySequence<byte> packet);

        /// <summary>
        /// Occurs when a new packet got received.
        /// </summary>
        public event PacketReceivedHandler PacketReceived;

        /// <summary>
        /// Occurs when the client disconnected.
        /// </summary>
        public event DisconnectedHandler Disconnected;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IConnection"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected => this.socketConnection?.Socket != null && !this.disconnected && this.socketConnection.Socket.Connected;

        private PipeWriter Output => this.encryptionPipe?.Writer ?? this.socketConnection.Output;

        /// <summary>
        /// Begins receiving from the client.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task BeginReceive()
        {
            await this.ReadSource();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            using (ThreadContext.Stacks["RemoteEndPoint"].Push(this.remoteEndPoint.ToString()))
            {
                if (this.disconnected)
                {
                    this.log.Debug("Connection already disconnected.");
                    return;
                }

                this.log.Debug("Disconnecting...");
                if (this.socketConnection != null)
                {
                    this.ReadCancellationTokenSource.Cancel();
                    this.socketConnection.Dispose();
                    this.socketConnection = null;
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

        /// <summary>
        /// Sends the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public void Send(ReadOnlySpan<byte> packet)
        {
            if (this.Connected)
            {
                using (ThreadContext.Stacks["RemoteEndPoint"].Push(this.remoteEndPoint.ToString()))
                {
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.Debug($"Send (before encryption): {packet.ToArray().AsString()}");
                    }

                    var currentOutput = this.Output;
                    if (currentOutput != null && this.Connected)
                    {
                        try
                        {
                            currentOutput.Write(packet);
                        }
                        catch (Exception ex)
                        {
                            this.log.Debug($"Error while sending packet {packet.ToArray().AsString()}", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads the mu online packet by raising <see cref="PacketReceived"/>.
        /// </summary>
        /// <param name="packet">The mu online packet.</param>
        protected override void ReadPacket(ReadOnlySequence<byte> packet)
        {
            this.PacketReceived?.Invoke(this, packet);
        }
    }
}
