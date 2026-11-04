// <copyright file="DiscordBackgroundQueueTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord;

/// <summary>
/// Tests the bounded lifecycle of background notifications.
/// </summary>
[TestFixture]
public class DiscordBackgroundQueueTest
{
    /// <summary>
    /// Tests shutdown before Ready, including pending work and a full queue.
    /// </summary>
    [Test]
    public async Task ShutdownCancelsReadyWaitAndRejectsPendingWorkAsync()
    {
        var queue = new DiscordBackgroundQueue(NullLogger.Instance, 1);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var pendingRan = false;
        Assert.That(queue.Enqueue(async cancellationToken =>
        {
            started.SetResult();
            await ready.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        }), Is.True);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(queue.Enqueue(_ =>
        {
            pendingRan = true;
            return Task.CompletedTask;
        }), Is.True);
        Assert.That(queue.Enqueue(_ => Task.CompletedTask), Is.False);
        await queue.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(pendingRan, Is.False);
        Assert.That(queue.Enqueue(_ => Task.CompletedTask), Is.False);
    }

    /// <summary>
    /// Tests that a failed notification does not stop later work.
    /// </summary>
    [TestCase(false)]
    [TestCase(true)]
    public async Task FailureDoesNotStopQueueAsync(bool operationTimedOut)
    {
        await using var queue = new DiscordBackgroundQueue(NullLogger.Instance);
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        queue.Enqueue(_ => Task.FromException(operationTimedOut ? new OperationCanceledException("request timed out") : new InvalidOperationException("failed")));
        queue.Enqueue(_ =>
        {
            completed.SetResult();
            return Task.CompletedTask;
        });
        await completed.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
    }
}
