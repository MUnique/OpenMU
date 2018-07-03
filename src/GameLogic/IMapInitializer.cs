// <copyright file="IMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// An interface for a map initializer which is responsible to create new instances of <see cref="GameMap"/>s
    /// and it's initialization.
    /// </summary>
    public interface IMapInitializer
    {
        /// <summary>
        /// Creates a new game map instance of the specified game map number.
        /// </summary>
        /// <param name="mapNumber">The map number.</param>
        /// <returns>The new game map instance.</returns>
        GameMap CreateGameMap(ushort mapNumber);

        /// <summary>
        /// Initializes the state of the previously created game map (e.g. by creating NPC instances).
        /// </summary>
        /// <param name="createdMap">The created map.</param>
        void InitializeState(GameMap createdMap);
    }
}