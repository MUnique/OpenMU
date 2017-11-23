// <copyright file="GameServerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the game server configuration.
    /// </summary>
    public class GameServerConfiguration
    {
        /// <summary>
        /// Gets or sets the supported packet handlers, for each version.
        /// </summary>
        public virtual ICollection<MainPacketHandlerConfiguration> SupportedPacketHandlers { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum npc count.
        /// </summary>
        public short MaximumNPCs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum number of players which can connect.
        /// </summary>
        public short MaximumPlayers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maps which should be hosted on the server.
        /// </summary>
        public virtual ICollection<GameMapDefinition> Maps
        {
            get;
            protected set;
        }
    }
}
