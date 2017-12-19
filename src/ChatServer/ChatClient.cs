// <copyright file="ChatClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.Network;

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
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatClient));
        private static readonly IDecryptor TokenDecryptor = new Xor3Decryptor(TokenOffset);

        /// <summary>
        /// Decryptor for chat messages. The incoming chat messages are "encrypted" with the commonly known XOR-3 encryption for reasons we don't know ;)
        /// </summary>
        private static readonly IDecryptor MessageDecryptor = new Xor3Decryptor(MessageOffset);

        /// <summary>
        /// Encryptor for chat messages. The outgoing chat messages are "encrypted" with the commonly known XOR-3 encryption for reasons we don't know ;)
        /// </summary>
        private static readonly IEncryptor MessageEncryptor = new Xor3Encryptor(MessageOffset);

        private readonly ChatRoomManager manager;
        private IConnection connection;
        private ChatRoom room;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatClient"/> class.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="manager">The chat room manager.</param>
        public ChatClient(Socket socket, ChatRoomManager manager)
            : this(new Connection(socket, null, new Decryptor()), manager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatClient"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="manager">The manager.</param>
        public ChatClient(IConnection connection, ChatRoomManager manager)
        {
            this.connection = connection;
            this.manager = manager;
            this.connection.Disconnected += (sender, e) => this.LogOff();
            this.connection.PacketReceived += (sender, packet) => this.PacketReceived(packet);
            this.connection.BeginReceive();
            this.LastActivity = DateTime.Now;
        }

        /// <summary>
        /// Occurs when the client has been disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <inheritdoc/>
        public byte Index
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string AuthenticationToken { get; private set; }

        /// <inheritdoc/>
        public string Nickname
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public DateTime LastActivity { get; private set; }

        /// <inheritdoc/>
        public void SendMessage(byte senderId, string message)
        {
            var messageByteLength = Encoding.UTF8.GetByteCount(message);
            var packet = new byte[5 + messageByteLength];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0x04;
            packet[3] = senderId;
            packet[4] = (byte)messageByteLength;
            Encoding.UTF8.GetBytes(message, 0, message.Length, packet, 5);
            MessageEncryptor.Encrypt(packet);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void SendChatRoomClientList(IReadOnlyCollection<IChatClient> clients)
        {
            var packet = new byte[8 + (11 * clients.Count)];
            packet[0] = 0xC2;
            packet[1] = ((ushort)packet.Length).GetHighByte();
            packet[2] = ((ushort)packet.Length).GetLowByte();
            packet[3] = 2; // packet type
            packet[6] = (byte)clients.Count;
            int i = 0;
            foreach (var client in clients)
            {
                var offset = 8 + (i * 11);
                packet[offset] = client.Index;
                Encoding.UTF8.GetBytes(client.Nickname, 0, client.Nickname.Length, packet, offset + 1);
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void SendChatRoomClientUpdate(byte updatedClientId, string updatedClientName, ChatRoomClientUpdateType updateType)
        {
            var packet = new byte[0x0F];
            packet[0] = 0xC1;
            packet[1] = 0x0F;
            packet[2] = 0x01;
            packet[3] = (byte)updateType;
            packet[4] = updatedClientId;
            Encoding.UTF8.GetBytes(updatedClientName, 0, updatedClientName.Length, packet, 5);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void LogOff()
        {
            if (this.connection == null)
            {
                Log.Debug($"Client {this.Nickname} is already disconnected.");
                return;
            }

            Log.Debug($"Client {this.connection} is going to be disconnected.");
            if (this.room != null)
            {
                this.room.Leave(this);
                this.room = null;
            }

            this.connection = null;
            this.connection?.Disconnect();
            if (this.Disconnected != null)
            {
                this.Disconnected(this, EventArgs.Empty);
                this.Disconnected = null;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Connection:{this.connection}, Client name:{this.Nickname}, Room-ID:{this.room?.RoomId}, Index: {this.Index}";
        }

        private void PacketReceived(byte[] packet)
        {
            if (packet.Length < 3 || packet[0] != 0xC1)
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
                    if (this.room != null && this.CheckMessage(packet))
                    {
                        MessageDecryptor.Decrypt(ref packet);
                        var message = packet.ExtractString(5, int.MaxValue, Encoding.UTF8);
                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug($"Message reveived from {this.Index}: \"{message}\"");
                        }

                        this.room.SendMessage(this.Index, message);
                    }

                    break;

                case 5:
                    // This is something like a keep-connection-alive packet.
                    // Last activity is always set, so we have to do nothing here.
                    Log.Debug("Keep-alive received");
                    break;

                default:
                    Log.Error($"Received unknown packet of type {packet[2]}: {packet.AsString()}");
                    this.LogOff();
                    break;
            }
        }

        private bool CheckMessage(byte[] packet)
        {
            return packet[1] == packet.Length && (packet[4] + 5) == packet.Length;
        }

        private void Authenticate(byte[] packet)
        {
            var roomid = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            var requestedRoom = this.manager.GetChatRoom(roomid);
            if (requestedRoom == null)
            {
                Log.Error($"Requested room {roomid} has not been registered before.");
                this.LogOff();
                return;
            }

            TokenDecryptor.Decrypt(ref packet);
            var tokenAsString = packet.ExtractString(TokenOffset, 10, Encoding.UTF8);
            if (!uint.TryParse(tokenAsString, out uint _))
            {
                Log.Error($"Token '{tokenAsString}' is not a parseable integer.");
                this.LogOff();
                return;
            }

            this.AuthenticationToken = tokenAsString;
            if (requestedRoom.TryJoin(this))
            {
                this.room = requestedRoom;
            }
            else
            {
                this.LogOff();
            }
        }
    }
}
