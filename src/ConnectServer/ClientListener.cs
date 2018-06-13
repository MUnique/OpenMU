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
    using log4net;
    using MUnique.OpenMU.ConnectServer.PacketHandler;

    /// <summary>
    /// The listener which is waiting for new connecting clients.
    /// </summary>
    internal class ClientListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientListener));

        private readonly ClientPool clientPool;
        private readonly object clientListLock = new object();
        private readonly Settings settings;

        private TcpListener clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientListener"/> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        public ClientListener(IConnectServer connectServer)
        {
            this.settings = connectServer.Settings;
            this.clientPool = new ClientPool(new ClientPacketHandler(connectServer), connectServer.Settings);
            this.Clients = new List<Client>();
            this.ClientSocketAcceptPlugins = new List<IAfterSocketAcceptPlugin>();
            this.ClientSocketDisconnectPlugins = new List<IAfterDisconnectPlugin>();
        }

        /// <summary>
        /// Occurs when the number of connected clients changed.
        /// </summary>
        public event EventHandler ConnectedClientsChanged;

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
            this.clientListener = new TcpListener(IPAddress.Any, this.settings.ClientListenerPort);
            this.clientListener.Start(this.settings.ListenerBacklog);
            Task.Run(this.BeginAccept);
            Logger.InfoFormat("Client Listener started, Port {0}", this.settings.ClientListenerPort);
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void StopListener()
        {
            this.clientListener.Stop();
            Logger.Info("Client Listener stopped");
        }

        private async Task BeginAccept()
        {
            Logger.Debug("Begin accepting new clients.");
            Socket newClient;
            try
            {
                newClient = await this.clientListener.AcceptSocketAsync();
            }
            catch (ObjectDisposedException ex)
            {
                Logger.Warn("gslistener has been disposed", ex);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error("An unexpected error occured when awaiting the next client socket.", ex);
                return;
            }

            this.HandleNewSocket(newClient);

            // Accept the next Client:
            if (this.clientListener.Server.IsBound)
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
            var client = this.clientPool.GetClient(socket);
            if (client != null)
            {
                lock (this.clientListLock)
                {
                    this.Clients.Add(client);
                }

                client.Connection.Disconnected += (sender, e) => this.OnClientDisconnect(client);
                Logger.DebugFormat("Client connected: {0}, current client count: {1}", socket.RemoteEndPoint, this.Clients.Count);
                client.SendHello();
                client.Connection.BeginReceive();
            }
            else
            {
                Logger.WarnFormat("Client object could not get created for: {0}, current client count: {1}", socket.RemoteEndPoint, this.Clients.Count);
                socket.Dispose();
            }
        }

        private void OnClientDisconnect(Client client)
        {
            foreach (var plugin in this.ClientSocketDisconnectPlugins)
            {
                plugin.OnAfterDisconnect(client);
            }

            Logger.DebugFormat("Connection to Client {0}:{1} disconnected.", client.Address, client.Port);
            lock (this.clientListLock)
            {
                this.Clients.Remove(client);
            }

            this.clientPool.Add(client);
            this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
