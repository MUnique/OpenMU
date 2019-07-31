// <copyright file="DefaultTcpGameServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using log4net;
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
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServer));

        private readonly GameServerEndpoint endPoint;
        private readonly IGameServerInfo gameServerInfo;

        private readonly GameServerContext gameContext;

        private readonly IGameServerStateObserver stateObserver;
        private readonly IIpAddressResolver addressResolver;
        private TcpListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTcpGameServerListener" /> class.
        /// </summary>
        /// <param name="endPoint">The endpoint to which this listener is listening.</param>
        /// <param name="gameServerInfo">The game server information.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="stateObserver">The connect server.</param>
        /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
        public DefaultTcpGameServerListener(GameServerEndpoint endPoint, IGameServerInfo gameServerInfo, GameServerContext gameContext, IGameServerStateObserver stateObserver, IIpAddressResolver addressResolver)
        {
            this.endPoint = endPoint;
            this.gameServerInfo = gameServerInfo;
            this.gameContext = gameContext;
            this.stateObserver = stateObserver;
            this.addressResolver = addressResolver;
        }

        /// <inheritdoc/>
        public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;

        /// <inheritdoc/>
        public void Start()
        {
            if (this.listener != null && this.listener.Server.IsBound)
            {
                Logger.Debug("listener is already running.");
                return;
            }

            var port = this.endPoint.NetworkPort;
            Logger.InfoFormat("Starting Server Listener, port {0}", port);
            this.listener = new TcpListener(IPAddress.Any, port);
            this.listener.Start();
            this.stateObserver.RegisterGameServer(this.gameServerInfo, new IPEndPoint(this.addressResolver.GetIPv4(), port));
            Task.Run(this.BeginAccept);
            Logger.Info("Server listener started.");
        }

        /// <inheritdoc/>
        public void Stop()
        {
            var port = this.endPoint.NetworkPort;
            this.stateObserver.UnregisterGameServer(this.gameServerInfo);
            Logger.Info($"Stopping listener on port {port}.");
            if (this.listener == null || !this.listener.Server.IsBound)
            {
                Logger.Debug("listener not running, nothing to shut down.");
                return;
            }

            this.listener.Stop();

            Logger.Info($"Stopped listener on port {port}.");
        }

        private async Task BeginAccept()
        {
            this.Log(l => l.Debug("Begin accepting new clients."));
            Socket newClient;
            try
            {
                newClient = await this.listener.AcceptSocketAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException ex)
            {
                this.Log(l => l.Debug("listener has been disposed", ex));
                return;
            }
            catch (Exception ex)
            {
                this.Log(l => l.Error("An unexpected error occured when awaiting the next client socket.", ex));
                return;
            }

            this.HandleNewSocket(newClient);

            // Accept the next Client:
            if (this.listener.Server.IsBound)
            {
                await this.BeginAccept().ConfigureAwait(false);
            }
        }

        private void HandleNewSocket(Socket socket)
        {
            if (socket == null)
            {
                return;
            }

            var remoteEndPoint = socket.RemoteEndPoint;
            this.Log(l => l.DebugFormat($"Game Client connected, Address {remoteEndPoint}"));
            if (this.gameContext.PlayerList.Count >= this.gameContext.ServerConfiguration.MaximumPlayers)
            {
                this.Log(l => l.DebugFormat($"The server is full... disconnecting the game client {remoteEndPoint}"));

                // MAYBE TODO: wait until another player disconnects?
                socket.Dispose();
            }
            else
            {
                socket.NoDelay = true;
                var socketConnection = SocketConnection.Create(socket);
                var clientVersion = new ClientVersion(this.endPoint.Client.Season, this.endPoint.Client.Episode, this.endPoint.Client.Language);
                var encryptionFactoryPlugIn = this.gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(clientVersion)
                                                ?? this.gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);
                IConnection connection;
                if (encryptionFactoryPlugIn == null)
                {
                    this.Log(l => l.WarnFormat("No network encryption plugin for version {0} available. It falls back to default encryption.", clientVersion));
                    connection = new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output));
                }
                else
                {
                    connection = new Connection(socketConnection, encryptionFactoryPlugIn.CreateDecryptor(socketConnection.Input, DataDirection.ClientToServer), encryptionFactoryPlugIn.CreateEncryptor(socketConnection.Output, DataDirection.ServerToClient));
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
                this.Log(l => l.Error($"Event {nameof(this.PlayerConnected)} was not handled."));
            }
        }

        private IDisposable PushServerLogContext()
        {
            if (log4net.ThreadContext.Stacks["gameserver"].Count > 0)
            {
                return null;
            }

            return log4net.ThreadContext.Stacks["gameserver"].Push(this.gameContext.Id.ToString());
        }

        private IDisposable PushEndpointContext()
        {
            if (log4net.ThreadContext.Stacks["endpoint"].Count > 0)
            {
                return null;
            }

            return log4net.ThreadContext.Stacks["endpoint"].Push(this.endPoint.ToString());
        }

        private void Log(Action<ILog> logAction)
        {
            using (this.PushServerLogContext())
            using (this.PushEndpointContext())
            {
                logAction(Logger);
            }
        }
    }
}