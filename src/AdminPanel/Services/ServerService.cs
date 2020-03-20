// <copyright file="ServerService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A service to manage servers.
    /// </summary>
    public class ServerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerService"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public ServerService(IList<IManageableServer> servers)
        {
            this.Servers = servers;
        }

        /// <summary>
        /// Gets the manageable servers.
        /// </summary>
        public IList<IManageableServer> Servers { get; }

        /// <summary>
        /// Gets the game server of the specified identifier.
        /// </summary>
        /// <param name="gameServerId">The game server identifier.</param>
        /// <returns>The game server of the specified identifier.</returns>
        public IGameServer GetGameServer(int gameServerId)
        {
            return this.Servers.OfType<IGameServer>().First(s => s.Id == gameServerId);
        }
    }
}