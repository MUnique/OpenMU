// <copyright file="MaximumConnectionsPerIpPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Generic;
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
        var player1 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player1.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player2.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var joiningPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        joiningPlayer.IpAddress = "127.0.0.1";
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
        var player1 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player1.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player2.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var player3 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player3.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        joiningPlayer.IpAddress = "127.0.0.1";
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.True);
    }

    [Test]
    public async ValueTask DifferentIpDoesNotCountAsync()
    {
        // Arrange
        var player1 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player1.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player2.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        var player3 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player3.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        joiningPlayer.IpAddress = "192.168.1.100";
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
        var player1 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player1.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player1).ConfigureAwait(false);

        var player2 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player2.IpAddress = "127.0.0.1";
        await this._gameContext.AddPlayerAsync(player2).ConfigureAwait(false);

        // This player is only at the login screen
        var player3 = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        player3.IpAddress = "127.0.0.1";
        typeof(StateMachine).GetProperty("CurrentState")!.SetValue(player3.PlayerState, PlayerState.LoginScreen);
        await this._gameContext.AddPlayerAsync(player3).ConfigureAwait(false);

        var joiningPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        joiningPlayer.IpAddress = "127.0.0.1";
        await joiningPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);

        var eventArgs = new StateMachine.StateChangeEventArgs { NextState = PlayerState.Authenticated };

        // Act
        await this._plugin.PlayerStateChangingAsync(joiningPlayer, eventArgs).ConfigureAwait(false);

        // Assert
        Assert.That(eventArgs.Cancel, Is.False);
    }
}
