// <copyright file="WalkSpeedValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The pure math of the walk speed check: the distance of the walk steps which the server executed
/// within a sliding time window costs a certain amount of walking time, and that cost must not exceed
/// the time which actually passed within the window (plus a tolerance).
/// It has no dependencies on players, clocks or configuration, so it can be unit-tested deterministically.
/// Policy (tolerance, punishment) stays in the plugin and its collaborators.
/// </summary>
/// <remarks>
/// The check deliberately doesn't look at the timing of walk packets. Only steps which the server
/// actually executed are counted, with their execution time, so a walk request which runs ahead of the
/// character can't inflate the measured distance. Over a long window, only a sustained pace above the
/// step delay can exceed the budget.
/// </remarks>
internal static class WalkSpeedValidator
{
    /// <summary>
    /// The default length of the sliding window in seconds.
    /// Also the default of <see cref="SpeedHackDetectConfiguration.WalkSpeedWindowSeconds"/>.
    /// </summary>
    internal const int DefaultWindowSeconds = 15;

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
    /// <c>toleranceMs</c>, not this margin.
    /// </summary>
    internal const double BaseStepDelayMarginMs = 50.0;

    /// <summary>
    /// The floor for the expected per-tile time.
    /// A named constant instead of configuration: below this, a client could
    /// no longer animate the step, so anything faster is suspicious by itself.
    /// </summary>
    internal const double MinStepDelayMs = 50.0;

    /// <summary>
    /// Resolves the configured window length, falling back to the default for
    /// existing databases which predate the setting and deserialize it as 0.
    /// </summary>
    /// <param name="configuredSeconds">The configured window length in seconds.</param>
    /// <returns>The window length in seconds.</returns>
    internal static int ResolveWindowSeconds(int configuredSeconds)
    {
        return configuredSeconds > 0 ? configuredSeconds : DefaultWindowSeconds;
    }

    /// <summary>
    /// Gets the minimum time which walking the given distance takes, with the margin applied.
    /// </summary>
    /// <param name="distanceInTiles">
    /// The distance of one executed walk step, in tiles. It is the euclidean distance, because the
    /// client takes longer for a diagonal step (see <see cref="PlayerMovement"/>).
    /// </param>
    /// <param name="stepDelayMs">The current step delay of the player.</param>
    /// <returns>The expected time in milliseconds.</returns>
    internal static double GetExpectedTimeMs(double distanceInTiles, double stepDelayMs)
    {
        double scalingFactor = stepDelayMs / NormalStepDelayMs;
        double stepDelayMarginMs = BaseStepDelayMarginMs * scalingFactor;
        double checkStepDelayMs = Math.Max(Math.Min(MinStepDelayMs, stepDelayMs), stepDelayMs - stepDelayMarginMs);
        return distanceInTiles * checkStepDelayMs;
    }

    /// <summary>
    /// Gets the minimum time which one executed walk step takes, with the margin applied.
    /// </summary>
    /// <param name="distanceInTiles">The euclidean distance of the step, in tiles.</param>
    /// <param name="pace">The pacing delay which the walker used for the whole step.</param>
    /// <returns>The expected time in milliseconds.</returns>
    internal static double GetExpectedTimeMs(double distanceInTiles, TimeSpan pace)
    {
        double stepDelayMs = pace.TotalMilliseconds / Math.Max(1.0, distanceInTiles);
        return GetExpectedTimeMs(distanceInTiles, stepDelayMs);
    }

    /// <summary>
    /// Evaluates the walking time of a window against the time which passed within it.
    /// </summary>
    /// <param name="windowCostMs">The summed expected walking time of the distance within the window.</param>
    /// <param name="windowMs">The length of the window, which is the time which passed.</param>
    /// <param name="toleranceMs">
    /// The extra time which the executed steps may cost beyond the time which passed,
    /// e.g. because of timer granularity or bursty server step execution.
    /// </param>
    /// <returns>The verdict.</returns>
    internal static Verdict Evaluate(double windowCostMs, double windowMs, double toleranceMs)
    {
        double budgetMs = windowMs + Math.Max(0, toleranceMs);
        return windowCostMs > budgetMs
            ? new Verdict(true, windowCostMs, budgetMs, windowCostMs - budgetMs)
            : new Verdict(false, windowCostMs, budgetMs, 0);
    }

    /// <summary>
    /// The outcome of one walk speed evaluation.
    /// </summary>
    /// <param name="IsViolation">Whether the walking time within the window exceeded the time budget.</param>
    /// <param name="ExpectedTimeMs">The walking time which the distance within the window costs.</param>
    /// <param name="BudgetMs">The time budget of the window, including the tolerance.</param>
    /// <param name="OvershootMs">By how much the cost exceeded the budget; zero when accepted.</param>
    internal readonly record struct Verdict(bool IsViolation, double ExpectedTimeMs, double BudgetMs, double OvershootMs);
}
