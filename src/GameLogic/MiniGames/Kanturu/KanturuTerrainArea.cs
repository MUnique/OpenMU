// <copyright file="KanturuTerrainArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// A rectangular terrain area of the Kanturu event map.
/// </summary>
public class KanturuTerrainArea
{
    /// <summary>
    /// Gets or sets the first x coordinate of the area.
    /// </summary>
    public byte StartX { get; set; }

    /// <summary>
    /// Gets or sets the first y coordinate of the area.
    /// </summary>
    public byte StartY { get; set; }

    /// <summary>
    /// Gets or sets the last x coordinate of the area.
    /// </summary>
    public byte EndX { get; set; }

    /// <summary>
    /// Gets or sets the last y coordinate of the area.
    /// </summary>
    public byte EndY { get; set; }
}
