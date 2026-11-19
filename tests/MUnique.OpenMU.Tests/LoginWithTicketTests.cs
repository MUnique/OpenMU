// <copyright file="LoginWithTicketTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.LoginTickets;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.Login;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the login with a ticket, which resolves the account by the ticket and then takes the same path as the login with a password.
/// </summary>
[TestFixture]
public class LoginWithTicketTests
{
    private const string LoginName = "qa";

    private const string Token = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    private readonly Guid _accountId = Guid.NewGuid();

    private readonly List<GameServerContext> _contexts = new();

    private Mock<ILoginServer> _loginServer = null!;

    private Mock<IEventPublisher> _eventPublisher = null!;

    private Mock<IPlayerContext> _playerContext = null!;

    private Mock<ILoginTicketService> _tickets = null!;

    private Account _account = null!;

    /// <summary>
    /// Sets up the mocks of a fresh game server, with a valid ticket for the account.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._account = new Persistence.BasicModel.Account { LoginName = LoginName };
        this._loginServer = new Mock<ILoginServer>();
        this._loginServer.Setup(l => l.TryLoginAsync(LoginName, It.IsAny<byte>())).ReturnsAsync(true);
        this._eventPublisher = new Mock<IEventPublisher>();
        this._playerContext = new Mock<IPlayerContext> { DefaultValue = DefaultValue.Mock };
        this._playerContext
            .Setup(c => c.GetAccountByLoginNameAsync(LoginName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => this._account);
        this.SetupAccountState(AccountState.Normal);
        this._tickets = new Mock<ILoginTicketService>();
        this.SetupRedemption(LoginTicketRedeemStatus.Redeemed);
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
    /// A redeemed ticket logs into the account of the ticket.
    /// </summary>
    [Test]
    public async ValueTask ValidTicketLogsInAsync()
    {
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginWithTicketAsync(player, Token, this._tickets.Object).ConfigureAwait(false);

        Assert.That(player.Account, Is.SameAs(this._account));
        this.VerifyLoginResult(player, LoginResult.Ok);
    }

    /// <summary>
    /// A ticket which isn't redeemed is refused before the login server is asked, so a replay never logs in.
    /// </summary>
    /// <param name="status">The status of the redemption.</param>
    [TestCase(LoginTicketRedeemStatus.Unknown)]
    [TestCase(LoginTicketRedeemStatus.Expired)]
    [TestCase(LoginTicketRedeemStatus.AlreadyUsed)]
    public async ValueTask InvalidTicketIsRefusedAsync(LoginTicketRedeemStatus status)
    {
        this.SetupRedemption(status);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginWithTicketAsync(player, Token, this._tickets.Object).ConfigureAwait(false);

        Assert.That(player.Account, Is.Null);
        this.VerifyLoginResult(player, LoginResult.AccountInvalid);
        this._loginServer.Verify(l => l.TryLoginAsync(It.IsAny<string>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// A ticket of an account which was deleted in the meantime is refused.
    /// </summary>
    [Test]
    public async ValueTask TicketOfDeletedAccountIsRefusedAsync()
    {
        this._playerContext
            .Setup(c => c.GetAccountLoginStateAsync(this._accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((string, AccountState)?)null);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginWithTicketAsync(player, Token, this._tickets.Object).ConfigureAwait(false);

        this.VerifyLoginResult(player, LoginResult.AccountInvalid);
        this._loginServer.Verify(l => l.TryLoginAsync(It.IsAny<string>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// A banned account is refused like at the login with a password, although the ticket is valid.
    /// </summary>
    /// <param name="state">The state of the account.</param>
    /// <param name="expectedResult">The expected login result.</param>
    [TestCase(AccountState.Banned, LoginResult.AccountBlocked)]
    [TestCase(AccountState.TemporarilyBanned, LoginResult.TemporaryBlocked)]
    public async ValueTask BannedAccountIsRefusedAsync(AccountState state, LoginResult expectedResult)
    {
        this.SetupAccountState(state);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginWithTicketAsync(player, Token, this._tickets.Object).ConfigureAwait(false);

        Assert.That(player.Account, Is.Null);
        this.VerifyLoginResult(player, expectedResult);
        this._loginServer.Verify(l => l.TryLoginAsync(It.IsAny<string>(), It.IsAny<byte>()), Times.Never);
    }

    /// <summary>
    /// An account which is already connected is refused like at the login with a password.
    /// </summary>
    [Test]
    public async ValueTask AlreadyConnectedAccountIsRefusedAsync()
    {
        this._loginServer.Setup(l => l.TryLoginAsync(LoginName, It.IsAny<byte>())).ReturnsAsync(false);
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await new LoginAction().LoginWithTicketAsync(player, Token, this._tickets.Object).ConfigureAwait(false);

        Assert.That(player.Account, Is.Not.SameAs(this._account));
        this.VerifyLoginResult(player, LoginResult.AccountAlreadyConnected);
        this._eventPublisher.Verify(e => e.PlayerAlreadyLoggedInAsync(It.IsAny<byte>(), LoginName), Times.Once);
    }

    /// <summary>
    /// The packet handler redeems the ticket of the packet and logs in, end to end with the in-memory persistence.
    /// The same packet doesn't log in a second time.
    /// </summary>
    [Test]
    public async ValueTask HandlerLogsInWithIssuedTicketOnceAsync()
    {
        var persistence = new InMemoryPersistenceContextProvider();
        Guid accountId;
        using (var context = persistence.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = LoginName;
            await context.SaveChangesAsync().ConfigureAwait(false);
            accountId = account.GetId();
        }

        var ticket = await new LoginTicketService(() => persistence.CreateNewPlayerContext(new GameConfiguration())).IssueAsync(accountId).ConfigureAwait(false);
        var gameServer = LoginTestSetup.CreateGameServerContext(persistence, this._loginServer.Object, this._eventPublisher.Object);
        this._contexts.Add(gameServer);
        var handler = new LoginWithTicketHandlerPlugIn();

        var player = new LoginTestPlayer(gameServer);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await handler.HandlePacketAsync(player, CreatePacket(ticket!.Token)).ConfigureAwait(false);

        var replayingPlayer = new LoginTestPlayer(gameServer);
        await replayingPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await handler.HandlePacketAsync(replayingPlayer, CreatePacket(ticket.Token)).ConfigureAwait(false);

        Assert.That(player.Account?.LoginName, Is.EqualTo(LoginName));
        this.VerifyLoginResult(player, LoginResult.Ok);
        Assert.That(replayingPlayer.Account, Is.Null);
        this.VerifyLoginResult(replayingPlayer, LoginResult.AccountInvalid);
    }

    private static Memory<byte> CreatePacket(string token)
    {
        var packet = new byte[LoginWithTicket.Length];
        _ = new LoginWithTicket(packet) { Token = token };
        return packet;
    }

    private void SetupRedemption(LoginTicketRedeemStatus status)
    {
        this._tickets
            .Setup(t => t.RedeemAsync(Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginTicketRedemption(status, status == LoginTicketRedeemStatus.Redeemed ? this._accountId : Guid.Empty));
    }

    private void SetupAccountState(AccountState state)
    {
        this._playerContext
            .Setup(c => c.GetAccountLoginStateAsync(this._accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginName, state));
    }

    private void VerifyLoginResult(Player player, LoginResult result)
    {
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowLoginResultPlugIn>()!)
            .Verify(p => p.ShowLoginResultAsync(result), Times.Once);
    }

    private async ValueTask<Player> CreatePlayerAtLoginScreenAsync()
    {
        var gameServer = LoginTestSetup.CreateGameServerContext(LoginTestSetup.CreatePersistence(this._playerContext.Object), this._loginServer.Object, this._eventPublisher.Object);
        this._contexts.Add(gameServer);
        var player = new LoginTestPlayer(gameServer);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        return player;
    }
}
