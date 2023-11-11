// <copyright file="Gate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a gate through which a player can exit or enter to other maps.
/// </summary>
[Cloneable]
public partial class Gate
{
    /// <summary>
    /// Gets or sets the upper left corner, x-coordinate.
    /// </summary>
    public byte X1 { get; set; }

    /// <summary>
    /// Gets or sets the upper left corner, y-coordinate.
    /// </summary>
    public byte Y1 { get; set; }

    /// <summary>
    /// Gets or sets the bottom right corner, x-coordinate.
    /// </summary>
    public byte X2 { get; set; }

    /// <summary>
    /// Gets or sets the bottom right corner, y-coordinate.
    /// </summary>
    public byte Y2 { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var start = $"({this.X1}, {this.Y1})";
        if (this.X1 == this.X2 && this.Y1 == this.Y2)
        {
            return start;
        }

        return $"{start} - ({this.X2}, {this.Y2})";
    }
}