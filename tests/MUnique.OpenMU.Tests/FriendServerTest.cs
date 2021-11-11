// <copyright file="FriendServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the friend server.
/// </summary>
[TestFixture]
public sealed class FriendServerTest
{
    private Character _player1 = null!;
    private Character _player2 = null!;
    private IDictionary<int, IGameServer> _gameServers = null!;
    private Mock<IGameServer> _gameServer1 = null!;
    private Mock<IGameServer> _gameServer2 = null!;
    private IFriendServer _friendServer = null!;

    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    /// <summary>
    /// Sets up the environment with 2 game servers.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameServer1 = new Mock<IGameServer>();
        this._gameServer1.Setup(gs => gs.Id).Returns(1);
        this._gameServer2 = new Mock<IGameServer>();
        this._gameServer2.Setup(gs => gs.Id).Returns(2);

        this._gameServers = new Dictionary<int, IGameServer>
        {
            { this._gameServer1.Object.Id, this._gameServer1.Object },
            { this._gameServer2.Object.Id, this._gameServer2.Object },
        };
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        this._friendServer = new FriendServer.FriendServer(this._gameServers, null, this._persistenceContextProvider);
        var context = this._persistenceContextProvider.CreateNewContext();
        this._player1 = context.CreateNew<Character>();
        this._player1.Name = "player1";
        this._player2 = context.CreateNew<Character>();
        this._player2.Name = "player2";
    }

    /// <summary>
    /// Tests what happens when a player adds a friend, while the friend is offline.
    /// The player should have a friend list entry.
    /// </summary>
    [Test]
    public void FriendAddRequestOffline()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        var added = this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        Assert.That(added, Is.True);
        this._gameServer2.Verify(g => g.FriendRequest(this._player1.Name, this._player2.Name), Times.Never);
        this.CheckFriendItemsAfterRequest();
    }

    /// <summary>
    /// Tests what happens when a player adds a friend, while the friend is online.
    /// The online friend should have got a friend request, the player should have a friend list entry.
    /// </summary>
    [Test]
    public void FriendAddRequestOnline()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);
        var added = this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        Assert.That(added, Is.True);
        this._gameServer2.Verify(g => g.FriendRequest(this._player1.Name, this._player2.Name), Times.Once);

        this.CheckFriendItemsAfterRequest();
    }

    /// <summary>
    /// Tests if a friend is not added twice when the player sends two friend requests for the same friend.
    /// </summary>
    [Test]
    public void FriendAddRequestRepeated()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);

        var added = this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        Assert.That(added, Is.True);
        var notAdded = this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        Assert.That(notAdded, Is.False);

        this._gameServer2.Verify(g => g.FriendRequest(this._player1.Name, this._player2.Name), Times.Exactly(2));
    }

    /// <summary>
    /// Tests if both friends have each other in the friend list with visible server number, after the friend accepted friendship.
    /// </summary>
    [Test]
    public void FriendAddRequestAccept()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);

        this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);

        this._friendServer.FriendResponse(this._player2.Name, this._player1.Name, true);
        this._gameServer1.Verify(g => g.FriendOnlineStateChanged(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.AtLeastOnce);
        this._gameServer2.Verify(g => g.FriendOnlineStateChanged(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.AtLeastOnce);

        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem1 = context.GetFriends(this._player1.Id).FirstOrDefault();
        Assert.That(friendItem1, Is.Not.Null);
        Assert.That(friendItem1!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem1.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem1.RequestOpen, Is.False);
        Assert.That(friendItem1.Accepted, Is.True);

        var friendListItem1 = this._friendServer.GetFriendList(this._player1.Id).FirstOrDefault();
        Assert.That(friendListItem1, Is.EqualTo(this._player2.Name));

        var friendItem2 = context.GetFriends(this._player2.Id).FirstOrDefault();
        Assert.That(friendItem2, Is.Not.Null);
        Assert.That(friendItem2!.CharacterName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem2.FriendName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem2.RequestOpen, Is.False);
        Assert.That(friendItem2.Accepted, Is.True);

        var friendListItem2 = this._friendServer.GetFriendList(this._player2.Id).FirstOrDefault();
        Assert.That(friendListItem2, Is.EqualTo(this._player1.Name));
    }

    /// <summary>
    /// Tests if the player has the friend in his friendlist, but unaccepted.
    /// Also checks if the friend which declined, does not get the friend list entry.
    /// </summary>
    [Test]
    public void FriendAddRequestDecline()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);
        this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);

        this._friendServer.FriendResponse(this._player2.Name, this._player1.Name, false);
        this._gameServer1.Verify(g => g.FriendOnlineStateChanged(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.Never);
        this._gameServer2.Verify(g => g.FriendOnlineStateChanged(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.Never);

        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem = context.GetFriends(this._player1.Id).FirstOrDefault();
        Assert.That(friendItem, Is.Not.Null);
        Assert.That(friendItem!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem.RequestOpen, Is.False);
        Assert.That(friendItem.Accepted, Is.False);

        Assert.That(this._friendServer.GetFriendList(this._player2.Id), Is.Empty);
    }

    /// <summary>
    /// Tests that a friend response without a corresponding request does not create friend list entries.
    /// </summary>
    [Test]
    public void FriendResponseWithoutRequest()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);

        this._friendServer.FriendResponse(this._player2.Name, this._player1.Name, true);

        this._gameServer1.Verify(g => g.FriendOnlineStateChanged(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.Never);
        this._gameServer2.Verify(g => g.FriendOnlineStateChanged(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.Never);

        Assert.That(this._friendServer.GetFriendList(this._player1.Id), Is.Empty);
        Assert.That(this._friendServer.GetFriendList(this._player2.Id), Is.Empty);
    }

    /// <summary>
    /// Tests if a friend can get deleted from the friend list, but the friend still has the player.
    /// </summary>
    [Test]
    public void FriendDelete()
    {
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);
        this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        this._friendServer.FriendResponse(this._player2.Name, this._player1.Name, true);
        Assert.That(this._friendServer.GetFriendList(this._player1.Id), Is.Not.Empty);
        Assert.That(this._friendServer.GetFriendList(this._player2.Id), Is.Not.Empty);

        this._friendServer.DeleteFriend(this._player1.Name, this._player2.Name);
        this._gameServer2.Verify(g => g.FriendOnlineStateChanged(this._player2.Name, this._player1.Name, FriendServer.FriendServer.OfflineServerId), Times.Once);

        Assert.That(this._friendServer.GetFriendList(this._player1.Id), Is.Empty);
        Assert.That(this._friendServer.GetFriendList(this._player2.Id), Is.Not.Empty);
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
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);
        this._friendServer.FriendRequest(this._player1.Name, this._player2.Name);
        this._friendServer.FriendResponse(this._player2.Name, this._player1.Name, true);
        this.SetPlayerOnline(this._player1.Id, this._player1.Name, FriendServer.FriendServer.OfflineServerId);
        this.SetPlayerOnline(this._player2.Id, this._player2.Name, FriendServer.FriendServer.OfflineServerId);

        this._gameServer1.Invocations.Clear();
        this._gameServer2.Invocations.Clear();

        this._friendServer.SetOnlineState(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id);
        this._friendServer.SetOnlineState(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id);
        this._gameServer1.Verify(gs => gs.FriendOnlineStateChanged(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.Once);

        this._friendServer.SetOnlineState(this._player1.Id, this._player1.Name, FriendServer.FriendServer.OfflineServerId);
        this._gameServer2.Verify(gs => gs.FriendOnlineStateChanged(this._player2.Name, this._player1.Name, FriendServer.FriendServer.OfflineServerId), Times.Once);
    }

    private void SetPlayerOnline(Guid playerId, string playerName, int serverId)
    {
        this._friendServer.SetOnlineState(playerId, playerName, (byte)serverId);
        if (this._gameServers.TryGetValue((byte)serverId, out var gameServer))
        {
            Mock.Get(gameServer).Setup(g => g.IsPlayerOnline(playerName)).Returns(serverId != FriendServer.FriendServer.OfflineServerId);
        }
    }

    private void CheckFriendItemsAfterRequest()
    {
        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem = context.GetFriends(this._player1.Id).FirstOrDefault();
        Assert.That(friendItem, Is.Not.Null);
        Assert.That(friendItem!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem.RequestOpen, Is.True);
        Assert.That(friendItem.Accepted, Is.False);

        Assert.That(this._friendServer.GetFriendList(this._player2.Id), Is.Empty);
    }
}