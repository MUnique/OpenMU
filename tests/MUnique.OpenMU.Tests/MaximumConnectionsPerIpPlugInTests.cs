// <copyright file="MaximumConnectionsPerIpPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;


using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unit tests for the <see cref="MaximumConnectionsPerIpPlugIn"/>.
/// </summary>
[TestFixture]
public class MaximumConnectionsPerIpPlugInTests
{
    private GameContext _gameContext = null!;
    private MaximumConnectionsPerIpPlugIn _plugin = null!;

    [SetUp]
    public async Task SetUp()
    {
        var dummyPlayer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        this._gameContext = (GameContext)dummyPlayer.GameContext;
        this._plugin = new MaximumConnectionsPerIpPlugIn
        {
            Configuration = new MaximumConnectionsPerIpPlugInConfiguration
            {
                MaximumConnectionsPerIp = 3
            }
        };
    }

    [Test]
    public async ValueTask UnderLimitSucceedsAsync()
    {
        // Arrange
        var player1 = new TestPlayer(this._gameContext);
        player1.SetIpAddress("127.0.0.1");
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = new TestPlayer(this._gameContext);
        player2.SetIpAddress("127.0.0.1");
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var joiningPlayer = new TestPlayer(this._gameContext);
        joiningPlayer.SetIpAddress("127.0.0.1");
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.False);
    }

    [Test]
    public async ValueTask AtLimitCancelsTransitionAsync()
    {
        // Arrange
        var player1 = new TestPlayer(this._gameContext);
        player1.SetIpAddress("127.0.0.1");
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = new TestPlayer(this._gameContext);
        player2.SetIpAddress("127.0.0.1");
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var player3 = new TestPlayer(this._gameContext);
        player3.SetIpAddress("127.0.0.1");
        await player3.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player3.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = new TestPlayer(this._gameContext);
        joiningPlayer.SetIpAddress("127.0.0.1");
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.True);
        Assert.That(joiningPlayer.LoginResultOverride, Is.EqualTo(GameLogic.Views.Login.LoginResult.ServerIsFull));
    }

    [Test]
    public async ValueTask DifferentIpDoesNotCountAsync()
    {
        // Arrange
        var player1 = new TestPlayer(this._gameContext);
        player1.SetIpAddress("127.0.0.1");
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = new TestPlayer(this._gameContext);
        player2.SetIpAddress("127.0.0.1");
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var player3 = new TestPlayer(this._gameContext);
        player3.SetIpAddress("127.0.0.1");
        await player3.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player3.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = new TestPlayer(this._gameContext);
        joiningPlayer.SetIpAddress(new System.Net.IPAddress(new byte[] { 192, 168, 1, 100 }).ToString());
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.False);
    }

    [Test]
    public async ValueTask LoginScreenAndInitialPlayersDoNotCountAsync()
    {
        // Arrange
        var player1 = new TestPlayer(this._gameContext);
        player1.SetIpAddress("127.0.0.1");
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player1.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = new TestPlayer(this._gameContext);
        player2.SetIpAddress("127.0.0.1");
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player2.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        // This player is only at the login screen
        var player3 = new TestPlayer(this._gameContext);
        player3.SetIpAddress("127.0.0.1");
        await player3.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = new TestPlayer(this._gameContext);
        joiningPlayer.SetIpAddress("127.0.0.1");
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.False);
    }

    private class TestPlayer : Player, IHasIpAddress
    {
        private string? _ipAddress;

        public TestPlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        /// <inheritdoc />
        public string? IpAddress => this._ipAddress;

        /// <summary>Sets the IP address for testing purposes.</summary>
        public void SetIpAddress(string? ip) => this._ipAddress = ip;

        protected override ICustomPlugInContainer<GameLogic.Views.IViewPlugIn> CreateViewPlugInContainer()
        {
            return new MockViewPlugInContainer();
        }
    }
}
