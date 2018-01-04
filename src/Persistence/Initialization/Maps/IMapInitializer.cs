// <copyright file="IMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// A common interface for a map initializer.
    /// </summary>
    internal interface IMapInitializer
    {
        /// <summary>
        /// Initializes the data for the implemented game map.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The created game map definition for the implemented game map.
        /// </returns>
        GameMapDefinition Initialize(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration);
    }
}
