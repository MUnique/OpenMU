// <copyright file="GameServerMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A game map initializer which takes aspects of the <see cref="GameServer"/> into account.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.MapInitializer" />
    internal class GameServerMapInitializer : MapInitializer
    {
        private readonly GameServerDefinition serverDefinition;
        private readonly byte serverId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerMapInitializer"/> class.
        /// </summary>
        /// <param name="serverDefinition">The server definition.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="serverStateObserver">The server state observer.</param>
        public GameServerMapInitializer(GameServerDefinition serverDefinition, byte serverId)
            : base(serverDefinition.GameConfiguration)
        {
            this.serverDefinition = serverDefinition;
            this.serverId = serverId;
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
