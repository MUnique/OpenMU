// <copyright file="GameMapTerrain.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The terrain of a map.
/// </summary>
public class GameMapTerrain
{
    /// <summary>
    /// The size of the map in each dimension (byte range: 0–255).
    /// </summary>
    private const int MapSize = 256;

    /// <summary>
    /// The default terrain where all coordinates are walkable and not a safezone.
    /// </summary>
    private static readonly byte[] DefaultTerrain = Enumerable.Repeat<byte>(0, short.MaxValue).ToArray();

    /// <summary>
    /// Pre-computed array of walkable, non-safezone points.
    /// Built once during construction for O(1) random spawn lookups.
    /// </summary>
    private readonly Point[] _spawnPoints;

    private readonly Point? _anyWalkableCoordinate;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapTerrain"/> class.
    /// </summary>
    /// <param name="definition">The game map definition.</param>
    public GameMapTerrain(GameMapDefinition definition)
        : this(definition?.TerrainData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapTerrain"/> class.
    /// </summary>
    /// <param name="terrainData">The terrain data.</param>
    public GameMapTerrain(byte[]? terrainData)
    {
        if (terrainData is { })
        {
            this.ReadTerrainData(terrainData.AsSpan(3));
        }
        else
        {
            this.ReadTerrainData(DefaultTerrain);
        }

        this._spawnPoints = this.BuildSpawnPoints(out this._anyWalkableCoordinate);
    }

    /// <summary>
    /// Gets a grid of all safezone coordinates.
    /// </summary>
    public bool[,] SafezoneMap { get; } = new bool[MapSize, MapSize];

    /// <summary>
    /// Gets a grid of all walkable coordinates.
    /// </summary>
    public bool[,] WalkMap { get; } = new bool[MapSize, MapSize];

    /// <summary>
    /// Gets a grid of the walkable coordinates of monsters.
    /// </summary>
    public byte[,] AIgrid { get; } = new byte[MapSize, MapSize];

    /// <summary>
    /// Gets a random walkable, non-safezone point anywhere on the map.
    /// Samples from a pre-computed array in O(1) per call.
    /// </summary>
    public Point? RandomWalkableCoordinate
    {
        get
        {
            var points = this._spawnPoints;
            if (points.Length == 0)
            {
                return null;
            }

            return points[Random.Shared.Next(points.Length)];
        }
    }

    /// <summary>
    /// Gets a walkable coordinate anywhere on the map, preferring a safezone tile. Used to get a
    /// player off a blocked tile when its spawn gate has none - unlike
    /// <see cref="RandomWalkableCoordinate"/>, which samples the monster spawn points and therefore
    /// deliberately excludes every safezone tile, this is allowed to land in a town.
    /// </summary>
    /// <value>
    /// A safezone coordinate; the first walkable one if the map has no safezone at all; or
    /// <c>null</c> if the map has no walkable tile whatsoever.
    /// </value>
    public Point? AnyWalkableCoordinate => this._anyWalkableCoordinate;

    /// <summary>
    /// Gets the first walkable coordinate within the specified gate, if it has one. A gate without a
    /// single walkable tile strands whoever is placed there, because a player is placed at a gate by
    /// one random roll which is never retried - so callers use this to check a gate up front, or to
    /// get out of one afterwards.
    /// </summary>
    /// <param name="gate">The gate to search. Its coordinates are bytes, so this grid can't be overrun.</param>
    /// <returns>The first walkable coordinate of the gate, or <c>null</c> if it has none.</returns>
    public Point? GetWalkableCoordinate(Gate? gate)
    {
        if (gate is null)
        {
            return null;
        }

        // The counters are ints on purpose: a gate reaching to coordinate 255 would make a byte
        // counter wrap around and loop forever.
        for (int x = gate.X1; x <= gate.X2; x++)
        {
            for (int y = gate.Y1; y <= gate.Y2; y++)
            {
                if (this.WalkMap[x, y])
                {
                    return new Point((byte)x, (byte)y);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a random drop coordinate at the specified point in the specified radius.
    /// </summary>
    /// <param name="point">The target point.</param>
    /// <param name="maximumRadius">The maximum radius around the specified coordinate.</param>
    /// <returns>The random drop coordinate.</returns>
    public Point GetRandomCoordinate(Point point, byte maximumRadius)
    {
        byte tempx = (byte)Rand.NextInt(Math.Max(0, point.X - maximumRadius), Math.Min(255, point.X + maximumRadius + 1));
        byte tempy = (byte)Rand.NextInt(Math.Max(0, point.Y - maximumRadius), Math.Min(255, point.Y + maximumRadius + 1));
        int i = 0;
        while (!this.WalkMap[tempx, tempy] && i < 20)
        {
            tempx = (byte)Rand.NextInt(Math.Max(0, point.X - maximumRadius), Math.Min(255, point.X + maximumRadius + 1));
            tempy = (byte)Rand.NextInt(Math.Max(0, point.Y - maximumRadius), Math.Min(255, point.Y + maximumRadius + 1));
            i++;
        }

        if (i == 20)
        {
            return point;
        }

        return new Point(tempx, tempy);
    }

    /// <summary>
    /// Updates the ai grid value at the specified coordinate.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateAiGridValue(byte x, byte y)
    {
        this.AIgrid[x, y] = (byte)((this.WalkMap[x, y] ? 1 : 0) | (this.SafezoneMap[x, y] ? 0b1000_0000 : 0));
    }

    /// <summary>
    /// Reads the terrain data from a stream.
    /// </summary>
    /// <param name="data">The data.</param>
    private void ReadTerrainData(ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            byte x = (byte)(i & 0xFF);
            byte y = (byte)((i >> 8) & 0xFF);
            byte value = data[i];
            this.WalkMap[x, y] = value == 0 || value == 1;
            this.SafezoneMap[x, y] = value == 1;
            this.UpdateAiGridValue(x, y);
        }
    }

    /// <summary>
    /// Scans the terrain for the two lookups which are derived from it, in a single pass: the
    /// monster spawn points, and the coordinate behind <see cref="AnyWalkableCoordinate"/>.
    /// </summary>
    /// <param name="anyWalkableCoordinate">A safezone coordinate; the first walkable one if the map
    /// has no safezone at all; or <c>null</c> if nothing on the map is walkable.</param>
    /// <returns>The walkable coordinates outside the safezone, where monsters may spawn.</returns>
    private Point[] BuildSpawnPoints(out Point? anyWalkableCoordinate)
    {
        var result = new List<Point>(MapSize * MapSize);
        Point? safezone = null;
        Point? outsideSafezone = null;

        for (var x = 0; x < MapSize; x++)
        {
            for (var y = 0; y < MapSize; y++)
            {
                if (!this.WalkMap[x, y])
                {
                    continue;
                }

                var point = new Point((byte)x, (byte)y);
                if (this.SafezoneMap[x, y])
                {
                    safezone ??= point;
                }
                else
                {
                    result.Add(point);
                    outsideSafezone ??= point;
                }
            }
        }

        anyWalkableCoordinate = safezone ?? outsideSafezone;
        return result.ToArray();
    }
}