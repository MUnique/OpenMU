// <copyright file="ServerInfoRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System;
    using log4net;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handles the server info request of a client, which means the client wants to know the connect data of the server it just clicked on.
    /// </summary>
    internal class ServerInfoRequestHandler : IPacketHandler<Client>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerInfoRequestHandler));
        private readonly IConnectServer connectServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoRequestHandler"/> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        public ServerInfoRequestHandler(IConnectServer connectServer)
        {
            this.connectServer = connectServer;
        }

        /// <inheritdoc/>
        public void HandlePacket(Client client, Span<byte> packet)
        {
            var serverId = packet.Length > 5 ? (ushort)(packet[4] | packet[5] << 8) : packet[4];
            Log.DebugFormat("Client {0}:{1} requested Connection Info of ServerId {2}", client.Address, client.Port, serverId);
            if (client.ServerInfoRequestCount >= this.connectServer.Settings.MaxIpRequests)
            {
                Log.Debug($"Client {client.Address}:{client.Port} reached max ip requests.");
                client.Connection.Disconnect();
            }

            if (this.connectServer.ConnectInfos.TryGetValue(serverId, out byte[] connectInfo))
            {
                using (var writer = client.Connection.StartSafeWrite(connectInfo[0], connectInfo.Length))
                {
                    connectInfo.CopyTo(writer.Span);
                    writer.Commit();
                }
            }
            else
            {
                Log.Debug($"Client {client.Address}:{client.Port}: Connection Info not found, sending Server List instead.");
                var serverList = this.connectServer.ServerList.Serialize();
                using (var writer = client.Connection.StartSafeWrite(serverList[0], serverList.Length))
                {
                    serverList.CopyTo(writer.Span);
                    writer.Commit();
                }
            }

            client.SendHello();
            client.ServerInfoRequestCount++;
        }
    }
}
