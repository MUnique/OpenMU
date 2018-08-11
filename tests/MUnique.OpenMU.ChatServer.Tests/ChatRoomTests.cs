// <copyright file="ChatRoomTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests
{
    using System;
    using Moq;
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.Interfaces;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="ChatRoom"/>.
    /// </summary>
    [TestFixture]
    public class ChatRoomTests
    {
        /// <summary>
        /// Tests if a new room returns the specified room id, which was given to the constructor.
        /// </summary>
        [Test]
        public void RoomId()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            Assert.That(room.RoomId, Is.EqualTo(roomId));
        }

        /// <summary>
        /// Tries to register a client with a different room id. This should fail with an <see cref="ArgumentException"/>.
        /// </summary>
        [Test]
        public void RegisterClientWithDifferentRoomId()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId - 1, "Bob", "123456789");
            Assert.Throws<ArgumentException>(() => room.RegisterClient(authenticationInfo));
        }

        /// <summary>
        /// Tries to register a client with a correct room id.
        /// </summary>
        [Test]
        public void RegisterClientWithCorrectRoomId()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            Assert.DoesNotThrow(() => room.RegisterClient(authenticationInfo));
        }

        /// <summary>
        /// Tries to join a null client. This should fail with an <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void TryJoinNullClient()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            Assert.Throws<ArgumentNullException>(() => room.TryJoin(null));
        }

        /// <summary>
        /// Tries to join the room with an unauthenticated client. This should fail.
        /// </summary>
        [Test]
        public void TryJoinWithUnauthenticatedClient()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            var chatClient = new Mock<IChatClient>();
            Assert.That(room.TryJoin(chatClient.Object), Is.False);
        }

        /// <summary>
        /// Tries to join the room with a client with a wrong authentication token. This should fail.
        /// </summary>
        [Test]
        public void TryJoinWithAuthenticatedClientButWrongToken()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            var chatClient = new Mock<IChatClient>();
            chatClient.Setup(c => c.AuthenticationToken).Returns("987654321");
            Assert.That(room.TryJoin(chatClient.Object), Is.False);
        }

        /// <summary>
        /// Tries to join the room with an authenticated client. This should be successful.
        /// </summary>
        [Test]
        public void TryJoinWithAuthenticatedClient()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            var chatClient = new Mock<IChatClient>();
            chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
            Assert.That(room.TryJoin(chatClient.Object), Is.True);
        }

        /// <summary>
        /// Tests if <see cref="IChatClient.SendChatRoomClientList"/> is called after a client successfully joined a room.
        /// </summary>
        [Test]
        public void ChatRoomClientListSent()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            var chatClient = new Mock<IChatClient>();
            chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
            room.TryJoin(chatClient.Object);
            chatClient.Verify(c => c.SendChatRoomClientList(room.ConnectedClients), Times.Once);
        }

        /// <summary>
        /// Tests if <see cref="ChatRoom.ConnectedClients"/> returns the successfully authenticated and joined client.
        /// </summary>
        [Test]
        public void ConnectedClients()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo);
            var chatClient = new Mock<IChatClient>();
            chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
            room.TryJoin(chatClient.Object);
            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(chatClient.Object));
        }

        /// <summary>
        /// Tests if <see cref="IChatClient.SendChatRoomClientUpdate"/> is called as soon as another client joined the room.
        /// </summary>
        [Test]
        public void SendJoinedMessage()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId0 = room.GetNextClientIndex();
            var clientId1 = room.GetNextClientIndex();
            var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", "99999");
            var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo0);
            room.RegisterClient(authenticationInfo1);

            var chatClient0 = new Mock<IChatClient>();
            chatClient0.SetupAllProperties();
            chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);
            var chatClient1 = new Mock<IChatClient>();
            chatClient1.SetupAllProperties();
            chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0.Object);
            room.TryJoin(chatClient1.Object);

            chatClient0.Verify(c => c.SendChatRoomClientUpdate(clientId1, authenticationInfo1.ClientName, ChatRoomClientUpdateType.Joined), Times.Once);
        }

        /// <summary>
        /// Tests if <see cref="IChatClient.SendChatRoomClientUpdate"/> is called as soon as another client left the room.
        /// </summary>
        [Test]
        public void SendLeftMessage()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId0 = room.GetNextClientIndex();
            var clientId1 = room.GetNextClientIndex();
            var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", "99999");
            var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo0);
            room.RegisterClient(authenticationInfo1);

            var chatClient0 = new Mock<IChatClient>();
            chatClient0.SetupAllProperties();
            chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

            var chatClient1 = new Mock<IChatClient>();
            chatClient1.SetupAllProperties();
            chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0.Object);
            room.TryJoin(chatClient1.Object);

            room.Leave(chatClient0.Object);
            chatClient1.Verify(c => c.SendChatRoomClientUpdate(clientId0, authenticationInfo0.ClientName, ChatRoomClientUpdateType.Left), Times.Once);
        }

        /// <summary>
        /// Tests if <see cref="IChatClient.SendMessage"/> is called as soon as a message is sent through the room.
        /// </summary>
        [Test]
        public void SendMessage()
        {
            const ushort roomId = 4711;
            const string chatMessage = "foobar1234567890";
            var room = new ChatRoom(roomId);
            var clientId0 = room.GetNextClientIndex();
            var clientId1 = room.GetNextClientIndex();
            var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", "99999");
            var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo0);
            room.RegisterClient(authenticationInfo1);

            var chatClient0 = new Mock<IChatClient>();
            chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

            var chatClient1 = new Mock<IChatClient>();
            chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0.Object);
            room.TryJoin(chatClient1.Object);

            room.SendMessage(clientId1, chatMessage);
            chatClient0.Verify(c => c.SendMessage(clientId1, chatMessage), Times.Once);
            chatClient1.Verify(c => c.SendMessage(clientId1, chatMessage), Times.Once);
        }

        /// <summary>
        /// Tests if <see cref="ChatRoom.RoomClosed"/> is fired as soon as all connected clients left the room.
        /// </summary>
        [Test]
        public void RoomClosedEvent()
        {
            const ushort roomId = 4711;
            var room = new ChatRoom(roomId);
            var clientId0 = room.GetNextClientIndex();
            var clientId1 = room.GetNextClientIndex();
            var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", "99999");
            var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", "123456789");
            room.RegisterClient(authenticationInfo0);
            room.RegisterClient(authenticationInfo1);

            var chatClient0 = new Mock<IChatClient>();
            chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

            var chatClient1 = new Mock<IChatClient>();
            chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

            var eventCalled = false;
            room.RoomClosed += (sender, e) => eventCalled = true;
            room.TryJoin(chatClient0.Object);
            room.TryJoin(chatClient1.Object);
            room.Leave(chatClient0.Object);
            room.Leave(chatClient1.Object);
            Assert.That(eventCalled, Is.True);
        }
    }
}
