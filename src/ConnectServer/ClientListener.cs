// <copyright file="ClientListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.ConnectServer.PacketHandler;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

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
        private Listener? listener;

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
        public IList<IAfterSocketAcceptPlugin> ClientSocketAcceptPlugins { get; }

        /// <summary>
        /// Gets the client socket disconnect plugins.
        /// </summary>
        public ICollection<IAfterDisconnectPlugin> ClientSocketDisconnectPlugins { get; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void StartListener()
        {
            this.listener = new Listener(this.connectServerSettings.ClientListenerPort, null, null, this.loggerFactory);
            this.listener.ClientAccepting += this.OnClientAccepting;
            this.listener.ClientAccepted += this.OnClientAccepted;
            this.listener.Start(this.connectServerSettings.ListenerBacklog);

            this.logger.LogInformation("Client Listener started, Port {0}", this.connectServerSettings.ClientListenerPort);
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void StopListener()
        {
            this.listener?.Stop();
            this.logger.LogInformation("Client Listener stopped");
        }

        private void OnClientAccepting(object? sender, ClientAcceptingEventArgs e)
        {
            for (var i = 0; i < this.ClientSocketAcceptPlugins.Count; ++i)
            {
                var plugin = this.ClientSocketAcceptPlugins[i];
                if (!plugin.OnAfterSocketAccept(e.AcceptingSocket))
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void OnClientAccepted(object? sender, ClientAcceptedEventArgs e)
        {
            var connection = e.AcceptedConnection;
            var client = new Client(connection, this.connectServerSettings.Timeout, this.packetHandler, this.connectServerSettings.MaximumReceiveSize, this.loggerFactory.CreateLogger<Client>());
            var ipEndpoint = connection.EndPoint as IPEndPoint;
            client.Address = ipEndpoint?.Address ?? IPAddress.None;
            client.Port = ipEndpoint?.Port ?? 0;
            client.Timeout = this.connectServerSettings.Timeout;

            lock (this.clientListLock)
            {
                this.Clients.Add(client);
            }

            client.Connection.Disconnected += (sender, e) => this.OnClientDisconnect(client);
            this.logger.LogDebug("Client connected: {0}, current client count: {1}", connection.EndPoint, this.Clients.Count);
            client.SendHello();
            client.Connection.BeginReceive();
            this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
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
