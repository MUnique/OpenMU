// <copyright file="CollectibleAssert.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Asserts that an object is no longer reachable, i.e. that nothing long-living still holds it after it was released.
/// </summary>
/// <remarks>
/// Create the object and its <see cref="WeakReference"/> in a separate, not inlined method. Otherwise the
/// debug JIT keeps the local variable of the test method alive until its end, and the assertion fails for no fault of the code under test.
/// </remarks>
public static class CollectibleAssert
{
    private const int MaximumCollectionAttempts = 3;

    /// <summary>
    /// Asserts that the object of the weak reference gets collected by the garbage collector.
    /// </summary>
    /// <param name="reference">The weak reference to the released object.</param>
    /// <param name="description">The description of the object, for the failure message.</param>
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is what this assertion is for.")]
    public static void IsCollected(WeakReference reference, string description)
    {
        for (var attempt = 0; attempt < MaximumCollectionAttempts && reference.IsAlive; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.That(reference.IsAlive, Is.False, $"The {description} is still reachable after it was released.");
    }

    /// <summary>
    /// Asserts that the object of the weak reference gets collected by the garbage collector within the timeout.
    /// Use it when the release finishes asynchronously, e.g. a background loop which ends after a cancellation.
    /// </summary>
    /// <param name="reference">The weak reference to the released object.</param>
    /// <param name="description">The description of the object, for the failure message.</param>
    /// <param name="timeout">The time to wait for the object to become unreachable.</param>
    /// <returns>The task.</returns>
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is what this assertion is for.")]
    public static async Task IsCollectedAsync(WeakReference reference, string description, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < deadline)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            if (!reference.IsAlive)
            {
                return;
            }

            await Task.Delay(50).ConfigureAwait(false);
        }

        IsCollected(reference, description);
    }
}
