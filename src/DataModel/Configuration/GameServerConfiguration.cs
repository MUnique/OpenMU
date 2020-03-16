// <copyright file="GameServerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Defines the game server configuration.
    /// </summary>
    [AggregateRoot]
    public class GameServerConfiguration
    {
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
