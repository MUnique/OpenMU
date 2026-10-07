// <copyright file="LostConnectionDetectionPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Tests for the <see cref="LostConnectionDetectionPlugIn"/> and the <see cref="PingHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class LostConnectionDetectionPlugInTest
{
    /// <summary>
    /// Tests that the ping of the client reports that it's alive.
    /// </summary>
    [Test]
    public async ValueTask PingReportsTheClientAliveAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Assert.That(player.LastAliveReport, Is.Null);

        await new PingHandlerPlugIn().HandlePacketAsync(player, new byte[Ping.Length]).ConfigureAwait(false);

        Assert.That(player.LastAliveReport, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
    }

    /// <summary>
    /// Tests that only a player whose client stopped to report that it's alive is disconnected.
    /// A player whose client never reported it, e.g. because it doesn't support it, stays connected.
    /// </summary>
    [Test]
    public async ValueTask PlayerWhichStoppedReportingIsDisconnectedAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var lostPlayer = await CreateConnectedPlayerAsync(gameContext).ConfigureAwait(false);
        var alivePlayer = await CreateConnectedPlayerAsync(gameContext).ConfigureAwait(false);
        var silentPlayer = await CreateConnectedPlayerAsync(gameContext).ConfigureAwait(false);
        var plugIn = new LostConnectionDetectionPlugIn
        {
            Configuration = new LostConnectionDetectionConfiguration { Timeout = TimeSpan.FromMilliseconds(100) },
        };

        lostPlayer.ReportAlive();
        await Task.Delay(300).ConfigureAwait(false);
        alivePlayer.ReportAlive();
        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);

        var remainingPlayers = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        Assert.That(remainingPlayers, Does.Not.Contain(lostPlayer));
        Assert.That(remainingPlayers, Does.Contain(alivePlayer));
        Assert.That(remainingPlayers, Does.Contain(silentPlayer));
    }

    /// <summary>
    /// Tests that a timeout of zero disables the detection.
    /// </summary>
    [Test]
    public async ValueTask ZeroTimeoutDisablesTheDetectionAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var player = await CreateConnectedPlayerAsync(gameContext).ConfigureAwait(false);
        var plugIn = new LostConnectionDetectionPlugIn
        {
            Configuration = new LostConnectionDetectionConfiguration { Timeout = TimeSpan.Zero },
        };

        player.ReportAlive();
        await Task.Delay(100).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);

        Assert.That(await gameContext.GetPlayersAsync().ConfigureAwait(false), Does.Contain(player));
    }

    private static async ValueTask<Player> CreateConnectedPlayerAsync(IGameContext gameContext)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }
}
