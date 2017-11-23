// <copyright file="ClientPacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    using log4net;

    /// <summary>
    /// The handler of packets coming from the client.
    /// </summary>
    internal class ClientPacketHandler : IPacketHandler<Client>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientPacketHandler));

        private readonly IDictionary<byte, IPacketHandler<Client>> packetHandlers = new Dictionary<byte, IPacketHandler<Client>>();

        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPacketHandler"/> class.
        /// </summary>
        /// <param name="connectServer">The connect server.</param>
        public ClientPacketHandler(IConnectServer connectServer)
        {
            this.settings = connectServer.Settings;
            this.packetHandlers.Add(0x05, new FtpRequestHandler(connectServer.Settings));
            this.packetHandlers.Add(0xF4, new ServerListHandler(connectServer));
        }

        /// <inheritdoc/>
        public void HandlePacket(Client client, byte[] packet)
        {
            try
            {
                if (packet[1] > this.settings.MaxReceiveSize || packet.Length < 4)
                {
                    this.DisconnectClientUnknownPacket(client, packet);
                    return;
                }

                var packetType = packet[2];
                if (this.packetHandlers.TryGetValue(packetType, out IPacketHandler<Client> packetHandler))
                {
                    packetHandler.HandlePacket(client, packet);
                }
                else if (this.settings.DcOnUnknownPacket)
                {
                    this.DisconnectClientUnknownPacket(client, packet);
                }
            }
            catch (SocketException ex)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("SocketException occured in Client.ReceivePacket, Client Address: {0}:{1}, Packet: [{2}], Exception: {3}", client.Address, client.Port, packet.ToHexString(), ex);
                }
            }
            catch (Exception ex)
            {
                Log.WarnFormat("Exception occured in Client.ReceivePacket, Client Address: {0}:{1}, Packet: [{2}], Exception: {3}", client.Address, client.Port, packet.ToHexString(), ex);
            }
        }

        private void DisconnectClientUnknownPacket(Client client, byte[] packet)
        {
            Log.InfoFormat("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToHexString());
            client.Connection.Disconnect();
        }
    }
}
