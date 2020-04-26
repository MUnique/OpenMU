// <copyright file="GameServerMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// A game map initializer which takes aspects of the <see cref="GameServer"/> into account.
    /// </summary>
    internal class GameServerMapInitializer : MapInitializer
    {
        private readonly GameServerDefinition serverDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerMapInitializer"/> class.
        /// </summary>
        /// <param name="serverDefinition">The server definition.</param>
        public GameServerMapInitializer(GameServerDefinition serverDefinition)
            : base(serverDefinition.GameConfiguration)
        {
            this.serverDefinition = serverDefinition;
        }

        /// <summary>
        /// Gets the map definition by searching for it at the <see cref="GameServerConfiguration"/>.
        /// </summary>
        /// <param name="mapNumber">The map number.</param>
        /// <returns>The game map definition.</returns>
        protected override GameMapDefinition GetMapDefinition(ushort mapNumber)
        {
            return this.serverDefinition.ServerConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber);
        }

        /// <inheritdoc/>
        protected override GameMap InternalCreateGameMap(GameMapDefinition definition)
        {
            var gameMap = base.InternalCreateGameMap(definition);
            return gameMap;
        }
    }
}
