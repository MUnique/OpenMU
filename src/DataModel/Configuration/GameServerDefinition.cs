// <copyright file="GameServerDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Defines the configuration of a game server.
    /// </summary>
    [AggregateRoot]
    public class GameServerDefinition
    {
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public byte ServerID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the server configuration.
        /// </summary>
        public virtual GameServerConfiguration ServerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the game configuration.
        /// </summary>
        public virtual GameConfiguration GameConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the endpoints of the game server.
        /// </summary>
        [MemberOfAggregate]
        public virtual ICollection<GameServerEndpoint> Endpoints { get; protected set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[GameServerDefinition ServerID={0}, Description={1}]", this.ServerID, this.Description);
        }
    }
}
