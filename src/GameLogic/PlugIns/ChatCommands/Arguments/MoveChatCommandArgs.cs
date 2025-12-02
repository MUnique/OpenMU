// <copyright file="MoveChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Arguments used by MoveChatCommandPlugIn.
/// </summary>
public class MoveChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the target.
    /// The target can be the map name, map id or character name.
    /// </summary>
    [Argument("target")]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the name or id of the map.
    /// </summary>
    [Argument("mapIdOrName", false)]
    public string? MapIdOrName { get; set; }

    /// <summary>
    /// Gets or sets the coordinate X.
    /// </summary>
    [Argument("x", false)]
    public byte X { get; set; }

    /// <summary>
    /// Gets or sets the coordinate Y.
    /// </summary>
    [Argument("y", false)]
    public byte Y { get; set; }

    /// <summary>
    /// Gets the coordinates X and Y.
    /// </summary>
    public Point Coordinates => new(this.X, this.Y);
}