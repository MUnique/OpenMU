﻿// <copyright file="ChatClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using System.Buffers;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Xor;

/// <summary>
/// ChatClient implementation, uses socket connections.
/// </summary>
/// <remarks>
/// Messages are decrypted and encrypted again with the same XOR3 key - in theory we could optimize this (and
/// the conversion to a string) away. However, we'll leave it for easier debugging.
/// </remarks>
internal class ChatClient : IChatClient
{
    private const int TokenOffset = 6;
    private const int MessageOffset = 5;
    private static readonly ISpanDecryptor TokenDecryptor = new Xor3Decryptor(TokenOffset);

    /// <summary>
    /// <see cref="ISpanDecryptor"/> for chat messages. The incoming chat messages are "encrypted" with the commonly known XOR-3 encryption for reasons we don't know ;).
    /// </summary>
    private static readonly ISpanDecryptor MessageDecryptor = new Xor3Decryptor(MessageOffset);

    /// <summary>
    /// <see cref="ISpanEncryptor"/> for chat messages. The outgoing chat messages are "encrypted" with the commonly known XOR-3 encryption for reasons we don't know ;).
    /// </summary>
    private static readonly ISpanEncryptor MessageEncryptor = new Xor3Encryptor(MessageOffset);

    private readonly ChatRoomManager _manager;
    private readonly ILogger<ChatClient> _logger;
    private readonly byte[] _packetBuffer = new byte[0xFF];
    private IConnection? _connection;
    private ChatRoom? _room;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="logger">The logger.</param>
    public ChatClient(IConnection connection, ChatRoomManager manager, ILogger<ChatClient> logger)
    {
        this._manager = manager;
        this._logger = logger;
        this._connection = connection;
        this._connection.PacketReceived += this.ReadPacket;
        this._connection.Disconnected += (_, _) => this.LogOff();

        this.LastActivity = DateTime.Now;
        this._connection.BeginReceive();
    }

    /// <summary>
    /// Occurs when the client has been disconnected.
    /// </summary>
    public event EventHandler? Disconnected;

    /// <inheritdoc/>
    public byte Index
    {
        get;
        set;
    }

    /// <inheritdoc />
    public string? AuthenticationToken { get; private set; }

    /// <inheritdoc/>
    public string? Nickname { get; set; }

    /// <inheritdoc/>
    public DateTime LastActivity { get; private set; }

    /// <inheritdoc/>
    public void SendMessage(byte senderId, string message)
    {
        if (this._connection is null)
        {
            return;
        }

        var messageByteLength = Encoding.UTF8.GetByteCount(message);
        using var writer = this._connection.StartSafeWrite(0xC1, 5 + messageByteLength);
        var packet = writer.Span;
        packet[2] = 0x04;
        packet[3] = senderId;
        packet[4] = (byte)messageByteLength;
        packet.Slice(5).WriteString(message, Encoding.UTF8);
        MessageEncryptor.Encrypt(packet);
        writer.Commit();
    }

    /// <inheritdoc/>
    public void SendChatRoomClientList(IReadOnlyCollection<IChatClient> clients)
    {
        if (this._connection is null)
        {
            return;
        }

        const int sizePerClient = 11;
        using var writer = this._connection.StartSafeWrite(0xC2, 8 + (sizePerClient * clients.Count));
        var packet = writer.Span;
        packet[3] = 2; // packet type
        packet[6] = (byte)clients.Count;
        int i = 0;
        foreach (var client in clients)
        {
            var clientBlock = packet.Slice(8 + (i * sizePerClient), sizePerClient);
            clientBlock[0] = client.Index;
            clientBlock.Slice(1).WriteString(client.Nickname ?? string.Empty, Encoding.UTF8);
            i++;
        }

        writer.Commit();
    }

    /// <inheritdoc/>
    public void SendChatRoomClientUpdate(byte updatedClientId, string updatedClientName, ChatRoomClientUpdateType updateType)
    {
        if (this._connection is null)
        {
            return;
        }

        using var writer = this._connection.StartSafeWrite(0xC1, 0x0F);
        var packet = writer.Span;
        packet[2] = 0x01;
        packet[3] = (byte)updateType;
        packet[4] = updatedClientId;
        packet.Slice(5).WriteString(updatedClientName, Encoding.UTF8);

        writer.Commit();
    }

    /// <inheritdoc/>
    public void LogOff()
    {
        if (this._connection is null)
        {
            this._logger.LogDebug($"Client {this.Nickname} is already disconnected.");
            return;
        }

        this._logger.LogDebug($"Client {this._connection} is going to be disconnected.");
        if (this._room != null)
        {
            this._room.Leave(this);
            this._room = null;
        }

        this._connection?.Disconnect();
        this._connection = null;
        this.Disconnected?.Invoke(this, EventArgs.Empty);
        this.Disconnected = null;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"Connection:{this._connection}, Client name:{this.Nickname}, Room-ID:{this._room?.RoomId}, Index: {this.Index}";
    }

    private void ReadPacket(object sender, ReadOnlySequence<byte> sequence)
    {
        if (sequence.Length < 3)
        {
            return;
        }

        sequence.CopyTo(this._packetBuffer);
        var packet = this._packetBuffer.AsSpan(0, (int)sequence.Length);
        if (packet[0] != 0xC1)
        {
            return;
        }

        this.LastActivity = DateTime.Now;
        switch (packet[2])
        {
            case 0:
                this.Authenticate(packet);
                break;
            case 1:
            case 2:
            case 3:
                // We did never capture such packets, but they don't seem to be wrong (next is 4), so do nothing.
                break;
            case 4:
                if (this._room != null && this.CheckMessage(packet))
                {
                    MessageDecryptor.Decrypt(packet);
                    var message = packet.ExtractString(5, int.MaxValue, Encoding.UTF8);
                    if (this._logger.IsEnabled(LogLevel.Debug))
                    {
                        this._logger.LogDebug($"Message received from {this.Index}: \"{message}\"");
                    }

                    this._room.SendMessage(this.Index, message);
                }

                break;

            case 5:
                // This is something like a keep-connection-alive packet.
                // Last activity is always set, so we have to do nothing here.
                this._logger.LogDebug("Keep-alive received");
                break;

            default:
                this._logger.LogError($"Received unknown packet of type {packet[2]}: {packet.AsString()}");
                this.LogOff();
                break;
        }
    }

    private bool CheckMessage(Span<byte> packet)
    {
        return packet.Length > 4 && (packet[4] + 5) == packet.Length;
    }

    private void Authenticate(Span<byte> packet)
    {
        var roomId = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
        var requestedRoom = this._manager.GetChatRoom(roomId);
        if (requestedRoom is null)
        {
            this._logger.LogError($"Requested room {roomId} has not been registered before.");
            this.LogOff();
            return;
        }

        TokenDecryptor.Decrypt(packet);
        var tokenAsString = packet.ExtractString(TokenOffset, 10, Encoding.UTF8);
        if (!uint.TryParse(tokenAsString, out uint _))
        {
            this._logger.LogError($"Token '{tokenAsString}' is not a parseable integer.");
            this.LogOff();
            return;
        }

        this.AuthenticationToken = tokenAsString;
        if (requestedRoom.TryJoin(this))
        {
            this._room = requestedRoom;
        }
        else
        {
            this.LogOff();
        }
    }
}