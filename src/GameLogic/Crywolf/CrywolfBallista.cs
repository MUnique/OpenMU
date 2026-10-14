// <copyright file="CrywolfBallista.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// A ballista of the army of Balgass, which bombards a point of the fortress.
/// </summary>
/// <remarks>
/// The ballista is identified by the coordinates of its monster spawn.
/// </remarks>
public class CrywolfBallista
{
    /// <summary>
    /// Gets or sets the x coordinate of the monster spawn of the ballista.
    /// </summary>
    public byte X { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate of the monster spawn of the ballista.
    /// </summary>
    public byte Y { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate of the point, which the ballista bombards.
    /// </summary>
    public byte TargetX { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate of the point, which the ballista bombards.
    /// </summary>
    public byte TargetY { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({this.X}, {this.Y}) -> ({this.TargetX}, {this.TargetY})";
    }
}
