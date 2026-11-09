// <copyright file="BotMapZones.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The walkable terrain of a map, split by the warp entry each tile is closest to on foot. A map which
/// can be walked through in one piece still holds very different monsters near its entries - the
/// first entry of Elvenland leads to level 4 to 13, its third to level 30 to 48, about 180 steps apart.
/// The bots use this to enter a map through the entry next to the monsters they came for, and to judge
/// an entry by those monsters only. On a map whose parts cannot be walked between (see
/// <see cref="BotMapRegions"/>), each zone also stays within the part its entry leads to.
/// </summary>
internal sealed class BotMapZones
{
    private const int MapSize = 256;

    private readonly BotMapRegions _regions;

    /// <summary>
    /// The zone of every tile (index <c>y * 256 + x</c>): the index of the closest entry plus one;
    /// 0 for tiles which no entry reaches.
    /// </summary>
    private readonly byte[] _zones = new byte[MapSize * MapSize];

    private readonly bool[] _hasTiles;

    private readonly ConcurrentDictionary<MonsterSpawnArea, int> _areaZones = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Initializes a new instance of the <see cref="BotMapZones"/> class.
    /// </summary>
    /// <param name="regions">The regions of the map.</param>
    /// <param name="entries">The gates of the warp entries; an earlier one wins tiles at equal distance.</param>
    internal BotMapZones(BotMapRegions regions, IReadOnlyList<ExitGate> entries)
    {
        this._regions = regions;
        this._hasTiles = new bool[entries.Count];
        this.Fill(entries);
    }

    /// <summary>
    /// Gets the zone of a tile.
    /// </summary>
    /// <param name="point">The tile.</param>
    /// <returns>The index of the closest entry; -1, if no entry reaches the tile.</returns>
    public int GetZone(Point point) => this._zones[(point.Y * MapSize) + point.X] - 1;

    /// <summary>
    /// Determines whether a player can land on a walkable tile through the entry.
    /// </summary>
    /// <param name="entry">The index of the entry.</param>
    /// <returns>True, if the entry has a zone.</returns>
    public bool HasZone(int entry) => this._hasTiles[entry];

    /// <summary>
    /// Gets the zone a spawn area belongs to: the one holding most of the tiles in which a bot can pick a
    /// hunting point of it (see <see cref="BotMapRegions.GetAreaTiles"/>).
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="scatterSpan">The size below which the area counts as a single spot.</param>
    /// <param name="scatterRange">The range around a single spot in which hunting points are picked.</param>
    /// <returns>The index of the entry; -1, if no entry reaches the area.</returns>
    /// <remarks>
    /// Some maps state their spawns as huge rectangles - Elvenland as quarters of the map. Such a quarter
    /// reaches into the zones of several entries, but counting it for each of them made an entry look
    /// as if it led to monsters which mostly stand far away from it.
    /// </remarks>
    public int GetAreaZone(MonsterSpawnArea area, int scatterSpan, int scatterRange)
    {
        return this._areaZones.GetOrAdd(
            area,
            a => this._regions.GetAreaTiles(a, scatterSpan, scatterRange)
                .Select(tile => this._zones[tile] - 1)
                .Where(zone => zone >= 0)
                .GroupBy(zone => zone)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .Select(group => group.Key)
                .DefaultIfEmpty(-1)
                .First());
    }

    /// <summary>
    /// Determines whether the spawn area belongs to the zone of the entry (see <see cref="GetAreaZone"/>).
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="entry">The index of the entry.</param>
    /// <param name="scatterSpan">The size below which the area counts as a single spot.</param>
    /// <param name="scatterRange">The range around a single spot in which hunting points are picked.</param>
    /// <returns>True, if the area lies within the zone.</returns>
    public bool IsAreaWithin(MonsterSpawnArea area, int entry, int scatterSpan, int scatterRange)
    {
        return this.GetAreaZone(area, scatterSpan, scatterRange) == entry;
    }

    private void Fill(IReadOnlyList<ExitGate> entries)
    {
        // One breadth-first search from the landing points of all entries at once: every tile is
        // reached first from the entry closest to it on foot.
        var pending = new Queue<int>();
        for (var entry = 0; entry < entries.Count && entry < byte.MaxValue; entry++)
        {
            if (entries[entry].X2 < entries[entry].X1 || entries[entry].Y2 < entries[entry].Y1)
            {
                // A misconfigured gate has no landing domain (GetPossibleLandingPoints would throw).
                continue;
            }

            foreach (var point in entries[entry].GetPossibleLandingPoints())
            {
                var index = (point.Y * MapSize) + point.X;
                if (this._regions.GetRegion(index) != 0 && this._zones[index] == 0)
                {
                    this._zones[index] = (byte)(entry + 1);
                    this._hasTiles[entry] = true;
                    pending.Enqueue(index);
                }
            }
        }

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
                    if (this._zones[nextIndex] == 0 && this._regions.GetRegion(nextIndex) != 0)
                    {
                        this._zones[nextIndex] = this._zones[index];
                        pending.Enqueue(nextIndex);
                    }
                }
            }
        }
    }
}
