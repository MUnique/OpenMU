// <copyright file="CheckMaximumConnectionsPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net.Sockets;
    using log4net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Plugin which checks if the maximum number of connections got exceeded. Refuses the connection to new clients, if that happens.
    /// </summary>
    internal class CheckMaximumConnectionsPlugin : IAfterSocketAcceptPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMaximumConnectionsPlugin));
        private readonly ClientListener clientListener;
        private readonly IConnectServerSettings connectServerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckMaximumConnectionsPlugin"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public CheckMaximumConnectionsPlugin(ConnectServer server)
        {
            this.clientListener = server.ClientListener;
            this.connectServerSettings = server.Settings;
        }

        /// <inheritdoc/>
        public bool OnAfterSocketAccept(Socket socket)
        {
            var maxConnections = this.connectServerSettings.MaxConnections;
            if (maxConnections <= this.clientListener.Clients.Count)
            {
                Logger.WarnFormat("Connection refused from {0}: maximum connections ({1}) reached.", socket.RemoteEndPoint, maxConnections);
                return false;
            }

            return true;
        }
    }
}
