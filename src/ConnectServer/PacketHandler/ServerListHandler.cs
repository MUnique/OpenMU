// <copyright file="ServerListHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System.Collections.Generic;
    using log4net;

    /// <summary>
    /// Handles the requests of server data.
    /// </summary>
    internal class ServerListHandler : IPacketHandler<Client>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerListHandler));
        private readonly Settings settings;
        private readonly IDictionary<byte, IPacketHandler<Client>> packetHandlers = new Dictionary<byte, IPacketHandler<Client>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListHandler"/> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        public ServerListHandler(IConnectServer connectServer)
        {
            this.settings = connectServer.Settings;
            this.packetHandlers.Add(0x03, new ServerInfoRequestHandler(connectServer));
            this.packetHandlers.Add(0x06, new ServerListRequestHandler(connectServer));
        }

        /// <inheritdoc/>
        public void HandlePacket(Client client, byte[] packet)
        {
            var packetSubType = packet[3];
            if (this.packetHandlers.TryGetValue(packetSubType, out IPacketHandler<Client> packetHandler))
            {
                packetHandler.HandlePacket(client, packet);
            }
            else if (this.settings.DcOnUnknownPacket)
            {
                Log.InfoFormat("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToHexString());
                client.Connection.Disconnect();
            }
        }
    }
}
