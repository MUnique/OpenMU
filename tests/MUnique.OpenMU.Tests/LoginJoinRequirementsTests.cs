// <copyright file="LoginJoinRequirementsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
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
/// Tests the <see cref="GameServerDefinition.JoinRequirements"/> at the login.
/// </summary>
[TestFixture]
public class LoginJoinRequirementsTests
{
    private const string LoginName = "vip";

    private const int RequiredValue = 1;

    private static readonly AttributeDefinition Vip = new(Guid.NewGuid(), "VIP", string.Empty);

    private Mock<ILoginServer> _loginServer = null!;

    private Mock<IPlayerContext> _playerContext = null!;

    private Account _account = null!;

    private bool _hasRequirement;

    /// <summary>
    /// Sets up the mocks of a fresh game server which requires the VIP attribute.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._hasRequirement = true;
        this._account = new Persistence.BasicModel.Account { LoginName = LoginName };
        this._loginServer = new Mock<ILoginServer>();
        this._loginServer.Setup(l => l.TryLoginAsync(LoginName, It.IsAny<byte>())).ReturnsAsync(true);
        this._playerContext = new Mock<IPlayerContext> { DefaultValue = DefaultValue.Mock };
        this._playerContext
            .Setup(c => c.AuthenticateAsync(LoginName, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AccountState.Normal);
        this._playerContext
            .Setup(c => c.GetAccountByLoginNameAsync(LoginName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => this._account);
    }

    /// <summary>
    /// Every account can join a game server without requirements.
    /// </summary>
    [Test]
    public async ValueTask ServerWithoutRequirementsAcceptsEveryAccountAsync()
    {
        this._hasRequirement = false;

        await this.LoginAndVerifyAsync(LoginResult.Ok).ConfigureAwait(false);
    }

    /// <summary>
    /// A valid grant of the attribute fulfills the requirement.
    /// </summary>
    [Test]
    public async ValueTask ValidGrantFulfillsRequirementAsync()
    {
        this.AddGrant(DateTime.UtcNow.AddDays(1));

        await this.LoginAndVerifyAsync(LoginResult.Ok).ConfigureAwait(false);
    }

    /// <summary>
    /// A permanent attribute of the account fulfills the requirement.
    /// </summary>
    [Test]
    public async ValueTask PermanentAccountAttributeFulfillsRequirementAsync()
    {
        this._account.Attributes.Add(new Persistence.BasicModel.StatAttribute(Vip, RequiredValue));

        await this.LoginAndVerifyAsync(LoginResult.Ok).ConfigureAwait(false);
    }

    /// <summary>
    /// An account whose grant expired is refused, and its reservation at the login server is released.
    /// </summary>
    [Test]
    public async ValueTask ExpiredGrantIsRefusedAsync()
    {
        this.AddGrant(DateTime.UtcNow.AddDays(-1));

        var player = await this.LoginAndVerifyAsync(LoginResult.SubscriptionTermOver).ConfigureAwait(false);

        Assert.That(player.Account, Is.Not.SameAs(this._account));
        this._loginServer.Verify(l => l.LogOffAsync(LoginName, It.IsAny<byte>()), Times.Once);
    }

    /// <summary>
    /// An account which never had the attribute is refused.
    /// </summary>
    [Test]
    public async ValueTask AccountWithoutAttributeIsRefusedAsync()
    {
        var player = await this.LoginAndVerifyAsync(LoginResult.NoChargeInfo).ConfigureAwait(false);

        Assert.That(player.Account, Is.Not.SameAs(this._account));
        this._loginServer.Verify(l => l.LogOffAsync(LoginName, It.IsAny<byte>()), Times.Once);
    }

    /// <summary>
    /// Game masters can join every game server.
    /// </summary>
    [Test]
    public async ValueTask GameMasterIgnoresRequirementsAsync()
    {
        this._account.State = AccountState.GameMaster;

        await this.LoginAndVerifyAsync(LoginResult.Ok).ConfigureAwait(false);
    }

    private void AddGrant(DateTime validUntil)
    {
        this._account.AttributeGrants.Add(new Persistence.BasicModel.AccountAttributeGrant { Attribute = Vip, Value = RequiredValue, ValidUntil = validUntil });
    }

    private async ValueTask<Player> LoginAndVerifyAsync(LoginResult expectedResult)
    {
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, "pw").ConfigureAwait(false);

        this.VerifyLoginResult(player, expectedResult);
        return player;
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
        var serverDefinition = new Persistence.BasicModel.GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new DataModel.Configuration.GameServerConfiguration() };
        if (this._hasRequirement)
        {
            serverDefinition.JoinRequirements.Add(new Persistence.BasicModel.AttributeRequirement { Attribute = Vip, MinimumValue = RequiredValue });
        }

        var gameServer = new GameServerContext(
            serverDefinition,
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
