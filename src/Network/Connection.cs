// <copyright file="Connection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using log4net;

    /// <summary>
    /// The standard connection which uses a tcp socket.
    /// </summary>
    public sealed class Connection : IConnection
    {
        private const int BufferSize = 100;
        private readonly ILog log = LogManager.GetLogger(typeof(Connection));
        private readonly byte[] buffer = new byte[BufferSize];
        private readonly ListPacketQueue packetBuffer = new ListPacketQueue();
        private readonly IEncryptor encryptor;
        private readonly IDecryptor decryptor;
        private readonly SocketAsyncEventArgs receiveEventArgs;
        private readonly EndPoint remoteEndPoint;
        private Socket socket;

        private bool disconnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="encryptor">The encryptor.</param>
        /// <param name="decryptor">The decryptor.</param>
        public Connection(Socket socket, IEncryptor encryptor, IDecryptor decryptor)
        {
            this.socket = socket;
            this.remoteEndPoint = socket.RemoteEndPoint;
            this.encryptor = encryptor;
            this.decryptor = decryptor;
            encryptor?.Reset();
            decryptor?.Reset();

            this.receiveEventArgs = new SocketAsyncEventArgs();
            this.receiveEventArgs.SetBuffer(this.buffer, 0, this.buffer.Length);
            this.receiveEventArgs.Completed += (sender, args) => this.EndReceive(args);
        }

        /// <inheritdoc/>
        public event PacketReceivedHandler PacketReceived;

        /// <inheritdoc/>
        public event DisconnectedHandler Disconnected;

        /// <summary>
        /// Gets or sets the socket.
        /// </summary>
        /// <value>
        /// The socket.
        /// </value>
        public Socket Socket
        {
            get => this.socket;

            set
            {
                this.socket = value;
                this.disconnected = false;
            }
        }

        /// <inheritdoc/>
        public bool Connected => this.socket != null && !this.disconnected && this.socket.Connected;

        /// <inheritdoc/>
        public void Send(byte[] packet)
        {
            if (this.Connected)
            {
                using (ThreadContext.Stacks["RemoteEndPoint"].Push(this.remoteEndPoint.ToString()))
                {
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.Debug($"Send (before encryption): {packet.AsString()}");
                    }

                    if (this.encryptor != null)
                    {
                        packet = this.encryptor.Encrypt(packet);
                    }

                    if (this.log.IsDebugEnabled)
                    {
                        this.log.Debug($"Send (after encryption): {packet.AsString()}");
                    }

                    var currentSocket = this.socket;
                    if (currentSocket != null && this.Connected)
                    {
                        try
                        {
                            currentSocket.Send(packet, SocketFlags.None);
                        }
                        catch (Exception ex)
                        {
                            this.log.Debug($"Error while sending packet {packet.AsString()}", ex);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
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
                if (this.socket != null)
                {
                    this.socket.Dispose();
                    this.socket = null;
                }

                this.log.Debug("Disconnected");
                this.disconnected = true;

                this.Disconnected?.Invoke(this, System.EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public void BeginReceive()
        {
            if (this.socket != null)
            {
                this.socket.ReceiveAsync(this.receiveEventArgs);
            }
            else
            {
                this.Disconnect();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.log.Debug("Reseting connection object.");
            this.packetBuffer.Reset();
            this.encryptor?.Reset();
            this.decryptor?.Reset();

            this.Disconnected = null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.log.Debug("Disposing connection");
            this.Disconnect();
            this.PacketReceived = null;
            this.Disconnected = null;
            this.receiveEventArgs.Dispose();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var currentSocket = this.Socket;
            if (currentSocket == null)
            {
                return string.Empty;
            }

            return currentSocket.RemoteEndPoint.ToString();
        }

        private void EndReceive(SocketAsyncEventArgs args)
        {
            using (ThreadContext.Stacks["RemoteEndPoint"].Push(this.remoteEndPoint.ToString()))
            {
                if (args.LastOperation == SocketAsyncOperation.Disconnect)
                {
                    this.Disconnect();
                }

                int length = args.BytesTransferred;
                if (length == 0)
                {
                    this.log.Debug("client disconnected");
                    this.Disconnect();
                    return;
                }

                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug($"Data received, length: {length}");
                }

                this.packetBuffer.AddRange(args.Buffer.Take(length));
                this.RaisePacketReceivedEvents();
                this.BeginReceive();
            }
        }

        private void RaisePacketReceivedEvents()
        {
            var packet = this.packetBuffer.DequeueNextPacket();
            while (packet != null)
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug($"Packet received (before decryption): {packet.AsString()}");
                }

                if (this.decryptor != null && !this.decryptor.Decrypt(ref packet))
                {
                    this.log.Error("packet is malformed or can't be decrypted -> disconnect");
                    this.Disconnect();
                }

                var packetReceivedHandler = this.PacketReceived;
                if (packetReceivedHandler != null)
                {
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.Debug($"Packet received (after decryption): {packet.AsString()}");
                    }

                    try
                    {
                        packetReceivedHandler(this, packet);
                    }
                    catch (Exception exception)
                    {
                        this.log.Error($"An error occured while processing an incoming packet: {packet.AsString()}", exception);
                    }
                }

                packet = this.packetBuffer.DequeueNextPacket();
            }
        }
    }
}
