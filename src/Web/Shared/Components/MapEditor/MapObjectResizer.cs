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
    /// Resizes a map area based on the dragged corner handle.
    /// </summary>
    /// <param name="area">The map area to adjust.</param>
    /// <param name="position">The corner handle being dragged.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    public static void Resize(IMapArea area, Resizers.ResizerPosition position, byte x, byte y)
    {
        switch (position)
        {
            case Resizers.ResizerPosition.TopLeft:
                area.X1 = Math.Min(x, area.X2);
                area.Y1 = Math.Min(y, area.Y2);
                break;
            case Resizers.ResizerPosition.TopRight:
                area.X1 = Math.Min(x, area.X2);
                area.Y2 = Math.Max(y, area.Y1);
                break;
            case Resizers.ResizerPosition.BottomRight:
                area.X2 = Math.Max(x, area.X1);
                area.Y2 = Math.Max(y, area.Y1);
                break;
            case Resizers.ResizerPosition.BottomLeft:
                area.X2 = Math.Max(x, area.X1);
                area.Y1 = Math.Min(y, area.Y2);
                break;
            default:
                // Not supported.
                break;
        }
    }
}
