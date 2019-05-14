// <copyright file="ChatServerEndpoint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// A client-version-specific endpoint for a chat server.
    /// </summary>
    public class ChatServerEndpoint
    {
        /// <summary>
        /// Gets or sets the tcp network port under which the server is listening for new clients.
        /// </summary>
        public int NetworkPort { get; set; }

        /// <summary>
        /// Gets or sets the client version for which the endpoint is meant for.
        /// </summary>
        public ClientVersion ClientVersion { get; set; }
    }
}