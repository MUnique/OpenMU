// <copyright file="IServerStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// An interface of an observer which observes the state of a server.
    /// </summary>
    public interface IServerStateObserver
    {
        /// <summary>
        /// Gets the map state observer for the specified server identifier.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>The map state observer for the specified server identifier.</returns>
        IMapStateObserver GetMapStateObserver(int serverId);

        /// <summary>
        /// Notifies the client after the state of the specified server has been changed.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="newState">The new state.</param>
        void ServerStateChanged(int serverId, ServerState newState);

        /// <summary>
        /// Notifies the client after the player count of the specified server has been changed.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="playerCount">The player count.</param>
        void PlayerCountChanged(int serverId, int playerCount);

        /// <summary>
        /// Notifies the client after a map instance has been added to the server.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapInfo">The map information.</param>
        void MapAdded(int serverId, IGameMapInfo mapInfo);

        /// <summary>
        /// Notifies the client after a map instance has been removed from the server.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <remarks>
        /// Currently, no maps ever get removed. In the future it might be needed, because maps could be dynamically added and removed,
        /// e.g. for events or because maps could be created/removed by demand.
        /// </remarks>
        void MapRemoved(int serverId, int mapId);
    }
}
