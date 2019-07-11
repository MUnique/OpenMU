// <copyright file="ChatServerSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Settings for the <see cref="ChatServer"/>.
    /// </summary>
    public class ChatServerSettings
    {
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public int ServerId { get; set; } = SpecialServerIds.ChatServer;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Chat Server";

        /// <summary>
        /// Gets or sets the maximum connections.
        /// </summary>
        public int MaximumConnections { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the client timeout. When a client did not send any data in this timespan, it's automatically disconnected.
        /// </summary>
        /// <value>
        /// The client timeout.
        /// </value>
        public TimeSpan ClientTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets or sets the interval in which a client clean up takes place.
        /// For all connected clients it's checked whether or not the <see cref="ClientTimeout"/> has been reached.
        /// </summary>
        public TimeSpan ClientCleanUpInterval { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets or sets the interval in which empty chat rooms are cleaned up.
        /// </summary>
        public TimeSpan RoomCleanUpInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets the endpoints under which the chat server is available for specific game clients.
        /// </summary>
        public ICollection<ChatServerEndpoint> Endpoints { get; } = new List<ChatServerEndpoint>();
    }
}