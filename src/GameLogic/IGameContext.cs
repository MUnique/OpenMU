// <copyright file="IGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The context of the game.
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// Gets the repository manager. Used to retrieve data, e.g. from a database.
        /// </summary>
        IRepositoryManager RepositoryManager { get; }

        /// <summary>
        /// Gets the item power up factory.
        /// </summary>
        IItemPowerUpFactory ItemPowerUpFactory { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        GameConfiguration Configuration { get; }

        /// <summary>
        /// Gets the maps which are hosted by the game.
        /// </summary>
        IDictionary<ushort, GameMap> MapList { get; }

        /// <summary>
        /// Gets the players of the game in a list.
        /// </summary>
        IList<Player> PlayerList { get; }

        /// <summary>
        /// Gets the player object by character name.
        /// </summary>
        /// <param name="name">The character name.</param>
        /// <returns>The player object.</returns>
        Player GetPlayerByCharacterName(string name);
    }
}
