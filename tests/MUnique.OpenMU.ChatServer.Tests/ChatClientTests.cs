// <copyright file="ChatClientTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests
{
    using System.Linq;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Unit tests for the <see cref="ChatClient"/>.
    /// </summary>
    [TestFixture]
    public class ChatClientTests
    {
        /// <summary>
        /// Tests if the client joined the room when the authentification was successful.
        /// </summary>
        [Test]
        public void AuthentificationSuccess()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection.Raise(c => c.PacketReceived += null, connection, authentificationPacket);
            Assert.That(room.ConnectedClients, Contains.Item(client));
        }

        /// <summary>
        /// Tests if the authentification fails by providing the wrong token.
        /// </summary>
        [Test]
        public void AuthentificationFailedByWrongToken()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450674"));
            connection.Expect(c => c.Disconnect());

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection.Raise(c => c.PacketReceived += null, connection, authentificationPacket);
            Assert.That(room.ConnectedClients.Contains(client), Is.False);
            connection.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests what happens if two clients try to authenticate with the same token. The first connection should be connected and authenticated, the second disconnected.
        /// </summary>
        [Test]
        public void AuthentificationFailedForSecondConnection()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };

            var connection1 = MockRepository.GenerateMock<IConnection>();
            var client1 = new ChatClient(connection1, manager);
            connection1.Raise(c => c.PacketReceived += null, connection1, authentificationPacket);

            var connection2 = MockRepository.GenerateMock<IConnection>();
            var client2 = new ChatClient(connection2, manager);
            connection2.Expect(c => c.Disconnect()).Repeat.AtLeastOnce();
            connection2.Raise(c => c.PacketReceived += null, connection2, authentificationPacket);
            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(client1));
            connection2.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if a successful authentification sets the <see cref="IChatClient.Nickname"/>.
        /// </summary>
        [Test]
        public void SetNickname()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection.Raise(c => c.PacketReceived += null, connection, authentificationPacket);
            Assert.That(client.Nickname, Is.EqualTo("Bob"));
        }

        /// <summary>
        /// Tests if a successful authentification sets the <see cref="IChatClient.Nickname"/>.
        /// </summary>
        [Test]
        public void SetClientIndex()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, manager);
            var authInfo = new ChatServerAuthenticationInfo(3, roomId, "Bob", "128450673");
            room.RegisterClient(authInfo);

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection.Raise(c => c.PacketReceived += null, connection, authentificationPacket);
            Assert.That(client.Index, Is.EqualTo(authInfo.Index));
        }

        /// <summary>
        /// Tests if a message sent to the client gets "encrypted" properly by the XOR-3 key.
        /// </summary>
        [Test]
        public void SentMessageEncryptedProperly()
        {
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, null);
            var expectedPacket = new byte[] { 0xC1, 0x0B, 0x04, 0x01, 0x06, 0xBD, 0x8E, 0xEA, 0xBD, 0x8E, 0xEA };
            connection.Expect(c => c.Send(null)).IgnoreArguments();
            client.SendMessage(1, "AAAAAA");
            var arguments = connection.GetArgumentsForCallsMadeOn(c => c.Send(null));
            var sentPacket = arguments.Last()[0] as byte[];
            Assert.That(expectedPacket, Is.EqualTo(sentPacket));
        }

        /// <summary>
        /// Tests if the room client list is sent property to the client. It should contain the joined client.
        /// </summary>
        [Test]
        public void RoomClientListSentAfterJoin()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection = MockRepository.GenerateMock<IConnection>();
            var client = new ChatClient(connection, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));

            var authentificationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection.Raise(c => c.PacketReceived += null, connection, authentificationPacket);
            var arguments = connection.GetArgumentsForCallsMadeOn(c => c.Send(null));
            var sentPacket = arguments.FirstOrDefault(p => (p.First() as byte[])[0] == 0xC2)[0] as byte[];
            var expectedPacket = new byte[] { 0xC2, 0x00, 0x13, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0 };
            Assert.That(sentPacket, Is.Not.Null);
            Assert.That(sentPacket, Is.EquivalentTo(expectedPacket));
        }

        /// <summary>
        /// Tests if the room client list is sent to the client which joins as second. It should contain both clients.
        /// </summary>
        [Test]
        public void RoomClientListSentForSecondClient()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection1 = MockRepository.GenerateMock<IConnection>();
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authentificationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection1.Raise(c => c.PacketReceived += null, connection1, authentificationPacket1);

            var connection2 = MockRepository.GenerateMock<IConnection>();
            var client2 = new ChatClient(connection2, manager);
            var authentificationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            connection2.Raise(c => c.PacketReceived += null, connection2, authentificationPacket2);

            var arguments = connection2.GetArgumentsForCallsMadeOn(c => c.Send(null));
            var sentPacket = arguments.FirstOrDefault(p => (p.First() as byte[])[0] == 0xC2)[0] as byte[];
            var expectedPacket = new byte[]
            {
                0xC2, 0x00, 0x1E, 0x02, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
                0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
            };
            Assert.That(sentPacket, Is.Not.Null);
            Assert.That(sentPacket, Is.EquivalentTo(expectedPacket));
        }

        /// <summary>
        /// Tests if the first client gets notified correctly when the second client joins the chat room.
        /// </summary>
        [Test]
        public void ClientJoinedPacketSent()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection1 = MockRepository.GenerateMock<IConnection>();
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authentificationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection1.Raise(c => c.PacketReceived += null, connection1, authentificationPacket1);

            var connection2 = MockRepository.GenerateMock<IConnection>();
            var client2 = new ChatClient(connection2, manager);
            var authentificationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            connection2.Raise(c => c.PacketReceived += null, connection2, authentificationPacket2);
            var arguments = connection1.GetArgumentsForCallsMadeOn(c => c.Send(null));
            var sentPacket = arguments.FirstOrDefault(p => (p.First() as byte[])[0] == 0xC1)[0] as byte[];
            var expectedPacket = new byte[]
            {
                0xC1, 0x0F, 0x01, 0x00, 0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
            };
            Assert.That(sentPacket, Is.Not.Null);
            Assert.That(sentPacket, Is.EquivalentTo(expectedPacket));
        }

        /// <summary>
        /// Tests if a call to <see cref="ChatClient.LogOff"/> removes it from the chatroom,
        /// and the other remaining client gets notified about it.
        /// </summary>
        [Test]
        public void ClientLoggedOff()
        {
            var manager = new ChatRoomManager();
            var roomId = manager.CreateChatRoom();
            var room = manager.GetChatRoom(roomId);
            var connection1 = MockRepository.GenerateMock<IConnection>();
            var client1 = new ChatClient(connection1, manager);
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", "128450673"));
            room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", "94371960"));

            var authentificationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
            connection1.Raise(c => c.PacketReceived += null, connection1, authentificationPacket1);

            var connection2 = MockRepository.GenerateMock<IConnection>();
            var client2 = new ChatClient(connection2, manager);
            var authentificationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
            connection2.Raise(c => c.PacketReceived += null, connection2, authentificationPacket2);
            client1.LogOff();

            var arguments = connection2.GetArgumentsForCallsMadeOn(c => c.Send(null));
            var sentPacket = arguments.FirstOrDefault(p => (p.Last() as byte[])[0] == 0xC1)[0] as byte[];
            var expectedPacket = new byte[]
            {
                0xC1, 0x0F, 0x01, 0x01, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
            };

            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(client2));
            Assert.That(sentPacket, Is.Not.Null);
            Assert.That(sentPacket, Is.EquivalentTo(expectedPacket));
        }
    }
}