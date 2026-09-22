// <copyright file="GameMapTerrainTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests for the terrain lookups which are used to get a player off a tile it cannot stand on.
/// </summary>
[TestFixture]
public class GameMapTerrainTests
{
    private const byte Walkable = 0;
    private const byte Safezone = 1;
    private const byte Blocked = 4;
    private const byte NoGround = (byte)TerrainAttributeType.NoGround;
    private const byte Water = (byte)TerrainAttributeType.Water;

    /// <summary>
    /// Tests that a map whose walkable tiles are all safezone still yields a coordinate.
    /// <see cref="GameMapTerrain.RandomWalkableCoordinate"/> samples the monster spawn points, which
    /// exclude the safezone by construction, so it returns nothing here - which would leave a player
    /// stranded on a blocked tile of a town.
    /// </summary>
    [Test]
    public void AnyWalkableCoordinateIsFoundOnASafezoneOnlyMap()
    {
        var terrain = new GameMapTerrain(CreateTerrainData(safezoneAt: (10, 10), walkableAt: null));

        Assert.That(terrain.RandomWalkableCoordinate, Is.Null, "precondition: the map has no monster spawn point");
        Assert.That(terrain.AnyWalkableCoordinate, Is.EqualTo(new Pathfinding.Point(10, 10)));
    }

    /// <summary>
    /// Tests that the safezone is preferred over an ordinary walkable tile, so a player who is
    /// recovered from a blocked spawn gate ends up in town rather than in a hunting ground.
    /// </summary>
    [Test]
    public void AnyWalkableCoordinatePrefersTheSafezone()
    {
        // The walkable tile comes first in scan order, so a naive "first walkable tile" would pick it.
        var terrain = new GameMapTerrain(CreateTerrainData(safezoneAt: (20, 20), walkableAt: (5, 5)));

        Assert.That(terrain.AnyWalkableCoordinate, Is.EqualTo(new Pathfinding.Point(20, 20)));
    }

    /// <summary>
    /// Tests that a map without a single walkable tile reports that honestly, so the caller can log
    /// it instead of moving the player somewhere impossible.
    /// </summary>
    [Test]
    public void AnyWalkableCoordinateIsNullWhenNothingIsWalkable()
    {
        var terrain = new GameMapTerrain(CreateTerrainData(safezoneAt: null, walkableAt: null));

        Assert.That(terrain.AnyWalkableCoordinate, Is.Null);
    }

    /// <summary>
    /// Creates fully blocked terrain data with at most one safezone and one ordinary walkable tile.
    /// </summary>
    /// <param name="safezoneAt">The coordinate to mark as safezone, if any.</param>
    /// <param name="walkableAt">The coordinate to mark as walkable but outside the safezone, if any.</param>
    /// <returns>The terrain data, including its three byte header.</returns>
    private static byte[] CreateTerrainData((byte X, byte Y)? safezoneAt, (byte X, byte Y)? walkableAt)
    {
        var data = new byte[ushort.MaxValue + 3];
        Array.Fill(data, Blocked, 3, ushort.MaxValue);

        if (safezoneAt is { } safezone)
        {
            data[3 + (safezone.Y * 256) + safezone.X] = Safezone;
        }

        if (walkableAt is { } walkable)
        {
            data[3 + (walkable.Y * 256) + walkable.X] = Walkable;
        }

        return data;
    }

    /// <summary>
    /// Tests that a zero-length line is always visible, so attackers and targets
    /// sharing a tile (e.g. pressure-plate traps) are never blocked by their own tile.
    /// </summary>
    [Test]
    public void HasLineOfSightSameTileIsTrue()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((5, 5)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(5, 5), new Pathfinding.Point(5, 5)), Is.True);
    }

    /// <summary>
    /// Tests that horizontal, vertical and diagonal lines across fully walkable
    /// terrain are visible, covering the three Bresenham step directions.
    /// </summary>
    [Test]
    public void HasLineOfSightClearLineIsTrue()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((99, 99)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(14, 10)), Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(10, 14)), Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(13, 13)), Is.True);
    }

    /// <summary>
    /// Tests that a wall between attacker and target blocks sight in both directions,
    /// while a line running parallel to the wall stays visible.
    /// </summary>
    [Test]
    public void HasLineOfSightWallBetweenIsFalse()
    {
        // Vertical wall at x=12, from y=9 to y=13.
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((12, 9), (12, 10), (12, 11), (12, 12), (12, 13)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.False);
        // Attacker and target swapped: sight is symmetric.
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(14, 11), new Pathfinding.Point(10, 11)), Is.False);
        // Parallel to the wall, no crossing: still visible.
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(10, 13)), Is.True);
    }

    /// <summary>
    /// Tests that the endpoints themselves never block sight, so a monster or player
    /// standing on (or targeting) an impassable tile is still handled instead of
    /// being stuck in a permanently invisible state.
    /// </summary>
    [Test]
    public void HasLineOfSightEndpointsNeverBlock()
    {
        // The wall tile itself is an endpoint: standing on it (or targeting it) never blocks.
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((12, 11)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(12, 11), new Pathfinding.Point(14, 11)), Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(12, 11)), Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(11, 11), new Pathfinding.Point(12, 11)), Is.True);
    }

    /// <summary>
    /// Tests that a diagonal line squeezing exactly between two blocked tiles that
    /// touch only at a corner is blocked, so sight can't leak through a solid
    /// diagonal wall corner. Both directions are asserted because sight is symmetric.
    /// </summary>
    [Test]
    public void HasLineOfSightDiagonalCornerBetweenTwoWallsIsFalse()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((11, 12, Blocked), (12, 11, Blocked)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(12, 12)), Is.False);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(12, 12), new Pathfinding.Point(10, 10)), Is.False);
    }

    /// <summary>
    /// Tests that a diagonal line passing a corner where only one of the two
    /// adjacent tiles is blocked stays visible; only a fully closed corner blocks.
    /// </summary>
    [Test]
    public void HasLineOfSightDiagonalCornerWithSingleWallIsTrue()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((10, 11)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(12, 12)), Is.True);
    }

    /// <summary>
    /// Tests that diagonally adjacent tiles are always visible, even when both
    /// orthogonal neighbours are blocked, so melee-range targets are never
    /// considered behind a wall.
    /// </summary>
    [Test]
    public void HasLineOfSightDiagonallyAdjacentTilesAreTrue()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((10, 11), (11, 10)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(11, 11)), Is.True);
    }

    /// <summary>
    /// Tests that a hole (<c>NoGround</c>) between attacker and target does not
    /// block sight, even though the hole itself can't be walked on. Ranged
    /// attacks are supposed to fly across pits, e.g. in Dungeon or Chaos Castle.
    /// </summary>
    [Test]
    public void HasLineOfSightHoleDoesNotBlock()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, NoGround)));

        Assert.That(terrain.WalkMap[12, 11], Is.False, "precondition: the hole is not walkable");
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that water between attacker and target does not block sight,
    /// even though it can't be walked on.
    /// </summary>
    [Test]
    public void HasLineOfSightWaterDoesNotBlock()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, Water)));

        Assert.That(terrain.WalkMap[12, 11], Is.False, "precondition: water is not walkable");
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that a tile combining the wall and hole flags still blocks sight,
    /// because the wall bit is what matters for projectiles.
    /// </summary>
    [Test]
    public void HasLineOfSightBlockedHoleCombinationBlocks()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, (byte)(Blocked | NoGround))));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.False);
    }

    /// <summary>
    /// Tests that a diagonal line passing between two holes is visible: the
    /// closed-corner rule only applies to actual walls.
    /// </summary>
    [Test]
    public void HasLineOfSightDiagonalCornerBetweenHolesIsTrue()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((10, 11, NoGround), (11, 10, NoGround)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(12, 12)), Is.True);
    }

    /// <summary>
    /// Tests that setting the wall attribute at runtime makes a tile unwalkable
    /// and sight-blocking, e.g. when a castle gate closes.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeBlockedBlocksSightAndMovement()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((99, 99)));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Blocked, true);

        Assert.That(terrain.WalkMap[12, 11], Is.False);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.False);

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Blocked, false);

        Assert.That(terrain.WalkMap[12, 11], Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that removing an attribute only clears its own bit: other blocking
    /// bits on the tile survive, e.g. when a castle gate reopens over an
    /// original hole or water tile.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeRemoveKeepsOtherBits()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, (byte)(Blocked | NoGround))));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Blocked, false);

        Assert.That(terrain.WalkMap[12, 11], Is.False);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that removing an attribute with openArea clears every blocking bit,
    /// so mixed wall/hole rects (e.g. the Kanturu barrier) become passable.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeRemoveWithOpenAreaClearsAllBits()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, (byte)(Blocked | NoGround | Water))));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.NoGround, false, openArea: true);

        Assert.That(terrain.WalkMap[12, 11], Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that setting the hole attribute at runtime (e.g. collapsing Chaos
    /// Castle ground) makes a tile unwalkable while sight still passes across it.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeNoGroundBlocksMovementButNotSight()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((99, 99)));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.NoGround, true);

        Assert.That(terrain.WalkMap[12, 11], Is.False);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that the safezone attribute can be toggled at runtime without
    /// affecting walkability, as before.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeSafezoneKeepsWalkability()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithWall((99, 99)));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Safezone, true);

        Assert.That(terrain.WalkMap[12, 11], Is.True);
        Assert.That(terrain.SafezoneMap[12, 11], Is.True);

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Safezone, false);

        Assert.That(terrain.SafezoneMap[12, 11], Is.False);
    }

    /// <summary>
    /// Tests that setting a wall on a safezone tile blocks movement and sight
    /// without dropping the safezone status, and that removing it restores
    /// walkability while the safezone status is untouched throughout.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeBlockedOnSafezoneKeepsSafezone()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, Safezone)));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Blocked, true);

        Assert.That(terrain.WalkMap[12, 11], Is.False);
        Assert.That(terrain.SafezoneMap[12, 11], Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.False);

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.Blocked, false);

        Assert.That(terrain.WalkMap[12, 11], Is.True);
        Assert.That(terrain.SafezoneMap[12, 11], Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that removing an attribute only clears its own bit: a tile holding
    /// other bits stays as it was. Callers which restore exact previous state
    /// (e.g. the castle siege gate) rely on this; area opening is opt-in.
    /// </summary>
    [Test]
    public void ApplyTerrainAttributeRemoveKeepsOwnBitOnly(
        [Values(Walkable, NoGround)] byte rawValue)
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((12, 11, rawValue)));

        terrain.ApplyTerrainAttribute(12, 11, TerrainAttributeType.NoGround, false);

        Assert.That(terrain.WalkMap[12, 11], Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 11), new Pathfinding.Point(14, 11)), Is.True);
    }

    /// <summary>
    /// Tests that sight doesn't depend on the viewing direction when the line
    /// passes exactly through a lattice corner: Bresenham's tie-breaking would
    /// otherwise walk different tiles from each end. Both directions are asserted.
    /// </summary>
    [Test]
    public void HasLineOfSightTieBreakingIsSymmetric()
    {
        var terrain = new GameMapTerrain(CreateWalkableTerrainWithValues((10, 11, Blocked)));

        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(10, 10), new Pathfinding.Point(11, 12)), Is.True);
        Assert.That(terrain.HasLineOfSight(new Pathfinding.Point(11, 12), new Pathfinding.Point(10, 10)), Is.True);
    }

    /// <summary>
    /// Creates fully walkable terrain data with the specified coordinates blocked (wall).
    /// </summary>
    /// <param name="blocked">The coordinates to mark as blocked.</param>
    /// <returns>The terrain data, including its three byte header.</returns>
    private static byte[] CreateWalkableTerrainWithWall(params (byte X, byte Y)[] blocked)
    {
        var data = new byte[ushort.MaxValue + 3];
        Array.Fill(data, Walkable, 3, ushort.MaxValue);

        foreach (var (x, y) in blocked)
        {
            data[3 + (y * 256) + x] = Blocked;
        }

        return data;
    }

    /// <summary>
    /// Creates fully walkable terrain data with the specified coordinates set to
    /// the given raw attribute values (a <c>TerrainAttributeType</c> bitmask).
    /// </summary>
    /// <param name="tiles">The coordinates and their raw attribute values.</param>
    /// <returns>The terrain data, including its three byte header.</returns>
    private static byte[] CreateWalkableTerrainWithValues(params (byte X, byte Y, byte Value)[] tiles)
    {
        var data = new byte[ushort.MaxValue + 3];
        Array.Fill(data, Walkable, 3, ushort.MaxValue);

        foreach (var (x, y, value) in tiles)
        {
            data[3 + (y * 256) + x] = value;
        }

        return data;
    }
}
