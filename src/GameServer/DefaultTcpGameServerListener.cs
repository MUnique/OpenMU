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
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// A game server listener that listens on a TCP port which uses the default packet handlers (<see cref="GameServerContext.PacketHandlers"/>).
    /// To be visible in the server list, this listener also registers the game server at the connect server.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameServer.IGameServerListener" />
    public class DefaultTcpGameServerListener : IGameServerListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServer));

        private readonly int port;

        private readonly IGameServerInfo gameServerInfo;

        private readonly GameServerContext gameContext;

        private readonly IConnectServer connectServer;
        private readonly IMainPacketHandler mainPacketHandler;
        private readonly IIpAddressResolver addressResolver;
        private TcpListener gslistener;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTcpGameServerListener" /> class.
        /// </summary>
        /// <param name="port">The tcp port.</param>
        /// <param name="gameServerInfo">The game server information.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="connectServer">The connect server.</param>
        /// <param name="mainPacketHandler">The main packet handler which should be used by clients which connected through this listener.</param>
        /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
        public DefaultTcpGameServerListener(int port, IGameServerInfo gameServerInfo, GameServerContext gameContext, IConnectServer connectServer, IMainPacketHandler mainPacketHandler, IIpAddressResolver addressResolver)
        {
            this.port = port;
            this.gameServerInfo = gameServerInfo;
            this.gameContext = gameContext;
            this.connectServer = connectServer;
            this.mainPacketHandler = mainPacketHandler;
            this.addressResolver = addressResolver;
        }

        /// <inheritdoc/>
        public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;

        /// <inheritdoc/>
        public void Start()
        {
            if (this.gslistener != null && this.gslistener.Server.IsBound)
            {
                Logger.Debug("listener is already running.");
                return;
            }

            Logger.InfoFormat("Starting Server Listener, port {0}", this.port);
            this.gslistener = new TcpListener(IPAddress.Any, this.port);
            this.gslistener.Start();
            this.connectServer.RegisterGameServer(this.gameServerInfo, new IPEndPoint(this.addressResolver.GetIPv4(), this.port));
            Task.Run(this.BeginAccept);
            Logger.Info("Server listener started.");
        }

        /// <inheritdoc/>
        public void Stop()
        {
            this.connectServer.UnregisterGameServer(this.gameServerInfo);
            Logger.Info($"Stopping listener on port {this.port}.");
            if (this.gslistener == null || !this.gslistener.Server.IsBound)
            {
                Logger.Debug("listener not running, nothing to shut down.");
                return;
            }

            this.gslistener.Stop();

            Logger.Info($"Stopped listener on port {this.port}.");
        }

        private async Task BeginAccept()
        {
            this.Log(l => l.Debug("Begin accepting new clients."));
            Socket newClient;
            try
            {
                newClient = await this.gslistener.AcceptSocketAsync();
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
            if (this.gslistener.Server.IsBound)
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
                var connection = new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output));
                var remotePlayer = new RemotePlayer(this.gameContext, this.mainPacketHandler, connection);
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

        private void Log(Action<ILog> logAction)
        {
            using (this.PushServerLogContext())
            {
                logAction(Logger);
            }
        }
    }
}