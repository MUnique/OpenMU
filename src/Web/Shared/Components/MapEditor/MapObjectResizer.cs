// <copyright file="MapObjectResizer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Handles coordinate adjustments for resizing map objects at corner handles.
/// </summary>
internal static class MapObjectResizer
{
    /// <summary>
    /// Resizes a monster spawn area based on the dragged corner handle.
    /// </summary>
    /// <param name="spawn">The spawn to adjust.</param>
    /// <param name="position">The corner handle being dragged.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    public static void Resize(MonsterSpawnArea spawn, Resizers.ResizerPosition position, byte x, byte y)
    {
        switch (position)
        {
            case Resizers.ResizerPosition.TopLeft:
                spawn.X1 = Math.Min(x, spawn.X2);
                spawn.Y1 = Math.Min(y, spawn.Y2);
                break;
            case Resizers.ResizerPosition.TopRight:
                spawn.X1 = Math.Min(x, spawn.X2);
                spawn.Y2 = Math.Max(y, spawn.Y1);
                break;
            case Resizers.ResizerPosition.BottomRight:
                spawn.X2 = Math.Max(x, spawn.X1);
                spawn.Y2 = Math.Max(y, spawn.Y1);
                break;
            case Resizers.ResizerPosition.BottomLeft:
                spawn.X2 = Math.Max(x, spawn.X1);
                spawn.Y1 = Math.Min(y, spawn.Y2);
                break;
            default:
                // Not supported.
                break;
        }
    }

    /// <summary>
    /// Resizes a gate based on the dragged corner handle.
    /// </summary>
    /// <param name="gate">The gate to adjust.</param>
    /// <param name="position">The corner handle being dragged.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    public static void Resize(Gate gate, Resizers.ResizerPosition position, byte x, byte y)
    {
        switch (position)
        {
            case Resizers.ResizerPosition.TopLeft:
                gate.X1 = Math.Min(x, gate.X2);
                gate.Y1 = Math.Min(y, gate.Y2);
                break;
            case Resizers.ResizerPosition.TopRight:
                gate.X1 = Math.Min(x, gate.X2);
                gate.Y2 = Math.Max(y, gate.Y1);
                break;
            case Resizers.ResizerPosition.BottomRight:
                gate.X2 = Math.Max(x, gate.X1);
                gate.Y2 = Math.Max(y, gate.Y1);
                break;
            case Resizers.ResizerPosition.BottomLeft:
                gate.X2 = Math.Max(x, gate.X1);
                gate.Y1 = Math.Min(y, gate.Y2);
                break;
        }
    }
}
