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

    private readonly List<GameServerContext> _contexts = new();

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
    /// Disposes the game server contexts of the test, which stops their timers.
    /// </summary>
    /// <returns>The task.</returns>
    [TearDown]
    public async Task TearDownAsync()
    {
        foreach (var context in this._contexts)
        {
            await context.DisposeAsync().ConfigureAwait(false);
        }

        this._contexts.Clear();
    }

    /// <summary>
    /// Which states count as logged in: everything after the login screen, until the player disconnects.
    /// </summary>
    [Test]
    public void LoggedInStates()
    {
        var notLoggedIn = new[] { PlayerState.Initial, PlayerState.LoginScreen, PlayerState.Disconnected, PlayerState.Finished };
        var loggedIn = new[]
        {
            PlayerState.Authenticated, PlayerState.CharacterSelection, PlayerState.EnteredWorld, PlayerState.ChangingMap,
            PlayerState.Dead, PlayerState.NpcDialogOpened, PlayerState.PartyRequest, PlayerState.GuildRequest,
            PlayerState.TradeRequested, PlayerState.TradeOpened, PlayerState.TradeButtonPressed,
        };

        Assert.That(notLoggedIn.Where(s => s.IsLoggedIn()), Is.Empty);
        Assert.That(loggedIn.Where(s => !s.IsLoggedIn()), Is.Empty);
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
        this._playerContext.Verify(c => c.GetAccountByLoginNameAsync("b", It.IsAny<CancellationToken>()), Times.Never, "A full server refuses before the account is loaded.");
    }

    /// <summary>
    /// An account which is logged in somewhere else is still reported as such on a full server.
    /// </summary>
    [Test]
    public async ValueTask ReportsAnAccountWhichIsLoggedInElsewhereOnAFullServerAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var first = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var second = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await new LoginAction().LoginAsync(first, "a", Password).ConfigureAwait(false);
        this._loginServer.Setup(l => l.TryLoginAsync("b", It.IsAny<byte>())).ReturnsAsync(false);

        await new LoginAction().LoginAsync(second, "b", Password).ConfigureAwait(false);

        VerifyLoginResult(second, LoginResult.AccountAlreadyConnected);
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
    /// The place which another login in progress reserved counts like a logged in player, so two logins
    /// can't take the last place at the same time. When the reservation is released, the place is free.
    /// </summary>
    [Test]
    public async ValueTask ReservedPlaceOfALoginInProgressCountsAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);

        var otherLogin = gameContext.TryReserveLoginPlace(1, null, out _);
        Assert.That(otherLogin, Is.Not.Null);
        await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
        VerifyLoginResult(player, LoginResult.ServerIsFull);

        otherLogin!.Dispose();
        await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
        VerifyLoginResult(player, LoginResult.Ok);
    }

    /// <summary>
    /// A login releases its reservation when it's done, whether it succeeded or failed.
    /// </summary>
    [Test]
    public async ValueTask LoginReleasesItsReservedPlaceAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var first = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var second = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await new LoginAction().LoginAsync(first, "a", Password).ConfigureAwait(false);
        await new LoginAction().LoginAsync(second, "b", Password).ConfigureAwait(false);

        await first.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);

        using var reservation = gameContext.TryReserveLoginPlace(1, null, out var occupiedPlaces);
        Assert.That(reservation, Is.Not.Null);
        Assert.That(occupiedPlaces, Is.EqualTo(0));
    }

    /// <summary>
    /// A player which takes over the place of a logged in player, like the one which takes over its
    /// offline session, doesn't need a free place - but it does when that player isn't logged in anymore.
    /// </summary>
    [Test]
    public async ValueTask TakesOverThePlaceOfTheReplacedPlayerAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var replaced = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        var other = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await replaced.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);

        using (var takeover = gameContext.TryReserveLoginPlace(1, replaced, out _))
        {
            Assert.That(takeover, Is.Not.Null);
            Assert.That(gameContext.TryReserveLoginPlace(1, null, out _), Is.Null, "The place of the replaced player is reserved.");
        }

        // The replaced player left, and another player took the place in the meantime.
        await replaced.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);
        await other.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        Assert.That(gameContext.TryReserveLoginPlace(1, replaced, out _), Is.Null);
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
    /// A subscriber of the changed number of logged in players which throws doesn't break the
    /// disconnect of a player: it's still removed from the game.
    /// </summary>
    [Test]
    public async ValueTask FailingCountSubscriberDoesNotBreakTheDisconnectAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
        gameContext.LoggedInPlayerCountChanged += (_, _) => throw new InvalidOperationException("The subscriber failed.");

        await player.DisconnectAsync().ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
        Assert.That(gameContext.PlayerCount, Is.EqualTo(0));
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
    /// The game context counts a player before the other subscribers of its state change run, like the
    /// state changed plugins, so they already see the new number.
    /// </summary>
    [Test]
    public async ValueTask CountIsUpdatedBeforeOtherStateSubscribersRunAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = new LoginTestPlayer(gameContext);
        int? countSeenBySubscriber = null;
        player.PlayerState.StateChanged += args =>
        {
            if (args.CurrentStateState == PlayerState.Authenticated)
            {
                countSeenBySubscriber = gameContext.LoggedInPlayerCount;
            }

            return ValueTask.CompletedTask;
        };
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);

        Assert.That(countSeenBySubscriber, Is.EqualTo(1));
    }

    /// <summary>
    /// Only connections at the login screen wait for their login; logged in and disconnecting players don't.
    /// </summary>
    [Test]
    public async ValueTask WaitingForLoginCountsTheConnectionsAtTheLoginScreenAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 2);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        Assert.That(gameContext.WaitingForLoginCount, Is.EqualTo(1));

        await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
        Assert.That(gameContext.WaitingForLoginCount, Is.EqualTo(0));

        await player.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);
        Assert.That(gameContext.WaitingForLoginCount, Is.EqualTo(0));
        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(0));
        Assert.That(gameContext.PlayerCount, Is.EqualTo(1), "The player isn't removed yet.");

        await gameContext.RemovePlayerAsync(player).ConfigureAwait(false);
        await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        Assert.That(gameContext.WaitingForLoginCount, Is.EqualTo(1));
    }

    /// <summary>
    /// A player which was removed from the game isn't counted when its state changes afterwards.
    /// </summary>
    [Test]
    public async ValueTask RemovedPlayerIsNotCountedAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
        await gameContext.RemovePlayerAsync(player).ConfigureAwait(false);

        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);

        Assert.That(gameContext.LoggedInPlayerCount, Is.EqualTo(0));
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

    /// <summary>
    /// Connections which didn't log in yet are limited by the maximum players as well, while the
    /// logged in players don't count against this limit.
    /// </summary>
    [Test]
    public async ValueTask ListenerLimitsTheConnectionsWhichWaitForTheirLoginAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 1);
        var port = GetFreePort();
        var resolver = new Mock<IIpAddressResolver>();
        resolver.Setup(r => r.ResolveIPv4Async()).ReturnsAsync(IPAddress.Loopback);
        var endpoint = new Persistence.BasicModel.GameServerEndpoint
        {
            NetworkPort = port,
            Client = new Persistence.BasicModel.GameClientDefinition { Season = 6, Episode = 3, Language = ClientLanguage.English },
        };
        var listener = new DefaultTcpGameServerListener(endpoint, new ServerInfo(0, "test", 0, 1), gameContext, new Mock<IGameServerStateObserver>().Object, resolver.Object, new NullLoggerFactory());
        await listener.StartAsync().ConfigureAwait(false);
        try
        {
            var waiting = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
            Assert.That(await IsRefusedAsync(port).ConfigureAwait(false), Is.True, "One connection waits for its login already.");

            await new LoginAction().LoginAsync(waiting, "a", Password).ConfigureAwait(false);
            Assert.That(await IsRefusedAsync(port).ConfigureAwait(false), Is.False, "The logged in player doesn't count against this limit.");

            // It disconnects, and saves its progress before it's removed.
            await waiting.PlayerState.TryAdvanceToAsync(PlayerState.Disconnected).ConfigureAwait(false);
            Assert.That(await IsRefusedAsync(port).ConfigureAwait(false), Is.False, "A disconnecting player doesn't wait for its login.");
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>
    /// A start of the listener which failed, e.g. because the port is in use, doesn't leave its
    /// subscription behind: after the next start, every change is reported once.
    /// </summary>
    [Test]
    public async ValueTask ListenerReportsOnceAfterAFailedStartAsync()
    {
        var gameContext = this.CreateGameServerContext(maximumPlayers: 5);
        var reports = new List<int>();
        var observer = new Mock<IGameServerStateObserver>();
        observer
            .Setup(o => o.CurrentConnectionsChanged(It.IsAny<ushort>(), It.IsAny<int>()))
            .Callback((ushort _, int count) => reports.Add(count));
        var resolver = new Mock<IIpAddressResolver>();
        resolver.Setup(r => r.ResolveIPv4Async()).ReturnsAsync(IPAddress.Loopback);
        var port = GetFreePort();
        var endpoint = new Persistence.BasicModel.GameServerEndpoint
        {
            NetworkPort = port,
            Client = new Persistence.BasicModel.GameClientDefinition { Season = 6, Episode = 3, Language = ClientLanguage.English },
        };
        var listener = new DefaultTcpGameServerListener(endpoint, new ServerInfo(0, "test", 0, 5), gameContext, observer.Object, resolver.Object, new NullLoggerFactory());

        var portUser = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
        portUser.Start();
        try
        {
            Assert.CatchAsync(() => listener.StartAsync().AsTask(), "The port is in use.");
        }
        finally
        {
            portUser.Stop();
        }

        await listener.StartAsync().ConfigureAwait(false);
        try
        {
            var player = await this.CreatePlayerAtLoginScreenAsync(gameContext).ConfigureAwait(false);
            await new LoginAction().LoginAsync(player, "a", Password).ConfigureAwait(false);
        }
        finally
        {
            listener.Stop();
        }

        Assert.That(reports, Is.EqualTo(new[] { 0, 1 }));
    }

    private static int GetFreePort()
    {
        var probe = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }

    /// <summary>
    /// Connects to the listener and determines whether it closes the connection right away.
    /// </summary>
    private static async ValueTask<bool> IsRefusedAsync(int port)
    {
        using var client = new System.Net.Sockets.TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, port).ConfigureAwait(false);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        try
        {
            var read = await client.GetStream().ReadAsync(new byte[16], timeout.Token).ConfigureAwait(false);
            return read == 0;
        }
        catch (OperationCanceledException)
        {
            // Still connected; without a game server, nobody sends the greeting.
            return false;
        }
        catch (System.IO.IOException)
        {
            return true;
        }
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
        var context = LoginTestSetup.CreateGameServerContext(LoginTestSetup.CreatePersistence(this._playerContext.Object), this._loginServer.Object, maximumPlayers: maximumPlayers);
        this._contexts.Add(context);
        return context;
    }
}
