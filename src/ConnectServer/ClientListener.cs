// <copyright file="ClientListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.ConnectServer.PacketHandler;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// The listener which is waiting for new connecting clients.
    /// </summary>
    internal class ClientListener
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<ClientListener> logger;
        private readonly object clientListLock = new ();
        private readonly IConnectServerSettings connectServerSettings;
        private readonly IPacketHandler<Client> packetHandler;
        private TcpListener? clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientListener" /> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ClientListener(IConnectServer connectServer, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.connectServerSettings = connectServer.Settings;
            this.logger = this.loggerFactory.CreateLogger<ClientListener>();
            this.packetHandler = new ClientPacketHandler(connectServer, loggerFactory);
            this.Clients = new List<Client>();
            this.ClientSocketAcceptPlugins = new List<IAfterSocketAcceptPlugin>();
            this.ClientSocketDisconnectPlugins = new List<IAfterDisconnectPlugin>();
        }

        /// <summary>
        /// Occurs when the number of connected clients changed.
        /// </summary>
        public event EventHandler? ConnectedClientsChanged;

        /// <summary>
        /// Gets the connected clients.
        /// </summary>
        public ICollection<Client> Clients { get; }

        /// <summary>
        /// Gets the client socket accept plugins.
        /// </summary>
        public ICollection<IAfterSocketAcceptPlugin> ClientSocketAcceptPlugins { get; }

        /// <summary>
        /// Gets the client socket disconnect plugins.
        /// </summary>
        public ICollection<IAfterDisconnectPlugin> ClientSocketDisconnectPlugins { get; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void StartListener()
        {
            this.clientListener = new TcpListener(IPAddress.Any, this.connectServerSettings.ClientListenerPort);
            this.clientListener.Start(this.connectServerSettings.ListenerBacklog);
            Task.Run(this.BeginAccept);
            this.logger.LogInformation("Client Listener started, Port {0}", this.connectServerSettings.ClientListenerPort);
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void StopListener()
        {
            this.clientListener?.Stop();
            this.logger.LogInformation("Client Listener stopped");
        }

        private async Task BeginAccept()
        {
            this.logger.LogDebug("Begin accepting new clients.");
            Socket newClient;
            try
            {
                var listener = this.clientListener;
                if (listener is null)
                {
                    return;
                }

                newClient = await listener.AcceptSocketAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // this exception is expected when the clientListener got disposed. In this case we don't want to spam the log.
                return;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An unexpected error occured when awaiting the next client socket.");
                return;
            }

            this.HandleNewSocket(newClient);

            // Accept the next Client:
            if (this.clientListener?.Server.IsBound ?? false)
            {
                await this.BeginAccept().ConfigureAwait(false);
            }
        }

        private void HandleNewSocket(Socket socket)
        {
            foreach (var plugin in this.ClientSocketAcceptPlugins)
            {
                if (!plugin.OnAfterSocketAccept(socket))
                {
                    socket.Dispose();
                    return;
                }
            }

            this.AddClient(socket);
            this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void AddClient(Socket socket)
        {
            var connection = new Connection(SocketConnection.Create(socket), null, null, this.loggerFactory.CreateLogger<Connection>());
            var client = new Client(connection, this.connectServerSettings.Timeout, this.packetHandler, this.connectServerSettings.MaximumReceiveSize, this.loggerFactory.CreateLogger<Client>());
            var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
            client.Address = ipEndpoint?.Address ?? IPAddress.None;
            client.Port = ipEndpoint?.Port ?? 0;
            client.Timeout = this.connectServerSettings.Timeout;

            lock (this.clientListLock)
            {
                this.Clients.Add(client);
            }

            client.Connection.Disconnected += (sender, e) => this.OnClientDisconnect(client);
            this.logger.LogDebug("Client connected: {0}, current client count: {1}", socket.RemoteEndPoint, this.Clients.Count);
            client.SendHello();
            client.Connection.BeginReceive();
        }

        private void OnClientDisconnect(Client client)
        {
            foreach (var plugin in this.ClientSocketDisconnectPlugins)
            {
                plugin.OnAfterDisconnect(client);
            }

            this.logger.LogDebug("Connection to Client {0}:{1} disconnected.", client.Address, client.Port);
            lock (this.clientListLock)
            {
                this.Clients.Remove(client);
            }

            this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
