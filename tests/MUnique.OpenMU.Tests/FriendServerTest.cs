// <copyright file="FriendServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence.InMemory;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the friend server.
    /// </summary>
    [TestFixture]
    public sealed class FriendServerTest
    {
        private Character player1;
        private Character player2;
        private IDictionary<int, IGameServer> gameServers;
        private Mock<IGameServer> gameServer1;
        private Mock<IGameServer> gameServer2;
        private IFriendServer friendServer;

        private InMemoryPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Sets up the environment with 2 game servers.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.gameServer1 = new Mock<IGameServer>();
            this.gameServer1.Setup(gs => gs.Id).Returns(1);
            this.gameServer2 = new Mock<IGameServer>();
            this.gameServer2.Setup(gs => gs.Id).Returns(2);

            this.gameServers = new Dictionary<int, IGameServer>
            {
                { this.gameServer1.Object.Id, this.gameServer1.Object },
                { this.gameServer2.Object.Id, this.gameServer2.Object },
            };
            this.persistenceContextProvider = new InMemoryPersistenceContextProvider();
            this.friendServer = new FriendServer.FriendServer(this.gameServers, null, this.persistenceContextProvider);
            var context = this.persistenceContextProvider.CreateNewContext();
            this.player1 = context.CreateNew<Character>();
            this.player1.Name = "player1";
            this.player2 = context.CreateNew<Character>();
            this.player2.Name = "player2";
        }

        /// <summary>
        /// Tests what happens when a player adds a friend, while the friend is offline.
        /// The player should have a friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestOffline()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            var added = this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            Assert.That(added, Is.True);
            this.gameServer2.Verify(g => g.FriendRequest(this.player1.Name, this.player2.Name), Times.Never);
            this.CheckFriendItemsAfterRequest();
        }

        /// <summary>
        /// Tests what happens when a player adds a friend, while the friend is online.
        /// The online friend should have got a friend request, the player should have a friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestOnline()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);
            var added = this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            Assert.That(added, Is.True);
            this.gameServer2.Verify(g => g.FriendRequest(this.player1.Name, this.player2.Name), Times.Once);

            this.CheckFriendItemsAfterRequest();
        }

        /// <summary>
        /// Tests if a friend is not added twice when the player sends two friend requests for the same friend.
        /// </summary>
        [Test]
        public void FriendAddRequestRepeated()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);

            var added = this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            Assert.That(added, Is.True);
            var notAdded = this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            Assert.That(notAdded, Is.False);

            this.gameServer2.Verify(g => g.FriendRequest(this.player1.Name, this.player2.Name), Times.Exactly(2));
        }

        /// <summary>
        /// Tests if both friends have each other in the friend list with visible server number, after the friend accepted friendship.
        /// </summary>
        [Test]
        public void FriendAddRequestAccept()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);

            this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);

            this.friendServer.FriendResponse(this.player2.Name, this.player1.Name, true);
            this.gameServer1.Verify(g => g.FriendOnlineStateChanged(this.player1.Name, this.player2.Name, this.gameServer2.Object.Id), Times.AtLeastOnce);
            this.gameServer2.Verify(g => g.FriendOnlineStateChanged(this.player2.Name, this.player1.Name, this.gameServer1.Object.Id), Times.AtLeastOnce);

            var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            var friendItem1 = context.GetFriends(this.player1.Id).FirstOrDefault();
            Assert.That(friendItem1, Is.Not.Null);
            Assert.That(friendItem1.CharacterName, Is.EqualTo(this.player1.Name));
            Assert.That(friendItem1.FriendName, Is.EqualTo(this.player2.Name));
            Assert.That(friendItem1.RequestOpen, Is.False);
            Assert.That(friendItem1.Accepted, Is.True);

            var friendListItem1 = this.friendServer.GetFriendList(this.player1.Id).FirstOrDefault();
            Assert.That(friendListItem1, Is.EqualTo(this.player2.Name));

            var friendItem2 = context.GetFriends(this.player2.Id).FirstOrDefault();
            Assert.That(friendItem2, Is.Not.Null);
            Assert.That(friendItem2.CharacterName, Is.EqualTo(this.player2.Name));
            Assert.That(friendItem2.FriendName, Is.EqualTo(this.player1.Name));
            Assert.That(friendItem2.RequestOpen, Is.False);
            Assert.That(friendItem2.Accepted, Is.True);

            var friendListItem2 = this.friendServer.GetFriendList(this.player2.Id).FirstOrDefault();
            Assert.That(friendListItem2, Is.EqualTo(this.player1.Name));
        }

        /// <summary>
        /// Tests if the player has the friend in his friendlist, but unaccepted.
        /// Also checks if the friend which declined, does not get the friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestDecline()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);
            this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);

            this.friendServer.FriendResponse(this.player2.Name, this.player1.Name, false);
            this.gameServer1.Verify(g => g.FriendOnlineStateChanged(this.player1.Name, this.player2.Name, this.gameServer2.Object.Id), Times.Never);
            this.gameServer2.Verify(g => g.FriendOnlineStateChanged(this.player2.Name, this.player1.Name, this.gameServer1.Object.Id), Times.Never);

            var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            var friendItem = context.GetFriends(this.player1.Id).FirstOrDefault();
            Assert.That(friendItem, Is.Not.Null);
            Assert.That(friendItem.CharacterName, Is.EqualTo(this.player1.Name));
            Assert.That(friendItem.FriendName, Is.EqualTo(this.player2.Name));
            Assert.That(friendItem.RequestOpen, Is.False);
            Assert.That(friendItem.Accepted, Is.False);

            Assert.That(this.friendServer.GetFriendList(this.player2.Id), Is.Empty);
        }

        /// <summary>
        /// Tests that a friend response without a corresponding request does not create friend list entries.
        /// </summary>
        [Test]
        public void FriendResponseWithoutRequest()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);

            this.friendServer.FriendResponse(this.player2.Name, this.player1.Name, true);

            this.gameServer1.Verify(g => g.FriendOnlineStateChanged(this.player1.Name, this.player2.Name, this.gameServer2.Object.Id), Times.Never);
            this.gameServer2.Verify(g => g.FriendOnlineStateChanged(this.player2.Name, this.player1.Name, this.gameServer1.Object.Id), Times.Never);

            Assert.That(this.friendServer.GetFriendList(this.player1.Id), Is.Empty);
            Assert.That(this.friendServer.GetFriendList(this.player2.Id), Is.Empty);
        }

        /// <summary>
        /// Tests if a friend can get deleted from the friend list, but the friend still has the player.
        /// </summary>
        [Test]
        public void FriendDelete()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);
            this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            this.friendServer.FriendResponse(this.player2.Name, this.player1.Name, true);
            Assert.That(this.friendServer.GetFriendList(this.player1.Id), Is.Not.Empty);
            Assert.That(this.friendServer.GetFriendList(this.player2.Id), Is.Not.Empty);

            this.friendServer.DeleteFriend(this.player1.Name, this.player2.Name);
            this.gameServer2.Verify(g => g.FriendOnlineStateChanged(this.player2.Name, this.player1.Name, FriendServer.FriendServer.OfflineServerId), Times.Once);

            Assert.That(this.friendServer.GetFriendList(this.player1.Id), Is.Empty);
            Assert.That(this.friendServer.GetFriendList(this.player2.Id), Is.Not.Empty);
        }

        /// <summary>
        /// Here is tested if the notifications between players in the friend list are working properly.
        /// Is is tested with 2 players in 2 different gameservers.
        /// player1 on gameServer1
        /// player2 on gameServer2.
        /// </summary>
        [Test]
        public void TestOnlineList()
        {
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);
            this.friendServer.FriendRequest(this.player1.Name, this.player2.Name);
            this.friendServer.FriendResponse(this.player2.Name, this.player1.Name, true);
            this.SetPlayerOnline(this.player1.Id, this.player1.Name, FriendServer.FriendServer.OfflineServerId);
            this.SetPlayerOnline(this.player2.Id, this.player2.Name, FriendServer.FriendServer.OfflineServerId);

            this.gameServer1.Invocations.Clear();
            this.gameServer2.Invocations.Clear();

            this.friendServer.SetOnlineState(this.player1.Id, this.player1.Name, this.gameServer1.Object.Id);
            this.friendServer.SetOnlineState(this.player2.Id, this.player2.Name, this.gameServer2.Object.Id);
            this.gameServer1.Verify(gs => gs.FriendOnlineStateChanged(this.player1.Name, this.player2.Name, this.gameServer2.Object.Id), Times.Once);

            this.friendServer.SetOnlineState(this.player1.Id, this.player1.Name, FriendServer.FriendServer.OfflineServerId);
            this.gameServer2.Verify(gs => gs.FriendOnlineStateChanged(this.player2.Name, this.player1.Name, FriendServer.FriendServer.OfflineServerId), Times.Once);
        }

        private void SetPlayerOnline(Guid playerId, string playerName, int serverId)
        {
            this.friendServer.SetOnlineState(playerId, playerName, (byte)serverId);
            if (this.gameServers.TryGetValue((byte)serverId, out IGameServer gameServer))
            {
                Mock.Get(gameServer).Setup(g => g.IsPlayerOnline(playerName)).Returns(serverId != FriendServer.FriendServer.OfflineServerId);
            }
        }

        private void CheckFriendItemsAfterRequest()
        {
            var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            var friendItem = context.GetFriends(this.player1.Id).FirstOrDefault();
            Assert.That(friendItem, Is.Not.Null);
            Assert.That(friendItem.CharacterName, Is.EqualTo(this.player1.Name));
            Assert.That(friendItem.FriendName, Is.EqualTo(this.player2.Name));
            Assert.That(friendItem.RequestOpen, Is.True);
            Assert.That(friendItem.Accepted, Is.False);

            Assert.That(this.friendServer.GetFriendList(this.player2.Id), Is.Empty);
        }
    }
}