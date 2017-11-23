// <copyright file="ChatRoomTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests
{
    using System;
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.Interfaces;
    using NUnit.Framework;
    using Rhino.Mocks;

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
            var chatClient = MockRepository.GenerateStub<IChatClient>();
            Assert.That(room.TryJoin(chatClient), Is.False);
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
            var chatClient = MockRepository.GenerateStub<IChatClient>();
            chatClient.Stub(c => c.AuthenticationToken).Return("987654321");
            Assert.That(room.TryJoin(chatClient), Is.False);
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
            var chatClient = MockRepository.GenerateStub<IChatClient>();
            chatClient.Stub(c => c.AuthenticationToken).Return(authenticationInfo.AuthenticationToken);
            Assert.That(room.TryJoin(chatClient), Is.True);
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
            var chatClient = MockRepository.GenerateMock<IChatClient>();
            chatClient.Stub(c => c.AuthenticationToken).Return(authenticationInfo.AuthenticationToken);
            chatClient.Expect(c => c.SendChatRoomClientList(room.ConnectedClients));
            room.TryJoin(chatClient);
            chatClient.VerifyAllExpectations();
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
            var chatClient = MockRepository.GenerateStub<IChatClient>();
            chatClient.Stub(c => c.AuthenticationToken).Return(authenticationInfo.AuthenticationToken);
            room.TryJoin(chatClient);
            Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
            Assert.That(room.ConnectedClients, Contains.Item(chatClient));
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

            var chatClient0 = MockRepository.GenerateMock<IChatClient>();
            chatClient0.Stub(c => c.AuthenticationToken).Return(authenticationInfo0.AuthenticationToken);
            chatClient0.Expect(c => c.SendChatRoomClientUpdate(clientId1, authenticationInfo1.ClientName, ChatRoomClientUpdateType.Joined));

            var chatClient1 = MockRepository.GenerateStub<IChatClient>();
            chatClient1.Stub(c => c.AuthenticationToken).Return(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0);
            room.TryJoin(chatClient1);

            chatClient0.VerifyAllExpectations();
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

            var chatClient0 = MockRepository.GenerateStub<IChatClient>();
            chatClient0.Stub(c => c.AuthenticationToken).Return(authenticationInfo0.AuthenticationToken);

            var chatClient1 = MockRepository.GenerateMock<IChatClient>();
            chatClient1.Stub(c => c.AuthenticationToken).Return(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0);
            room.TryJoin(chatClient1);

            chatClient1.Expect(c => c.SendChatRoomClientUpdate(clientId0, authenticationInfo0.ClientName, ChatRoomClientUpdateType.Left));
            room.Leave(chatClient0);

            chatClient1.VerifyAllExpectations();
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

            var chatClient0 = MockRepository.GenerateMock<IChatClient>();
            chatClient0.Stub(c => c.AuthenticationToken).Return(authenticationInfo0.AuthenticationToken);

            var chatClient1 = MockRepository.GenerateMock<IChatClient>();
            chatClient1.Stub(c => c.AuthenticationToken).Return(authenticationInfo1.AuthenticationToken);

            room.TryJoin(chatClient0);
            room.TryJoin(chatClient1);

            chatClient0.Expect(c => c.SendMessage(clientId1, chatMessage));
            chatClient1.Expect(c => c.SendMessage(clientId1, chatMessage));
            room.SendMessage(clientId1, chatMessage);

            chatClient0.VerifyAllExpectations();
            chatClient1.VerifyAllExpectations();
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

            var chatClient0 = MockRepository.GenerateStub<IChatClient>();
            chatClient0.Stub(c => c.AuthenticationToken).Return(authenticationInfo0.AuthenticationToken);

            var chatClient1 = MockRepository.GenerateStub<IChatClient>();
            chatClient1.Stub(c => c.AuthenticationToken).Return(authenticationInfo1.AuthenticationToken);

            var eventCalled = false;
            room.RoomClosed += (sender, e) => eventCalled = true;
            room.TryJoin(chatClient0);
            room.TryJoin(chatClient1);
            room.Leave(chatClient0);
            room.Leave(chatClient1);
            Assert.That(eventCalled, Is.True);
        }
    }
}
