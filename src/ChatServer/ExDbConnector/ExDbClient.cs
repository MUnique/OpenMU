// <copyright file="ExDbClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector;

using System.Buffers;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using Pipelines.Sockets.Unofficial;
using static System.Buffers.Binary.BinaryPrimitives;

/// <summary>
/// The connected exDB server. This class includes the communication implementation between chat server and exDB server.
/// It registers clients for the chat server and hands back their authentication details.
/// </summary>
public class ExDbClient
{
    private readonly ILogger<ExDbClient> _logger;

    private readonly string _host;
    private readonly int _port;
    private readonly IChatServer _chatServer;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ushort _chatServerPort;
    private readonly byte[] _packetBuffer = new byte[0xFF];

    private IConnection? _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExDbClient" /> class.
    /// </summary>
    /// <param name="host">The host address of the exDB server.</param>
    /// <param name="port">The host port of the exDB server.</param>
    /// <param name="chatServer">The chat server.</param>
    /// <param name="chatServerPort">The chat server port.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ExDbClient(string host, int port, IChatServer chatServer, int chatServerPort, ILoggerFactory loggerFactory)
    {
        this._host = host;
        this._port = port;
        this._chatServer = chatServer;
        this._loggerFactory = loggerFactory;
        this._chatServerPort = (ushort)chatServerPort;
        this._logger = this._loggerFactory.CreateLogger<ExDbClient>();
        Task.Run(async () => await this.Connect().ConfigureAwait(false));
    }

    /// <summary>
    /// Disconnects the exDB server.
    /// </summary>
    public void Disconnect()
    {
        this._connection?.Disconnect();
    }

    private async Task Connect()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        while (!socket.Connected)
        {
            try
            {
                socket.Connect(this._host, this._port);
            }
            catch
            {
                this._logger.LogWarning($"Connection to ExDB-Server ({this._host}:{this._port}) failed, trying again in 10 Seconds...");
                await Task.Delay(10000).ConfigureAwait(false);
            }
        }

        this._logger.LogInformation("Connection to ExDB-Server established");

        this._connection = new Connection(SocketConnection.Create(socket), null, null, this._loggerFactory.CreateLogger<Connection>());
        this._connection.PacketReceived += this.ExDbPacketReceived;
        this._connection.Disconnected += (_, _) => Task.Run(async () => await this.Connect().ConfigureAwait(false));
        this.SendHello();
        await this._connection!.BeginReceive();
    }

    private void SendHello()
    {
        // C1 3A 00 02 AC DA 43 68 61 74 53 65 72 76 65 72 00 ...
        using (var writer = this._connection!.StartSafeWrite(0xC1, 0x3A))
        {
            var packet = writer.Span;
            packet[3] = 0x02;
            packet[4] = this._chatServerPort.GetLowByte();
            packet[5] = this._chatServerPort.GetHighByte();
            packet.Slice(6).WriteString("ChatServer", Encoding.UTF8);
            writer.Commit();
        }

        this._logger.LogInformation("Sent registration packet to ExDB-Server");
    }

    /// <summary>
    /// Is called when a packet is received from the exDB-Server.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="sequence">The packet.</param>
    private void ExDbPacketReceived(object sender, ReadOnlySequence<byte> sequence)
    {
        try
        {
            sequence.CopyTo(this._packetBuffer);
            var packet = this._packetBuffer.AsSpan(0, (int)sequence.Length);
            if (packet[0] != 0xC1)
            {
                this._logger.LogWarning($"Unknown packet received from ExDB-Server, type: {packet[0]}");
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
                default:
                    this._logger.LogWarning($"Unknown packet received from ExDB-Server, code: {packet[2]}");
                    break;
            }
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, $"An error occured while processing an incoming packet from ExDB: {this._packetBuffer.AsString()}");
        }
    }

    /// <summary>
    /// Reads the invitation to an existing chat room and registers the invited client.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <remarks>
    /// Example: C1 15 A1 00 00 00 61 62 63 64 65 66 67 68 69 6F 20 01 00 01 57
    /// Index 4 and 5 is the room id, the next 10 bytes is the client name, after that the player id, game server id and a "type".
    /// The chat server answers this with the same packets as above(ticket 96862210):
    /// C1 2C A0 01 00 00 61 62 63 64 65 66 67 68 69 6F CC CC CC CC CC CC CC CC CC CC 53 54 55 56 CC CC 02 00 C6 05 CC CC CC CC 57 CC CC CC.
    /// </remarks>
    private void ReadChatRoomInvitation(Span<byte> packet)
    {
        var roomId = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
        var clientName = packet.ExtractString(6, 10, Encoding.UTF8);
        var clientPlayerId = packet.TryMakeWordBigEndian(16);
        var clientServerId = packet.TryMakeWordBigEndian(18);
        var type = packet.Length > 20 ? packet[20] : (byte)0x57;
        this._logger.LogDebug($"Received request to invite {clientName} to chat room {roomId}, Client-ID: {clientPlayerId}, Server-ID: {clientServerId}");
        var authentication = this._chatServer.RegisterClient(roomId, clientName);
        this.SendAuthentication(authentication, null, clientPlayerId, clientServerId, type);
    }

    /// <summary>
    /// Reads the chat room creation message, creates a new chat room and registers the clients.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <remarks>
    /// For example, we get here the following packet in:
    /// C1 20 A0 41 42 43 44 45 46 47 48 49 4A 50 51 52 53 54 55 56 57 58 59 00 E0 2E 01 00 E1 2E 01 00
    /// This packet includes the header and both names of the creator and the invited chat partner (each 10 bytes long).
    /// The server should then send the following data back to the exDB-Server:
    ///           s | rid ||-----client name-----------||---------other client name-||plid| |svid||---| |-ticket--| |--------???----------|
    /// C1 2C A0 01 00 00 41 42 43 44 45 46 47 48 49 4A 50 51 52 53 54 55 56 57 58 59 00 00 00 00 CC CC 00 00 11 04 CC CC CC CC 00 CC CC CC
    /// C1 2C A0 01 00 00 50 51 52 53 54 55 56 57 58 59 41 42 43 44 45 46 47 48 49 4A 00 00 00 00 CC CC 01 00 BB 05 CC CC CC CC 01 CC CC CC.
    /// </remarks>
    private void ReadChatRoomCreation(Span<byte> packet)
    {
        string clientName = packet.ExtractString(3, 10, Encoding.UTF8);
        string friendName = packet.ExtractString(13, 10, Encoding.UTF8);
        var clientPlayerId = packet.TryMakeWordBigEndian(24);
        var clientServerId = packet.TryMakeWordBigEndian(26);
        var friendPlayerId = packet.TryMakeWordBigEndian(28);
        var friendServerId = packet.TryMakeWordBigEndian(30);
        var roomId = this._chatServer.CreateChatRoom();
        this._logger.LogDebug($"Received request to create chat room for {clientName} and {friendName}; Room-ID: {roomId}; Client-ID: {clientPlayerId}; Server-ID: {clientServerId}; Friend-ID: {friendPlayerId}; Friend-Server: {friendServerId}");
        var requesterAuthentication = this._chatServer.RegisterClient(roomId, clientName);
        var friendAuthentication = this._chatServer.RegisterClient(roomId, friendName);
        this.SendAuthentication(requesterAuthentication, friendAuthentication, clientPlayerId, clientServerId, requesterAuthentication.Index);
        this.SendAuthentication(friendAuthentication, requesterAuthentication, friendPlayerId, friendServerId, friendAuthentication.Index);
    }

    /// <summary>
    /// Sends the authentication information back to the ExDB-Server.
    /// </summary>
    /// <param name="authenticationInfo">The authentication information.</param>
    /// <param name="friendAuthenticationInfo">The friend authentication information.</param>
    /// <param name="clientId">The client identifier on the server where the client plays on.</param>
    /// <param name="serverId">The server identifier where the client plays on.</param>
    /// <param name="type">The type. Usually 0 for the player who requested the chat and 1 for the other player.</param>
    private void SendAuthentication(ChatServerAuthenticationInfo authenticationInfo, ChatServerAuthenticationInfo? friendAuthenticationInfo, ushort clientId, ushort serverId, byte type)
    {
        this._logger.LogDebug($"Registered client {authenticationInfo.ClientName} with index {authenticationInfo.Index} and token {authenticationInfo.AuthenticationToken}");
        var token = uint.Parse(authenticationInfo.AuthenticationToken);
        uint friendToken = 0;
        if (friendAuthenticationInfo != null)
        {
            friendToken = uint.Parse(friendAuthenticationInfo.AuthenticationToken);
        }

        var roomId = authenticationInfo.RoomId;
        using var writer = this._connection!.StartSafeWrite(0xC1, 0x2C);
        var packet = writer.Span;
        packet[2] = 0xA0;
        packet[3] = 0x01;
        WriteUInt16LittleEndian(packet.Slice(4), roomId);
        packet.Slice(6).WriteString(authenticationInfo.ClientName, Encoding.UTF8);
        if (friendAuthenticationInfo != null)
        {
            packet.Slice(16).WriteString(friendAuthenticationInfo.ClientName, Encoding.UTF8);
        }

        WriteUInt16LittleEndian(packet.Slice(26), clientId);
        WriteUInt16LittleEndian(packet.Slice(28), serverId);
        WriteUInt32LittleEndian(packet.Slice(32), token);
        WriteUInt32LittleEndian(packet.Slice(36), friendToken);
        packet[40] = type;

        writer.Commit();
    }
}