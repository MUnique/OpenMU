// <copyright file="MapObjectStyleService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Provides CSS class and inline style computation for map objects
/// rendered within the map editor.
/// </summary>
public sealed class MapObjectStyleService
{
    /// <summary>
    /// Returns the combined CSS size and position style for a given map object.
    /// </summary>
    /// <param name="obj">The map object, expected to be a <see cref="Gate"/> or <see cref="MonsterSpawnArea"/>.</param>
    /// <returns>
    /// An inline CSS style string, or <see cref="string.Empty"/> if the object type is not supported.
    /// The values are expressed in map tiles and multiplied with the <c>--map-scale</c> CSS variable
    /// (pixels per tile, set by the map editor's JavaScript module), so the style does not depend on
    /// the zoom level and zooming doesn't require a re-render.
    /// </returns>
    public string GetSizeAndPositionStyle(object obj) => obj switch
    {
        Gate gate => GetGateStyle(gate),
        MonsterSpawnArea spawn => GetSpawnStyle(spawn),
        _ => string.Empty,
    };

    /// <summary>
    /// Returns the CSS class string for a given map object, including a focus indicator
    /// if the object is currently selected.
    /// </summary>
    /// <param name="obj">The map object to style.</param>
    /// <param name="focusedObject">The currently focused/selected object, if any.</param>
    /// <returns>A CSS class string appropriate for the object type and focus state.</returns>
    public string GetCssClass(object obj, object? focusedObject)
    {
        var result = obj switch
        {
            EnterGate => "gate-enter",
            ExitGate => "gate-exit",
            MonsterSpawnArea spawn => GetSpawnClass(spawn),
            _ => string.Empty,
        };

        if (focusedObject == obj)
        {
            result += " focused-object";
        }

        return result;
    }

    private static string GetSpawnClass(MonsterSpawnArea spawn)
    {
        var kind = spawn.MonsterDefinition?.ObjectKind;
        var typeClass = kind switch
        {
            NpcObjectKind.Monster => "spawn-monster",
            NpcObjectKind.PassiveNpc or NpcObjectKind.Guard => "spawn-npc",
            _ => "spawn-other",
        };

        return spawn.IsPoint() ? $"{typeClass}-single" : $"{typeClass}-area";
    }

    /// <summary>
    /// Builds an inline CSS style string for a <see cref="Gate"/> object.
    /// </summary>
    /// <param name="gate">The gate to compute styles for.</param>
    /// <returns>An inline CSS style string defining width, height, top, and left.</returns>
    private static string GetGateStyle(Gate gate)
    {
        var width = 1 + gate.Y2 - gate.Y1;
        var height = 1 + gate.X2 - gate.X1;
        var top = (float)gate.X1;
        var left = (float)gate.Y1;

        return FormatStyle(width, height, top, left);
    }

    /// <summary>
    /// Builds an inline CSS style string for a <see cref="MonsterSpawnArea"/> object,
    /// applying an additional scale factor for point spawns so they remain visible.
    /// </summary>
    /// <param name="spawn">The spawn area to compute styles for.</param>
    /// <returns>An inline CSS style string defining width, height, top, and left.</returns>
    private static string GetSpawnStyle(MonsterSpawnArea spawn)
    {
        var objScale = spawn.IsPoint() ? 1.75f : 1.0f;
        var width = objScale * (1 + spawn.Y2 - spawn.Y1);
        var height = objScale * (1 + spawn.X2 - spawn.X1);
        var offset = (objScale - 1.0f) * 0.5f;
        var top = spawn.X1 - offset;
        var left = spawn.Y1 - offset;

        return FormatStyle(width, height, top, left);
    }

    /// <summary>
    /// Formats the computed dimensions and position into an inline CSS style string.
    /// </summary>
    /// <param name="width">Element width in map tiles.</param>
    /// <param name="height">Element height in map tiles.</param>
    /// <param name="top">Top offset in map tiles.</param>
    /// <param name="left">Left offset in map tiles.</param>
    /// <returns>A formatted inline CSS style string.</returns>
    private static string FormatStyle(float width, float height, float top, float left)
    {
        var inv = CultureInfo.InvariantCulture;
        return $"width: calc(var(--map-scale) * {width.ToString(inv)}); " +
               $"height: calc(var(--map-scale) * {height.ToString(inv)}); " +
               $"top: calc(var(--map-scale) * {top.ToString(inv)}); " +
               $"left: calc(var(--map-scale) * {left.ToString(inv)});";
    }
}