// <copyright file="PingClockValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The pure comparison between the client tick count and the server's monotonic clock.
/// It has no dependencies on players, clocks or configuration, so it can be unit-tested
/// deterministically. Policy (logging, tolerance) stays in <see cref="SpeedHackDetectPlugIn"/>.
/// </summary>
internal static class PingClockValidator
{
    /// <summary>
    /// A client tick delta above this value is treated as a restarted client
    /// instead of a clock which is running ahead of the server.
    /// </summary>
    internal const double MaxPlausibleTickDeltaMs = 3_600_000.0;

    /// <summary>
    /// Compares the tick count delta reported by the client with the time which passed on the server.
    /// </summary>
    /// <param name="previousTickCount">The tick count of the previous ping.</param>
    /// <param name="currentTickCount">The tick count of the current ping.</param>
    /// <param name="serverElapsedMs">The monotonic time which passed between both pings.</param>
    /// <param name="toleranceMs">The allowed difference between client and server time.</param>
    /// <returns>The evaluation of the client clock.</returns>
    internal static Result Evaluate(uint previousTickCount, uint currentTickCount, double serverElapsedMs, double toleranceMs)
    {
        // Unsigned subtraction makes the comparison safe across the 32-bit tick count wrap.
        double clientElapsedMs = unchecked(currentTickCount - previousTickCount);
        if (clientElapsedMs > MaxPlausibleTickDeltaMs)
        {
            return new Result(clientElapsedMs, IsClientAhead: false, IsClockReset: true);
        }

        bool isClientAhead = clientElapsedMs > serverElapsedMs + Math.Max(0, toleranceMs);
        return new Result(clientElapsedMs, isClientAhead, IsClockReset: false);
    }

    /// <summary>
    /// The outcome of one client clock comparison.
    /// </summary>
    /// <param name="ClientElapsedMs">The time which passed according to the client tick count.</param>
    /// <param name="IsClientAhead">Whether the client claims more time passed than the server observed, beyond the tolerance.</param>
    /// <param name="IsClockReset">Whether the client tick count restarted and the baseline should be reset.</param>
    internal readonly record struct Result(double ClientElapsedMs, bool IsClientAhead, bool IsClockReset);
}
