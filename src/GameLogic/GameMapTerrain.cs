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
    /// Gets the size of the map in each dimension.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Gets a grid of all safezone coordinates.
    /// </summary>
    public bool[,] SafezoneMap { get; private set; } = new bool[1, 1];

    /// <summary>
    /// Gets a grid of all walkable coordinates.
    /// </summary>
    public bool[,] WalkMap { get; private set; } = new bool[1, 1];

    /// <summary>
    /// Gets a grid of the walkable coordinates of monsters.
    /// </summary>
    public byte[,] AIgrid { get; private set; } = new byte[1, 1];

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
    /// Gets the walkable coordinate nearest the center of this map.
    /// </summary>
    /// <returns>A center coordinate suitable for an initial spawn.</returns>
    public Point GetCenterCoordinate()
    {
        var center = this.Size / 2;
        if (this.WalkMap[center, center])
        {
            return new Point((ushort)center, (ushort)center);
        }

        for (var radius = 1; radius < this.Size; radius++)
        {
            var min = Math.Max(0, center - radius);
            var max = Math.Min(this.Size - 1, center + radius);
            for (var x = min; x <= max; x++)
            {
                for (var y = min; y <= max; y++)
                {
                    if (this.WalkMap[x, y])
                    {
                        return new Point((ushort)x, (ushort)y);
                    }
                }
            }
        }

        return new Point((ushort)center, (ushort)center);
    }

    /// <summary>
    /// Gets a random drop coordinate at the specified point in the specified radius.
    /// </summary>
    /// <param name="point">The target point.</param>
    /// <param name="maximumRadius">The maximum radius around the specified coordinate.</param>
    /// <returns>The random drop coordinate.</returns>
    public Point GetRandomCoordinate(Point point, byte maximumRadius)
    {
        int tempx = Rand.NextInt(Math.Max(0, point.X - maximumRadius), Math.Min(this.Size - 1, point.X + maximumRadius + 1));
        int tempy = Rand.NextInt(Math.Max(0, point.Y - maximumRadius), Math.Min(this.Size - 1, point.Y + maximumRadius + 1));
        int i = 0;
        while (!this.WalkMap[tempx, tempy] && i < 20)
        {
            tempx = Rand.NextInt(Math.Max(0, point.X - maximumRadius), Math.Min(this.Size - 1, point.X + maximumRadius + 1));
            tempy = Rand.NextInt(Math.Max(0, point.Y - maximumRadius), Math.Min(this.Size - 1, point.Y + maximumRadius + 1));
            i++;
        }

        if (i == 20)
        {
            return point;
        }

        return new Point(checked((ushort)tempx), checked((ushort)tempy));
    }

    /// <summary>
    /// Updates the ai grid value at the specified coordinate.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateAiGridValue(int x, int y)
    {
        this.AIgrid[x, y] = (byte)((this.WalkMap[x, y] ? 1 : 0) | (this.SafezoneMap[x, y] ? 0b1000_0000 : 0));
    }

    /// <summary>
    /// Reads the terrain data from a stream.
    /// </summary>
    /// <param name="data">The data.</param>
    private void ReadTerrainData(ReadOnlySpan<byte> data)
    {
        // Older persisted configurations contain two bytes fewer than a 256x256
        // terrain payload. Preserve those maps by treating the missing tail cells
        // as default walkable terrain while newer and larger maps remain strict.
        byte[]? legacyPadding = null;
        if (data.Length == (256 * 256) - 2 || data.Length == (512 * 512) - 2)
        {
            var expectedSize = data.Length == (512 * 512) - 2 ? 512 * 512 : 256 * 256;
            legacyPadding = new byte[expectedSize];
            data.CopyTo(legacyPadding);
            data = legacyPadding;
        }

        var size = (int)Math.Sqrt(data.Length);
        if (size * size != data.Length)
        {
            throw new ArgumentException($"Terrain data length {data.Length} is not a perfect square.");
        }

        this.Size = size;
        this.SafezoneMap = new bool[size, size];
        this.WalkMap = new bool[size, size];
        this.AIgrid = new byte[size, size];

        for (int i = 0; i < data.Length; i++)
        {
            int x = i % size;
            int y = i / size;
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
        var result = new List<Point>(this.Size * this.Size);

        for (var x = 0; x < this.Size; x++)
        {
            for (var y = 0; y < this.Size; y++)
            {
                if (this.WalkMap[x, y] && !this.SafezoneMap[x, y])
                {
                    result.Add(new Point((ushort)x, (ushort)y));
                }
            }
        }

        return result.ToArray();
    }
}
