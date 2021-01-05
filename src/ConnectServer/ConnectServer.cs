// <copyright file="ConnectServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// The connect server.
    /// </summary>
    internal class ConnectServer : IConnectServer, OpenMU.Interfaces.IConnectServer
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;
        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServer" /> class.
        /// </summary>
        /// <param name="connectServerSettings">The settings.</param>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="configurationId">The configuration identifier.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConnectServer(IConnectServerSettings connectServerSettings, ClientVersion clientVersion, Guid configurationId, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.ClientVersion = clientVersion;
            this.ConfigurationId = configurationId;
            this.Settings = connectServerSettings;

            this.logger = this.loggerFactory.CreateLogger<ConnectServer>();

            this.ConnectInfos = new Dictionary<ushort, byte[]>();
            this.ServerList = new ServerList(this.ClientVersion);

            this.ClientListener = new ClientListener(this, loggerFactory);
            this.ClientListener.ConnectedClientsChanged += (sender, args) =>
            {
                this.RaisePropertyChanged(nameof(this.CurrentConnections));
            };
            this.CreatePlugins();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

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

        /// <inheritdoc cref="IConnectServer"/>
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

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // this.Start();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Shutdown();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Start()
        {
            this.logger.LogInformation("Begin starting");
            var oldState = this.ServerState;
            this.ServerState = OpenMU.Interfaces.ServerState.Starting;
            try
            {
                this.ClientListener.StartListener();
                this.ServerState = OpenMU.Interfaces.ServerState.Started;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                this.ServerState = oldState;
            }

            this.logger.LogInformation("Finished starting");
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            this.logger.LogInformation("Begin stopping");
            this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
            this.ClientListener.StopListener();
            this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
            this.logger.LogInformation("Finished stopping");
        }

        /// <inheritdoc/>
        public void RegisterGameServer(IGameServerInfo gameServer, IPEndPoint publicEndPoint)
        {
            this.logger.LogInformation("GameServer {0} is registering with endpoint {1}", gameServer, publicEndPoint);
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
                this.logger.LogError("Error during registration process", ex);
                throw;
            }

            this.logger.LogInformation("GameServer {0} has registered with endpoint {1}", gameServer, publicEndPoint);
        }

        /// <inheritdoc/>
        public void UnregisterGameServer(IGameServerInfo gameServer)
        {
            this.logger.LogInformation("GameServer {0} is unregistering", gameServer);
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

            this.logger.LogInformation("GameServer {0} has unregistered", gameServer);
        }

        private void CreatePlugins()
        {
            this.logger.LogDebug("Begin creating plugins");
            this.ClientListener.ClientSocketAcceptPlugins.Add(new CheckMaximumConnectionsPlugin(this, this.loggerFactory.CreateLogger<CheckMaximumConnectionsPlugin>()));
            var clientCountPlugin = new ClientConnectionCountPlugin(this.Settings, this.loggerFactory.CreateLogger<ClientConnectionCountPlugin>());
            this.ClientListener.ClientSocketAcceptPlugins.Add(clientCountPlugin);
            this.ClientListener.ClientSocketDisconnectPlugins.Add(clientCountPlugin);
            this.logger.LogDebug("Finished creating plugins");
        }

        private void HandleServerPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IGameServerInfo.OnlinePlayerCount))
            {
                return;
            }

            if (sender is not IGameServerInfo server)
            {
                return;
            }

            var serverListItem = this.ServerList.Servers.FirstOrDefault(s => s.ServerId == server.Id);
            if (serverListItem is null)
            {
                return;
            }

            serverListItem.ServerLoad = (byte)(server.OnlinePlayerCount * 100f / server.MaximumPlayers);
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
