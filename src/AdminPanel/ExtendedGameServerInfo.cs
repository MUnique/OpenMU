// <copyright file="ExtendedGameServerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A wrapper class for the game server information.
    /// </summary>
    internal class ExtendedGameServerInfo
    {
        private readonly IGameServer gameServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedGameServerInfo"/> class.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        public ExtendedGameServerInfo(IGameServer gameServer)
        {
            this.gameServer = gameServer;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public int Id => this.gameServer.Id;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description => this.gameServer.Description;

        /// <summary>
        /// Gets the currently available maps which are running on the server instance.
        /// </summary>
        public IList<GameMapInfo> Maps
        {
            get
            {
                return this.gameServer.ServerInfo.Maps.Select(map => new GameMapInfo { Id = map.MapNumber, Name = map.MapName, PlayerCount = map.Players.Count, ServerId = this.gameServer.Id }).ToList();
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public ServerState State => this.gameServer.ServerState;

        /// <summary>
        /// Gets the online player count.
        /// </summary>
        public int OnlinePlayerCount => this.gameServer.ServerInfo.OnlinePlayerCount;

        /// <summary>
        /// Gets the maximum players.
        /// </summary>
        public int MaximumPlayers => this.gameServer.ServerInfo.MaximumPlayers;

        /// <summary>
        /// Information about a game map instance.
        /// </summary>
        public class GameMapInfo
        {
            /// <summary>
            /// Gets or sets the identifier of the map. Its the <see cref="GameMapDefinition.Number"/>.
            /// </summary>
            public short Id { get; set; }

            /// <summary>
            /// Gets or sets the server identifier on which the map instance is running.
            /// </summary>
            public int ServerId { get; set; }

            /// <summary>
            /// Gets or sets the name of the map.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the number of players which are currently on this map instance.
            /// </summary>
            public int PlayerCount { get; set; }
        }
    }
}
