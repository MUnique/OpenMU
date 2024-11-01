// <copyright file="ExitGateExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions for <see cref="ExitGate"/>.
/// </summary>
public static class ExitGateExtensions
{
    /// <summary>
    /// Gets a random point at the exit gate.
    /// </summary>
    /// <param name="gate">The gate.</param>
    /// <returns>The random point.</returns>
    public static Point GetRandomPoint(this ExitGate gate)
    {
        return new Point((byte)Rand.NextInt(gate.X1, gate.X2), (byte)Rand.NextInt(gate.Y1, gate.Y2));
    }
}