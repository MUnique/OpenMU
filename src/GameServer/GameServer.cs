// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The game server to which game clients can connect.
    /// </summary>
    public sealed class GameServer : IGameServer, IDisposable
    {
        private readonly ILogger<GameServer> logger;

        private readonly GameServerContext gameContext;

        private readonly ICollection<IGameServerListener> listeners = new List<IGameServerListener>();

        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServer" /> class.
        /// </summary>
        /// <param name="gameServerDefinition">The game server definition.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public GameServer(
            GameServerDefinition gameServerDefinition,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IPersistenceContextProvider persistenceContextProvider,
            IFriendServer friendServer,
            ILoggerFactory loggerFactory,
            PlugInManager plugInManager)
        {
            this.Id = gameServerDefinition.ServerID;
            this.Description = gameServerDefinition.Description;
            this.ConfigurationId = gameServerDefinition.GetId();
            this.logger = loggerFactory.CreateLogger<GameServer>();
            try
            {
                var mapInitializer = new GameServerMapInitializer(gameServerDefinition, loggerFactory.CreateLogger<GameServerMapInitializer>());
                this.gameContext = new GameServerContext(gameServerDefinition, guildServer, loginServer, friendServer, persistenceContextProvider, mapInitializer, loggerFactory, plugInManager);
                this.gameContext.GameMapCreated += (sender, e) => this.OnPropertyChanged(nameof(this.Context));
                this.gameContext.GameMapRemoved += (sender, e) => this.OnPropertyChanged(nameof(this.Context));
                mapInitializer.PlugInManager = this.gameContext.PlugInManager;
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Error during map initialization");
                throw;
            }

            this.ServerInfo = new GameServerInfoAdapter(this, gameServerDefinition.ServerConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a ServerConfiguration"));
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the identifier of the server.
        /// </summary>
        public byte Id { get; }

        /// <inheritdoc />
        public Guid ConfigurationId { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <summary>
        /// Gets the server context.
        /// </summary>
        public GameServerContext Context => this.gameContext;

        /// <inheritdoc/>
        public ServerState ServerState
        {
            get => this.serverState;
            set
            {
                if (this.serverState != value)
                {
                    this.serverState = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public ServerType Type => ServerType.GameServer;

        /// <inheritdoc/>
        public int MaximumConnections => this.ServerInfo.MaximumPlayers;

        /// <inheritdoc/>
        public int CurrentConnections => this.ServerInfo.OnlinePlayerCount;

        /// <inheritdoc/>
        int IManageableServer.Id => this.Id;

        /// <inheritdoc/>
        public IGameServerInfo ServerInfo { get; }

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(IGameServerListener listener)
        {
            this.listeners.Add(listener);
            listener.PlayerConnected += this.OnPlayerConnected;
        }

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
            using var logScope = this.logger.BeginScope(("GameServer", this.Id));
            this.ServerState = ServerState.Starting;
            try
            {
                foreach (var listener in this.listeners)
                {
                    listener.Start();
                }

                this.ServerState = ServerState.Started;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"Could not start the server listeners: {e.Message}");
                foreach (var listener in this.listeners)
                {
                    listener.Stop();
                }

                this.ServerState = ServerState.Stopped;
            }
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            using var logScope = this.logger.BeginScope(("GameServer", this.Id));
            this.ServerState = ServerState.Stopping;
            this.logger.LogInformation("Server is shutting down. Stopping listener...");
            foreach (var listener in this.listeners)
            {
                listener.Stop();
            }

            this.logger.LogInformation("Saving all open sessions...");
            for (int i = this.gameContext.PlayerList.Count - 1; i >= 0; i--)
            {
                this.gameContext.PlayerList[i].Disconnect();
            }

            this.ServerState = ServerState.Stopped;
            this.logger.LogInformation("Server shutted down.");
        }

        /// <inheritdoc/>
        public void GuildChatMessage(uint guildId, string sender, string message)
        {
            var guildPlayers = from player in this.gameContext.PlayerList
                where player.GuildStatus?.GuildId == guildId
                select player;

            string messageSend = message;

            if (!messageSend.StartsWith("@", StringComparison.InvariantCulture))
            {
                messageSend = "@" + message;
            }

            foreach (var player in guildPlayers)
            {
                player.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()?.ChatMessage(messageSend, sender, 0);
            }
        }

        /// <inheritdoc/>
        public void AllianceChatMessage(uint guildId, string sender, string message)
        {
            var guildPlayers = from player in this.gameContext.PlayerList
                where player.GuildStatus?.GuildId == guildId
                select player;

            string messageSend = message;

            if (!messageSend.StartsWith("@@", StringComparison.InvariantCulture))
            {
                messageSend = "@@" + message;
            }

            // TODO: determine alliance
            foreach (var player in guildPlayers)
            {
                player.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()?.ChatMessage(messageSend, sender, ChatMessageType.Alliance);
            }
        }

        /// <inheritdoc/>
        public void LetterReceived(LetterHeader letter)
        {
            if (letter.ReceiverName is null)
            {
                throw new InvalidOperationException("Letter ReceiverName must be provided.");
            }

            var player = this.gameContext.GetPlayerByCharacterName(letter.ReceiverName);
            if (player != null)
            {
                var newLetterIndex = player.SelectedCharacter!.Letters.Count;
                player.PersistenceContext.Attach(letter);
                player.SelectedCharacter.Letters.Add(letter);
                player.ViewPlugIns.GetPlugIn<IAddToLetterListPlugIn>()?.AddToLetterList(letter, (ushort)newLetterIndex, true);
            }
        }

        /// <inheritdoc/>
        public bool IsPlayerOnline(string playerName)
        {
            return this.gameContext.GetPlayerByCharacterName(playerName) != null;
        }

        /// <inheritdoc/>
        public bool IsAccountOnline(string accountName)
        {
            return this.gameContext.PlayerList.Any(player => player.Account?.LoginName == accountName);
        }

        /// <inheritdoc />
        public bool DisconnectPlayer(string playerName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player != null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You got disconnected by a game master.", MessageType.BlueNormal);
                player.Disconnect();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool BanPlayer(string playerName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player?.Account is not null)
            {
                player.Account.State = AccountState.TemporarilyBanned;
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Your account has been temporarily banned by a game master.", MessageType.BlueNormal);
                player.Disconnect();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void SendGlobalMessage(string message, MessageType messageType)
        {
            this.gameContext.SendGlobalMessage(message, messageType);
        }

        /// <inheritdoc/>
        public void FriendRequest(string requester, string receiver)
        {
            var player = this.gameContext.GetPlayerByCharacterName(receiver);
            player?.ViewPlugIns.GetPlugIn<IShowFriendRequestPlugIn>()?.ShowFriendRequest(requester);
        }

        /// <inheritdoc/>
        public void FriendOnlineStateChanged(string player, string friend, int serverId)
        {
            var observerPlayer = this.gameContext.GetPlayerByCharacterName(player);
            observerPlayer?.ViewPlugIns.GetPlugIn<IFriendStateUpdatePlugIn>()?.FriendStateUpdate(friend, serverId);
        }

        /// <inheritdoc/>
        public void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string creatorName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(authenticationInfo.ClientName);
            player?.ViewPlugIns.GetPlugIn<IChatRoomCreatedPlugIn>()?.ChatRoomCreated(authenticationInfo, creatorName, true);
        }

        /// <inheritdoc/>
        public void GuildDeleted(uint guildId)
        {
            this.gameContext.PlayerList
                .Where(player => player.GuildStatus?.GuildId == guildId)
                .ForEach(RemovePlayerFromGuild);
            this.gameContext.RaiseGuildDeleted(guildId);
            //// todo: alliance things?
        }

        /// <inheritdoc/>
        public void GuildPlayerKicked(string playerName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player is null)
            {
                return;
            }

            RemovePlayerFromGuild(player);
        }

        /// <inheritdoc/>
        public void RegisterMapObserver(ushort mapId, object worldObserver)
        {
            if (worldObserver is not ILocateable locateableObserver)
            {
                throw new ArgumentException("worldObserver needs to implement ILocateable", nameof(worldObserver));
            }

            var map = this.gameContext.GetMap(mapId);
            if (map != null)
            {
                map.Add(locateableObserver);
            }
            else
            {
                var message = $"map with id {mapId} not found.";
                throw new ArgumentException(message);
            }
        }

        /// <inheritdoc/>
        public void UnregisterMapObserver(ushort mapId, ushort worldObserverId)
        {
            if (this.gameContext.GetMap(mapId) is { } map && map.GetObject(worldObserverId) is { } observer)
            {
                map.Remove(observer);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Id} - [{this.Description}]";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "gameContext", Justification = "Null-conditional confuses the code analysis.")]
        public void Dispose()
        {
            this.gameContext?.Dispose();
        }

        private static void RemovePlayerFromGuild(Player player)
        {
            player.ForEachObservingPlayer(observer => observer.ViewPlugIns.GetPlugIn<IPlayerLeftGuildPlugIn>()?.PlayerLeftGuild(player), true);
            player.GuildStatus = null;
            player.ViewPlugIns.GetPlugIn<IGuildKickResultPlugIn>()?.GuildKickResult(GuildKickSuccess.KickSucceeded);
        }

        private void OnPlayerConnected(object? sender, PlayerConnectedEventArgs e)
        {
            var player = e.ConntectedPlayer;
            this.gameContext.AddPlayer(player);
            player.ViewPlugIns.GetPlugIn<IShowLoginWindowPlugIn>()?.ShowLoginWindow();
            player.PlayerState.TryAdvanceTo(PlayerState.LoginScreen);
            e.ConntectedPlayer.PlayerDisconnected += (s, args) => this.OnPlayerDisconnected(player);
            this.OnPropertyChanged(nameof(this.CurrentConnections));
        }

        private void OnPlayerDisconnected(Player remotePlayer)
        {
            this.SetOfflineAtFriendServer(remotePlayer);
            this.SaveSessionOfPlayer(remotePlayer);
            this.SetOfflineAtLoginServer(remotePlayer);
            remotePlayer.Dispose();
            this.OnPropertyChanged(nameof(this.CurrentConnections));
        }

        private void SetOfflineAtLoginServer(Player player)
        {
            try
            {
                if (player.Account?.LoginName is { } loginName)
                {
                    this.Context.LoginServer.LogOff(loginName, this.Id);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Couldn't set offline state at login server. Player: {this}", ex);
            }
        }

        private void SetOfflineAtFriendServer(Player player)
        {
            try
            {
                if (player.SelectedCharacter != null)
                {
                    this.Context.FriendServer.SetOnlineState(player.SelectedCharacter.Id, player.SelectedCharacter.Name, (byte)SpecialServerId.Offline);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Couldn't set offline state at friend server. Player: {this}", ex);
            }
        }

        private void SaveSessionOfPlayer(Player player)
        {
            try
            {
                if (!player.PersistenceContext.SaveChanges())
                {
                    this.logger.LogWarning($"Could not save session of player {player}");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Couldn't Save at Disconnect. Player: {this}", ex);

                // TODO: Log Character/Account values, to be able to restore players data if necessary.
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
