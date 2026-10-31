// <copyright file="GameServerFullTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that only logged in players take a place on a game server: a full server refuses the
/// login, while connections which stay at the login screen don't count.
/// </summary>
[TestFixture]
public class GameServerFullTests
{
    private const string Password = "pw";

    private Mock<ILoginServer> _loginServer = null!;

    private Mock<IPlayerContext> _playerContext = null!;

    /// <summary>
    /// Sets up the mocks of a fresh game server.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._loginServer = new Mock<ILoginServer>();
        this._loginServer.Setup(l => l.TryLoginAsync(It.IsAny<string>(), It.IsAny<byte>())).ReturnsAsync(true);
        this._playerContext = new Mock<IPlayerContext> { DefaultValue = DefaultValue.Mock };
        this._playerContext
            .Setup(c => c.AuthenticateAsync(It.IsAny<string>(), Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AccountState.Normal);
        this._playerContext
            .Setup(c => c.GetAccountByLoginNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string loginName, CancellationToken _) => new Persistence.BasicModel.Account { LoginName = loginName });
    }

    /// <summary>
    /// A connection which stays at the login screen doesn't take the last place.
    /// </summary>
    [Test]
    public async ValueTask ConnectionsAtTheLoginScreenDoNotTakeAPlaceAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);

        VerifyLoginResult(player, LoginResult.Ok);
        Assert.That(gameContext.PlayerCount, Is.EqualTo(2));
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(1));
    }

    /// <summary>
    /// When all places are taken by logged in players, the login is refused with
    /// <see cref="LoginResult.ServerIsFull"/>, and the account is released at the login server.
    /// The connection stays at the login screen.
    /// </summary>
    [Test]
    public async ValueTask RefusesTheLoginWhenTheServerIsFullAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var first = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var second = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await new LoginAction().LoginAsync(first, "a", Password).ConfigureAwait(false);

        await new LoginAction().LoginAsync(second, "b", Password).ConfigureAwait(false);

        VerifyLoginResult(second, LoginResult.ServerIsFull);
        Assert.That(second.Account, Is.Null);
        Assert.That(second.PlayerState.CurrentState, Is.EqualTo(PlayerState.LoginScreen));
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(1));
        this._loginServer.Verify(l => l.LogOffAsync("b", It.IsAny<byte>()), Times.Once);
        this._loginServer.Verify(l => l.LogOffAsync("a", It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// A player frees its place as soon as it disconnects, before it's removed from the game.
    /// </summary>
    [Test]
    public async ValueTask DisconnectedPlayerFreesItsPlaceAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var first = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var second = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await new LoginAction().LoginAsync(first, "a", Password).ConfigureAwait(false);

        await first.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);
        await new LoginAction().LoginAsync(second, "b", Password).ConfigureAwait(false);

        VerifyLoginResult(second, LoginResult.Ok);
        Assert.That(gameContext.PlayerCount, Is.EqualTo(2));
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(1));
    }

    /// <summary>
    /// A login checks for a free place only after the login which holds the completion lock is done,
    /// so two logins can't take the last place at the same time.
    /// </summary>
    [Test]
    public async ValueTask WaitsForTheLoginWhichTakesTheLastPlaceAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var completing = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);

        var completionLock = await gameContext.LockLoginCompletionAsync().ConfigureAwait(false);
        var login = new LoginAction().LoginAsync(player, "b", Password).AsTask();
        Assert.That(login.IsCompleted, Is.False, "The login should wait for the completion lock.");

        // The other login completes and takes the last place.
        await completing.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        completionLock.Dispose();
        await login.ConfigureAwait(false);

        VerifyLoginResult(player, LoginResult.ServerIsFull);
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(1));
    }

    /// <summary>
    /// A player which is already logged in when it's added, like an offline player, counts as well,
    /// until it's removed.
    /// </summary>
    [Test]
    public async ValueTask PlayerWhichIsAddedLoggedInCountsAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = new LoginTestPlayer(gameContext);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);

        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(1));

        await gameContext.RemovePlayerAsync(player).ConfigureAwait(false);
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(0));
    }

    /// <summary>
    /// The game context follows the state of its players, and a state change still waits for the
    /// subscribers which were there before, like the one which runs the state changed plugins.
    /// </summary>
    [Test]
    public async ValueTask StateChangeWaitsForEarlierSubscribersAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = new LoginTestPlayer(gameContext);
        var earlierSubscriberCompleted = false;
        player.PlayerState.StateChanged += async _ =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            earlierSubscriberCompleted = true;
        };
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);

        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        Assert.That(earlierSubscriberCompleted, Is.True);
    }

    /// <summary>
    /// The listener reports the logged in players to the connect server: not the connections at the
    /// login screen, and a disconnected player right away instead of after it saved its progress.
    /// </summary>
    [Test]
    public async ValueTask ListenerReportsTheLoggedInPlayersAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 5);
        var reports = new List<int>();
        var observer = new Mock<IGameServerStateObserver>();
        observer
            .Setup(o => o.CurrentConnectionsChanged(It.IsAny<ushort>(), It.IsAny<int>()))
            .Callback((ushort _, int count) => reports.Add(count));
        var resolver = new Mock<IIpAddressResolver>();
        resolver.Setup(r => r.ResolveIPv4Async()).ReturnsAsync(IPAddress.Loopback);
        var endpoint = new Persistence.BasicModel.GameServerEndpoint
        {
            NetworkPort = 0,
            Client = new Persistence.BasicModel.GameClientDefinition { Season = 6, Episode = 3, Language = ClientLanguage.English },
        };
        var listener = new DefaultTcpGameServerListener(endpoint, new ServerInfo(0, "test", 0, 5), gameContext, observer.Object, resolver.Object, new NullLoggerFactory());

        await listener.StartAsync().ConfigureAwait(false);
        try
        {
            var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
            await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
            await player.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);

            Assert.That(gameContext.PlayerCount, Is.EqualTo(1), "The disconnected player wasn't removed yet.");
        }
        finally
        {
            listener.Stop();
        }

        Assert.That(reports, Is.EqualTo(new[] { 0, 1, 0 }));
    }

    private static void VerifyLoginResult(Player player, LoginResult result)
    {
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()!)
            .Verify(p => p.ShowLoginResultAsync(result), Times.Once);
    }

    private async ValueTask<Player> CreatePlayerAtLoginScreenAsync(IGameContext gameContext)
    {
        var player = new LoginTestPlayer(gameContext);
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        return player;
    }

    private GameServerContext CreateGameServerContext(short maximumPlayers)
    {
        var gameConfiguration = new Persistence.BasicModel.GameConfiguration();
        gameConfiguration.Maps.Add(new Persistence.BasicModel.GameMapDefinition());
        var persistence = new Mock<IPersistenceContextProvider> { DefaultValue = DefaultValue.Mock };
        persistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(this._playerContext.Object);
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServer = new GameServerContext(
            new Persistence.BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new Persistence.BasicModel.GameServerConfiguration { MaximumPlayers = maximumPlayers },
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            this._loginServer.Object,
            new Mock<IFriendServer>().Object,
            persistence.Object,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServer.PlugInManager;
        mapInitializer.PathFinderPool = gameServer.PathFinderPool;
        return gameServer;
    }

    private sealed class LoginTestPlayer : Player
    {
        public LoginTestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }
}
