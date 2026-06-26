// <copyright file="IMapArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Represents a rectangular area on a map, defined by two corner coordinates.
/// </summary>
public interface IMapArea
{
    /// <summary>
    /// Gets or sets the upper-left corner X coordinate.
    /// </summary>
    byte X1 { get; set; }

    /// <summary>
    /// Gets or sets the upper-left corner Y coordinate.
    /// </summary>
    byte Y1 { get; set; }

    /// <summary>
    /// Gets or sets the bottom-right corner X coordinate.
    /// </summary>
    byte X2 { get; set; }

    /// <summary>
    /// Gets or sets the bottom-right corner Y coordinate.
    /// </summary>
    byte Y2 { get; set; }
}
