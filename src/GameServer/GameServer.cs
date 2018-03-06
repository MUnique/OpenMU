// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The game server to which game clients can connect.
    /// </summary>
    public sealed class GameServer : IGameServer, IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServer));

        private readonly GameServerContext gameContext;

        private readonly ConcurrentBag<ushort> freePlayerIds;

        private readonly ICollection<IGameServerListener> listeners = new List<IGameServerListener>();

        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServer"/> class.
        /// </summary>
        /// <param name="gameServerDefinition">The game server definition.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="friendServer">The friend server.</param>
        public GameServer(
            GameServerDefinition gameServerDefinition,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IRepositoryManager repositoryManager,
            IFriendServer friendServer)
        {
            this.Id = gameServerDefinition.ServerID;
            this.Description = gameServerDefinition.Description;

            try
            {
                this.gameContext = new GameServerContext(gameServerDefinition, guildServer, loginServer, friendServer, repositoryManager);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw;
            }

            this.freePlayerIds =
                new ConcurrentBag<ushort>(
                    Enumerable.Range(
                        gameServerDefinition.ServerConfiguration.MaximumNPCs + 1,
                        gameServerDefinition.ServerConfiguration.MaximumPlayers).Select(id => (ushort)id));
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
        public ServerState ServerState { get; set; }

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
            listener.PlayerIdRequested += this.OnPlayerIdRequested;
            listener.PlayerConnected += this.OnPlayerConnected;
        }

        /// <inheritdoc/>
        public void Start()
        {
            using (this.PushServerLogContext())
            {
                this.Initialize();
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
        public void GuildChatMessage(Guid guildId, string sender, string message)
        {
            var guildplayers = from player in this.gameContext.PlayerList
                where player.SelectedCharacter?.GuildMemberInfo?.GuildId == guildId
                select player;

            foreach (var player in guildplayers)
            {
                player.PlayerView.ChatMessage("@" + message, sender, 0);
            }
        }

        /// <inheritdoc/>
        public void AllianceChatMessage(Guid guildId, string sender, string message)
        {
            var guildplayers = from player in this.gameContext.PlayerList
                where player.SelectedCharacter?.GuildMemberInfo?.GuildId == guildId
                select player;

            // TODO: determine alliance
            foreach (var player in guildplayers)
            {
                player.PlayerView.ChatMessage("@@" + message, sender, 0);
            }
        }

        /// <inheritdoc/>
        public void LetterReceived(LetterHeader letter)
        {
            var player = this.gameContext.GetPlayerByCharacterName(letter.Receiver);
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
        public void GuildDeleted(Guid guildId)
        {
            this.gameContext.PlayerList
                .Where(
                    player =>
                        player.SelectedCharacter?.GuildMemberInfo != null &&
                        player.SelectedCharacter.GuildMemberInfo.GuildId == guildId)
                .ForEach(RemovePlayerFromGuild);
            this.gameContext.PlayerList
                .SelectMany(player => player.Account?.Characters).Where(c => c.GuildMemberInfo?.GuildId == guildId)
                .ForEach(character => character.GuildMemberInfo = null);
            ////todo: alliance things?
        }

        /// <inheritdoc/>
        public void GuildPlayerKicked(string playerName)
        {
            Player player = this.gameContext.GetPlayerByCharacterName(playerName);
            if (player == null)
            {
                this.RemovePlayerFromGuild(playerName);
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

            if (this.gameContext.MapList.TryGetValue(mapId, out GameMap map))
            {
                map.Add(locateableObserver);
            }
            else
            {
                var message = $"map with id {mapId} not found.";
                if (!this.gameContext.MapList.Any())
                {
                    message += " Maps not initialized yet.";
                }

                throw new ArgumentException(message);
            }
        }

        /// <inheritdoc/>
        public void UnregisterMapObserver(ushort mapId, ushort worldObserverId)
        {
            if (this.gameContext.MapList.TryGetValue(mapId, out GameMap map))
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
            player.ShortGuildID = 0;
            player.SelectedCharacter.GuildMemberInfo = null;
            player.ForEachObservingPlayer(observer => observer.PlayerView.GuildView.PlayerLeftGuild(player), true);
        }

        private void RemovePlayerFromGuild(string characterName)
        {
            var affectedPlayer =
                this.gameContext.PlayerList.FirstOrDefault(
                    p => p.Account?.Characters.Any(c => c.Name == characterName) ?? false);
            var character = affectedPlayer?.Account.Characters.FirstOrDefault(c => c.Name == characterName);
            if (character != null)
            {
                character.GuildMemberInfo = null;
            }
        }

        private void OnPlayerIdRequested(object sender, RequestPlayerIdEventArgs e)
        {
            e.Cancel = !this.freePlayerIds.TryTake(out ushort newPlayerId);
            e.PlayerId = newPlayerId;
        }

        private void OnPlayerConnected(object sender, PlayerConnectedEventArgs e)
        {
            var player = e.ConntectedPlayer;
            this.gameContext.AddPlayer(player);
            player.PlayerView.ShowLoginWindow();
            player.PlayerState.TryAdvanceTo(PlayerState.LoginScreen);
            e.ConntectedPlayer.PlayerDisconnected += (s, args) => this.OnPlayerDisconnected(player);
        }

        private void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            Logger.Info("Initializing game server...");

            this.InitializeMaps();
            this.InitializeNpcs();
            this.initialized = true;
        }

        private void InitializeMaps()
        {
            Logger.Info("Initializing Maps...");
            var configuration = this.gameContext.ServerConfiguration;
            foreach (var map in configuration.Maps.OrderBy(m => m.Number))
            {
                var gameMap = new GameMap(map, 60, 8, (ushort)(configuration.MaximumNPCs + configuration.MaximumPlayers + 1));
                this.Context.MapList.Add(map.Number.ToUnsigned(), gameMap);
            }
        }

        private void InitializeNpcs()
        {
            ushort npcId = 0;
            var dropGenerator = new DefaultDropGenerator(this.Context.Configuration, Rand.GetRandomizer());
            foreach (var map in this.Context.MapList.Values.Where(m => m.Definition.MonsterSpawns.Any()))
            {
                Logger.Debug($"Start creating monster instances for map {map}");
                foreach (var spawn in map.Definition.MonsterSpawns)
                {
                    for (int i = 0; i < spawn.Quantity; i++)
                    {
                        npcId++;
                        if (npcId > this.Context.ServerConfiguration.MaximumNPCs)
                        {
                            throw new InvalidOperationException(
                                "Maximum npc count exceeded. Either increase the limit or remove monster instances.");
                        }

                        var monsterDef = spawn.MonsterDefinition;
                        if (monsterDef.AttackDelay > TimeSpan.Zero)
                        {
                            Logger.Debug($"Creating monster {spawn}");
                            var monster = new Monster(spawn, monsterDef, npcId, map, dropGenerator, new BasicMonsterIntelligence(map));
                            monster.Respawn();
                        }
                        else
                        {
                            Logger.Debug($"Creating npc {spawn}");
                            var npc = new NonPlayerCharacter(spawn, monsterDef, npcId, map);
                            npc.Respawn();
                        }
                    }
                }

                Logger.Debug($"Finished creating monster instances for map {map}");
            }
        }

        private void OnPlayerDisconnected(Player remotePlayer)
        {
            this.SetOfflineAtFriendServer(remotePlayer);
            this.SaveSessionOfPlayer(remotePlayer);
            this.SetOfflineAtLoginServer(remotePlayer);
            this.freePlayerIds.Add(remotePlayer.Id);
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

                // TODO: Log Character/Account values, to be able to restore players data if neccessary.
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

        private class GameServerInfoAdapter : IGameServerInfo
        {
            private readonly GameServer gameServer;
            private readonly GameServerConfiguration configuration;

            public GameServerInfoAdapter(GameServer gameServer, GameServerConfiguration configuration)
            {
                this.gameServer = gameServer;
                this.configuration = configuration;
            }

            public byte Id => this.gameServer.Id;

            public string Description => this.gameServer.Description;

            public ServerState State => this.gameServer.ServerState;

            public int OnlinePlayerCount => this.gameServer.Context.PlayerList.Count;

            public int MaximumPlayers => this.configuration.MaximumPlayers;

            public IList<IGameMapInfo> Maps
            {
                get { return this.gameServer.Context.MapList.Values.Select(map => new GameMapInfo(map, this.gameServer.Context.PlayerList.Where(p => p.CurrentMap == map).ToList())).ToList<IGameMapInfo>(); }
            }

            public override string ToString()
            {
                return this.gameServer.ToString();
            }

            private class GameMapInfo : IGameMapInfo
            {
                private readonly GameMap map;

                private readonly IEnumerable<Player> players;

                public GameMapInfo(GameMap map, IEnumerable<Player> players)
                {
                    this.map = map;
                    this.players = players;
                }

                public GameMapDefinition Map => this.map.Definition;

                public System.Collections.Generic.IList<IPlayerInfo> Players
                {
                    get { return this.players.Select(p => new PlayerInfo(p) as IPlayerInfo).ToList(); }
                }
            }

            private class PlayerInfo : IPlayerInfo
            {
                private readonly Player player;

                public PlayerInfo(Player player)
                {
                    this.player = player;
                }

                public string HostAdress
                {
                    get
                    {
                        var remotePlayer = this.player as RemotePlayer;
                        if (remotePlayer?.Connection != null)
                        {
                            return remotePlayer.Connection.ToString();
                        }

                        return "N/A";
                    }
                }

                public string CharacterName
                {
                    get
                    {
                        var character = this.player.SelectedCharacter;
                        if (character != null)
                        {
                            return character.Name;
                        }

                        return "N/A";
                    }
                }

                public string AccountName => this.player.Account.LoginName;

                public byte LocationX => this.player.X;

                public byte LocationY => this.player.Y;
            }
        }
    }
}
