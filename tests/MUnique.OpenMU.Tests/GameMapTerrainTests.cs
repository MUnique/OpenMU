// <copyright file="GameMapTerrainTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

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
}
