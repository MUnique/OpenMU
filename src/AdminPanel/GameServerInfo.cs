// <copyright file="GameServerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A wrapper class for the game server information.
    /// </summary>
    public class GameServerInfo : ServerInfo
    {
        private readonly IGameServer gameServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerInfo"/> class.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        public GameServerInfo(IGameServer gameServer)
            : base(gameServer)
        {
            this.gameServer = gameServer;
        }

        /// <summary>
        /// Gets the currently available maps which are running on the server instance.
        /// </summary>
        [JsonProperty("maps")]
        public IList<GameMapInfo> Maps
        {
            get
            {
                return this.gameServer.ServerInfo.Maps.Select(map => new GameMapInfo { Id = map.MapNumber, Name = map.MapName, PlayerCount = map.Players.Count, ServerId = this.gameServer.Id }).ToList();
            }
        }

        /// <summary>
        /// Information about a game map instance.
        /// </summary>
        public class GameMapInfo
        {
            /// <summary>
            /// Gets or sets the identifier of the map. Its the <see cref="GameMapDefinition.Number"/>.
            /// </summary>
            [JsonProperty("id")]
            public short Id { get; set; }

            /// <summary>
            /// Gets or sets the server identifier on which the map instance is running.
            /// </summary>
            [JsonProperty("serverId")]
            public int ServerId { get; set; }

            /// <summary>
            /// Gets or sets the name of the map.
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the number of players which are currently on this map instance.
            /// </summary>
            [JsonProperty("playerCount")]
            public int PlayerCount { get; set; }
        }
    }
}
