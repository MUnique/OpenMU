// <copyright file="Measure.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using System.Diagnostics;

/// <summary>
/// Helpers to measure and print durations and allocations.
/// </summary>
internal static class Measure
{
    /// <summary>
    /// Measures the time and the allocated bytes of the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The result.</returns>
    public static Result Run(Action action)
    {
        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return new Result(stopwatch.Elapsed, GC.GetTotalAllocatedBytes(true) - allocatedBefore);
    }

    /// <summary>
    /// Measures the time and the allocated bytes of the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="function">The function.</param>
    /// <returns>The measurement result and the return value of the function.</returns>
    public static async ValueTask<(Result Result, T Value)> RunAsync<T>(Func<ValueTask<T>> function)
    {
        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var stopwatch = Stopwatch.StartNew();
        var value = await function().ConfigureAwait(false);
        stopwatch.Stop();
        return (new Result(stopwatch.Elapsed, GC.GetTotalAllocatedBytes(true) - allocatedBefore), value);
    }

    /// <summary>
    /// Prints the header of a section.
    /// </summary>
    /// <param name="title">The title.</param>
    public static void Section(string title)
    {
        Console.WriteLine($"## {title}");
        Console.WriteLine();
        Console.WriteLine("| Measurement | Time (ms) | Allocated (MB) | Note |");
        Console.WriteLine("|---|---:|---:|---|");
    }

    /// <summary>
    /// Prints a row of a section.
    /// </summary>
    /// <param name="name">The name of the measurement.</param>
    /// <param name="result">The result.</param>
    /// <param name="note">An optional note.</param>
    public static void Row(string name, Result result, string? note = null)
    {
        Console.WriteLine($"| {name} | {result.Duration.TotalMilliseconds:0.0} | {result.AllocatedBytes / 1024.0 / 1024.0:0.0} | {note} |");
    }

    /// <summary>
    /// Prints a row with the cold (first) result and the median of the warm results.
    /// </summary>
    /// <param name="name">The name of the measurement.</param>
    /// <param name="results">The results; the first one is considered as cold.</param>
    /// <param name="note">An optional note.</param>
    public static void ColdAndWarmRows(string name, IReadOnlyList<Result> results, string? note = null)
    {
        Row($"{name} (cold)", results[0], note);
        if (results.Count > 1)
        {
            var warm = results.Skip(1).OrderBy(r => r.Duration).ToList();
            var median = warm[warm.Count / 2];
            Row($"{name} (warm, median of {warm.Count})", median, note);
        }
    }

    /// <summary>
    /// The result of a measurement.
    /// </summary>
    /// <param name="Duration">The duration.</param>
    /// <param name="AllocatedBytes">The allocated bytes.</param>
    public readonly record struct Result(TimeSpan Duration, long AllocatedBytes);
}
