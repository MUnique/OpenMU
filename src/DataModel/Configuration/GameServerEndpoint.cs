// <copyright file="GameServerEndpoint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Defines an endpoint of a game server.
    /// </summary>
    public class GameServerEndpoint : ServerEndpoint
    {
        /// <summary>
        /// Gets or sets the network proxied port for network analysis.
        /// </summary>
        public int ProxiedPort { get; set; }
    }
}