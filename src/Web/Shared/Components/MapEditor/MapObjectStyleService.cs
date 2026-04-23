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
    /// <param name="effectiveScale">The effective pixel scale factor at the current zoom level.</param>
    /// <returns>An inline CSS style string, or <see cref="string.Empty"/> if the object type is not supported.</returns>
    public string GetSizeAndPositionStyle(object obj, float effectiveScale) => obj switch
    {
        Gate gate => GetGateStyle(gate, effectiveScale),
        MonsterSpawnArea spawn => GetSpawnStyle(spawn, effectiveScale),
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
            MonsterSpawnArea spawn => spawn.IsPoint() ? "spawn-single" : "spawn-area",
            _ => string.Empty,
        };

        if (focusedObject == obj)
        {
            result += " focused-object";
        }

        return result;
    }

    /// <summary>
    /// Builds an inline CSS style string for a <see cref="Gate"/> object.
    /// </summary>
    /// <param name="gate">The gate to compute styles for.</param>
    /// <param name="scale">The effective pixel scale factor.</param>
    /// <returns>An inline CSS style string defining width, height, top, and left.</returns>
    private static string GetGateStyle(Gate gate, float scale)
    {
        var width = scale * (1 + gate.Y2 - gate.Y1);
        var height = scale * (1 + gate.X2 - gate.X1);
        var top = scale * gate.X1;
        var left = scale * gate.Y1;

        return FormatStyle(width, height, top, left);
    }

    /// <summary>
    /// Builds an inline CSS style string for a <see cref="MonsterSpawnArea"/> object,
    /// applying an additional scale factor for point spawns so they remain visible.
    /// </summary>
    /// <param name="spawn">The spawn area to compute styles for.</param>
    /// <param name="scale">The effective pixel scale factor.</param>
    /// <returns>An inline CSS style string defining width, height, top, and left.</returns>
    private static string GetSpawnStyle(MonsterSpawnArea spawn, float scale)
    {
        var objScale = spawn.IsPoint() ? 1.75f : 1.0f;
        var width = objScale * scale * (1 + spawn.Y2 - spawn.Y1);
        var height = objScale * scale * (1 + spawn.X2 - spawn.X1);
        var offset = (objScale - 1.0f) * scale * 0.5f;
        var top = (scale * spawn.X1) - offset;
        var left = (scale * spawn.Y1) - offset;

        return FormatStyle(width, height, top, left);
    }

    /// <summary>
    /// Formats the computed dimensions and position into an inline CSS style string.
    /// </summary>
    /// <param name="width">Element width in pixels.</param>
    /// <param name="height">Element height in pixels.</param>
    /// <param name="top">Top offset in pixels.</param>
    /// <param name="left">Left offset in pixels.</param>
    /// <returns>A formatted inline CSS style string.</returns>
    private static string FormatStyle(float width, float height, float top, float left)
    {
        var inv = CultureInfo.InvariantCulture;
        return $"width: {width.ToString(inv)}px; " +
               $"height: {height.ToString(inv)}px; " +
               $"top: {top.ToString(inv)}px; " +
               $"left: {left.ToString(inv)}px;";
    }
}