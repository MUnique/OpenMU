// <copyright file="FriendServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.FriendServer;

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

        var gameServers = new Dictionary<int, IGameServer>
        {
            { this._gameServer1.Object.Id, this._gameServer1.Object },
            { this._gameServer2.Object.Id, this._gameServer2.Object },
        };
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        var notifier = new FriendNotifierToGameServer(gameServers); // todo: mock this
        this._friendServer = new FriendServer.FriendServer(notifier, new Mock<IChatServer>().Object, this._persistenceContextProvider, NullLogger<FriendServer.FriendServer>.Instance);
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
    public async ValueTask FriendAddRequestOfflineAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        var added = await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        Assert.That(added, Is.True);
        this._gameServer2.Verify(g => g.FriendRequestAsync(this._player1.Name, this._player2.Name), Times.Never);
        await this.CheckFriendItemsAfterRequestAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests what happens when a player adds a friend, while the friend is online.
    /// The online friend should have got a friend request, the player should have a friend list entry.
    /// </summary>
    [Test]
    public async ValueTask FriendAddRequestOnlineAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);
        var added = await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        Assert.That(added, Is.True);
        this._gameServer2.Verify(g => g.FriendRequestAsync(this._player1.Name, this._player2.Name), Times.Once);

        await this.CheckFriendItemsAfterRequestAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests if a friend is not added twice when the player sends two friend requests for the same friend.
    /// </summary>
    [Test]
    public async ValueTask FriendAddRequestRepeatedAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);

        var added = await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        Assert.That(added, Is.True);
        var notAdded = await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        Assert.That(notAdded, Is.False);

        this._gameServer2.Verify(g => g.FriendRequestAsync(this._player1.Name, this._player2.Name), Times.Exactly(2));
    }

    /// <summary>
    /// Tests if both friends have each other in the friend list with visible server number, after the friend accepted friendship.
    /// </summary>
    [Test]
    public async ValueTask FriendAddRequestAcceptAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);

        await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);

        await this._friendServer.FriendResponseAsync(this._player2.Name, this._player1.Name, true).ConfigureAwait(false);
        this._gameServer1.Verify(g => g.FriendOnlineStateChangedAsync(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.AtLeastOnce);
        this._gameServer2.Verify(g => g.FriendOnlineStateChangedAsync(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.AtLeastOnce);

        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem1 = (await context.GetFriendsAsync(this._player1.Id).ConfigureAwait(false)).FirstOrDefault();
        Assert.That(friendItem1, Is.Not.Null);
        Assert.That(friendItem1!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem1.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem1.RequestOpen, Is.False);
        Assert.That(friendItem1.Accepted, Is.True);

        var friendItem2 = (await context.GetFriendsAsync(this._player2.Id).ConfigureAwait(false)).FirstOrDefault();
        Assert.That(friendItem2, Is.Not.Null);
        Assert.That(friendItem2!.CharacterName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem2.FriendName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem2.RequestOpen, Is.False);
        Assert.That(friendItem2.Accepted, Is.True);
    }

    /// <summary>
    /// Tests if the player has the friend in his friendlist, but unaccepted.
    /// Also checks if the friend which declined, does not get the friend list entry.
    /// </summary>
    [Test]
    public async ValueTask FriendAddRequestDeclineAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);
        await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);

        await this._friendServer.FriendResponseAsync(this._player2.Name, this._player1.Name, false).ConfigureAwait(false);
        this._gameServer1.Verify(g => g.FriendOnlineStateChangedAsync(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.Never);
        this._gameServer2.Verify(g => g.FriendOnlineStateChangedAsync(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.Never);

        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem = (await context.GetFriendsAsync(this._player1.Id).ConfigureAwait(false)).FirstOrDefault();
        Assert.That(friendItem, Is.Not.Null);
        Assert.That(friendItem!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem.RequestOpen, Is.False);
        Assert.That(friendItem.Accepted, Is.False);
    }

    /// <summary>
    /// Tests that a friend response without a corresponding request does not create friend list entries.
    /// </summary>
    [Test]
    public async ValueTask FriendResponseWithoutRequestAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);

        await this._friendServer.FriendResponseAsync(this._player2.Name, this._player1.Name, true).ConfigureAwait(false);

        this._gameServer1.Verify(g => g.FriendOnlineStateChangedAsync(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.Never);
        this._gameServer2.Verify(g => g.FriendOnlineStateChangedAsync(this._player2.Name, this._player1.Name, this._gameServer1.Object.Id), Times.Never);
    }

    /// <summary>
    /// Tests if a friend can get deleted from the friend list, but the friend still has the player.
    /// </summary>
    [Test]
    public async ValueTask FriendDeleteAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);
        await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        await this._friendServer.FriendResponseAsync(this._player2.Name, this._player1.Name, true).ConfigureAwait(false);

        await this._friendServer.DeleteFriendAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        this._gameServer2.Verify(g => g.FriendOnlineStateChangedAsync(this._player2.Name, this._player1.Name, FriendServer.FriendServer.OfflineServerId), Times.Once);
    }

    /// <summary>
    /// Here is tested if the notifications between players in the friend list are working properly.
    /// Is is tested with 2 players in 2 different gameservers.
    /// player1 on gameServer1
    /// player2 on gameServer2.
    /// </summary>
    [Test]
    public async ValueTask TestOnlineListAsync()
    {
        await this.PlayerEnteredGameAsync(this._player1.Id, this._player1.Name, this._gameServer1.Object.Id).ConfigureAwait(false);
        await this.PlayerEnteredGameAsync(this._player2.Id, this._player2.Name, this._gameServer2.Object.Id).ConfigureAwait(false);
        await this._friendServer.FriendRequestAsync(this._player1.Name, this._player2.Name).ConfigureAwait(false);
        await this._friendServer.FriendResponseAsync(this._player2.Name, this._player1.Name, true).ConfigureAwait(false);
        await this._friendServer.PlayerLeftGameAsync(this._player1.Id, this._player1.Name).ConfigureAwait(false);
        await this._friendServer.PlayerLeftGameAsync(this._player2.Id, this._player2.Name).ConfigureAwait(false);

        this._gameServer1.Invocations.Clear();
        this._gameServer2.Invocations.Clear();

        await this._friendServer.PlayerEnteredGameAsync((byte)this._gameServer1.Object.Id, this._player1.Id, this._player1.Name).ConfigureAwait(false);
        await this._friendServer.PlayerEnteredGameAsync((byte)this._gameServer2.Object.Id, this._player2.Id, this._player2.Name).ConfigureAwait(false);
        this._gameServer1.Verify(gs => gs.FriendOnlineStateChangedAsync(this._player1.Name, this._player2.Name, this._gameServer2.Object.Id), Times.AtLeastOnce);

        await this._friendServer.PlayerLeftGameAsync(this._player1.Id, this._player1.Name).ConfigureAwait(false);
        this._gameServer2.Verify(gs => gs.FriendOnlineStateChangedAsync(this._player2.Name, this._player1.Name, FriendServer.FriendServer.OfflineServerId), Times.AtLeastOnce);
    }

    private async ValueTask PlayerEnteredGameAsync(Guid playerId, string playerName, int serverId)
    {
        await this._friendServer.PlayerEnteredGameAsync((byte)serverId, playerId, playerName).ConfigureAwait(false);
    }

    private async ValueTask CheckFriendItemsAfterRequestAsync()
    {
        var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friendItem = (await context.GetFriendsAsync(this._player1.Id).ConfigureAwait(false)).FirstOrDefault();
        Assert.That(friendItem, Is.Not.Null);
        Assert.That(friendItem!.CharacterName, Is.EqualTo(this._player1.Name));
        Assert.That(friendItem.FriendName, Is.EqualTo(this._player2.Name));
        Assert.That(friendItem.RequestOpen, Is.True);
        Assert.That(friendItem.Accepted, Is.False);
    }
}