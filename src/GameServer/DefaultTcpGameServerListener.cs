// <copyright file="DefaultTcpGameServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A game server listener that listens on a TCP port.
    /// To be visible in the server list, this listener also registers the game server at the connect server.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameServer.IGameServerListener" />
    public class DefaultTcpGameServerListener : IGameServerListener
    {
        private readonly ILogger<DefaultTcpGameServerListener> logger;

        private readonly GameServerEndpoint endPoint;
        private readonly IGameServerInfo gameServerInfo;

        private readonly GameServerContext gameContext;

        private readonly IGameServerStateObserver stateObserver;
        private readonly IIpAddressResolver addressResolver;
        private readonly ILoggerFactory loggerFactory;
        private TcpListener? listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTcpGameServerListener" /> class.
        /// </summary>
        /// <param name="endPoint">The endpoint to which this listener is listening.</param>
        /// <param name="gameServerInfo">The game server information.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="stateObserver">The connect server.</param>
        /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public DefaultTcpGameServerListener(GameServerEndpoint endPoint, IGameServerInfo gameServerInfo, GameServerContext gameContext, IGameServerStateObserver stateObserver, IIpAddressResolver addressResolver, ILoggerFactory loggerFactory)
        {
            this.endPoint = endPoint;
            this.gameServerInfo = gameServerInfo;
            this.gameContext = gameContext;
            this.stateObserver = stateObserver;
            this.addressResolver = addressResolver;
            this.loggerFactory = loggerFactory;
            this.logger = this.loggerFactory.CreateLogger<DefaultTcpGameServerListener>();
        }

        /// <inheritdoc/>
        public event EventHandler<PlayerConnectedEventArgs>? PlayerConnected;

        /// <inheritdoc/>
        public void Start()
        {
            if (this.listener != null && this.listener.Server.IsBound)
            {
                this.logger.LogDebug("listener is already running.");
                return;
            }

            var port = this.endPoint.NetworkPort;
            this.logger.LogInformation("Starting Server Listener, port {port}", port);
            this.listener = new TcpListener(IPAddress.Any, port);
            this.listener.Start();
            if (this.endPoint.AlternativePublishedPort > 0)
            {
                port = this.endPoint.AlternativePublishedPort;
                this.logger.LogWarning("GameServer endpoint of port {0} has registered an alternative public port of {1}.", this.endPoint.NetworkPort, port);
            }

            this.stateObserver.RegisterGameServer(this.gameServerInfo, new IPEndPoint(this.addressResolver.ResolveIPv4(), port));
            Task.Run(this.BeginAccept);
            this.logger.LogInformation("Server listener started.");
        }

        /// <inheritdoc/>
        public void Stop()
        {
            var port = this.endPoint.NetworkPort;
            this.stateObserver.UnregisterGameServer(this.gameServerInfo);
            this.logger.LogInformation($"Stopping listener on port {port}.");
            if (this.listener is null || !this.listener.Server.IsBound)
            {
                this.logger.LogDebug("listener not running, nothing to shut down.");
                return;
            }

            this.listener.Stop();

            this.logger.LogInformation($"Stopped listener on port {port}.");
        }

        private async Task BeginAccept()
        {
            this.Log(l => l.LogDebug("Begin accepting new clients."));
            Socket newClient;
            try
            {
                if (this.listener is null)
                {
                    return;
                }

                newClient = await this.listener.AcceptSocketAsync().ConfigureAwait(false);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
            {
                this.Log(l => l.LogDebug(ex, "listener has been closed"));
                return;
            }
            catch (ObjectDisposedException ex)
            {
                this.Log(l => l.LogDebug(ex, "listener has been disposed"));
                return;
            }
            catch (Exception ex)
            {
                this.Log(l => l.LogError(ex, "An unexpected error occured when awaiting the next client socket."));
                return;
            }

            this.HandleNewSocket(newClient);

            // Accept the next Client:
            if (this.listener?.Server.IsBound ?? false)
            {
                await this.BeginAccept().ConfigureAwait(false);
            }
        }

        private void HandleNewSocket(Socket socket)
        {
            if (socket is null)
            {
                return;
            }

            var remoteEndPoint = socket.RemoteEndPoint;
            this.Log(l => l.LogDebug($"Game Client connected, Address {remoteEndPoint}"));
            if (this.gameContext.PlayerList.Count >= this.gameContext.ServerConfiguration.MaximumPlayers)
            {
                this.Log(l => l.LogDebug($"The server is full... disconnecting the game client {remoteEndPoint}"));

                // MAYBE TODO: wait until another player disconnects?
                socket.Dispose();
            }
            else
            {
                socket.NoDelay = true;
                var socketConnection = SocketConnection.Create(socket);
                var clientVersion = new ClientVersion(this.endPoint.Client!.Season, this.endPoint.Client.Episode, this.endPoint.Client.Language);
                var encryptionFactoryPlugIn = this.gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(clientVersion)
                                                ?? this.gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);
                IConnection connection;
                if (encryptionFactoryPlugIn is null)
                {
                    this.Log(l => l.LogWarning("No network encryption plugin for version {clientVersion} available. It falls back to default encryption.", clientVersion));
                    connection = new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output), this.loggerFactory.CreateLogger<Connection>());
                }
                else
                {
                    connection = new Connection(socketConnection, encryptionFactoryPlugIn.CreateDecryptor(socketConnection.Input, DataDirection.ClientToServer), encryptionFactoryPlugIn.CreateEncryptor(socketConnection.Output, DataDirection.ServerToClient), this.loggerFactory.CreateLogger<Connection>());
                }

                var remotePlayer = new RemotePlayer(this.gameContext, connection, clientVersion);
                this.OnPlayerConnected(remotePlayer);
                connection.Disconnected += (sender, e) => remotePlayer.Disconnect();

                // we don't want to await the call.
                connection.BeginReceive();
            }
        }

        private void OnPlayerConnected(Player player)
        {
            var eventHandler = this.PlayerConnected;
            if (eventHandler != null)
            {
                eventHandler(this, new PlayerConnectedEventArgs(player));
            }
            else
            {
                this.Log(l => l.LogError($"Event {nameof(this.PlayerConnected)} was not handled."));
            }
        }

        private void Log(Action<ILogger<DefaultTcpGameServerListener>> logAction)
        {
            using var contextScope = this.logger.BeginScope(("GameServer", this.gameContext.Id), ("EndPoint", this.endPoint));
            logAction(this.logger);
        }
    }
}