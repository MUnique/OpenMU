// <copyright file="LoginTimeoutPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests for the <see cref="LoginTimeoutPlugIn"/>.
/// </summary>
[TestFixture]
public class LoginTimeoutPlugInTests
{
    private GameServerContext _gameContext = null!;

    /// <summary>
    /// Sets up a game server context.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var persistence = LoginTestSetup.CreatePersistence(new Mock<IPlayerContext> { DefaultValue = DefaultValue.Mock }.Object);
        this._gameContext = LoginTestSetup.CreateGameServerContext(persistence, new Mock<ILoginServer>().Object);
    }

    /// <summary>
    /// Disposes the game server context.
    /// </summary>
    /// <returns>The task.</returns>
    [TearDown]
    public async Task TearDownAsync()
    {
        await this._gameContext.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// A client which stays at the login screen longer than the timeout is disconnected.
    /// </summary>
    [Test]
    public async ValueTask DisconnectsAClientWhichDidNotLogInInTimeAsync()
    {
        var plugIn = new LoginTimeoutPlugIn { Configuration = new LoginTimeoutPlugInConfiguration { Timeout = TimeSpan.Zero } };
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await plugIn.ExecuteTaskAsync(this._gameContext).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
        Assert.That(this._gameContext.PlayerCount, Is.EqualTo(0));
    }

    /// <summary>
    /// A client which is at the login screen for a shorter time than the timeout stays connected.
    /// </summary>
    [Test]
    public async ValueTask KeepsAClientBeforeTheTimeoutAsync()
    {
        var plugIn = new LoginTimeoutPlugIn { Configuration = new LoginTimeoutPlugInConfiguration { Timeout = TimeSpan.FromHours(1) } };
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);

        await plugIn.ExecuteTaskAsync(this._gameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(this._gameContext).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.LoginScreen));
    }

    /// <summary>
    /// A logged in player isn't affected by the timeout.
    /// </summary>
    [Test]
    public async ValueTask KeepsALoggedInPlayerAsync()
    {
        var plugIn = new LoginTimeoutPlugIn { Configuration = new LoginTimeoutPlugInConfiguration { Timeout = TimeSpan.Zero } };
        var player = await this.CreatePlayerAtLoginScreenAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);

        await plugIn.ExecuteTaskAsync(this._gameContext).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
    }

    private async ValueTask<Player> CreatePlayerAtLoginScreenAsync()
    {
        var player = new LoginTestPlayer(this._gameContext);
        await this._gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        return player;
    }
}
