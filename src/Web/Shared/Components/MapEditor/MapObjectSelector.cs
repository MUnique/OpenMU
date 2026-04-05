// <copyright file="MapObjectSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Performs hit-testing against map objects to determine which object,
/// if any, occupies a given map coordinate.
/// </summary>
public sealed class MapObjectSelector
{
    /// <summary>
    /// Returns a display string for the size of a focused gate or area spawn,
    /// or <see langword="null"/> if the object is not focused or is a point spawn.
    /// </summary>
    /// <param name="obj">The map object to compute the size for.</param>
    /// <param name="focusedObject">The currently focused object.</param>
    /// <returns>A formatted size string, or <see langword="null"/>.</returns>
    public static string? GetObjectSize(object obj, object? focusedObject)
    {
        if (obj != focusedObject)
        {
            return null;
        }

        return obj switch
        {
            MonsterSpawnArea spawn when !spawn.IsPoint() =>
                $"{1 + spawn.Y2 - spawn.Y1}x{1 + spawn.X2 - spawn.X1}",
            EnterGate gate =>
                $"{1 + gate.Y2 - gate.Y1}x{1 + gate.X2 - gate.X1}",
            ExitGate gate =>
                $"{1 + gate.Y2 - gate.Y1}x{1 + gate.X2 - gate.X1}",
            _ => null,
        };
    }

    /// <summary>
    /// Returns the most appropriate scroll target coordinates for the given map object.
    /// For point spawns this is the center, for areas and gates this is the top left corner.
    /// </summary>
    /// <param name="obj">The map object to compute the scroll target for.</param>
    /// <returns>A tuple of (X, Y) coordinates, or <see langword="null"/> if the type is not supported.</returns>
    public static (float X, float Y)? GetScrollTarget(object obj)
    {
        return obj switch
        {
            MonsterSpawnArea spawn when spawn.IsPoint() => GetObjectCenter(spawn),
            MonsterSpawnArea spawn => (spawn.X1, spawn.Y1),
            Gate gate => (gate.X1, gate.Y1),
            _ => null,
        };
    }

    /// <summary>
    /// Returns the center map coordinates of the given map object.
    /// </summary>
    /// <param name="obj">The map object to compute the center for.</param>
    /// <returns>A tuple of (X, Y) center coordinates, or <see langword="null"/> if the type is not supported.</returns>
    public static (float X, float Y)? GetObjectCenter(object obj)
    {
        return obj switch
        {
            MonsterSpawnArea spawn when spawn.IsPoint() => (spawn.X1, spawn.Y1),
            MonsterSpawnArea spawn => ((spawn.X1 + spawn.X2) / 2.0f, (spawn.Y1 + spawn.Y2) / 2.0f),
            Gate gate => ((gate.X1 + gate.X2) / 2.0f, (gate.Y1 + gate.Y2) / 2.0f),
            _ => null,
        };
    }

    /// <summary>
    /// Determines whether the given object is visible under the specified filter.
    /// </summary>
    /// <param name="obj">The map object to test.</param>
    /// <param name="filter">The active object type filter.</param>
    /// <returns><see langword="true"/> if the object should be considered visible.</returns>
    public static bool MatchesFilters(object? obj, ObjectTypeFilter filter)
    {
        return filter switch
        {
            ObjectTypeFilter.All => true,
            ObjectTypeFilter.Gates => obj is EnterGate or ExitGate,
            _ => obj is MonsterSpawnArea spawn && SpawnMatchesFilter(spawn, filter),
        };
    }

    /// <summary>
    /// Returns the map object at the specified coordinates, giving priority
    /// to point spawns over area spawns, and spawns over gates.
    /// </summary>
    /// <param name="map">The map definition containing all objects to test.</param>
    /// <param name="x">The map X coordinate to test.</param>
    /// <param name="y">The map Y coordinate to test.</param>
    /// <param name="filter">The active object type filter to apply.</param>
    /// <returns>
    /// The best matching <see cref="MonsterSpawnArea"/>, <see cref="ExitGate"/>,
    /// or <see cref="EnterGate"/> at the position; or <see langword="null"/> if none found.
    /// </returns>
    public object? GetObjectAtPosition(
        GameMapDefinition map,
        byte x,
        byte y,
        ObjectTypeFilter filter)
    {
        var bestSpawn = this.FindBestSpawn(map.MonsterSpawns, x, y, filter);
        if (bestSpawn is not null)
        {
            return bestSpawn;
        }

        if (filter is ObjectTypeFilter.All or ObjectTypeFilter.Gates)
        {
            foreach (var gate in map.ExitGates)
            {
                if (IsPointInGate(gate, x, y))
                {
                    return gate;
                }
            }

            foreach (var gate in map.EnterGates)
            {
                if (IsPointInGate(gate, x, y))
                {
                    return gate;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Computes the distance from the given position to the spawn area,
    /// and indicates whether the position is considered inside the spawn.
    /// </summary>
    /// <param name="spawn">The spawn area to test.</param>
    /// <param name="x">The map X coordinate.</param>
    /// <param name="y">The map Y coordinate.</param>
    /// <param name="isInside">
    /// <see langword="true"/> if the coordinate is within the spawn's hit region.
    /// </param>
    /// <returns>The distance from the position to the spawn's center or origin point.</returns>
    private static float GetDistanceToSpawn(
        MonsterSpawnArea spawn, byte x, byte y, out bool isInside)
    {
        if (spawn.IsPoint())
        {
            const float hitRadius = 2.5f;
            var dx = x - spawn.X1;
            var dy = y - spawn.Y1;
            var distance = MathF.Sqrt((dx * dx) + (dy * dy));
            isInside = distance <= hitRadius;
            return distance;
        }

        isInside = x >= spawn.X1 && x <= spawn.X2
                                 && y >= spawn.Y1 && y <= spawn.Y2;

        if (!isInside)
        {
            return float.MaxValue;
        }

        var cx = (spawn.X1 + spawn.X2) / 2.0f;
        var cy = (spawn.Y1 + spawn.Y2) / 2.0f;
        return MathF.Sqrt(MathF.Pow(x - cx, 2) + MathF.Pow(y - cy, 2));
    }

    /// <summary>
    /// Determines whether the given coordinate falls within a gate's bounds.
    /// </summary>
    /// <param name="gate">The gate to test against.</param>
    /// <param name="x">The map X coordinate.</param>
    /// <param name="y">The map Y coordinate.</param>
    /// <returns><see langword="true"/> if the coordinate is inside the gate's bounds.</returns>
    private static bool IsPointInGate(Gate gate, byte x, byte y) =>
        x >= gate.X1 && x <= gate.X2 && y >= gate.Y1 && y <= gate.Y2;

    /// <summary>
    /// Returns whether a spawn matches the active filter.
    /// </summary>
    private static bool SpawnMatchesFilter(MonsterSpawnArea spawn, ObjectTypeFilter filter)
    {
        var objectKind = spawn.MonsterDefinition?.ObjectKind;

        return filter switch
        {
            ObjectTypeFilter.Gates => false,
            ObjectTypeFilter.Monsters => objectKind == NpcObjectKind.Monster,
            ObjectTypeFilter.Npcs => objectKind == NpcObjectKind.PassiveNpc || objectKind == NpcObjectKind.Guard,
            ObjectTypeFilter.Others => objectKind == NpcObjectKind.Trap
                                       || objectKind == NpcObjectKind.Statue
                                       || objectKind == NpcObjectKind.SoccerBall
                                       || objectKind == NpcObjectKind.Destructible,
            _ => true,
        };
    }

    /// <summary>
    /// Finds the best matching spawn area at the given position,
    /// preferring point spawns and closer center distances.
    /// </summary>
    /// <param name="spawns">The collection of spawn areas to search.</param>
    /// <param name="x">The map X coordinate.</param>
    /// <param name="y">The map Y coordinate.</param>
    /// <param name="filter">The active object type filter.</param>
    /// <returns>
    /// The best matching <see cref="MonsterSpawnArea"/>, or <see langword="null"/> if none intersect the position.
    /// </returns>
    private MonsterSpawnArea? FindBestSpawn(IEnumerable<MonsterSpawnArea> spawns, byte x, byte y, ObjectTypeFilter filter)
    {
        MonsterSpawnArea? best = null;
        float bestDistance = float.MaxValue;
        bool bestIsPoint = false;

        foreach (var spawn in spawns)
        {
            if (!SpawnMatchesFilter(spawn, filter))
            {
                continue;
            }

            var distance = GetDistanceToSpawn(spawn, x, y, out var isInside);
            if (!isInside)
            {
                continue;
            }

            var isPoint = spawn.IsPoint();
            var isBetter = best is null
                || (isPoint && !bestIsPoint)
                || (isPoint == bestIsPoint && distance < bestDistance);

            if (isBetter)
            {
                best = spawn;
                bestDistance = distance;
                bestIsPoint = isPoint;
            }
        }

        return best;
    }
}