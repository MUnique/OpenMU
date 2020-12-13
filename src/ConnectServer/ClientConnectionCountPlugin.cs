// <copyright file="ClientConnectionCountPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The client connection count plugin.
    /// </summary>
    internal class ClientConnectionCountPlugin : IAfterSocketAcceptPlugin, IAfterDisconnectPlugin
    {
        private readonly ILogger<ClientConnectionCountPlugin> logger;
        private readonly ClientConnectionCounter clientCounter;
        private readonly IConnectServerSettings connectServerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectionCountPlugin" /> class.
        /// </summary>
        /// <param name="connectServerSettings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public ClientConnectionCountPlugin(IConnectServerSettings connectServerSettings, ILogger<ClientConnectionCountPlugin> logger)
        {
            this.connectServerSettings = connectServerSettings;
            this.logger = logger;
            this.clientCounter = new ClientConnectionCounter();
        }

        /// <inheritdoc/>
        public bool OnAfterSocketAccept(Socket socket)
        {
            var ipAddress = (socket.RemoteEndPoint as IPEndPoint)?.Address;
            if (ipAddress is null)
            {
                // should never happen - but who knows. In this case, we allow the connection.
                this.logger.LogDebug($"Non-IPEndPoint connected: {socket.RemoteEndPoint}.");
                return true;
            }

            if (this.connectServerSettings.CheckMaxConnectionsPerAddress
                && this.clientCounter.GetConnectionCount(ipAddress) >= this.connectServerSettings.MaxConnectionsPerAddress)
            {
                this.logger.LogWarning("Maximum Connections per IP reached: {0}, Connection refused.", ipAddress);
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
