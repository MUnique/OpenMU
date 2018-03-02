// <copyright file="FriendServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests for the friend server.
    /// </summary>
    [TestFixture]
    public sealed class FriendServerTest : IDisposable
    {
        private const string Player1Name = "player1";
        private const string Player2Name = "player2";
        private static readonly Guid Player1Id = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        private static readonly Guid Player2Id = new Guid(2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        private IDictionary<int, IGameServer> gameServers;
        private IGameServer gameServer1;
        private IGameServer gameServer2;
        private IFriendServer friendServer;

        private TestRepositoryManager repositoryManager;

        /// <summary>
        /// Sets up the environment with 2 game servers.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.gameServer1 = MockRepository.GenerateMock<IGameServer>();
            this.gameServer1.Stub(gs => gs.Id).Return(1);
            this.gameServer2 = MockRepository.GenerateMock<IGameServer>();
            this.gameServer2.Stub(gs => gs.Id).Return(2);

            this.gameServers = new Dictionary<int, IGameServer>
            {
                { this.gameServer1.Id, this.gameServer1 },
                { this.gameServer2.Id, this.gameServer2 }
            };
            this.repositoryManager = new TestRepositoryManager();
            this.friendServer = new FriendServer.FriendServer(this.gameServers, null, this.repositoryManager);
        }

        /// <summary>
        /// Tears down the environment.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.repositoryManager.Dispose();
        }

        /// <summary>
        /// Tests what happens when a player adds a friend, while the friend is offline.
        /// The player should have a friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestOffline()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.gameServer2.Expect(g => g.FriendRequest(Player1Name, Player2Name)).Repeat.Never();

            var added = this.friendServer.FriendRequest(Player1Name, Player2Name);
            Assert.That(added, Is.True);
            this.gameServer2.VerifyAllExpectations();
            this.repositoryManager.FriendRepository.UpdateIds();
            this.CheckFriendItemsAfterRequest();
        }

        /// <summary>
        /// Tests what happens when a player adds a friend, while the friend is online.
        /// The online friend should have got a friend request, the player should have a friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestOnline()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.gameServer2.Expect(g => g.FriendRequest(Player1Name, Player2Name)).Repeat.Once();

            var added = this.friendServer.FriendRequest(Player1Name, Player2Name);
            Assert.That(added, Is.True);
            this.gameServer2.VerifyAllExpectations();

            this.CheckFriendItemsAfterRequest();
        }

        /// <summary>
        /// Tests if a friend is not added twice when the player sends two friend requests for the same friend.
        /// </summary>
        [Test]
        public void FriendAddRequestRepeated()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.gameServer2.Expect(g => g.FriendRequest(Player1Name, Player2Name)).Repeat.Twice();
            var added = this.friendServer.FriendRequest(Player1Name, Player2Name);
            Assert.That(added, Is.True);
            var notAdded = this.friendServer.FriendRequest(Player1Name, Player2Name);
            Assert.That(notAdded, Is.False);

            this.gameServer2.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if both friends have each other in the friend list with visible server number, after the friend accepted friendship.
        /// </summary>
        [Test]
        public void FriendAddRequestAccept()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);

            this.friendServer.FriendRequest(Player1Name, Player2Name);

            this.gameServer1.Expect(g => g.FriendOnlineStateChanged(Player1Name, Player2Name, this.gameServer2.Id)).Repeat.AtLeastOnce();
            this.gameServer2.Expect(g => g.FriendOnlineStateChanged(Player2Name, Player1Name, this.gameServer1.Id)).Repeat.AtLeastOnce();
            this.friendServer.FriendResponse(Player2Name, Player1Name, true);
            this.gameServer1.VerifyAllExpectations();
            this.gameServer2.VerifyAllExpectations();
            this.repositoryManager.FriendRepository.UpdateIds();

            var friendItem1 = this.friendServer.GetFriendList(Player1Id).FirstOrDefault();
            Assert.That(friendItem1, Is.Not.Null);
            Assert.That(friendItem1.CharacterName, Is.EqualTo(Player1Name));
            Assert.That(friendItem1.FriendName, Is.EqualTo(Player2Name));
            Assert.That(friendItem1.RequestOpen, Is.False);
            Assert.That(friendItem1.Accepted, Is.True);

            var friendItem2 = this.friendServer.GetFriendList(Player2Id).FirstOrDefault();
            Assert.That(friendItem2, Is.Not.Null);
            Assert.That(friendItem2.CharacterName, Is.EqualTo(Player2Name));
            Assert.That(friendItem2.FriendName, Is.EqualTo(Player1Name));
            Assert.That(friendItem2.RequestOpen, Is.False);
            Assert.That(friendItem2.Accepted, Is.True);
        }

        /// <summary>
        /// Tests if the player has the friend in his friendlist, but unaccepted.
        /// Also checks if the friend which declined, does not get the friend list entry.
        /// </summary>
        [Test]
        public void FriendAddRequestDecline()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.friendServer.FriendRequest(Player1Name, Player2Name);

            this.gameServer1.Expect(g => g.FriendOnlineStateChanged(Player1Name, Player2Name, this.gameServer2.Id)).Repeat.Never();
            this.gameServer2.Expect(g => g.FriendOnlineStateChanged(Player2Name, Player1Name, this.gameServer1.Id)).Repeat.Never();
            this.friendServer.FriendResponse(Player2Name, Player1Name, false);
            this.gameServer1.VerifyAllExpectations();
            this.gameServer2.VerifyAllExpectations();
            this.repositoryManager.FriendRepository.UpdateIds();

            var friendItem = this.friendServer.GetFriendList(Player1Id).FirstOrDefault();
            Assert.That(friendItem, Is.Not.Null);
            Assert.That(friendItem.CharacterName, Is.EqualTo(Player1Name));
            Assert.That(friendItem.FriendName, Is.EqualTo(Player2Name));
            Assert.That(friendItem.RequestOpen, Is.False);
            Assert.That(friendItem.Accepted, Is.False);

            Assert.That(this.friendServer.GetFriendList(Player2Id), Is.Empty);
        }

        /// <summary>
        /// Tests that a friend response without a corresponding request does not create friend list entries.
        /// </summary>
        [Test]
        public void FriendResponseWithoutRequest()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.gameServer1.Expect(g => g.FriendOnlineStateChanged(Player1Name, Player2Name, this.gameServer2.Id)).Repeat.Never();
            this.gameServer2.Expect(g => g.FriendOnlineStateChanged(Player2Name, Player1Name, this.gameServer1.Id)).Repeat.Never();
            this.friendServer.FriendResponse(Player2Name, Player1Name, true);

            this.gameServer1.VerifyAllExpectations();
            this.gameServer2.VerifyAllExpectations();

            Assert.That(this.friendServer.GetFriendList(Player1Id), Is.Empty);
            Assert.That(this.friendServer.GetFriendList(Player2Id), Is.Empty);
        }

        /// <summary>
        /// Tests if a friend can get deleted from the friend list, but the friend still has the player.
        /// </summary>
        [Test]
        public void FriendDelete()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.friendServer.FriendRequest(Player1Name, Player2Name);
            this.friendServer.FriendResponse(Player2Name, Player1Name, true);
            this.repositoryManager.FriendRepository.UpdateIds();
            Assert.That(this.friendServer.GetFriendList(Player1Id), Is.Not.Empty);
            Assert.That(this.friendServer.GetFriendList(Player2Id), Is.Not.Empty);

            this.gameServer2.Expect(g => g.FriendOnlineStateChanged(Player2Name, Player1Name, FriendServer.FriendServer.OfflineServerId)).Repeat.Once();
            this.friendServer.DeleteFriend(Player1Name, Player2Name);
            this.gameServer2.VerifyAllExpectations();

            Assert.That(this.friendServer.GetFriendList(Player1Id), Is.Empty);
            Assert.That(this.friendServer.GetFriendList(Player2Id), Is.Not.Empty);
        }

        /// <summary>
        /// Here is tested if the notifications between players in the friend list are working properly.
        /// Is is tested with 2 players in 2 different gameservers.
        /// player1 on gameServer1
        /// player2 on gameServer2
        /// </summary>
        [Test]
        public void TestOnlineList()
        {
            this.SetPlayerOnline(Player1Id, Player1Name, this.gameServer1.Id);
            this.SetPlayerOnline(Player2Id, Player2Name, this.gameServer2.Id);
            this.friendServer.FriendRequest(Player1Name, Player2Name);
            this.friendServer.FriendResponse(Player2Name, Player1Name, true);
            this.SetPlayerOnline(Player1Id, Player1Name, FriendServer.FriendServer.OfflineServerId);
            this.SetPlayerOnline(Player2Id, Player2Name, FriendServer.FriendServer.OfflineServerId);

            this.friendServer.SetOnlineState(Player1Id, Player1Name, this.gameServer1.Id);
            this.gameServer1.Expect(gs => gs.FriendOnlineStateChanged(Player1Name, Player2Name, this.gameServer2.Id));
            this.friendServer.SetOnlineState(Player2Id, Player2Name, this.gameServer2.Id);
            this.gameServer1.VerifyAllExpectations();

            this.gameServer2.Expect(gs => gs.FriendOnlineStateChanged(Player2Name, Player1Name, FriendServer.FriendServer.OfflineServerId));
            this.friendServer.SetOnlineState(Player1Id, Player1Name, FriendServer.FriendServer.OfflineServerId);
            this.gameServer2.VerifyAllExpectations();
        }

        private void SetPlayerOnline(Guid playerId, string playerName, int serverId)
        {
            this.friendServer.SetOnlineState(playerId, playerName, (byte)serverId);
            if (this.gameServers.TryGetValue((byte)serverId, out IGameServer gameServer))
            {
                gameServer.Stub(g => g.IsPlayerOnline(playerName)).Return(serverId != FriendServer.FriendServer.OfflineServerId).Repeat.Any();
            }

            this.repositoryManager.FriendRepository.UpdateIds();
        }

        private void CheckFriendItemsAfterRequest()
        {
            this.repositoryManager.FriendRepository.UpdateIds();
            var friendItem = this.friendServer.GetFriendList(Player1Id).FirstOrDefault();
            Assert.That(friendItem, Is.Not.Null);
            Assert.That(friendItem.CharacterName, Is.EqualTo(Player1Name));
            Assert.That(friendItem.FriendName, Is.EqualTo(Player2Name));
            Assert.That(friendItem.RequestOpen, Is.True);
            Assert.That(friendItem.Accepted, Is.False);

            Assert.That(this.friendServer.GetFriendList(Player2Id), Is.Empty);
        }

        private class TestRepositoryManager : BaseRepositoryManager
        {
            public TestRepositoryManager()
            {
                this.FriendRepository = new FriendMemoryRepository();
                this.RegisterRepository(this.FriendRepository);
            }

            public FriendMemoryRepository FriendRepository { get; }

            public override IContext CreateNewContext()
            {
                var context = MockRepository.GenerateStub<IContext>();
                context.Stub(c => c.SaveChanges()).Return(true);

                return context;
            }

            public override IContext CreateNewAccountContext(GameConfiguration gameConfiguration)
            {
                return this.CreateNewContext();
            }

            public override IContext CreateNewFriendServerContext()
            {
                return this.CreateNewContext();
            }

            public override T CreateNew<T>(params object[] args)
            {
                var obj = base.CreateNew<T>(args);
                if (obj is FriendViewItem)
                {
                    this.FriendRepository.Store(obj as FriendViewItem);
                }

                return obj;
            }
        }

        private class FriendMemoryRepository : MemoryRepository<FriendViewItem>, IFriendViewItemRepository<FriendViewItem>
        {
            public FriendViewItem GetByFriend(string characterName, string friendName)
            {
                return this.GetAll().FirstOrDefault(item => item.CharacterName == characterName && item.FriendName == friendName);
            }

            public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
            {
                return this.GetAll().Where(item => item.CharacterId == characterId);
            }

            public void Delete(string characterName, string friendName)
            {
                this.Delete(this.GetByFriend(characterName, friendName));
            }

            public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
            {
                return this.GetAll().Where(item => item.CharacterId == characterId && item.RequestOpen).Select(item => item.FriendName);
            }

            public FriendViewItem CreateNewFriendViewItem(string characterName, string friendName)
            {
                var item = new FriendViewItem { CharacterName = characterName, FriendName = friendName };
                this.SetCharacterIds(item);
                this.Store(item);
                return item;
            }

            public void Store(FriendViewItem item)
            {
                this.SetCharacterIds(item);
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                if (this.GetById(item.Id) == null)
                {
                    this.Add(item.Id, item);
                }
            }

            public void UpdateIds()
            {
                foreach (var item in this.GetAll())
                {
                    this.SetCharacterIds(item);
                }
            }

            private void SetCharacterIds(FriendViewItem item)
            {
                switch (item.CharacterName)
                {
                    case Player1Name:
                        item.CharacterId = Player1Id;
                        break;
                    case Player2Name:
                        item.CharacterId = Player2Id;
                        break;
                }

                switch (item.FriendName)
                {
                    case Player1Name:
                        item.FriendId = Player1Id;
                        break;
                    case Player2Name:
                        item.FriendId = Player2Id;
                        break;
                }
            }
        }
    }
}
