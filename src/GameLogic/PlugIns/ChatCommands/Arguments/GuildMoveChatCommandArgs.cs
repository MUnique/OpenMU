// <copyright file="GuildMoveChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Arguments used by GuildMoveChatCommandPlugIn.
/// </summary>
public class GuildMoveChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    [Argument("guild")]
    [ValueReference(ChatCommandValueReference.GuildName)]
    public string? GuildName { get; set; }

    /// <summary>
    /// Gets or sets the name or id of the map.
    /// </summary>
    [Argument("mapIdOrName")]
    [ValueReference(ChatCommandValueReference.Map)]
    public string? MapIdOrName { get; set; }

    /// <summary>
    /// Gets or sets the coordinate X.
    /// </summary>
    [Argument("x", false)]
    [ValueReference(ChatCommandValueReference.MapCoordinateX)]
    public byte X { get; set; }

    /// <summary>
    /// Gets or sets the coordinate Y.
    /// </summary>
    [Argument("y", false)]
    [ValueReference(ChatCommandValueReference.MapCoordinateY)]
    public byte Y { get; set; }

    /// <summary>
    /// Gets the coordinates X and Y.
    /// </summary>
    public Point Coordinates => new(this.X, this.Y);
}