// <copyright file="JsInteropQueueTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MUnique.OpenMU.Web.Map;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Tests for the <see cref="JsInteropQueue"/>.
/// </summary>
[TestFixture]
public class JsInteropQueueTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Tests that a void call returns before the browser answered it.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task VoidCallDoesNotWaitForTheBrowserAsync()
    {
        var browser = new BlockingBrowser();
        await using var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, new CircuitConnectionState());

        var call = queue.InvokeVoidAsync("map.world.objectMoved", 1);

        Assert.That(call.IsCompleted, Is.True);
        Assert.That(browser.Started.Wait(Timeout), Is.True, "the pump delivers the call");
        browser.Release();
    }

    /// <summary>
    /// Tests that the calls reach the browser in the order in which they were made.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task CallsAreDeliveredInOrderAsync()
    {
        var browser = new RecordingBrowser();
        await using var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, new CircuitConnectionState());

        for (var i = 0; i < 100; i++)
        {
            await queue.InvokeVoidAsync("call", i).ConfigureAwait(false);
        }

        Assert.That(SpinWait.SpinUntil(() => browser.Calls.Count == 100, Timeout), Is.True);
        Assert.That(browser.Calls.Select(c => (int)c[0]!), Is.EqualTo(Enumerable.Range(0, 100)));
    }

    /// <summary>
    /// Tests that calls are dropped while the browser is disconnected.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task CallsAreDroppedWhileDisconnectedAsync()
    {
        var browser = new RecordingBrowser();
        var circuitState = new CircuitConnectionState();
        await using var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, circuitState);

        await circuitState.OnConnectionDownAsync(null!, CancellationToken.None).ConfigureAwait(false);
        await queue.InvokeVoidAsync("call", 1).ConfigureAwait(false);
        await circuitState.OnConnectionUpAsync(null!, CancellationToken.None).ConfigureAwait(false);
        await queue.InvokeVoidAsync("call", 2).ConfigureAwait(false);

        Assert.That(SpinWait.SpinUntil(() => !browser.Calls.IsEmpty, Timeout), Is.True);
        Assert.That(browser.Calls.Select(c => (int)c[0]!), Is.EqualTo(new[] { 2 }));
    }

    /// <summary>
    /// Tests that a full queue drops further calls, and that the resynchronization is requested
    /// once the browser received the calls which were queued before, but not while it doesn't answer.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task ResynchronizationIsRequestedWhenTheBrowserCaughtUpAsync()
    {
        var browser = new BlockingBrowser();
        await using var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, new CircuitConnectionState());
        var resynchronizations = 0;
        queue.ResynchronizationRequired += () => Interlocked.Increment(ref resynchronizations);

        // The first call is taken by the pump and blocks there, the others fill the channel.
        await queue.InvokeVoidAsync("call", -1).ConfigureAwait(false);
        Assert.That(browser.Started.Wait(Timeout), Is.True);
        for (var i = 0; i < JsInteropQueue.Capacity + 10; i++)
        {
            await queue.InvokeVoidAsync("call", i).ConfigureAwait(false);
        }

        Assert.That(queue.PendingCount, Is.EqualTo(JsInteropQueue.Capacity));
        Assert.That(resynchronizations, Is.Zero, "a browser which doesn't answer gets no rebuild");

        browser.Release();
        Assert.That(SpinWait.SpinUntil(() => Volatile.Read(ref resynchronizations) == 1, Timeout), Is.True);

        await queue.InvokeVoidAsync("dropped", 1).ConfigureAwait(false);
        Assert.That(queue.PendingCount, Is.Zero, "calls are dropped until the pending ones were discarded");

        queue.DiscardPending();
        await queue.InvokeVoidAsync("accepted", 2).ConfigureAwait(false);
        Assert.That(SpinWait.SpinUntil(() => browser.Identifiers.Contains("accepted"), Timeout), Is.True);
        Assert.That(browser.Identifiers, Does.Not.Contain("dropped"));
        Assert.That(resynchronizations, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a call which expects a result is passed through instead of being queued.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task CallWithResultIsPassedThroughAsync()
    {
        var browser = new Mock<IJSRuntime>();
        browser.Setup(r => r.InvokeAsync<int>("answer", It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
            .Returns(ValueTask.FromResult(42));
        await using var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, new CircuitConnectionState());

        var result = await queue.InvokeAsync<int>("answer", CancellationToken.None, []).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(42));
    }

    /// <summary>
    /// Tests that the disposal doesn't wait for a browser which never answers.
    /// </summary>
    /// <returns>The task of this test.</returns>
    [Test]
    public async Task DisposeDoesNotWaitForAnUnansweredCallAsync()
    {
        var browser = new BlockingBrowser();
        var queue = new JsInteropQueue(browser.Object, NullLogger.Instance, new CircuitConnectionState());
        await queue.InvokeVoidAsync("call", 1).ConfigureAwait(false);
        Assert.That(browser.Started.Wait(Timeout), Is.True);

        var dispose = queue.DisposeAsync().AsTask();

        Assert.That(await Task.WhenAny(dispose, Task.Delay(Timeout)).ConfigureAwait(false), Is.SameAs(dispose));
        browser.Release();
    }

    /// <summary>
    /// A browser which records the calls.
    /// </summary>
    private sealed class RecordingBrowser
    {
        private readonly Mock<IJSRuntime> _mock = new();

        public RecordingBrowser()
        {
            this._mock
                .Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
                .Returns((string _, CancellationToken _, object?[] args) =>
                {
                    this.Calls.Enqueue(args);
                    return ValueTask.FromResult<IJSVoidResult>(null!);
                });
        }

        public IJSRuntime Object => this._mock.Object;

        public ConcurrentQueue<object?[]> Calls { get; } = new();
    }

    /// <summary>
    /// A browser which doesn't answer until it's released.
    /// </summary>
    private sealed class BlockingBrowser
    {
        private readonly Mock<IJSRuntime> _mock = new();
        private readonly TaskCompletionSource<IJSVoidResult> _answer = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public BlockingBrowser()
        {
            this._mock
                .Setup(r => r.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object?[]>()))
                .Returns((string identifier, CancellationToken _, object?[] _) =>
                {
                    this.Identifiers.Enqueue(identifier);
                    this.Started.Set();
                    return new ValueTask<IJSVoidResult>(this._answer.Task);
                });
        }

        public IJSRuntime Object => this._mock.Object;

        public ManualResetEventSlim Started { get; } = new();

        public ConcurrentQueue<string> Identifiers { get; } = new();

        public void Release() => this._answer.TrySetResult(null!);
    }
}
