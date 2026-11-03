// <copyright file="EventExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests;

using System.Threading;

/// <summary>
/// Tests for the <see cref="EventExtensions"/>: every subscriber of an async event is awaited, not
/// only the last one, which a direct invocation of the multicast delegate would return.
/// </summary>
[TestFixture]
public class EventExtensionsTest
{
    /// <summary>
    /// All subscribers ran to completion when the invocation completes, in their order.
    /// </summary>
    [Test]
    public async ValueTask AwaitsEverySubscriberAsync()
    {
        var completed = new List<int>();
        AsyncEventHandler<int>? handler = null;
        handler += async value =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            lock (completed)
            {
                completed.Add(value);
            }
        };
        handler += value =>
        {
            lock (completed)
            {
                completed.Add(value * 10);
            }

            return ValueTask.CompletedTask;
        };

        await handler.SafeInvokeAsync(1).ConfigureAwait(false);

        Assert.That(completed, Is.EqualTo(new[] { 1, 10 }));
    }

    /// <summary>
    /// The same for the event without arguments.
    /// </summary>
    [Test]
    public async ValueTask AwaitsEverySubscriberWithoutArgumentsAsync()
    {
        var completed = 0;
        AsyncEventHandler? handler = null;
        handler += async () =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            Interlocked.Increment(ref completed);
        };
        handler += () =>
        {
            Interlocked.Increment(ref completed);
            return ValueTask.CompletedTask;
        };

        await handler.SafeInvokeAsync().ConfigureAwait(false);

        Assert.That(completed, Is.EqualTo(2));
    }

    /// <summary>
    /// A subscriber which throws doesn't keep the following subscribers from running, and its
    /// exception is thrown afterwards.
    /// </summary>
    [Test]
    public void ThrowsAfterAllSubscribersRan()
    {
        var secondRan = false;
        AsyncEventHandler<int>? handler = null;
        handler += _ => throw new InvalidOperationException("first");
        handler += _ =>
        {
            secondRan = true;
            return ValueTask.CompletedTask;
        };

        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => handler.SafeInvokeAsync(1).AsTask());

        Assert.That(exception!.Message, Is.EqualTo("first"));
        Assert.That(secondRan, Is.True);
    }

    /// <summary>
    /// When several subscribers throw, all exceptions are thrown together.
    /// </summary>
    [Test]
    public void ThrowsAllExceptionsOfSeveralSubscribers()
    {
        AsyncEventHandler<int>? handler = null;
        handler += _ => throw new InvalidOperationException("first");
        handler += async _ =>
        {
            await Task.Yield();
            throw new ArgumentException("second");
        };

        var exception = Assert.ThrowsAsync<AggregateException>(() => handler.SafeInvokeAsync(1).AsTask());

        Assert.That(exception!.InnerExceptions.Select(e => e.Message), Is.EqualTo(new[] { "first", "second" }));
    }
}
