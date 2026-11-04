// <copyright file="LoginReservationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

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
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// OpenMU-BR: tests that a login reserves the account at the login server before it loads the
/// account, so that a quick reconnect can't load a copy which the old session is still saving over
/// (weavisel/Visel, 2026-10-15).
/// </summary>
[TestFixture]
public class LoginReservationTests
{
    private const string LoginName = "qa";

    private readonly List<string> _calls = new();

    private Mock<ILoginServer> _loginServer = null!;

    private Mock<IEventPublisher> _eventPublisher = null!;

    private Mock<IPlayerContext> _playerContext = null!;

    private Account? _account;

    /// <summary>
    /// Sets up the mocks of a fresh game server.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._calls.Clear();
        this._account = new Persistence.BasicModel.Account { LoginName = LoginName };
        this._loginServer = new Mock<ILoginServer>();
        this._eventPublisher = new Mock<IEventPublisher>();
        this._playerContext = new Mock<IPlayerContext> { DefaultValue = DefaultValue.Mock };
        this._playerContext
            .Setup(c => c.AuthenticateAsync(LoginName, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AccountState.Normal);
        this._playerContext
            .Setup(c => c.GetAccountByLoginNameAsync(LoginName, It.IsAny<CancellationToken>()))
            .Callback(() => this._calls.Add("load"))
            .ReturnsAsync(() => this._account);
    }

    /// <summary>
    /// The account is reserved first and loaded afterwards.
    /// </summary>
    [Test]
    public async ValueTask ReservesTheAccountBeforeLoadingItAsync()
    {
        this.SetupReservation(true);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, "pw").ConfigureAwait(false);

        Assert.That(this._calls, Is.EqualTo(new[] { "reserve", "load" }));
        Assert.That(player.Account, Is.SameAs(this._account));
        this.VerifyLoginResult(player, LoginResult.Ok);
        this._loginServer.Verify(l => l.LogOffAsync(It.IsAny<string>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// An account which is still reserved by another session is refused, and the reservation of
    /// the other session is left alone.
    /// </summary>
    [Test]
    public async ValueTask RefusesAnAccountWhichIsStillReservedAsync()
    {
        this.SetupReservation(false);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, "pw").ConfigureAwait(false);

        Assert.That(player.Account, Is.Not.SameAs(this._account));
        this.VerifyLoginResult(player, LoginResult.AccountAlreadyConnected);
        this._eventPublisher.Verify(e => e.PlayerAlreadyLoggedInAsync(It.IsAny<byte>(), LoginName), Times.Once);
        this._loginServer.Verify(l => l.LogOffAsync(It.IsAny<string>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// A template account may be logged in several times, so it neither needs nor keeps a reservation.
    /// </summary>
    /// <param name="reserved">Whether the login server granted the reservation.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask TemplateAccountDoesNotKeepTheReservationAsync(bool reserved)
    {
        this._account!.IsTemplate = true;
        this.SetupReservation(reserved);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, "pw").ConfigureAwait(false);

        Assert.That(player.Account, Is.SameAs(this._account));
        this.VerifyLoginResult(player, LoginResult.Ok);
        this._loginServer.Verify(l => l.LogOffAsync(LoginName, It.IsAny<byte>()), reserved ? Times.Once() : Times.Never());
    }

    /// <summary>
    /// When the account can't be loaded after it was reserved, the reservation is released again.
    /// </summary>
    [Test]
    public async ValueTask ReleasesTheReservationWhenTheAccountCanNotBeLoadedAsync()
    {
        this._account = null;
        this.SetupReservation(true);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, "pw").ConfigureAwait(false);

        this.VerifyLoginResult(player, LoginResult.ConnectionError);
        this._loginServer.Verify(l => l.LogOffAsync(LoginName, It.IsAny<byte>()), Times.Once);
    }

    /// <summary>
    /// A session which gave up its account doesn't save anymore.
    /// </summary>
    private void SetupReservation(bool result)
    {
        this._loginServer
            .Setup(l => l.TryLoginAsync(LoginName, It.IsAny<byte>()))
            .Callback(() => this._calls.Add("reserve"))
            .ReturnsAsync(result);
    }

    private void VerifyLoginResult(Player player, LoginResult result)
    {
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()!)
            .Verify(p => p.ShowLoginResultAsync(result), Times.Once);
    }

    private async ValueTask<Player> CreatePlayerAtLoginScreenAsync()
    {
        var gameConfiguration = new Persistence.BasicModel.GameConfiguration();
        gameConfiguration.Maps.Add(new Persistence.BasicModel.GameMapDefinition());
        var persistence = new Mock<IPersistenceContextProvider> { DefaultValue = DefaultValue.Mock };
        persistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(this._playerContext.Object);
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServer = new GameServerContext(
            new Persistence.BasicModel.GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new DataModel.Configuration.GameServerConfiguration() },
            new Mock<IGuildServer>().Object,
            this._eventPublisher.Object,
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

        var player = new LoginTestPlayer(gameServer);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        return player;
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
