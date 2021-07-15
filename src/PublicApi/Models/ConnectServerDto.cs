// <copyright file="ConnectServerDto.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Data transfer object for the <see cref="IConnectServer"/>.
    /// </summary>
    public class ConnectServerDto
    {
        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; } = 44405;

        /// <summary>
        /// Gets the version.
        /// </summary>
        public byte[] Version { get; private set; } = new byte[5];

        /// <summary>
        /// Gets the season.
        /// </summary>
        public byte Season { get; private set; }

        /// <summary>
        /// Gets the episode.
        /// </summary>
        public byte Episode { get; private set; }

        /// <summary>
        /// Gets the patch address.
        /// </summary>
        public string PatchAddress { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the current patch version.
        /// </summary>
        public byte[] CurrentPatchVersion { get; private set; } = new byte[4];

        /// <summary>
        /// Gets the server state.
        /// </summary>
        public ServerState State { get; private set; }

        /// <summary>
        /// Gets the available game servers.
        /// </summary>
        public ICollection<GameServerDto> GameServers { get; private set; } = new List<GameServerDto>();

        /// <summary>
        /// Creates the DTO for the specified connect server.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        /// <param name="gameServers">The game servers.</param>
        /// <returns>
        /// The DTO.
        /// </returns>
        public static ConnectServerDto Create(IConnectServer connectServer, ICollection<IGameServer> gameServers)
        {
            return new ()
            {
                PatchAddress = connectServer.Settings.PatchAddress,
                CurrentPatchVersion = connectServer.Settings.CurrentPatchVersion,
                Version = connectServer.Settings.Client.Version,
                Season = connectServer.Settings.Client.Season,
                Episode = connectServer.Settings.Client.Episode,
                Port = connectServer.Settings.ClientListenerPort,
                State = connectServer.ServerState,
                GameServers = connectServer.GameServerEndPoints
                    .Select(ep => GameServerDto.Create(
                        ep.Endpoint, gameServers.First(gs => gs.Id == ep.Id))).ToList(),
            };
        }
    }
}
