// <copyright file="ServerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A wrapper class for the game server information.
    /// </summary>
    public class ServerInfo
    {
        private readonly IManageableServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public ServerInfo(IManageableServer server)
        {
            this.server = server;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public int Id => this.server.Id;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description => this.server.Description;

        /// <summary>
        /// Gets the state.
        /// </summary>
        public ServerState State => this.server.ServerState;

        /// <summary>
        /// Gets the online player count.
        /// </summary>
        public int OnlinePlayerCount => this.server.CurrentConnections;

        /// <summary>
        /// Gets the maximum players.
        /// </summary>
        public int MaximumPlayers => this.server.MaximumConnections;

        /// <summary>
        /// Gets the type of the server.
        /// </summary>
        public ServerType Type => this.server.Type;
    }
}