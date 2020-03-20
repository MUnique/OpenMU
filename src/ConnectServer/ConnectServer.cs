// <copyright file="ConnectServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Runtime.CompilerServices;

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// The connect server.
    /// </summary>
    internal class ConnectServer : IConnectServer, OpenMU.Interfaces.IConnectServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectServer));
        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServer" /> class.
        /// </summary>
        /// <param name="connectServerSettings">The settings.</param>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="configurationId">The configuration identifier.</param>
        public ConnectServer(IConnectServerSettings connectServerSettings, ClientVersion clientVersion, Guid configurationId)
        {
            this.ClientVersion = clientVersion;
            this.ConfigurationId = configurationId;
            this.Settings = connectServerSettings;

            this.ConnectInfos = new Dictionary<ushort, byte[]>();
            this.ServerList = new ServerList(this.ClientVersion);

            this.ClientListener = new ClientListener(this);
            this.ClientListener.ConnectedClientsChanged += (sender, args) =>
            {
                this.RaisePropertyChanged(nameof(this.CurrentConnections));
            };
            this.CreatePlugins();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public ServerState ServerState
        {
            get => this.serverState;
            private set
            {
                if (value != this.serverState)
                {
                    this.serverState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public ServerType Type => ServerType.ConnectServer;

        /// <inheritdoc/>
        public string Description => this.Settings.Description;

        /// <inheritdoc/>
        public int Id => SpecialServerIds.ConnectServer + this.Settings.ServerId;

        /// <inheritdoc />
        public Guid ConfigurationId { get; }

        /// <inheritdoc/>
        public IDictionary<ushort, byte[]> ConnectInfos { get; }

        /// <inheritdoc/>
        public ServerList ServerList { get; }

        /// <inheritdoc/>
        public IConnectServerSettings Settings { get; }

        /// <inheritdoc />
        public ClientVersion ClientVersion { get; }

        /// <summary>
        /// Gets the client listener.
        /// </summary>
        public ClientListener ClientListener { get; }

        /// <summary>
        /// Gets the maximum allowed connections.
        /// </summary>
        public int MaximumConnections => this.Settings.MaxConnections;

        /// <summary>
        /// Gets the current connection count.
        /// </summary>
        public int CurrentConnections => this.ClientListener.Clients.Count;

        /// <inheritdoc />
        public ICollection<(ushort, IPEndPoint)> GameServerEndPoints => this.ServerList.Servers.Select(s => (s.ServerId, s.EndPoint)).ToList();

        /// <inheritdoc/>
        public void Start()
        {
            Logger.Info("Begin starting");
            var oldState = this.ServerState;
            this.ServerState = OpenMU.Interfaces.ServerState.Starting;
            try
            {
                this.ClientListener.StartListener();
                this.ServerState = OpenMU.Interfaces.ServerState.Started;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                this.ServerState = oldState;
            }

            Logger.Info("Finished starting");
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            Logger.Info("Begin stopping");
            this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
            this.ClientListener.StopListener();
            this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
            Logger.Info("Finished stopping");
        }

        /// <inheritdoc/>
        public void RegisterGameServer(IGameServerInfo gameServer, IPEndPoint publicEndPoint)
        {
            Logger.InfoFormat("GameServer {0} is registering with endpoint {1}", gameServer, publicEndPoint);
            try
            {
                var serverListItem = new ServerListItem(this.ServerList)
                {
                    ServerId = gameServer.Id,
                    EndPoint = publicEndPoint,
                    ServerLoad = (byte)(gameServer.OnlinePlayerCount * 100f / gameServer.MaximumPlayers),
                };
                if (gameServer is INotifyPropertyChanged notifier)
                {
                    notifier.PropertyChanged += this.HandleServerPropertyChanged;
                }

                this.ConnectInfos.Add(serverListItem.ServerId, serverListItem.ConnectInfo);
                this.ServerList.Servers.Add(serverListItem);
                this.ServerList.InvalidateCache();
            }
            catch (Exception ex)
            {
                Logger.Error("Error during registration process", ex);
                throw;
            }

            Logger.InfoFormat("GameServer {0} has registered with endpoint {1}", gameServer, publicEndPoint);
        }

        /// <inheritdoc/>
        public void UnregisterGameServer(IGameServerInfo gameServer)
        {
            Logger.InfoFormat("GameServer {0} is unregistering", gameServer);
            var serverListItem = this.ServerList.Servers.FirstOrDefault(s => s.ServerId == gameServer.Id);
            if (serverListItem != null)
            {
                this.ConnectInfos.Remove(serverListItem.ServerId);
                this.ServerList.Servers.Remove(serverListItem);
                this.ServerList.InvalidateCache();
            }

            if (gameServer is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged -= this.HandleServerPropertyChanged;
            }

            Logger.InfoFormat("GameServer {0} has unregistered", gameServer);
        }

        private void CreatePlugins()
        {
            Logger.Debug("Begin creating plugins");
            this.ClientListener.ClientSocketAcceptPlugins.Add(new CheckMaximumConnectionsPlugin(this));
            var clientCountPlugin = new ClientConnectionCountPlugin(this.Settings);
            this.ClientListener.ClientSocketAcceptPlugins.Add(clientCountPlugin);
            this.ClientListener.ClientSocketDisconnectPlugins.Add(clientCountPlugin);
            Logger.Debug("Finished creating plugins");
        }

        private void HandleServerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "OnlinePlayerCount")
            {
                return;
            }

            if (!(sender is IGameServerInfo server))
            {
                return;
            }

            var serverListItem = this.ServerList.Servers.FirstOrDefault(s => s.ServerId == server.Id);
            if (serverListItem == null)
            {
                return;
            }

            serverListItem.ServerLoad = (byte)(server.OnlinePlayerCount * 100f / server.MaximumPlayers);
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
