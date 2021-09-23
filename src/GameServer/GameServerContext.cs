// <copyright file="GameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The context of a game server which contains all important configurations and services used by one game server instance.
    /// </summary>
    public class GameServerContext : GameContext, IGameServerContext
    {
        private readonly GameServerDefinition gameServerDefinition;

        private readonly ConcurrentDictionary<uint, List<Player>> playersByGuild = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerContext" /> class.
        /// </summary>
        /// <param name="gameServerDefinition">The game server definition.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="mapInitializer">The map initializer.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public GameServerContext(
            GameServerDefinition gameServerDefinition,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IFriendServer friendServer,
            IPersistenceContextProvider persistenceContextProvider,
            IMapInitializer mapInitializer,
            ILoggerFactory loggerFactory,
            PlugInManager plugInManager)
            : base(gameServerDefinition.GameConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a GameConfiguration"), persistenceContextProvider, mapInitializer, loggerFactory, plugInManager)
        {
            this.gameServerDefinition = gameServerDefinition;
            this.Id = gameServerDefinition.ServerID;
            this.GuildServer = guildServer;
            this.LoginServer = loginServer;
            this.FriendServer = friendServer;
            this.ServerConfiguration = gameServerDefinition.ServerConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a ServerConfiguration");
        }

        /// <summary>
        /// Occurs when a guild has been deleted.
        /// </summary>
        public event EventHandler<GuildDeletedEventArgs>? GuildDeleted;

        /// <inheritdoc/>
        public byte Id { get; }

        /// <inheritdoc/>
        public IGuildServer GuildServer { get; }

        /// <inheritdoc/>
        public ILoginServer LoginServer { get; }

        /// <inheritdoc/>
        public IFriendServer FriendServer { get; }

        /// <inheritdoc/>
        public GameServerConfiguration ServerConfiguration { get; }

        /// <inheritdoc />
        public override float ExperienceRate => base.ExperienceRate * this.gameServerDefinition.ExperienceRate;

        /// <inheritdoc />
        public void ForEachGuildPlayer(uint guildId, Action<Player> action)
        {
            if (!this.playersByGuild.TryGetValue(guildId, out var playerList))
            {
                return;
            }

            lock (playerList)
            {
                for (int i = playerList.Count - 1; i >= 0; i--)
                {
                    var player = playerList[i];
                    action(player);
                }
            }
        }

        /// <inheritdoc />
        public void ForEachAlliancePlayer(uint guildId, Action<Player> action)
        {
            if (!this.playersByGuild.TryGetValue(guildId, out var playerList))
            {
                return;
            }

            // TODO: iterate other guilds of the alliance as well; maybe introduce another dictionary with alliance players

            lock (playerList)
            {
                for (int i = playerList.Count - 1; i >= 0; i--)
                {
                    var player = playerList[i];
                    action(player);
                }
            }
        }

        /// <inheritdoc/>
        public override void AddPlayer(Player player)
        {
            base.AddPlayer(player);
            player.PlayerLeftWorld += this.PlayerLeftWorld;
            player.PlayerEnteredWorld += this.PlayerEnteredWorld;
        }

        /// <inheritdoc/>
        public override void RemovePlayer(Player player)
        {
            player.PlayerEnteredWorld -= this.PlayerEnteredWorld;
            player.PlayerLeftWorld -= this.PlayerLeftWorld;
            base.RemovePlayer(player);
        }

        /// <inheritdoc />
        public void RemoveGuild(uint guildId)
        {
            this.playersByGuild.Remove(guildId, out _);
            this.GuildDeleted?.Invoke(this, new GuildDeletedEventArgs(guildId));
        }

        /// <inheritdoc />
        public void RegisterGuildMember(Player guildMember)
        {
            if (guildMember.GuildStatus is null)
            {
                return;
            }

            var guildId = guildMember.GuildStatus.GuildId;
            var guildList = this.playersByGuild.GetOrAdd(guildId, id => new List<Player>());
            lock (guildList)
            {
                guildList.Add(guildMember);
            }
        }

        /// <inheritdoc />
        public void UnregisterGuildMember(Player guildMember)
        {
            if (guildMember.GuildStatus is null)
            {
                return;
            }

            var guildId = guildMember.GuildStatus.GuildId;
            if (!this.playersByGuild.TryGetValue(guildId, out var guildList))
            {
                return;
            }

            lock (guildList)
            {
                guildList.Remove(guildMember);
            }
        }

        private void PlayerEnteredWorld(object? sender, EventArgs e)
        {
            if (sender is not Player { SelectedCharacter: { } selectedCharacter } player)
            {
                return;
            }

            this.FriendServer.SetOnlineState(selectedCharacter.Id, selectedCharacter.Name, this.Id);
            player.GuildStatus = this.GuildServer.PlayerEnteredGame(selectedCharacter.Id, selectedCharacter.Name, this.Id);
            if (player.GuildStatus is null)
            {
                return;
            }

            player.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayerToGuild(player, true), true);
            this.RegisterGuildMember(player);
        }

        private void PlayerLeftWorld(object? sender, EventArgs e)
        {
            if (sender is not Player { SelectedCharacter: { } selectedCharacter } player)
            {
                return;
            }

            this.FriendServer.SetOnlineState(selectedCharacter.Id, selectedCharacter.Name, 0xFF);
            if (player.GuildStatus is not { } guildStatus)
            {
                return;
            }

            this.GuildServer.GuildMemberLeftGame(guildStatus.GuildId, selectedCharacter.Id, this.Id);
            this.UnregisterGuildMember(player);
            player.GuildStatus = null;
        }
    }
}
