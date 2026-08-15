// <copyright file="ClientReadyAfterMapChangeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Linq;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the handling of the client-ready packet which is sent after a map change.
/// </summary>
[TestFixture]
public class ClientReadyAfterMapChangeTests
{
    /// <summary>
    /// Tests that a repeated client-ready packet does not add the player to the
    /// area of interest a second time. The bucket does not deduplicate its entries,
    /// and its removal only strips the first occurrence, so a duplicate entry would
    /// outlive the player on the map.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task RepeatedClientReadyDoesNotAddPlayerTwiceAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        var map = player.CurrentMap;
        Assert.That(map, Is.Not.Null);
        var occurrencesAfterFirst = map!.GetAttackablesInRange(player.Position, 1).Count(o => o == player);
        Assert.That(occurrencesAfterFirst, Is.EqualTo(1));

        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        var occurrencesAfterSecond = map.GetAttackablesInRange(player.Position, 1).Count(o => o == player);
        Assert.That(occurrencesAfterSecond, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the player is fully removed from the area of interest after a
    /// repeated client-ready packet. A duplicate entry would leave a ghost behind,
    /// because the removal only takes out the first occurrence.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task PlayerLeavesNoGhostAfterRepeatedClientReadyAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        var map = player.CurrentMap;
        Assert.That(map, Is.Not.Null);
        var position = player.Position;
        await map!.RemoveAsync(player).ConfigureAwait(false);

        Assert.That(map.GetAttackablesInRange(position, 1).Any(o => o == player), Is.False);
    }
}
