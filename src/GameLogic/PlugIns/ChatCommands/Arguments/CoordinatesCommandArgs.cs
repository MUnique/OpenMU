// <copyright file="CoordinatesCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Arguments used by <see cref="TeleportChatCommandPlugIn"/> and others which just require an X and Y coordinate of a game map.
/// </summary>
public class CoordinatesCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the coordinate X.
    /// </summary>
    [Argument("x", true)]
    public byte X { get; set; }

    /// <summary>
    /// Gets or sets the coordinate Y.
    /// </summary>
    [Argument("y", true)]
    public byte Y { get; set; }

    /// <summary>
    /// Gets the coordinates X and Y.
    /// </summary>
    public Point Coordinates => new(this.X, this.Y);
}