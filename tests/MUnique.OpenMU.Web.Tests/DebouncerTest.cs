// <copyright file="DebouncerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.Threading;
using MUnique.OpenMU.Web.Shared.Components;

/// <summary>
/// Tests for <see cref="Debouncer"/>.
/// </summary>
[TestFixture]
public class DebouncerTests
{
    /// <summary>
    /// Tests that the constructor throws <see cref="ArgumentOutOfRangeException"/> when delay is zero.
    /// </summary>
    [Test]
    public void Constructor_WithZeroDelay_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Debouncer(0));
    }

    /// <summary>
    /// Tests that the constructor throws <see cref="ArgumentOutOfRangeException"/> when delay is negative.
    /// </summary>
    [Test]
    public void Constructor_WithNegativeDelay_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Debouncer(-1));
    }

    /// <summary>
    /// Tests that a single debounced action executes successfully.
    /// </summary>
    [Test]
    public async Task DebounceAsync_SingleCall_ExecutesAction()
    {
        using var debouncer = new Debouncer(50);
        var executed = false;

        await debouncer.DebounceAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Tests that rapid consecutive calls only execute the last action.
    /// </summary>
    [Test]
    public async Task DebounceAsync_RapidCalls_OnlyLastExecutes()
    {
        using var debouncer = new Debouncer(100);
        var callCount = 0;
        var lastValue = string.Empty;

        // Fire 5 rapid calls
        var tasks = new List<Task>();
        for (var i = 1; i <= 5; i++)
        {
            var captured = i;
            tasks.Add(debouncer.DebounceAsync(() =>
            {
                Interlocked.Increment(ref callCount);
                lastValue = $"call-{captured}";
                return Task.CompletedTask;
            }));

            if (i < 5)
            {
                await Task.Delay(20); // rapid but not instant
            }
        }

        await Task.WhenAll(tasks);

        Assert.That(callCount, Is.EqualTo(1));
        Assert.That(lastValue, Is.EqualTo("call-5"));
    }

    /// <summary>
    /// Tests that the cancellation token overload passes the token to the action.
    /// </summary>
    [Test]
    public async Task DebounceAsync_WithCancellationToken_PassesTokenToAction()
    {
        using var debouncer = new Debouncer(50);
        CancellationToken receivedToken = default;

        await debouncer.DebounceAsync(token =>
        {
            receivedToken = token;
            return Task.CompletedTask;
        });

        Assert.That(receivedToken.IsCancellationRequested, Is.False);
    }

    /// <summary>
    /// Tests that calling DebounceAsync after disposal returns without executing the action.
    /// </summary>
    [Test]
    public async Task DebounceAsync_AfterDispose_ReturnsWithoutExecuting()
    {
        var debouncer = new Debouncer(50);
        debouncer.Dispose();
        var executed = false;

        await debouncer.DebounceAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Tests that calling Cancel while an action is pending prevents its execution.
    /// </summary>
    [Test]
    public async Task Cancel_WhilePending_PreventsExecution()
    {
        using var debouncer = new Debouncer(200);
        var executed = false;

        var task = debouncer.DebounceAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        await Task.Delay(50);
        debouncer.Cancel();
        await task;

        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Tests that passing a null action throws ArgumentNullException.
    /// </summary>
    [Test]
    public void DebounceAsync_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var debouncer = new Debouncer(50);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => 
            debouncer.DebounceAsync((Func<Task>)null!));
    }

    /// <summary>
    /// Tests that passing a null cancellable action throws ArgumentNullException.
    /// </summary>
    [Test]
    public void DebounceAsync_WithNullCancellableAction_ThrowsArgumentNullException()
    {
        // Arrange
        using var debouncer = new Debouncer(50);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => 
            debouncer.DebounceAsync((Func<CancellationToken, Task>)null!));
    }

    /// <summary>
    /// Tests that actions spaced beyond the debounce window each execute independently.
    /// </summary>
    [Test]
    public async Task DebounceAsync_SpacedCalls_EachExecutes()
    {
        using var debouncer = new Debouncer(50);
        var callCount = 0;

        // Two calls spaced apart beyond the debounce window
        await debouncer.DebounceAsync(() =>
        {
            Interlocked.Increment(ref callCount);
            return Task.CompletedTask;
        });

        await Task.Delay(100);

        await debouncer.DebounceAsync(() =>
        {
            Interlocked.Increment(ref callCount);
            return Task.CompletedTask;
        });

        Assert.That(callCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that disposing multiple times does not throw an exception.
    /// </summary>
    [Test]
    public void Dispose_MultipleTimes_DoesNotThrow()
    {
        var debouncer = new Debouncer(50);

        Assert.DoesNotThrow(() =>
        {
            debouncer.Dispose();
            debouncer.Dispose();
        });
    }
}