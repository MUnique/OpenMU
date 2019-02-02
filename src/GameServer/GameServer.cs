﻿// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The game server to which game clients can connect.
    /// </summary>
    public sealed class GameServer : IGameServer, IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServer));

        private readonly GameServerContext gameContext;

        private readonly ICollection<IGameServerListener> listeners = new List<IGameServerListener>();
        private readonly IServerStateObserver stateObserver;

        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServer" /> class.
        /// </summary>
        /// <param name="gameServerDefinition">The game server definition.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="stateObserver">The state observer.</param>
        public GameServer(
            GameServerDefinition gameServerDefinition,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IPersistenceContextProvider persistenceContextProvider,
            IFriendServer friendServer,
            IServerStateObserver stateObserver)
        {
            this.Id = gameServerDefinition.ServerID;
            this.Description = gameServerDefinition.Description;
            this.stateObserver = stateObserver;

            try
            {
                var mapInitializer = new GameServerMapInitializer(gameServerDefinition, this.Id, stateObserver);
                this.gameContext = new GameServerContext(gameServerDefinition, guildServer, loginServer, friendServer, persistenceContextProvider, stateObserver, mapInitializer);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw;
            }

            this.ServerInfo = new GameServerInfoAdapter(this, gameServerDefinition.ServerConfiguration);
        }

        /// <summary>
        /// Gets the identifier of the server.
        /// </summary>
        public byte Id { get; }

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
                    this.stateObserver?.ServerStateChanged(this.Id, value);
                }
            }
        }

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

        /// <inheritdoc/>
        public void Start()
        {
            using (this.PushServerLogContext())
            {
                this.ServerState = ServerState.Starting;
                foreach (var listener in this.listeners)
                {
                    listener.Start();
                }

                this.ServerState = ServerState.Started;
            }
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            using (this.PushServerLogContext())
            {
                this.ServerState = ServerState.Stopping;
                Logger.Info("Server is shutting down. Stopping listener...");
                foreach (var listener in this.listeners)
                {
                    listener.Stop();
                }

                Logger.Info("Saving all open sessions...");
                for (int i = this.gameContext.PlayerList.Count - 1; i >= 0; i--)
                {
                    this.gameContext.PlayerList[i].Disconnect();
                }

                this.ServerState = ServerState.Stopped;
                Logger.Info("Server shutted down.");
            }
        }

        /// <inheritdoc/>
        public void GuildChatMessage(uint guildId, string sender, string message)
        {
            var guildplayers = from player in this.gameContext.PlayerList
                where player.GuildStatus?.GuildId == guildId
                select player;

            if (!message.StartsWith("@"))
            {
                message = "@" + message;
            }

            foreach (var player in guildplayers)
            {
                player.PlayerView.ChatMessage(message, sender, 0);
            }
        }

        /// <inheritdoc/>
        public void AllianceChatMessage(uint guildId, string sender, string message)
        {
            var guildplayers = from player in this.gameContext.PlayerList
                where player.GuildStatus?.GuildId == guildId
                select player;

            if (!message.StartsWith("@@"))
            {
                message = "@@" + message;
            }

            // TODO: determine alliance
            foreach (var player in guildplayers)
            {
                player.PlayerView.ChatMessage(message, sender, 0);
            }
        }

        /// <inheritdoc/>
        public void LetterReceived(LetterHeader letter)
        {
            var player = this.gameContext.GetPlayerByCharacterName(letter.ReceiverName);
            if (player != null)
            {
                var newLetterIndex = player.SelectedCharacter.Letters.Count;
                player.PersistenceContext.Attach(letter);
                player.SelectedCharacter.Letters.Add(letter);
                player.PlayerView.MessengerView.AddToLetterList(letter, (ushort)newLetterIndex, true);
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
            return this.gameContext.PlayerList.Any(player => player.Account.LoginName == accountName);
        }

        /// <inheritdoc />
        public bool DisconnectPlayer(string playerName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player != null)
            {
                player.PlayerView.ShowMessage("You got disconnected by a game master.", MessageType.BlueNormal);
                player.Disconnect();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool BanPlayer(string playerName)
        {
            var player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player != null)
            {
                player.Account.State = AccountState.TemporarilyBanned;
                player.PlayerView.ShowMessage("Your account has been temporarily banned by a game master.", MessageType.BlueNormal);
                player.Disconnect();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void SendGlobalMessage(string message, MessageType messageType)
        {
            for (int i = this.gameContext.PlayerList.Count - 1; i >= 0; i--)
            {
                var player = this.gameContext.PlayerList[i];
                player.PlayerView.ShowMessage(message, messageType);
            }
        }

        /// <inheritdoc/>
        public void FriendRequest(string requester, string receiver)
        {
            Player player = this.gameContext.GetPlayerByCharacterName(receiver);
            player?.PlayerView.MessengerView.ShowFriendRequest(requester);
        }

        /// <inheritdoc/>
        public void FriendOnlineStateChanged(string player, string friend, int serverId)
        {
            Player observerPlayer = this.gameContext.GetPlayerByCharacterName(player);
            observerPlayer?.PlayerView.MessengerView.FriendStateUpdate(friend, serverId);
        }

        /// <inheritdoc/>
        public void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string creatorName)
        {
            Player player = this.gameContext.GetPlayerByCharacterName(authenticationInfo.ClientName);
            player?.PlayerView.MessengerView.ChatRoomCreated(authenticationInfo, creatorName, true);
        }

        /// <inheritdoc/>
        public void GuildDeleted(uint guildId)
        {
            this.gameContext.PlayerList
                .Where(player => player.GuildStatus?.GuildId == guildId)
                .ForEach(RemovePlayerFromGuild);
            //// todo: alliance things?
        }

        /// <inheritdoc/>
        public void GuildPlayerKicked(string playerName)
        {
            Player player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player == null)
            {
                return;
            }

            RemovePlayerFromGuild(player);
        }

        /// <inheritdoc/>
        public void RegisterMapObserver(ushort mapId, object worldObserver)
        {
            var locateableObserver = worldObserver as ILocateable;
            if (locateableObserver == null)
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
            var map = this.gameContext.GetMap(mapId);
            if (map != null)
            {
                var observer = map.GetObject(worldObserverId);
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
            player.ForEachObservingPlayer(observer => observer.PlayerView.GuildView.PlayerLeftGuild(player), true);
            player.GuildStatus = null;
        }

        private void OnPlayerConnected(object sender, PlayerConnectedEventArgs e)
        {
            var player = e.ConntectedPlayer;
            this.gameContext.AddPlayer(player);
            player.PlayerView.ShowLoginWindow();
            player.PlayerState.TryAdvanceTo(PlayerState.LoginScreen);
            e.ConntectedPlayer.PlayerDisconnected += (s, args) => this.OnPlayerDisconnected(player);
        }

        private void OnPlayerDisconnected(Player remotePlayer)
        {
            this.SetOfflineAtFriendServer(remotePlayer);
            this.SaveSessionOfPlayer(remotePlayer);
            this.SetOfflineAtLoginServer(remotePlayer);
            remotePlayer.Dispose();
        }

        private void SetOfflineAtLoginServer(Player player)
        {
            try
            {
                this.Context.LoginServer.LogOff(player.Account?.LoginName, this.Id);
            }
            catch (Exception ex)
            {
                Logger.Error($"Couldn't set offline state at login server. Player: {this}", ex);
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
                Logger.Error($"Couldn't set offline state at friend server. Player: {this}", ex);
            }
        }

        private void SaveSessionOfPlayer(Player player)
        {
            try
            {
                if (!player.PersistenceContext.SaveChanges())
                {
                    Logger.Warn($"Could not save session of player {player}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Couldn't Save at Disconnect. Player: {this}", ex);

                // TODO: Log Character/Account values, to be able to restore players data if necessary.
            }
        }

        private IDisposable PushServerLogContext()
        {
            if (log4net.ThreadContext.Stacks["gameserver"].Count > 0)
            {
                return null;
            }

            return log4net.ThreadContext.Stacks["gameserver"].Push(this.Id.ToString());
        }
    }
}
