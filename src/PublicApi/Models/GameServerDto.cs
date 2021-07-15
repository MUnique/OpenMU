// <copyright file="GameServerDto.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi.Models
{
    using System.Net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The data transfer object for a game server.
    /// </summary>
    public class GameServerDto
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host { get; private set; } = "127.127.127.127";

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; } = 55901;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Description { get; private set; } = "GameServer";

        /// <summary>
        /// Gets the player count.
        /// </summary>
        public int PlayerCount { get; private set; }

        /// <summary>
        /// Gets the maximum player count.
        /// </summary>
        public int MaximumPlayerCount { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public ServerState State { get; private set; }

        /// <summary>
        /// Creates the DTO for specified end point and corresponding <see cref="IGameServer"/>.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="gameServer">The game server.</param>
        /// <returns>The created DTO.</returns>
        public static GameServerDto Create(IPEndPoint endPoint, IGameServer gameServer)
        {
            return new ()
            {
                Host = endPoint.Address.ToString(),
                Port = endPoint.Port,
                Description = gameServer.Description,
                MaximumPlayerCount = gameServer.MaximumConnections,
                PlayerCount = gameServer.CurrentConnections,
                State = gameServer.ServerState,
            };
        }
    }
}
