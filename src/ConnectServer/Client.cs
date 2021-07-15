// <copyright file="Client.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Buffers;
    using System.Net;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.ConnectServer.PacketHandler;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The client which connected to the connect server.
    /// </summary>
    internal sealed class Client : IDisposable
    {
        private static readonly byte[] HelloPacket = { 0xC1, 4, 0, 1 };

        private readonly ILogger<Client> logger;

        private readonly object disposeLock = new ();
        private readonly byte[] receiveBuffer;

        private readonly Timer onlineTimer;
        private readonly IPacketHandler<Client> packetHandler;
        private bool disposed;

        private DateTime lastReceive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="packetHandler">The packet handler.</param>
        /// <param name="maxPacketSize">Maximum size of the packet. This value is also used to initialize the receive buffer.</param>
        /// <param name="logger">The logger.</param>
        public Client(IConnection connection, TimeSpan timeout, IPacketHandler<Client> packetHandler, byte maxPacketSize, ILogger<Client> logger)
        {
            this.Connection = connection;
            this.Connection.PacketReceived += this.OnPacketReceived;
            this.Timeout = timeout;
            this.packetHandler = packetHandler;
            this.logger = logger;
            this.lastReceive = DateTime.Now;
            var checkInterval = new TimeSpan(0, 0, 20);
            this.onlineTimer = new Timer(this.OnOnlineTimerElapsed, null, checkInterval, checkInterval);
            this.receiveBuffer = new byte[maxPacketSize];
        }

        /// <summary>
        /// Gets or sets the timeout after which the client gets disconnected if he is inactive.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the server information request count.
        /// </summary>
        /// <remarks>Used for DOS protection.</remarks>
        public int ServerInfoRequestCount { get; set; }

        /// <summary>
        /// Gets or sets the FTP request count.
        /// </summary>
        /// <remarks>Used for DOS protection.</remarks>
        public int FtpRequestCount { get; set; }

        /// <summary>
        /// Gets or sets the server list request count.
        /// </summary>
        /// <remarks>Used for DOS protection.</remarks>
        public int ServerListRequestCount { get; set; }

        /// <summary>
        /// Gets or sets the ip from which the client is connecting.
        /// </summary>
        public IPAddress Address { get; set; } = IPAddress.None;

        /// <summary>
        /// Gets or sets the port from which the client is connecting.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets the connection from/to the client.
        /// </summary>
        internal IConnection Connection { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.disposed)
            {
                lock (this.disposeLock)
                {
                    if (!this.disposed)
                    {
                        this.onlineTimer.Dispose();
                        this.Connection.Dispose();
                        this.disposed = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sends the hello packet.
        /// </summary>
        internal void SendHello()
        {
            using var writer = this.Connection.StartSafeWrite(HelloPacket[0], HelloPacket.Length);
            HelloPacket.CopyTo(writer.Span);
            writer.Commit();
        }

        private void OnOnlineTimerElapsed(object? state)
        {
            try
            {
                if (this.Connection.Connected && DateTime.Now.Subtract(this.lastReceive) > this.Timeout)
                {
                    this.logger.LogDebug("Connection Timeout ({0}): Address {1}:{2} will be disconnected.", this.Timeout, this.Address, this.Port);
                    this.Connection.Disconnect();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error when disconnecting client.  Address {1}:{2}", this.Address, this.Port);
            }
        }

        private void OnPacketReceived(object? sender, ReadOnlySequence<byte> sequence)
        {
            this.lastReceive = DateTime.Now;
            if (sequence.Length > this.receiveBuffer.Length)
            {
                this.logger.LogInformation($"Client {this.Address}:{this.Port} will be disconnected because it sent a packet which was too big (size of {sequence.Length}");
                this.Connection.Disconnect();
            }

            sequence.CopyTo(this.receiveBuffer);
            this.packetHandler.HandlePacket(this, this.receiveBuffer.AsSpan(0, this.receiveBuffer.GetPacketSize()));
        }
    }
}
