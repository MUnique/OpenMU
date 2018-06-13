// <copyright file="IMapStateObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// An interface of a class which observes the map state.
    /// </summary>
    public interface IMapStateObserver
    {
        /// <summary>
        /// Notifies about player count changes on the specified map.
        /// </summary>
        /// <param name="mapId">The map identifier.</param>
        /// <param name="playerCount">The player count.</param>
        void PlayerCountChanged(int mapId, int playerCount);
    }
}
