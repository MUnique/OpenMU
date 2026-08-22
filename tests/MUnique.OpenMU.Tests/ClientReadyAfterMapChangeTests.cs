// <copyright file="ClientReadyAfterMapChangeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Linq;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
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

    /// <summary>
    /// Tests that a bot which arrives on a blocked tile of a map whose safezone spawn gate is itself
    /// blocked is recovered without recursing. The recovery warps to the safezone, and for a
    /// connection-less player that warp calls back into the client-ready handler inline - so an
    /// unconditional recovery recursed until the stack overflowed and took the game server down.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task BlockedSpawnGateRecoversWithoutRecursingAsync()
    {
        const byte gateX1 = 10;
        const byte gateY1 = 10;
        const byte gateX2 = 13;
        const byte gateY2 = 13;

        var gameContext = GameContextTestHelper.CreateGameContext();
        var mapDefinition = gameContext.Configuration.Maps.First();
        BlockTerrain(mapDefinition.TerrainData!, gateX1, gateY1, gateX2, gateY2);
        mapDefinition.ExitGates.Add(new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = mapDefinition,
            X1 = gateX1,
            Y1 = gateY1,
            X2 = gateX2,
            Y2 = gateY2,
            IsSpawnGate = true,
        });

        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);

        // Warping into the blocked gate is what the bot does when it picks this map; the arrival
        // triggers the recovery, inline, on this very stack.
        await player.WarpToAsync(mapDefinition.ExitGates.First()).ConfigureAwait(false);

        Assert.That(player.CurrentMap, Is.Not.Null);
        var position = player.Position;
        Assert.That(player.CurrentMap!.Terrain.WalkMap[position.X, position.Y], Is.True, "the bot ends up on a tile it can stand on");
    }

    /// <summary>
    /// Makes the given rectangle of the terrain non-walkable.
    /// </summary>
    /// <param name="terrainData">The terrain data, including its three byte header.</param>
    /// <param name="x1">The left coordinate.</param>
    /// <param name="y1">The top coordinate.</param>
    /// <param name="x2">The right coordinate.</param>
    /// <param name="y2">The bottom coordinate.</param>
    private static void BlockTerrain(byte[] terrainData, byte x1, byte y1, byte x2, byte y2)
    {
        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                // Only the values 0 (walkable) and 1 (safezone) are walkable - see GameMapTerrain.
                terrainData[3 + (y * 256) + x] = 4;
            }
        }
    }
}
