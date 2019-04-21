// <copyright file="ClientConnectionCountPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net;
    using System.Net.Sockets;
    using log4net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The client connection count plugin.
    /// </summary>
    internal class ClientConnectionCountPlugin : IAfterSocketAcceptPlugin, IAfterDisconnectPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientConnectionCountPlugin));
        private readonly ClientConnectionCounter clientCounter;
        private readonly IConnectServerSettings connectServerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionCountPlugin"/> class.
        /// </summary>
        /// <param name="connectServerSettings">The settings.</param>
        public ClientConnectionCountPlugin(IConnectServerSettings connectServerSettings)
        {
            this.connectServerSettings = connectServerSettings;
            this.clientCounter = new ClientConnectionCounter();
        }

        /// <inheritdoc/>
        public bool OnAfterSocketAccept(Socket socket)
        {
            var ipAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            if (this.connectServerSettings.CheckMaxConnectionsPerAddress
                && this.clientCounter.GetConnectionCount(ipAddress) >= this.connectServerSettings.MaxConnectionsPerAddress)
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
