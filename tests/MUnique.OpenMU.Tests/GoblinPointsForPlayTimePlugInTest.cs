// <copyright file="GoblinPointsForPlayTimePlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// Tests for the <see cref="GoblinPointsForPlayTimePlugIn"/>.
/// </summary>
[TestFixture]
public class GoblinPointsForPlayTimePlugInTest
{
    /// <summary>
    /// Tests that a player gets the points once per interval.
    /// With an interval of zero, the player gets them on the first check.
    /// </summary>
    [Test]
    public async Task PlayerGetsPointsForPlayTimeAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new GoblinPointsForPlayTimePlugIn { Configuration = new GoblinPointsForPlayTimeConfiguration { Interval = TimeSpan.Zero, Points = 5 } };

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        Assert.That(player.Account!.GoblinPoints, Is.EqualTo(5), "the second check is within the check interval");

        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        Assert.That(player.Account.GoblinPoints, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that a player doesn't get points before it played for the interval.
    /// </summary>
    [Test]
    public async Task NoPointsBeforeIntervalAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new GoblinPointsForPlayTimePlugIn { Configuration = new GoblinPointsForPlayTimeConfiguration { Interval = TimeSpan.FromHours(1), Points = 5 } };

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Assert.That(player.Account!.GoblinPoints, Is.Zero);
    }

    /// <summary>
    /// Tests that a player below the minimum level, or without the cash shop feature, doesn't get points.
    /// </summary>
    [Test]
    public async Task NoPointsForIneligiblePlayersAsync()
    {
        var lowLevelPlayer = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new GoblinPointsForPlayTimePlugIn { Configuration = new GoblinPointsForPlayTimeConfiguration { Interval = TimeSpan.Zero, Points = 5, MinimumLevel = 100 } };
        await plugIn.ExecuteTaskAsync((GameContext)lowLevelPlayer.GameContext).ConfigureAwait(false);
        Assert.That(lowLevelPlayer.Account!.GoblinPoints, Is.Zero);

        var playerWithoutCashShop = await CreatePlayerAsync(isCashShopActive: false).ConfigureAwait(false);
        var otherPlugIn = new GoblinPointsForPlayTimePlugIn { Configuration = new GoblinPointsForPlayTimeConfiguration { Interval = TimeSpan.Zero, Points = 5 } };
        await otherPlugIn.ExecuteTaskAsync((GameContext)playerWithoutCashShop.GameContext).ConfigureAwait(false);
        Assert.That(playerWithoutCashShop.Account!.GoblinPoints, Is.Zero);
    }

    private static async ValueTask<Player> CreatePlayerAsync(bool isCashShopActive = true)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        if (isCashShopActive)
        {
            player.GameContext.FeaturePlugIns.AddPlugIn(new CashShopFeaturePlugIn(), true);
        }

        player.Attributes![Stats.Level] = 10;
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await player.GameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }
}
