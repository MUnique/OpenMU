// <copyright file="CheckMaximumConnectionsPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net.Sockets;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Plugin which checks if the maximum number of connections got exceeded. Refuses the connection to new clients, if that happens.
    /// </summary>
    internal class CheckMaximumConnectionsPlugin : IAfterSocketAcceptPlugin
    {
        private readonly ILogger<CheckMaximumConnectionsPlugin> logger;
        private readonly ClientListener clientListener;
        private readonly IConnectServerSettings connectServerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckMaximumConnectionsPlugin" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="logger">The logger.</param>
        public CheckMaximumConnectionsPlugin(ConnectServer server, ILogger<CheckMaximumConnectionsPlugin> logger)
        {
            this.logger = logger;
            this.clientListener = server.ClientListener;
            this.connectServerSettings = server.Settings;
        }

        /// <inheritdoc/>
        public bool OnAfterSocketAccept(Socket socket)
        {
            var maxConnections = this.connectServerSettings.MaxConnections;
            if (maxConnections <= this.clientListener.Clients.Count)
            {
                this.logger.LogWarning("Connection refused from {0}: maximum connections ({1}) reached.", socket.RemoteEndPoint, maxConnections);
                return false;
            }

            return true;
        }
    }
}
