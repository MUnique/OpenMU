// <copyright file="TerrainEditMode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

/// <summary>
/// Defines how the selected attributes are applied to the edited tiles of the <see cref="TerrainEditor"/>.
/// </summary>
public enum TerrainEditMode
{
    /// <summary>
    /// The selected attributes are added to the existing attributes of a tile.
    /// </summary>
    Add,

    /// <summary>
    /// The selected attributes are removed from the existing attributes of a tile.
    /// </summary>
    Remove,

    /// <summary>
    /// The attributes of a tile are replaced by the selected attributes.
    /// </summary>
    Replace,
}
