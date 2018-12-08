// <copyright file="ClientConnectionCountPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net;
    using System.Net.Sockets;
    using log4net;

    /// <summary>
    /// The client connection count plugin.
    /// </summary>
    internal class ClientConnectionCountPlugin : IAfterSocketAcceptPlugin, IAfterDisconnectPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientConnectionCountPlugin));
        private readonly ClientConnectionCounter clientCounter;
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionCountPlugin"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ClientConnectionCountPlugin(Settings settings)
        {
            this.settings = settings;
            this.clientCounter = new ClientConnectionCounter();
        }

        /// <inheritdoc/>
        public bool OnAfterSocketAccept(Socket socket)
        {
            var ipAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            if (this.settings.CheckMaxConnectionsPerAddress
                && this.clientCounter.GetConnectionCount(ipAddress) >= this.settings.MaxConnectionsPerAddress)
            {
                Logger.WarnFormat("Maximum Connections per IP reached: {0}, Connection refused.", ipAddress);
                return false;
            }

            this.clientCounter.AddConnection(ipAddress);
            return true;
        }

        /// <inheritdoc/>
        public void OnAfterDisconnect(Client client)
        {
            this.clientCounter.RemoveConnection(client.Address);
        }
    }
}
