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
    /// <remarks>
    /// This preserves the existing placement convention: differing upper bounds are exclusive, while
    /// equal lower and upper bounds describe one coordinate. Other gate operations, such as containment
    /// checks and terrain painting, treat the upper bounds as inclusive; this convention applies only
    /// to random placement.
    /// </remarks>
    public static Point GetRandomPoint(this ExitGate gate)
    {
        var maxXExclusive = GetExclusiveUpperBound(gate.X1, gate.X2, nameof(gate.X2));
        var maxYExclusive = GetExclusiveUpperBound(gate.Y1, gate.Y2, nameof(gate.Y2));
        return new Point((byte)Rand.NextInt(gate.X1, maxXExclusive), (byte)Rand.NextInt(gate.Y1, maxYExclusive));
    }

    /// <summary>
    /// Gets every coordinate which <see cref="GetRandomPoint"/> can select for the specified gate.
    /// </summary>
    /// <param name="gate">The gate.</param>
    /// <returns>The possible landing points.</returns>
    /// <remarks>
    /// This enumerates the placement domain of <see cref="GetRandomPoint"/>; it is not a general-purpose
    /// enumeration of the gate rectangle, whose upper bounds are treated as inclusive by other consumers.
    /// </remarks>
    internal static IEnumerable<Point> GetPossibleLandingPoints(this ExitGate gate)
    {
        var maxXExclusive = GetExclusiveUpperBound(gate.X1, gate.X2, nameof(gate.X2));
        var maxYExclusive = GetExclusiveUpperBound(gate.Y1, gate.Y2, nameof(gate.Y2));
        for (int x = gate.X1; x < maxXExclusive; x++)
        {
            for (int y = gate.Y1; y < maxYExclusive; y++)
            {
                yield return new Point((byte)x, (byte)y);
            }
        }
    }

    private static int GetExclusiveUpperBound(byte lowerBound, byte upperBound, string parameterName)
    {
        if (upperBound < lowerBound)
        {
            throw new ArgumentOutOfRangeException(parameterName, upperBound, "The upper gate bound must not be lower than its lower bound.");
        }

        return upperBound == lowerBound ? lowerBound + 1 : upperBound;
    }
}
