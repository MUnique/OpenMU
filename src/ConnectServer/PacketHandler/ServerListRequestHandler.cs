// <copyright file="ServerListRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System;
    using log4net;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handles the request of the server list.
    /// </summary>
    internal class ServerListRequestHandler : IPacketHandler<Client>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerListRequestHandler));
        private readonly IConnectServer connectServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListRequestHandler"/> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        public ServerListRequestHandler(IConnectServer connectServer)
        {
            this.connectServer = connectServer;
        }

        /// <inheritdoc/>
        public void HandlePacket(Client client, Span<byte> packet)
        {
            Log.DebugFormat("Client {0}:{1} requested Server List", client.Address, client.Port);
            if (client.ServerListRequestCount >= this.connectServer.Settings.MaxServerListRequests)
            {
                Log.DebugFormat("Client {0}:{1} reached maxListRequests", client.Address, client.Port);
                client.Connection.Disconnect();
            }

            var serverList = this.connectServer.ServerList.Serialize();
            using (var writer = client.Connection.StartSafeWrite(serverList[0], serverList.Length))
            {
                serverList.CopyTo(writer.Span);
                writer.Commit();
            }

            client.ServerListRequestCount++;
        }
    }
}
