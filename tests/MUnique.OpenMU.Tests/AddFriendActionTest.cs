// <copyright file="AddFriendActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="AddFriendAction"/>: a request to a name no character has is answered with
/// "Character 'X' not found." instead of failing silently.
/// </summary>
[TestFixture]
public class AddFriendActionTest
{
    /// <summary>
    /// A name which no character has: no request reaches the friend server and the player is told.
    /// </summary>
    [Test]
    public async ValueTask UnknownNameIsReportedAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        var player = await CreatePlayerAsync(friendServer.Object, "Existing").ConfigureAwait(false);

        await new AddFriendAction().AddFriendAsync(player, "Nobody").ConfigureAwait(false);

        friendServer.Verify(f => f.FriendRequestAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(v => v!.ShowMessageAsync("Character 'Nobody' not found.", MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// A character which exists: the request goes on to the friend server as before.
    /// </summary>
    [Test]
    public async ValueTask ExistingNameIsRequestedAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        var player = await CreatePlayerAsync(friendServer.Object, "Existing").ConfigureAwait(false);

        await new AddFriendAction().AddFriendAsync(player, "Existing").ConfigureAwait(false);

        friendServer.Verify(f => f.FriendRequestAsync("Requester", "Existing"), Times.Once);
    }

    /// <summary>
    /// Checking the name must not leave a friend record behind in the persistence, neither for an
    /// unknown nor for an existing name (the friend server creates the real one).
    /// </summary>
    [Test]
    public async ValueTask CheckingTheNameCreatesNoFriendRecordAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        var persistence = new InMemoryPersistenceContextProvider();
        var player = await CreatePlayerAsync(friendServer.Object, "Existing", persistence).ConfigureAwait(false);
        var friendsBefore = await CountFriendsAsync(persistence).ConfigureAwait(false);

        await new AddFriendAction().AddFriendAsync(player, "Nobody").ConfigureAwait(false);
        await new AddFriendAction().AddFriendAsync(player, "Existing").ConfigureAwait(false);

        var friendsAfter = await CountFriendsAsync(persistence).ConfigureAwait(false);
        Assert.That(friendsAfter, Is.EqualTo(friendsBefore));
    }

    private static async ValueTask<int> CountFriendsAsync(InMemoryPersistenceContextProvider persistence)
    {
        // The repository only exists after the first friend record was created.
        var repository = persistence.RepositoryProvider.GetRepository<Friend>();
        return repository is null ? 0 : (await repository.GetAllAsync().ConfigureAwait(false)).Count();
    }

    private static async ValueTask<Player> CreatePlayerAsync(IFriendServer friendServer, string existingCharacter, InMemoryPersistenceContextProvider? persistence = null)
    {
        persistence ??= new InMemoryPersistenceContextProvider();
        using (var setup = persistence.CreateNewContext())
        {
            setup.CreateNew<MUnique.OpenMU.DataModel.Entities.Character>().Name = existingCharacter;
            await setup.SaveChangesAsync().ConfigureAwait(false);
        }

        var context = persistence.CreateNewContext();
        var gameConfiguration = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameConfiguration>();
        gameConfiguration.MaximumPartySize = 5;
        gameConfiguration.RecoveryInterval = int.MaxValue;
        var mapDef = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition>();
        mapDef.Number = 0;
        mapDef.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfiguration.Maps.Add(mapDef);

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServer = new GameServerContext(
            new GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new GameServerConfiguration() },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            friendServer,
            persistence,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServer.PlugInManager;
        mapInitializer.PathFinderPool = gameServer.PathFinderPool;

        var player = await PlayerTestHelper.CreatePlayerAsync(gameServer).ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Requester";
        return player;
    }
}
