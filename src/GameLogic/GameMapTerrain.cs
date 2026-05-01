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

        this._spawnPoints = this.BuildSpawnPoints();
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
    /// Builds the array of valid spawn points.
    /// A valid spawn point is walkable and not in a safezone.
    /// </summary>
    /// <returns>Array of valid spawn points.</returns>
    private Point[] BuildSpawnPoints()
    {
        var result = new List<Point>(MapSize * MapSize);

        for (var x = 0; x < MapSize; x++)
        {
            for (var y = 0; y < MapSize; y++)
            {
                if (this.WalkMap[x, y] && !this.SafezoneMap[x, y])
                {
                    result.Add(new Point((byte)x, (byte)y));
                }
            }
        }

        return result.ToArray();
    }
}