// <copyright file="IServerListClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The interface for clients of the <see cref="ServerListHub"/>.
    /// </summary>
    public interface IServerListClient
    {
        /// <summary>
        /// Is called when a server got added to the <see cref="ServerListHub"/>.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <remarks>Currently probably never called, because there are no servers dynamically added or removed.</remarks>
        Task AddedServer(ServerInfo server);

        /// <summary>
        /// Is called when a server got removed from the <see cref="ServerListHub"/>.
        /// </summary>
        /// <param name="serverId">The indentifier of the removed server.</param>
        /// <remarks>Currently probably never called, because there are no servers dynamically added or removed.</remarks>
        Task RemovedServer(int serverId);

        /// <summary>
        /// Is called when the player count of the specified server changed.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="playerCount">The player count.</param>
        Task PlayerCountChanged(int serverId, int playerCount);

        /// <summary>
        /// Is called when the state of the specified server changed.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="newState">The new state.</param>
        Task ServerStateChanged(int serverId, ServerState newState);

        /// <summary>
        /// Is called when the player count of the specified map on the specified server changed.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <param name="playerCount">The player count.</param>
        Task MapPlayerCountChanged(int serverId, int mapId, int playerCount);

        /// <summary>
        /// Is called when a map has been removed from the specified server.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        Task MapRemoved(int serverId, int mapId);

        /// <summary>
        /// Is called when a map has been added. The server id is specified in the game map info.
        /// </summary>
        /// <param name="gameMapInfo">The game map information.</param>
        Task MapAdded(GameServerInfo.GameMapInfo gameMapInfo);

        /// <summary>
        /// Initializes the client with the specified servers.
        /// </summary>
        /// <param name="servers">The servers.</param>
        Task Initialize(IList<ServerInfo> servers);
    }
}