// <copyright file="IMapFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Map
{
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for a map factory.
    /// </summary>
    public interface IMapFactory
    {
        /// <summary>
        /// Creates the map for the specified server and map identifiers.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <returns>A disposable map controller.</returns>
        ValueTask<IMapController> CreateMap(int serverId, int mapId);

        /// <summary>
        /// Gets the map identifier which is used to identify the container DOM-Element.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="mapId">The map identifier.</param>
        /// <returns>The map identifier which is used to identify the container DOM-Element.</returns>
        string GetMapContainerIdentifier(int serverId, int mapId);
    }
}