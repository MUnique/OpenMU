// <copyright file="ExDbClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector
{
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The connected exDB server. This class includes the communication implementation between chat server and exDB server.
    /// It registers clients for the chat server and hands back their authentication details.
    /// </summary>
    public class ExDbClient
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(ExDbClient));

        private readonly string host;
        private readonly int port;
        private readonly IChatServer chatServer;
        private readonly ushort chatServerPort;

        private IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExDbClient" /> class.
        /// </summary>
        /// <param name="host">The host address of the exDB server.</param>
        /// <param name="port">The host port of the exDB server.</param>
        /// <param name="chatServer">The chat server.</param>
        /// <param name="chatServerPort">The chat server port.</param>
        public ExDbClient(string host, int port, IChatServer chatServer, int chatServerPort)
        {
            this.host = host;
            this.port = port;
            this.chatServer = chatServer;
            this.chatServerPort = (ushort)chatServerPort;
            Task.Run(async () => await this.Connect());
        }

        /// <summary>
        /// Disconnects the exDB server.
        /// </summary>
        public void Disconnect()
        {
            this.connection.Disconnect();
        }

        private async Task Connect()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (!socket.Connected)
            {
                try
                {
                    socket.Connect(this.host, this.port);
                }
                catch
                {
                    Log.Warn($"Connection to ExDB-Server ({this.host}:{this.port}) failed, trying again in 10 Seconds...");
                    await Task.Delay(10000);
                }
            }

            Log.Info("Connection to ExDB-Server established");

            this.connection = new Connection(socket, null, null);
            this.connection.PacketReceived += this.ExDbPacketReceived;
            this.connection.Disconnected += (sender, e) => Task.Run(async () => await this.Connect());
            this.connection.BeginReceive();
            this.SendHello();
        }

        private void SendHello()
        {
            // C1 3A 00 02 AC DA 43 68 61 74 53 65 72 76 65 72 00 ...
            var packet = new byte[0x3A];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[3] = 0x02;
            packet[4] = this.chatServerPort.GetLowByte();
            packet[5] = this.chatServerPort.GetHighByte();
            var chatServerString = "ChatServer";
            Encoding.UTF8.GetBytes(chatServerString, 0, chatServerString.Length, packet, 6);
            this.connection.Send(packet);
            Log.Info("Sent registration packet to ExDB-Server");
        }

        /// <summary>
        /// Is called when a packet is received from the exDB-Server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="packet">The packet.</param>
        private void ExDbPacketReceived(object sender, byte[] packet)
        {
            if (packet[0] != 0xC1)
            {
                Log.Warn($"Unknown packet received from ExDB-Server, type: {packet[0]}");
                return;
            }

            switch (packet[2])
            {
                case 0xA0:
                    this.ReadChatRoomCreation(packet);
                    break;
                case 0xA1:
                    this.ReadChatRoomInvitation(packet);
                    break;
            }
        }

        /// <summary>
        /// Reads the invitation to an existing chat room and registers the invited client.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <remarks>
        /// Example: C1 10 A1 00 00 00 61 62 63 64 65 66 67 68 69 6F
        /// Index 4 and 5 is the room id, the rest behind is the client name.
        /// The chat server answers this with the same packets as above(ticket 96862210):
        /// C1 2C A0 01 00 00 61 62 63 64 65 66 67 68 69 6F CC CC CC CC CC CC CC CC CC CC 53 54 55 56 CC CC 02 00 C6 05 CC CC CC CC 57 CC CC CC
        /// </remarks>
        private void ReadChatRoomInvitation(byte[] packet)
        {
            var roomId = NumberConversionExtensions.MakeWord(packet[5], packet[4]);
            var clientName = packet.ExtractString(6, 10, Encoding.UTF8);
            Log.Debug($"Received request to invite {clientName} to chat room {roomId}");
            this.RegisterAndSendAuth(clientName, string.Empty, roomId);
        }

        /// <summary>
        /// Reads the chat room creation message, creates a new chat room and registers the clients.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <remarks>
        /// For example, we get here the following packet in:
        /// C1 17 A0 41 42 43 44 45 46 47 48 49 4A 50 51 52 53 54 55 56 57 58 59
        /// This packet includes the header and both names of the creator and the invited chat partner (each 10 bytes long).
        /// The server should then send the following data back to the exDB-Server:
        ///           s | rid ||-----client name-----------||---------other client name-| |------???------| |-ticket--| |--------???----------|
        /// C1 2C A0 01 00 00 41 42 43 44 45 46 47 48 49 4A 50 51 52 53 54 55 56 57 58 59 00 00 00 00 CC CC 00 00 11 04 CC CC CC CC 00 CC CC CC
        /// C1 2C A0 01 00 00 50 51 52 53 54 55 56 57 58 59 41 42 43 44 45 46 47 48 49 4A 00 00 00 00 CC CC 01 00 BB 05 CC CC CC CC 01 CC CC CC
        /// </remarks>
        private void ReadChatRoomCreation(byte[] packet)
        {
            string clientName = packet.ExtractString(3, 10, Encoding.UTF8);
            string friendname = packet.ExtractString(13, 10, Encoding.UTF8);
            var roomId = this.chatServer.CreateChatRoom();
            Log.Debug($"Received request to create chat room for {clientName} and {friendname}; Room-ID: {roomId}");
            this.RegisterAndSendAuth(clientName, friendname, roomId);
            this.RegisterAndSendAuth(friendname, clientName, roomId);
        }

        private void RegisterAndSendAuth(string clientName, string friendName, ushort roomId)
        {
            var authenticationInformation = this.chatServer.RegisterClient(roomId, clientName);
            Log.Debug($"Registered client {clientName} with index {authenticationInformation.Index} and token {authenticationInformation.AuthenticationToken}");
            var token = uint.Parse(authenticationInformation.AuthenticationToken);
            var packet = new byte[]
            {
                0xC1, 0x2C, 0xA0, 0x01, (byte)roomId, (byte)(roomId >> 8),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0xCC, 0xCC,
                0, 0, 0, 0, // token
                0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC
            };

            packet.SetIntegerBigEndian(token, 32);

            Encoding.UTF8.GetBytes(clientName, 0, clientName.Length, packet, 6);
            Encoding.UTF8.GetBytes(friendName, 0, friendName.Length, packet, 16);
            this.connection.Send(packet);
        }
    }
}