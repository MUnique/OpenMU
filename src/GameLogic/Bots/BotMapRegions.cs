// <copyright file="BotMapRegions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The walkable terrain of a map, split into the parts which cannot be walked between. Some maps hold
/// several such parts under one map number - the seven floors of the Lost Tower, the three parts of the
/// Dungeon, Kanturu Ruins 3 - which a player only reaches through their own warp entry or through a gate
/// on the map. The bots use this to judge a warp by the monsters it actually leads to, and to look for
/// hunting grounds only where they can walk.
/// </summary>
/// <remarks>
/// Tiles are connected to all eight neighbours, the same way the bots' path finding moves.
/// The split is cached for the lifetime of the process, keyed by the map's id - terrain is static
/// configuration, so terrain edited through the admin panel needs a server restart to be taken into
/// account (like <see cref="BotNavigator.HasWalkableSpawnGate"/>).
/// </remarks>
internal sealed class BotMapRegions
{
    private const int MapSize = 256;

    private static readonly ConcurrentDictionary<Guid, BotMapRegions> Cache = new();

    /// <summary>
    /// The region of every tile (index <c>y * 256 + x</c>); 0 for tiles which are not walkable.
    /// </summary>
    private readonly ushort[] _regions;

    /// <summary>
    /// The regions of each spawn area, by reference: the persistent entities compare by their id, which
    /// is not unique for areas which were never stored.
    /// </summary>
    private readonly ConcurrentDictionary<MonsterSpawnArea, int[]> _areaRegions = new(ReferenceEqualityComparer.Instance);

    private readonly ConcurrentDictionary<string, BotMapZones> _zones = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BotMapRegions"/> class.
    /// </summary>
    /// <param name="walkMap">The walk map of the terrain.</param>
    internal BotMapRegions(bool[,] walkMap)
    {
        this._regions = new ushort[MapSize * MapSize];
        this.RegionCount = this.Label(walkMap);
    }

    /// <summary>
    /// Gets the number of walkable regions.
    /// </summary>
    public int RegionCount { get; }

    /// <summary>
    /// Gets the regions of the map.
    /// </summary>
    /// <param name="definition">The map definition.</param>
    /// <returns>The regions of the map.</returns>
    public static BotMapRegions Get(GameMapDefinition definition)
    {
        var id = definition.GetId();
        if (id == Guid.Empty)
        {
            // Caching under the fallback id would hand the first unidentifiable map's terrain to all others.
            return new BotMapRegions(new GameMapTerrain(definition).WalkMap);
        }

        return Cache.GetOrAdd(id, _ => new BotMapRegions(new GameMapTerrain(definition).WalkMap));
    }

    /// <summary>
    /// Gets the region of a tile.
    /// </summary>
    /// <param name="point">The tile.</param>
    /// <returns>The region; 0, if the tile is not walkable.</returns>
    public int GetRegion(Point point) => this._regions[(point.Y * MapSize) + point.X];

    /// <summary>
    /// Gets the regions in which a bot can pick a hunting point of the spawn area.
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="scatterSpan">The size below which the area counts as a single spot.</param>
    /// <param name="scatterRange">The range around a single spot in which hunting points are picked.</param>
    /// <returns>The distinct regions; empty, if no tile of the area is walkable.</returns>
    /// <remarks>See <see cref="GetAreaTiles"/> for which tiles of the area count.</remarks>
    public int[] GetAreaRegions(MonsterSpawnArea area, int scatterSpan, int scatterRange)
    {
        return this._areaRegions.GetOrAdd(
            area,
            a => this.GetAreaTiles(a, scatterSpan, scatterRange).Select(this.GetRegion).Distinct().ToArray());
    }

    /// <summary>
    /// Determines whether the spawn area lies (at least partly) within one of the regions.
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="regions">The regions.</param>
    /// <param name="scatterSpan">The size below which the area counts as a single spot.</param>
    /// <param name="scatterRange">The range around a single spot in which hunting points are picked.</param>
    /// <returns>True, if the area lies within one of the regions.</returns>
    public bool IsAreaWithin(MonsterSpawnArea area, IReadOnlyCollection<int> regions, int scatterSpan, int scatterRange)
    {
        return this.GetAreaRegions(area, scatterSpan, scatterRange).Any(regions.Contains);
    }

    /// <summary>
    /// Gets the zones of the map for the given warp entries (see <see cref="BotMapZones"/>).
    /// </summary>
    /// <param name="entries">The gates of the warp entries, in the order of preference for equal distances.</param>
    /// <returns>The zones.</returns>
    /// <remarks>
    /// Cached per set of entries: the bots of one map see only a few different sets, as the warp
    /// entries they may legally use only change with their level.
    /// </remarks>
    public BotMapZones GetZones(IReadOnlyList<ExitGate> entries)
    {
        var ids = entries.Select(e => e.GetId()).ToList();
        if (ids.Contains(Guid.Empty))
        {
            // Unidentifiable gates would share one cache key.
            return new BotMapZones(this, entries);
        }

        return this._zones.GetOrAdd(string.Join(',', ids), _ => new BotMapZones(this, entries));
    }

    /// <summary>
    /// Gets the walkable tiles in which a bot can pick a hunting point of the spawn area.
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="scatterSpan">The size below which the area counts as a single spot.</param>
    /// <param name="scatterRange">The range around a single spot in which hunting points are picked.</param>
    /// <returns>The tile indexes (<c>y * 256 + x</c>).</returns>
    /// <remarks>
    /// Mirrors how the navigator picks a point of the area: inside its rectangle, or around its center
    /// when the area is a single spot. A single spot on a walkable tile is that tile only - the scatter
    /// range could reach through a wall into another floor otherwise.
    /// </remarks>
    internal IEnumerable<int> GetAreaTiles(MonsterSpawnArea area, int scatterSpan, int scatterRange)
    {
        int minX = Math.Min(area.X1, area.X2);
        int maxX = Math.Max(area.X1, area.X2);
        int minY = Math.Min(area.Y1, area.Y2);
        int maxY = Math.Max(area.Y1, area.Y2);
        if ((maxX - minX) < scatterSpan && (maxY - minY) < scatterSpan)
        {
            var centerX = (minX + maxX) / 2;
            var centerY = (minY + maxY) / 2;
            if (this._regions[(centerY * MapSize) + centerX] != 0)
            {
                yield return (centerY * MapSize) + centerX;
                yield break;
            }

            minX = Math.Max(0, centerX - scatterRange);
            maxX = Math.Min(MapSize - 1, centerX + scatterRange);
            minY = Math.Max(0, centerY - scatterRange);
            maxY = Math.Min(MapSize - 1, centerY + scatterRange);
        }

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if (this._regions[(y * MapSize) + x] != 0)
                {
                    yield return (y * MapSize) + x;
                }
            }
        }
    }

    /// <summary>
    /// Gets the region of a tile by its index.
    /// </summary>
    /// <param name="index">The tile index (<c>y * 256 + x</c>).</param>
    /// <returns>The region; 0, if the tile is not walkable.</returns>
    internal int GetRegion(int index) => this._regions[index];

    private int Label(bool[,] walkMap)
    {
        var regionCount = 0;
        var pending = new Queue<int>();
        for (var startIndex = 0; startIndex < this._regions.Length; startIndex++)
        {
            if (this._regions[startIndex] != 0 || !walkMap[startIndex % MapSize, startIndex / MapSize])
            {
                continue;
            }

            // Pockets of a single walkable tile are common, but separate 8-connected regions take at
            // least every fourth tile, so even the worst map stays far within the ushort range.
            regionCount++;
            this._regions[startIndex] = (ushort)regionCount;
            pending.Enqueue(startIndex);
            while (pending.TryDequeue(out var index))
            {
                var x = index % MapSize;
                var y = index / MapSize;
                for (var offsetX = -1; offsetX <= 1; offsetX++)
                {
                    for (var offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        var nextX = x + offsetX;
                        var nextY = y + offsetY;
                        if ((uint)nextX >= MapSize || (uint)nextY >= MapSize)
                        {
                            continue;
                        }

                        var nextIndex = (nextY * MapSize) + nextX;
                        if (this._regions[nextIndex] == 0 && walkMap[nextX, nextY])
                        {
                            this._regions[nextIndex] = (ushort)regionCount;
                            pending.Enqueue(nextIndex);
                        }
                    }
                }
            }
        }

        return regionCount;
    }
}
