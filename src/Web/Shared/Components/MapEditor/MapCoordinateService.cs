// <copyright file="MapCoordinateService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Globalization;

/// <summary>
/// Provides coordinate transformation utilities for the map editor,
/// converting between client mouse positions and map tile coordinates.
/// </summary>
public sealed class MapCoordinateService
{
    /// <summary>
    /// Gets the base pixel scale factor before zoom is applied.
    /// </summary>
    public const int BaseScale = 3;

    private const int MapSize = 256;

    /// <summary>
    /// Converts client mouse coordinates to map tile coordinates.
    /// </summary>
    /// <param name="clientX">The client X position of the mouse.</param>
    /// <param name="clientY">The client Y position of the mouse.</param>
    /// <param name="rect">The bounding client rect of the map host element.</param>
    /// <param name="scroll">The current scroll position of the map host element.</param>
    /// <param name="zoomLevel">The current zoom level applied to the map.</param>
    /// <returns>
    /// A tuple of (X, Y) map coordinates if the position is within bounds;
    /// otherwise <see langword="null"/>.
    /// </returns>
    public (byte X, byte Y)? GetMapCoordinates(
        double clientX,
        double clientY,
        MapEditor.BoundingClientRect rect,
        MapEditor.ScrollInfo scroll,
        float zoomLevel)
    {
        var contentX = (clientX - rect.Left) + scroll.ScrollLeft;
        var contentY = (clientY - rect.Top) + scroll.ScrollTop;

        var scale = this.GetEffectiveScale(zoomLevel);
        var mapY = (int)(contentX / scale);
        var mapX = (int)(contentY / scale);

        if (mapX >= 0 && mapX < MapSize && mapY >= 0 && mapY < MapSize)
        {
            return ((byte)mapX, (byte)mapY);
        }

        return null;
    }

    /// <summary>
    /// Computes the effective pixel scale for the current zoom level.
    /// </summary>
    /// <param name="zoomLevel">The current zoom level.</param>
    /// <returns>The effective pixel scale factor.</returns>
    public float GetEffectiveScale(float zoomLevel) => BaseScale * zoomLevel;

    /// <summary>
    /// Builds a CSS background-size style string for the map grid overlay.
    /// </summary>
    /// <param name="effectiveScale">The effective pixel scale factor.</param>
    /// <returns>A CSS style string for the grid background size.</returns>
    public string GetGridStyle(float effectiveScale)
    {
        var gridSize = 16 * effectiveScale;
        var gridSizeString = gridSize.ToString(CultureInfo.InvariantCulture);
        return $"background-size: {gridSizeString}px {gridSizeString}px;";
    }
}