// <copyright file="TerrainTool.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

/// <summary>
/// The tool of the <see cref="TerrainEditor"/>, which defines which tiles are edited by a click.
/// </summary>
public enum TerrainTool
{
    /// <summary>
    /// Edits the square of tiles around the mouse cursor while the mouse button is pressed.
    /// </summary>
    Brush,

    /// <summary>
    /// Edits all tiles of a dragged rectangle.
    /// </summary>
    Rectangle,

    /// <summary>
    /// Edits all connected tiles which have the same attributes as the clicked tile.
    /// </summary>
    Fill,
}
