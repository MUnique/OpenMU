// <copyright file="LoginServerRegistrationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the registration of a player at the login server is released, so that the account
/// isn't registered as connected after the session ended.
/// </summary>
[TestFixture]
public class LoginServerRegistrationTest
{
    private const string LoginName = "owner";
    private const string Password = "password";

    /// <summary>
    /// Tests that a player which disconnects after the login server accepted its login, but before
    /// the account was assigned, still has its registration, so that it's logged off.
    /// Previously, the account was logged off by the assigned account, so it stayed registered as
    /// connected and every further login was rejected.
    /// </summary>
    [Test]
    public async ValueTask DisconnectDuringLoginReleasesTheRegistrationAsync()
    {
        Player? player = null;
        Task? disconnect = null;
        var loginServer = new Mock<ILoginServer>();
        loginServer
            .Setup(server => server.TryLoginAsync(LoginName, It.IsAny<byte>()))
            .Returns(() =>
            {
                // The connection is lost while the login server accepts the login.
                disconnect = Task.Run(() => player!.DisconnectAsync().AsTask());
                return Task.FromResult(true);
            });
        var gameContext = await CreateGameServerContextAsync(loginServer.Object).ConfigureAwait(false);
        player = new TestPlayer(gameContext);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        string? releasedAtDisconnect = null;
        player.PlayerDisconnected += disconnectedPlayer =>
        {
            releasedAtDisconnect = disconnectedPlayer.ReleaseLoginServerRegistration();
            return ValueTask.CompletedTask;
        };

        await new LoginAction().LoginAsync(player, LoginName, Password).ConfigureAwait(false);
        await disconnect!.ConfigureAwait(false);

        Assert.That(releasedAtDisconnect, Is.EqualTo(LoginName));
        Assert.That(player.ReleaseLoginServerRegistration(), Is.Null);
    }

    /// <summary>
    /// Tests that the registration is released by a failed login, so that it isn't logged off twice.
    /// </summary>
    [Test]
    public async ValueTask FailedLoginLogsOffOnceAsync()
    {
        var loginServer = new Mock<ILoginServer>();
        loginServer.Setup(server => server.TryLoginAsync(LoginName, It.IsAny<byte>())).ReturnsAsync(true);
        var gameContext = await CreateGameServerContextAsync(loginServer.Object).ConfigureAwait(false);
        var player = new FailingLoginPlayer(gameContext);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        await new LoginAction().LoginAsync(player, LoginName, Password).ConfigureAwait(false);

        loginServer.Verify(server => server.LogOffAsync(LoginName, It.IsAny<byte>()), Times.Once);
        Assert.That(player.ReleaseLoginServerRegistration(), Is.Null);
    }

    private static async ValueTask<GameServerContext> CreateGameServerContextAsync(ILoginServer loginServer)
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        using (var context = persistenceContextProvider.CreateNewContext())
        {
            var account = context.CreateNew<Account>();
            account.LoginName = LoginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var gameConfiguration = new BasicModel.GameConfiguration();
        var map = new BasicModel.GameMapDefinition { TerrainData = new byte[ushort.MaxValue + 3] };
        gameConfiguration.Maps.Add(map);
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            loginServer,
            new Mock<IFriendServer>().Object,
            persistenceContextProvider,
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;
        return gameServerContext;
    }

    private class TestPlayer : Player
    {
        public TestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ICustomPlugInContainer<GameLogic.Views.IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }

    /// <summary>
    /// A player whose login fails after the login server accepted it, because assigning the account fails.
    /// </summary>
    private sealed class FailingLoginPlayer : TestPlayer
    {
        public FailingLoginPlayer(IGameContext gameContext)
            : base(gameContext)
        {
            this.PlayerLoggedIn += _ => throw new InvalidOperationException("Login failed.");
        }
    }
}
