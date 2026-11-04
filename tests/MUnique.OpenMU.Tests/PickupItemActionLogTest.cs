// <copyright file="PickupItemActionLogTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Tests the logging of refused pick up requests of <see cref="PickupItemAction"/>.
/// </summary>
[TestFixture]
internal class PickupItemActionLogTest
{
    /// <summary>
    /// Tests that repeated refusals of the same player are not all logged as information,
    /// so a client can't flood the log with invalid pick up requests.
    /// </summary>
    [Test]
    public async ValueTask RepeatedRefusalsAreLoggedForDebuggingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        var levels = Enumerable.Range(0, 1000).Select(_ => PickupItemAction.GetLogLevel(player)).ToList();

        Assert.That(levels.Count(l => l == LogLevel.Information), Is.EqualTo(1));
        Assert.That(levels.Count(l => l == LogLevel.Debug), Is.EqualTo(999));
    }

    /// <summary>
    /// Tests that the first refusal of each player is logged as information.
    /// </summary>
    [Test]
    public async ValueTask FirstRefusalOfEachPlayerIsLoggedAsInformationAsync()
    {
        var player1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var player2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        Assert.That(PickupItemAction.GetLogLevel(player1), Is.EqualTo(LogLevel.Information));
        Assert.That(PickupItemAction.GetLogLevel(player2), Is.EqualTo(LogLevel.Information));
        Assert.That(PickupItemAction.GetLogLevel(player1), Is.EqualTo(LogLevel.Debug));
    }
}
