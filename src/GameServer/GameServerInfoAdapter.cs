// <copyright file="GameServerInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Adapter which takes an <see cref="GameServer"/> and adapts it to a <see cref="IGameServerInfo"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerInfo" />
    internal class GameServerInfoAdapter : IGameServerInfo
    {
        private readonly GameServer gameServer;
        private readonly GameServerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerInfoAdapter"/> class.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        /// <param name="configuration">The configuration.</param>
        public GameServerInfoAdapter(GameServer gameServer, GameServerConfiguration configuration)
        {
            this.gameServer = gameServer;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public byte Id => this.gameServer.Id;

        /// <inheritdoc/>
        public string Description => this.gameServer.Description;

        /// <inheritdoc/>
        public ServerState State => this.gameServer.ServerState;

        /// <inheritdoc/>
        public int OnlinePlayerCount => this.gameServer.Context.PlayerList.Count;

        /// <inheritdoc/>
        public int MaximumPlayers => this.configuration.MaximumPlayers;

        /// <inheritdoc/>
        public IList<IGameMapInfo> Maps
        {
            get
            {
                return this.gameServer.Context.Maps.Select(map =>
                        new GameMapInfoAdapter(
                            map,
                            this.gameServer.Context.PlayerList.Where(p => p.CurrentMap == map).ToList()))
                    .ToList<IGameMapInfo>();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.gameServer.ToString();
        }
    }
}
