// <copyright file="ConnectedAccountLoginAttemptActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;

/// <summary>
/// Tests for the <see cref="ConnectedAccountLoginAttemptAction"/>.
/// </summary>
[TestFixture]
public class ConnectedAccountLoginAttemptActionTest
{
    /// <summary>
    /// Tests that the connected session of the account is disconnected, so that the next login attempt succeeds.
    /// Previously, it was just warned, and a stale session blocked the login of the account.
    /// </summary>
    [Test]
    public async ValueTask ConnectedSessionOfTheAccountIsDisconnectedAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var connectedPlayer = await this.CreateConnectedPlayerAsync(gameContext, "owner").ConfigureAwait(false);
        var otherPlayer = await this.CreateConnectedPlayerAsync(gameContext, "other").ConfigureAwait(false);

        await new ConnectedAccountLoginAttemptAction().HandleAsync(gameContext, "owner").ConfigureAwait(false);

        var remainingPlayers = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        Assert.That(remainingPlayers, Does.Not.Contain(connectedPlayer));
        Assert.That(remainingPlayers, Does.Contain(otherPlayer));
    }

    private async ValueTask<Player> CreateConnectedPlayerAsync(IGameContext gameContext, string loginName)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.Account!.LoginName = loginName;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }
}
