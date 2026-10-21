// <copyright file="SharedMapTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using GameServerInstance = MUnique.OpenMU.GameServer.GameServer;

/// <summary>
/// Tests for maps which are shared between the game servers of the same process, because
/// only some of them host it (<see cref="InProcessMapHostLocator"/>).
/// </summary>
[TestFixture]
public class SharedMapTests
{
    private const byte HostingServerId = 1;

    private const byte NonHostingServerId = 0;

    private const short HomeMapNumber = 0;

    private const short SharedMapNumber = 1;

    private readonly Dictionary<int, IGameServer> _gameServers = new();

    private GameMapDefinition _homeMap = null!;

    private GameMapDefinition _sharedMap = null!;

    private GameServerInstance _hostingServer = null!;

    private GameServerInstance _nonHostingServer = null!;

    /// <summary>
    /// Sets up a game configuration with a home and a shared map, and two game servers:
    /// one which hosts both maps, and one which only hosts the home map.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        var context = persistenceContextProvider.CreateNewContext();
        var gameConfiguration = context.CreateNew<GameConfiguration>();
        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumPartySize = 5;
        gameConfiguration.ItemDropDuration = TimeSpan.FromMinutes(1);
        this._homeMap = CreateMap(context, gameConfiguration, HomeMapNumber);
        this._sharedMap = CreateMap(context, gameConfiguration, SharedMapNumber);
        this._sharedMap.SafezoneMap = this._homeMap;

        this._gameServers.Clear();
        var locator = new InProcessMapHostLocator(this._gameServers);
        this._hostingServer = this.CreateGameServer(context, persistenceContextProvider, gameConfiguration, locator, HostingServerId, this._homeMap, this._sharedMap);
        this._nonHostingServer = this.CreateGameServer(context, persistenceContextProvider, gameConfiguration, locator, NonHostingServerId, this._homeMap);
    }

    /// <summary>
    /// Disposes the game servers.
    /// </summary>
    [TearDown]
    public async ValueTask TearDownAsync()
    {
        await this._hostingServer.DisposeAsync().ConfigureAwait(false);
        await this._nonHostingServer.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a player enters the map instance of the hosting game server, when its own
    /// game server doesn't host the map.
    /// </summary>
    [Test]
    public async ValueTask PlayerEntersMapOfHostingGameServerAsync()
    {
        var player = await this.CreatePlayerAsync(this._nonHostingServer).ConfigureAwait(false);

        await this.WarpAsync(player, this._sharedMap).ConfigureAwait(false);

        var hostedMap = await this._hostingServer.Context.GetMapAsync((ushort)SharedMapNumber).ConfigureAwait(false);
        Assert.That(player.CurrentMap, Is.Not.Null.And.SameAs(hostedMap));
        Assert.That(await this._nonHostingServer.Context.GetMapAsync((ushort)SharedMapNumber).ConfigureAwait(false), Is.Null);
        Assert.That(player.GameContext, Is.SameAs(this._nonHostingServer.Context), "the player stays at its own game server");
    }

    /// <summary>
    /// Tests that players of different game servers meet each other on the shared map.
    /// </summary>
    [Test]
    public async ValueTask PlayersOfDifferentGameServersMeetOnSharedMapAsync()
    {
        var guest = await this.CreatePlayerAsync(this._nonHostingServer).ConfigureAwait(false);
        var host = await this.CreatePlayerAsync(this._hostingServer).ConfigureAwait(false);

        await this.WarpAsync(guest, this._sharedMap).ConfigureAwait(false);
        await this.WarpAsync(host, this._sharedMap).ConfigureAwait(false);

        Assert.That(guest.CurrentMap, Is.SameAs(host.CurrentMap));
        Assert.That(guest.CurrentMap!.GetPlayers(), Is.EquivalentTo(new[] { guest, host }));
        Assert.That(guest.Id, Is.Not.EqualTo(host.Id));
    }

    /// <summary>
    /// Tests that a map which is still hosted by the own game server is entered there,
    /// even if another game server hosts it, too.
    /// </summary>
    [Test]
    public async ValueTask OwnHostedMapIsPreferredAsync()
    {
        var player = await this.CreatePlayerAsync(this._nonHostingServer).ConfigureAwait(false);

        var ownMap = await this._nonHostingServer.Context.GetMapAsync((ushort)HomeMapNumber).ConfigureAwait(false);
        Assert.That(player.CurrentMap, Is.Not.Null.And.SameAs(ownMap));
    }

    /// <summary>
    /// Tests that a player is warped to its home map, when the only game server which
    /// hosts the map is stopped.
    /// </summary>
    [Test]
    public async ValueTask PlayerIsWarpedToHomeMapWhenHostIsStoppedAsync()
    {
        var player = await this.CreatePlayerAsync(this._nonHostingServer).ConfigureAwait(false);
        this._hostingServer.ServerState = ServerState.Stopped;

        await this.WarpAsync(player, this._sharedMap).ConfigureAwait(false);

        Assert.That(player.CurrentMap, Is.Null, "the player is warped again, so it waits for the client to load the home map");
        Assert.That(player.SelectedCharacter!.CurrentMap, Is.SameAs(this._homeMap));

        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        var ownMap = await this._nonHostingServer.Context.GetMapAsync((ushort)HomeMapNumber).ConfigureAwait(false);
        Assert.That(player.CurrentMap, Is.Not.Null.And.SameAs(ownMap));
    }

    /// <summary>
    /// Tests that the players of other game servers leave the shared map when its hosting
    /// game server shuts down, while they stay connected to their own game server.
    /// </summary>
    [Test]
    public async ValueTask GuestPlayersLeaveMapsOfStoppingGameServerAsync()
    {
        var guest = await this.CreatePlayerAsync(this._nonHostingServer).ConfigureAwait(false);
        await this.WarpAsync(guest, this._sharedMap).ConfigureAwait(false);
        var sharedMap = guest.CurrentMap!;

        await this._hostingServer.ShutdownAsync().ConfigureAwait(false);

        Assert.That(sharedMap.GetPlayers(), Does.Not.Contain(guest));
        Assert.That(guest.PlayerState.CurrentState, Is.Not.EqualTo(PlayerState.Disconnected));

        await guest.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        var ownMap = await this._nonHostingServer.Context.GetMapAsync((ushort)HomeMapNumber).ConfigureAwait(false);
        Assert.That(guest.CurrentMap, Is.Not.Null.And.SameAs(ownMap));
    }

    private static GameMapDefinition CreateMap(IContext context, GameConfiguration gameConfiguration, short number)
    {
        var map = context.CreateNew<GameMapDefinition>();
        map.Number = number;
        map.TerrainData = new byte[ushort.MaxValue + 3];
        var spawnGate = context.CreateNew<ExitGate>();
        spawnGate.Map = map;
        spawnGate.X1 = 10;
        spawnGate.Y1 = 10;
        spawnGate.X2 = 13;
        spawnGate.Y2 = 13;
        spawnGate.IsSpawnGate = true;
        map.ExitGates.Add(spawnGate);
        gameConfiguration.Maps.Add(map);
        return map;
    }

    private GameServerInstance CreateGameServer(IContext context, IPersistenceContextProvider persistenceContextProvider, GameConfiguration gameConfiguration, IMapHostLocator locator, byte serverId, params GameMapDefinition[] hostedMaps)
    {
        var serverConfiguration = context.CreateNew<GameServerConfiguration>();
        foreach (var map in hostedMaps)
        {
            serverConfiguration.Maps.Add(map);
        }

        var serverDefinition = context.CreateNew<GameServerDefinition>();
        serverDefinition.ServerID = serverId;
        serverDefinition.ExperienceRate = 1;
        serverDefinition.GameConfiguration = gameConfiguration;
        serverDefinition.ServerConfiguration = serverConfiguration;

        var gameServer = new GameServerInstance(
            serverDefinition,
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            persistenceContextProvider,
            new Mock<IFriendServer>().Object,
            new NullLoggerFactory(),
            new PlugInManager(null, new NullLoggerFactory(), null, null),
            new ConfigurationChangeMediator(),
            mapHostLocator: locator)
        {
            ServerState = ServerState.Started,
        };

        this._gameServers.Add(serverId, gameServer);
        return gameServer;
    }

    private async ValueTask<Player> CreatePlayerAsync(GameServerInstance gameServer)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameServer.Context).ConfigureAwait(false);
        Mock.Get(player.SelectedCharacter!.CharacterClass!).Setup(c => c.HomeMap).Returns(this._homeMap);
        return player;
    }

    private async ValueTask WarpAsync(Player player, GameMapDefinition targetMap)
    {
        await player.WarpToAsync(targetMap.ExitGates.First()).ConfigureAwait(false);

        // The test player has no game client which acknowledges the map change, so we do it here.
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
    }
}
