// <copyright file="SkippableDelayTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Tests for the <see cref="SkippableDelay"/>.
/// </summary>
[TestFixture]
public class SkippableDelayTests
{
    /// <summary>
    /// Tests that a skip requested during a running wait cuts it short.
    /// </summary>
    [Test]
    public async Task SkipDuringWaitCutsItShortAsync()
    {
        var delay = this.CreateDelay();
        var waitTask = delay.WaitAsync(TimeSpan.FromDays(1), CancellationToken.None);

        await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
        Assert.That(delay.IsWaitActive, Is.True);
        Assert.That(delay.TrySkip(), Is.True);

        Assert.That(await waitTask.ConfigureAwait(false), Is.True);
        Assert.That(delay.IsWaitActive, Is.False);
    }

    /// <summary>
    /// Tests that a skip without a running wait reports failure...
    /// </summary>
    [Test]
    public void SkipWithoutActiveWaitReturnsFalse()
    {
        var delay = this.CreateDelay();

        Assert.That(delay.TrySkip(), Is.False);
    }

    /// <summary>
    /// Tests that a skip without a running wait doesn't leak into the next wait.
    /// A stale skip must never brick a later, unrelated phase of an event.
    /// </summary>
    [Test]
    public async Task SkipWithoutActiveWaitDoesNotAffectNextWaitAsync()
    {
        var delay = this.CreateDelay();

        Assert.That(delay.TrySkip(), Is.False);

        Assert.That(await delay.WaitAsync(TimeSpan.FromMilliseconds(50), CancellationToken.None).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a wait without a skip runs its full duration.
    /// </summary>
    [Test]
    public async Task WaitWithoutSkipRunsFullDurationAsync()
    {
        var delay = this.CreateDelay();

        Assert.That(await delay.WaitAsync(TimeSpan.FromMilliseconds(50), CancellationToken.None).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a cancelled wait throws.
    /// </summary>
    [Test]
    public void CancelledWaitThrows()
    {
        var delay = this.CreateDelay();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.ThrowsAsync<TaskCanceledException>(() => delay.WaitAsync(TimeSpan.FromMinutes(1), cancellation.Token));
    }

    /// <summary>
    /// Tests that a zero duration returns without waiting.
    /// </summary>
    [Test]
    public async Task ZeroDurationReturnsImmediatelyAsync()
    {
        var delay = this.CreateDelay();

        Assert.That(await delay.WaitAsync(TimeSpan.Zero, CancellationToken.None).ConfigureAwait(false), Is.False);
        Assert.That(delay.IsWaitActive, Is.False);
    }

    private SkippableDelay CreateDelay()
    {
        return new SkippableDelay(NullLoggerFactory.Instance.CreateLogger("test"), new object());
    }
}
