// <copyright file="PersistenceLockTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;

/// <summary>
/// Tests for the per-player persistence lock (<see cref="MUnique.OpenMU.GameLogic.Player.RunPersistenceExclusiveAsync(System.Func{System.Threading.Tasks.ValueTask},System.Threading.CancellationToken)"/>),
/// which serializes a player's context mutations against its periodic/disconnect progress saves so they
/// can never run concurrently. Without it, a mutation running during <c>SaveChangesAsync</c> corrupts the
/// change tracker and rolls the whole session back.
/// </summary>
[TestFixture]
public class PersistenceLockTest
{
    /// <summary>
    /// Verifies that concurrent exclusive operations for the same player never overlap.
    /// </summary>
    [Test]
    public async Task ConcurrentAccessIsSerializedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var concurrent = 0;
        var overlapDetected = false;

        async ValueTask BodyAsync()
        {
            if (Interlocked.Increment(ref concurrent) > 1)
            {
                overlapDetected = true;
            }

            await Task.Delay(1).ConfigureAwait(false);
            Interlocked.Decrement(ref concurrent);
        }

        var tasks = Enumerable.Range(0, 50)
            .Select(_ => player.RunPersistenceExclusiveAsync(BodyAsync).AsTask())
            .ToArray();
        await Task.WhenAll(tasks).ConfigureAwait(false);

        Assert.That(overlapDetected, Is.False, "Two exclusive operations for the same player ran at the same time.");
    }

    /// <summary>
    /// Verifies that re-entering the lock from within an already-held exclusive scope does not deadlock
    /// (an inline save inside a packet handler is exactly this case).
    /// </summary>
    [Test]
    public async Task ReentrantAccessDoesNotDeadlockAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var executed = 0;

        var run = player.RunPersistenceExclusiveAsync(async () =>
        {
            Interlocked.Increment(ref executed);
            await player.RunPersistenceExclusiveAsync(async () =>
            {
                Interlocked.Increment(ref executed);
                await Task.Yield();
            }).ConfigureAwait(false);
        }).AsTask();

        // If reentrancy deadlocked, this would hang; fail fast instead of blocking the suite.
        await run.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        Assert.That(executed, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies that a re-entrant exclusive operation still runs while another flow holds the lock:
    /// the outer flow keeps the lock, an independent flow must wait, and the re-entrant call inside the
    /// outer flow proceeds without waiting for itself.
    /// </summary>
    [Test]
    public async Task IndependentFlowWaitsWhileLockIsHeldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var otherEntered = false;
        var holderHasLock = new TaskCompletionSource();
        var mayRelease = new TaskCompletionSource();

        // Holder runs on its own flow and keeps the lock until signalled.
        var holder = Task.Run(() => player.RunPersistenceExclusiveAsync(async () =>
        {
            holderHasLock.SetResult();

            // A re-entrant call from the holding flow must NOT block on the lock we already hold.
            await player.RunPersistenceExclusiveAsync(() => ValueTask.CompletedTask).ConfigureAwait(false);

            await mayRelease.Task.ConfigureAwait(false);
        }).AsTask());

        await holderHasLock.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        // Competing flow started from an INDEPENDENT context (does not inherit the reentrancy flag).
        var other = Task.Run(() => player.RunPersistenceExclusiveAsync(() =>
        {
            otherEntered = true;
            return ValueTask.CompletedTask;
        }).AsTask());

        await Task.Delay(50).ConfigureAwait(false);
        Assert.That(otherEntered, Is.False, "An independent flow entered while the lock was held.");

        mayRelease.SetResult();
        await holder.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        await other.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(otherEntered, Is.True, "The competing flow never ran after the lock was released.");
    }
}
