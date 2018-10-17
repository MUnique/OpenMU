// <copyright file="ChatClientTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests
{
    using System.Buffers;
    using System.Linq;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="ChatClient"/>.
    /// </summary>
    [TestFixture]
    public class ChatClientTests
    {
        /// <summary>
        /// Tests if the client joined the room when the authentication was successful.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task AuthenticationSuccess()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe.ReceivePipe.Writer.FlushAsync();

            Assert.That(room.ConnectedClients, Contains.Item(client));
        }

        /// <summary>
        /// Tests if the authentication fails by providing the wrong token.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task AuthenticationFailedByWrongToken()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450674"));
            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe.ReceivePipe.Writer.FlushAsync();

            Assert.That(room.ConnectedClients.Contains(client), Is.False);
            Assert.That(connection.Connected, Is.False);
        }

        /// <summary>
        /// Tests what happens if two clients try to authenticate with the same token. The first connection should be connected and authenticated, the second disconnected.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task AuthenticationFailedForSecondConnection()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            var duplexPipe1 = new DuplexPipe();
            var connection1 = new Connection(duplexPipe1, null, null);
            var client1 = new ChatClient(connection1, manager);
            await duplexPipe1.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe1.ReceivePipe.Writer.FlushAsync();

            var duplexPipe2 = new DuplexPipe();
            var connection2 = new Connection(duplexPipe2, null, null);
            var client2 = new ChatClient(connection2, manager);
            bool disconnectedRaised = false;
            client2.Disconnected += (sender, e) => disconnectedRaised = true;

            await duplexPipe2.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe2.ReceivePipe.Writer.FlushAsync();

            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(client1));
            Assert.That(connection2.Connected, Is.False);
            Assert.That(disconnectedRaised, Is.True);
        }

        /// <summary>
        /// Tests if a successful authentication sets the <see cref="IChatClient.Nickname"/>.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task SetNickname()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, manager);

            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe.ReceivePipe.Writer.FlushAsync();

            Assert.That(client.Nickname, Is.EqualTo("Bob"));
        }

        /// <summary>
        /// Tests if a successful authentication sets the <see cref="IChatClient.Index"/>.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task SetClientIndex()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, manager);

            var authInfo = new ChatServerAuthenticationInfo(3, roomId, "Bob", "128450673");
            room.RegisterClient(authInfo);

            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe.ReceivePipe.Writer.WriteAsync(authenticationPacket);
            await duplexPipe.ReceivePipe.Writer.FlushAsync();

            Assert.That(client.Index, Is.EqualTo(authInfo.Index));
        }

        /// <summary>
        /// Tests if a message sent to the client gets "encrypted" properly by the XOR-3 key.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task SentMessageEncryptedProperly()
        {
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, null);
            var expectedPacket = new byte[] { 0xC1, 0x0B, 0x04, 0x01, 0x06, 0xBD, 0x8E, 0xEA, 0xBD, 0x8E, 0xEA };
            client.SendMessage(1, "AAAAAA");
            var sendResult = await duplexPipe.SendPipe.Reader.ReadAsync();
            var sentPacket = sendResult.Buffer.ToArray();
            Assert.That(expectedPacket, Is.EqualTo(sentPacket));
        }

        /// <summary>
        /// Tests if the room client list is sent property to the client. It should contain the joined client.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task RoomClientListSentAfterJoin()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            duplexPipe.ReceivePipe.Writer.Write(authenticationPacket);
            await duplexPipe.ReceivePipe.Writer.FlushAsync();

            var expectedPacket = new byte[] { 0xC2, 0x00, 0x13, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0 };
            var readResult = await duplexPipe.SendPipe.Reader.ReadAsync();
            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(expectedPacket));
            Assert.That(client.Nickname, Is.EqualTo("Bob"));
        }

        /// <summary>
        /// Tests if the room client list is sent to the client which joins as second. It should contain both clients.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task RoomClientListSentForSecondClient()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe1 = new DuplexPipe();
            var connection1 = new Connection(duplexPipe1, null, null);
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authenticationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            duplexPipe1.ReceivePipe.Writer.Write(authenticationPacket1);
            await duplexPipe1.ReceivePipe.Writer.FlushAsync();

            var duplexPipe2 = new DuplexPipe();
            var connection2 = new Connection(duplexPipe2, null, null);
            var client2 = new ChatClient(connection2, manager);
            var authenticationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            duplexPipe2.ReceivePipe.Writer.Write(authenticationPacket2);
            await duplexPipe2.ReceivePipe.Writer.FlushAsync();

            var expectedPacket = new byte[]
            {
                0xC2, 0x00, 0x1E, 0x02, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
                0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
            };

            var readResult = await duplexPipe2.SendPipe.Reader.ReadAsync();
            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(expectedPacket));
            Assert.That(client1.Nickname, Is.EqualTo("Bob"));
            Assert.That(client2.Nickname, Is.EqualTo("Alice"));
        }

        /// <summary>
        /// Tests if the first client gets notified correctly when the second client joins the chat room.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task ClientJoinedPacketSent()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe1 = new DuplexPipe();
            var connection1 = new Connection(duplexPipe1, null, null);
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authenticationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe1.ReceivePipe.Writer.WriteAsync(authenticationPacket1);
            await duplexPipe1.ReceivePipe.Writer.FlushAsync();

            var duplexPipe2 = new DuplexPipe();
            var connection2 = new Connection(duplexPipe2, null, null);
            var client2 = new ChatClient(connection2, manager);
            var authenticationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            await duplexPipe2.ReceivePipe.Writer.WriteAsync(authenticationPacket2);
            await duplexPipe2.ReceivePipe.Writer.FlushAsync();

            var expectedPacket = new byte[]
            {
                0xC1, 0x0F, 0x01, 0x00, 0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
            };

            var readResult = await duplexPipe1.SendPipe.Reader.ReadAsync();
            var result = readResult.Buffer.ToArray().TakeLast(expectedPacket.Length).ToArray();

            Assert.That(result, Is.EquivalentTo(expectedPacket));
            Assert.That(client1.Nickname, Is.EqualTo("Bob"));
            Assert.That(client2.Nickname, Is.EqualTo("Alice"));
        }

        /// <summary>
        /// Tests if a call to <see cref="ChatClient.LogOff" /> removes it from the chatroom,
        /// and the other remaining client gets notified about it.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task ClientLoggedOff()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var duplexPipe1 = new DuplexPipe();
            var connection1 = new Connection(duplexPipe1, null, null);
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authenticationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            await duplexPipe1.ReceivePipe.Writer.WriteAsync(authenticationPacket1);
            await duplexPipe1.ReceivePipe.Writer.FlushAsync();

            var duplexPipe2 = new DuplexPipe();
            var connection2 = new Connection(duplexPipe2, null, null);
            var client2 = new ChatClient(connection2, manager);
            var authenticationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            await duplexPipe2.ReceivePipe.Writer.WriteAsync(authenticationPacket2);
            await duplexPipe2.ReceivePipe.Writer.FlushAsync();
            client1.LogOff();

            var expectedPacket = new byte[]
            {
                0xC1, 0x0F, 0x01, 0x01, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
            };

            var readResult = await duplexPipe2.SendPipe.Reader.ReadAsync();
            var packet = readResult.Buffer.ToArray().TakeLast(expectedPacket.Length).ToArray();

            Assert.That(packet, Is.EquivalentTo(expectedPacket));
            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(client2));
        }
    }
}