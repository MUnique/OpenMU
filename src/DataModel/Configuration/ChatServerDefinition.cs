// <copyright file="ChatServerDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System;
    using System.Collections.Generic;

    public class ChatServerDefinition
    {
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public byte ServerId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        public int MaximumConnections { get; set; } = int.MaxValue;
        public TimeSpan ClientTimeout { get; } = TimeSpan.FromMinutes(1);
        public TimeSpan ClientCleanUpInterval { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan RoomCleanUpInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets or sets the endpoints of the game server.
        /// </summary>
        public virtual ICollection<ChatServerEndpoint> Endpoints { get; protected set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[ChatServerDefinition ServerID={this.ServerId}, Description={this.Description}]";
        }
    }
}
