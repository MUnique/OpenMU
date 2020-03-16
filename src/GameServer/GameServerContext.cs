// <copyright file="GameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The context of a game server which contains all important configurations and services used by one game server instance.
    /// </summary>
    public class GameServerContext : GameContext, IGameServerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerContext" /> class.
        /// </summary>
        /// <param name="gameServerDefinition">The game server definition.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="mapInitializer">The map initializer.</param>
        public GameServerContext(
            GameServerDefinition gameServerDefinition,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IFriendServer friendServer,
            IPersistenceContextProvider persistenceContextProvider,
            IMapInitializer mapInitializer)
            : base(gameServerDefinition.GameConfiguration, persistenceContextProvider, mapInitializer)
        {
            this.Id = gameServerDefinition.ServerID;
            this.GuildServer = guildServer;
            this.LoginServer = loginServer;
            this.FriendServer = friendServer;
            this.ServerConfiguration = gameServerDefinition.ServerConfiguration;
        }

        /// <summary>
        /// Occurs when a guild has been deleted.
        /// </summary>
        public event EventHandler<GuildDeletedEventArgs> GuildDeleted;

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
            if (player == null)
            {
                return;
            }

            player.PlayerEnteredWorld -= this.PlayerEnteredWorld;
            player.PlayerLeftWorld -= this.PlayerLeftWorld;
            base.RemovePlayer(player);
        }

        /// <summary>
        /// Raises the <see cref="GuildDeleted"/> event.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        internal void RaiseGuildDeleted(uint guildId)
        {
            this.GuildDeleted?.Invoke(this, new GuildDeletedEventArgs(guildId));
        }

        private void PlayerEnteredWorld(object sender, EventArgs e)
        {
            if (sender is Player player)
            {
                this.FriendServer.SetOnlineState(player.SelectedCharacter.Id, player.SelectedCharacter.Name, this.Id);
                player.GuildStatus = this.GuildServer.PlayerEnteredGame(player.SelectedCharacter.Id, player.SelectedCharacter.Name, this.Id);
                if (player.GuildStatus != null)
                {
                    player.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IAssignPlayersToGuildPlugIn>()?.AssignPlayerToGuild(player, true), true);
                }
            }
        }

        private void PlayerLeftWorld(object sender, EventArgs e)
        {
            if (sender is Player player)
            {
                this.FriendServer.SetOnlineState(player.SelectedCharacter.Id, player.SelectedCharacter.Name, 0xFF);
                if (player.GuildStatus != null)
                {
                    this.GuildServer.GuildMemberLeftGame(player.GuildStatus.GuildId, player.SelectedCharacter.Id, this.Id);
                    player.GuildStatus = null;
                }
            }
        }
    }
}
