// <copyright file="WalkSpeedValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The pure time-budget math of the walk speed check: passed time earns credit,
/// walked tiles spend it. It has no dependencies on players, clocks or configuration,
/// so it can be unit-tested deterministically. Policy (tolerance, punishment) stays
/// in <see cref="SpeedHackDetectPlugIn"/>.
/// </summary>
internal static class WalkSpeedValidator
{
    /// <summary>
    /// The step delay of a normal character, the reference for scaling.
    /// A named constant instead of configuration: it mirrors the client pace
    /// calculation in <see cref="PlayerMovement"/>, not server policy.
    /// </summary>
    internal const double NormalStepDelayMs = 300.0;

    /// <summary>
    /// The margin subtracted from the step delay before comparing, per tile.
    /// A named constant instead of configuration: it absorbs timer granularity
    /// on both ends. Widening the effective tolerance is the caller's
    /// <c>maxCreditMs</c>, not this margin.
    /// </summary>
    internal const double BaseStepDelayMarginMs = 50.0;

    /// <summary>
    /// The floor for the expected per-tile time.
    /// A named constant instead of configuration: below this, a client could
    /// no longer animate the step, so anything faster is suspicious by itself.
    /// </summary>
    internal const double MinStepDelayMs = 50.0;

    /// <summary>
    /// Evaluates one walk against the earned time budget.
    /// </summary>
    /// <param name="storedCreditMs">The credit kept from previous walks.</param>
    /// <param name="maxCreditMs">The maximum credit a walk may start with.</param>
    /// <param name="elapsedMs">The time passed since the previous walk.</param>
    /// <param name="chargedTiles">The tiles charged for this walk.</param>
    /// <param name="stepDelayMs">The current step delay of the player.</param>
    /// <returns>The verdict with the remaining credit.</returns>
    internal static Verdict Evaluate(double storedCreditMs, double maxCreditMs, double elapsedMs, int chargedTiles, double stepDelayMs)
    {
        double scalingFactor = stepDelayMs / NormalStepDelayMs;
        double stepDelayMarginMs = BaseStepDelayMarginMs * scalingFactor;
        double checkStepDelayMs = Math.Max(Math.Min(MinStepDelayMs, stepDelayMs), stepDelayMs - stepDelayMarginMs);
        double expectedTimeMs = chargedTiles * checkStepDelayMs;

        // Note the order: the elapsed time is added before capping, so a walk which took
        // longer than its budget is always accepted, no matter how small the tolerance is.
        // Capping first would discard idle time beyond the tolerance and flag slow multi-tile
        // walks (e.g. 4 tiles in 2.3 s with an 800 ms tolerance). Banking stays bounded:
        // the stored credit never exceeds maxCreditMs, so a burst can only spend what was
        // actually earned, plus the time which really passed.
        double updatedCreditMs = Math.Min(maxCreditMs, storedCreditMs + elapsedMs - expectedTimeMs);
        if (updatedCreditMs < 0)
        {
            // The deficit is not carried over, so a single violation doesn't cause a chain of follow-up violations.
            return new Verdict(true, 0, expectedTimeMs, Math.Max(0, expectedTimeMs - storedCreditMs - elapsedMs));
        }

        return new Verdict(false, updatedCreditMs, expectedTimeMs, 0);
    }

    /// <summary>
    /// The outcome of one walk speed evaluation.
    /// </summary>
    /// <param name="IsViolation">Whether the walked tiles exceeded the earned time budget.</param>
    /// <param name="NewCreditMs">The remaining credit, or zero after a violation.</param>
    /// <param name="ExpectedTimeMs">The time budget the walked tiles cost.</param>
    /// <param name="DeficitMs">By how much the cost exceeded the available budget; zero when accepted.</param>
    internal readonly record struct Verdict(bool IsViolation, double NewCreditMs, double ExpectedTimeMs, double DeficitMs);
}
