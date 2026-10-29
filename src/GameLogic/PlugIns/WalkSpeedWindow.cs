// <copyright file="WalkSpeedWindow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Sums up the walking time which the distance walked by a player costs within a sliding time window.
/// It stores a running total and only the samples which are needed to look the total up for a
/// point in time one window ago, so the cost of any window length is a subtraction.
/// Not thread-safe: the caller has to synchronize access, and it has no clock of its own, so it
/// can be unit-tested deterministically.
/// </summary>
internal sealed class WalkSpeedWindow
{
    /// <summary>
    /// The minimum time between two stored samples in milliseconds.
    /// A named constant instead of configuration: it only bounds the memory of a player which floods the
    /// server with walk packets. A newer cost within this time is added to the last sample, which shifts it
    /// by at most this time and is negligible compared to a window of seconds.
    /// </summary>
    internal const double MinSampleSpacingMs = 50.0;

    /// <summary>
    /// The number of consumed samples at the start of the list from which on they are removed.
    /// A named constant instead of configuration: it's a performance detail of the sample list.
    /// </summary>
    private const int CompactionThreshold = 64;

    private readonly List<Sample> _samples = new();

    private int _firstActiveIndex;

    private double _totalCostMs;

    private double _baselineCostMs;

    /// <summary>
    /// Gets the number of stored samples which are newer than the start of the last evaluated window.
    /// </summary>
    internal int SampleCount => this._samples.Count - this._firstActiveIndex;

    /// <summary>
    /// Forgets everything. The next window starts empty.
    /// </summary>
    public void Clear()
    {
        this._samples.Clear();
        this._firstActiveIndex = 0;
        this._totalCostMs = 0;
        this._baselineCostMs = 0;
    }

    /// <summary>
    /// Adds the cost of a walked distance and returns the cost of the whole window which ends now.
    /// </summary>
    /// <param name="nowMs">The current time in milliseconds of a monotonic clock.</param>
    /// <param name="costMs">The walking time which the walked distance costs.</param>
    /// <param name="windowMs">The length of the window in milliseconds.</param>
    /// <returns>The summed walking time of the distance walked within the last <paramref name="windowMs"/>.</returns>
    public double Add(double nowMs, double costMs, double windowMs)
    {
        this._totalCostMs += costMs;

        var lastIndex = this._samples.Count - 1;
        if (lastIndex >= this._firstActiveIndex && nowMs - this._samples[lastIndex].TimeMs < MinSampleSpacingMs)
        {
            this._samples[lastIndex] = new Sample(this._samples[lastIndex].TimeMs, this._totalCostMs);
        }
        else
        {
            this._samples.Add(new Sample(nowMs, this._totalCostMs));
        }

        return this.GetCost(nowMs, windowMs);
    }

    /// <summary>
    /// Gets the cost of the whole window which ends now, without adding a new sample.
    /// </summary>
    /// <param name="nowMs">The current time in milliseconds of a monotonic clock.</param>
    /// <param name="windowMs">The length of the window in milliseconds.</param>
    /// <returns>The summed walking time of the distance walked within the last <paramref name="windowMs"/>.</returns>
    public double GetCost(double nowMs, double windowMs)
    {
        // The total is a step function of time. Its value at the start of the window is the one of
        // the latest sample which isn't newer than that. Everything above it was walked within the window,
        // even when the player stood still for longer than the window before.
        var windowStartMs = nowMs - windowMs;
        while (this._firstActiveIndex < this._samples.Count && this._samples[this._firstActiveIndex].TimeMs <= windowStartMs)
        {
            this._baselineCostMs = this._samples[this._firstActiveIndex].TotalCostMs;
            this._firstActiveIndex++;
        }

        if (this._firstActiveIndex >= CompactionThreshold && this._firstActiveIndex * 2 >= this._samples.Count)
        {
            this._samples.RemoveRange(0, this._firstActiveIndex);
            this._firstActiveIndex = 0;
        }

        return this._totalCostMs - this._baselineCostMs;
    }

    private readonly record struct Sample(double TimeMs, double TotalCostMs);
}
