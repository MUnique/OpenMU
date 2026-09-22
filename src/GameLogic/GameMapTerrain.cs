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
    /// The terrain attribute bits which block line of sight. Only actual walls
    /// (<see cref="TerrainAttributeType.Blocked"/>) stop projectiles; holes
    /// (<c>NoGround</c>) and water can be shot across. Higher bits observed in
    /// shipped files (e.g. 32, 64, 128 — client height/camera/action flags)
    /// carry no wall meaning and don't block either.
    /// </summary>
    private const byte SightBlockingAttributes = (byte)TerrainAttributeType.Blocked;

    /// <summary>
    /// The terrain attribute bit which marks a safezone.
    /// </summary>
    private const byte SafezoneBit = (byte)TerrainAttributeType.Safezone;

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
    /// Gets the raw terrain attribute flags per coordinate, as read from the
    /// map's <c>.att</c> file (a <see cref="TerrainAttributeType"/> bitmask:
    /// safezone, blocked, no ground, water, …). While <see cref="WalkMap"/>
    /// collapses everything unwalkable into one value, this grid preserves the
    /// distinction between walls (which block sight) and holes or water
    /// (which can be shot across).
    /// </summary>
    public byte[,] AttributeMap { get; } = new byte[MapSize, MapSize];

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
    /// Determines whether there is a clear line of sight between two coordinates.
    /// Uses Bresenham's line algorithm; any intermediate tile with a wall
    /// (<see cref="TerrainAttributeType.Blocked"/>) blocks sight. Holes (<c>NoGround</c>) and water can be shot across.
    /// The endpoints themselves are excluded, so that the tiles the attacker
    /// and target stand on never block the check. A diagonal step passing
    /// exactly between two wall tiles that touch only at a corner is also
    /// treated as blocked.
    /// Sight is symmetric: the endpoints are normalized before the walk, so
    /// both directions always agree.
    /// </summary>
    /// <param name="from">The attacking (viewing) coordinate.</param>
    /// <param name="to">The target coordinate.</param>
    /// <returns><c>true</c> when no wall blocks the line between the coordinates; otherwise, <c>false</c>.</returns>
    public bool HasLineOfSight(Point from, Point to)
    {
        int x0 = from.X;
        int y0 = from.Y;
        int x1 = to.X;
        int y1 = to.Y;

        if (x0 == x1 && y0 == y1)
        {
            return true;
        }

        // Melee range is always visible: adjacent tiles share at most a corner,
        // which must never read as a wall.
        if (Math.Abs(x1 - x0) <= 1 && Math.Abs(y1 - y0) <= 1)
        {
            return true;
        }

        // Normalize the direction so the traversal doesn't depend on which side is
        // looking: Bresenham picks a different cell at exact lattice corners
        // depending on the starting endpoint.
        if (x1 < x0 || (x1 == x0 && y1 < y0))
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
        }

        int dx = Math.Abs(x1 - x0);
        int dy = -Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        int x = x0;
        int y = y0;
        while (true)
        {
            int previousX = x;
            int previousY = y;

            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y += sy;
            }

            // The closed-corner check runs before the endpoint check on purpose:
            // a corner pocket next to the target must block exactly like the
            // same pocket next to the viewer, keeping sight symmetric.
            if (x != previousX && y != previousY
                && this.BlocksSight(previousX, y) && this.BlocksSight(x, previousY))
            {
                // Diagonal step squeezing between two wall tiles that
                // touch only at a corner: the line grazes a solid corner.
                return false;
            }

            // Reached the target tile: endpoints never block sight.
            if (x == x1 && y == y1)
            {
                return true;
            }

            if (this.BlocksSight(x, y))
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Determines whether the tile at the specified coordinate blocks line of
    /// sight, i.e. carries the <c>Blocked</c> (wall) attribute. Tiles which are
    /// merely unwalkable for other reasons (holes, water) do not block sight.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns><c>true</c> when the tile is a sight-blocking wall; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool BlocksSight(int x, int y)
    {
        return (this.AttributeMap[x, y] & SightBlockingAttributes) != 0;
    }

    /// <summary>
    /// Applies a runtime terrain attribute change to a single tile, e.g. for
    /// mini games which open or collapse parts of a map. Keeps the raw
    /// attributes, <see cref="WalkMap"/>, <see cref="SafezoneMap"/> and
    /// <see cref="AIgrid"/> consistent with each other.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="attribute">The attribute to set or remove.</param>
    /// <param name="setAttribute"><c>true</c> to set the attribute, <c>false</c> to remove it.</param>
    /// <param name="openArea">
    /// Only used when removing: <c>true</c> clears every movement-blocking bit instead of
    /// only <paramref name="attribute"/>, guaranteeing the tile becomes walkable. Opt into
    /// this only where a rect is known to mix tile kinds (e.g. the Kanturu barrier, whose
    /// wall band must open together with the holes for the corridor to become passable).
    /// Everywhere else removal must stay per-bit: callers like the castle siege gate
    /// restore the exact previous state, and clearing foreign bits would permanently
    /// convert original holes or water into walkable ground.
    /// </param>
    public void ApplyTerrainAttribute(byte x, byte y, TerrainAttributeType attribute, bool setAttribute, bool openArea = false)
    {
        if (attribute == TerrainAttributeType.Safezone)
        {
            // Safezone is tracked independently of the walkability bits,
            // exactly as before: setting it never changes walkability.
            this.AttributeMap[x, y] = setAttribute
                ? (byte)(this.AttributeMap[x, y] | SafezoneBit)
                : (byte)(this.AttributeMap[x, y] & ~SafezoneBit);
            this.SafezoneMap[x, y] = setAttribute;
            this.UpdateAiGridValue(x, y);
            return;
        }

        if (setAttribute)
        {
            this.AttributeMap[x, y] |= (byte)attribute;
        }
        else if (openArea)
        {
            // Re-open the whole tile: mixed rects can't rely on the configured
            // attribute matching the bits in the terrain file, so every
            // non-safezone bit is cleared instead of only the configured one.
            this.AttributeMap[x, y] &= SafezoneBit;
        }
        else
        {
            this.AttributeMap[x, y] = (byte)(this.AttributeMap[x, y] & ~(byte)attribute);
        }

        // SafezoneMap is deliberately left untouched: runtime non-safezone changes
        // never alter the safezone status, exactly as before.
        this.WalkMap[x, y] = IsWalkableValue(this.AttributeMap[x, y]);
        this.UpdateAiGridValue(x, y);
    }

    /// <summary>
    /// Updates the ai grid value at the specified coordinate.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateAiGridValue(byte x, byte y)
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
            this.AttributeMap[x, y] = data[i];
            this.RefreshTile(x, y);
        }
    }

    /// <summary>
    /// Recomputes the derived grids of a single tile from its raw attributes.
    /// A tile is walkable only with no attribute flags (or safezone only).
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private void RefreshTile(byte x, byte y)
    {
        byte value = this.AttributeMap[x, y];
        this.WalkMap[x, y] = IsWalkableValue(value);
        this.SafezoneMap[x, y] = value == 1;
        this.UpdateAiGridValue(x, y);
    }

    /// <summary>
    /// Determines whether a raw terrain attribute value describes a walkable tile.
    /// </summary>
    /// <param name="value">The raw attribute value.</param>
    /// <returns><c>true</c> when the tile can be walked on; otherwise, <c>false</c>.</returns>
    private static bool IsWalkableValue(byte value)
    {
        return (value & ~SafezoneBit) == 0;
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