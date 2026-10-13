// <copyright file="JsInteropQueue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

/// <summary>
/// An <see cref="IJSRuntime"/> which hands void calls over to a background pump instead of
/// waiting for the browser to answer them.
/// </summary>
/// <remarks>
/// The view plugins of the live map are called by the game logic while it holds the observer lock
/// of the moving object. Awaiting a javascript call there makes every step, kill and skill on the
/// whole map wait for a round trip to the browser of the admin, and a slow or disconnected browser
/// freezes the map for the players. Through this queue, the game logic only pays for writing the
/// call into a channel. The pump delivers the calls in their original order, so an object is still
/// added before it moves and moves before it's removed.
/// When the browser can't keep up and the queue is full, further calls are dropped and
/// <see cref="Overflowed"/> is raised once. The owner then rebuilds the state of the browser from
/// scratch (see <see cref="DiscardPending"/>), because that's the only state which is known to be
/// right after a gap. Calls which expect a result are passed through to the inner runtime.
/// </remarks>
public sealed class JsInteropQueue : IJSRuntime, IAsyncDisposable
{
    /// <summary>
    /// The maximum number of calls which wait for the browser. A browser which reaches it stopped
    /// reading, because even the initial population of a crowded map takes far less calls.
    /// </summary>
    public const int Capacity = 4096;

    /// <summary>
    /// The time which the disposal waits for the call which the pump is currently delivering.
    /// </summary>
    private static readonly TimeSpan PumpStopTimeout = TimeSpan.FromSeconds(1);

    private readonly IJSRuntime _inner;
    private readonly ILogger _logger;
    private readonly Func<bool> _isBrowserConnected;
    private readonly Channel<QueuedCall> _channel;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly Task _pump;

    private int _overflowed;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsInteropQueue"/> class.
    /// </summary>
    /// <param name="inner">The runtime which actually calls the browser.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="isBrowserConnected">Returns if the browser is currently reachable. While it isn't, calls are dropped.</param>
    public JsInteropQueue(IJSRuntime inner, ILogger logger, Func<bool> isBrowserConnected)
    {
        this._inner = inner;
        this._logger = logger;
        this._isBrowserConnected = isBrowserConnected;
        this._channel = Channel.CreateBounded<QueuedCall>(new BoundedChannelOptions(Capacity)
        {
            SingleReader = true,
            SingleWriter = false,

            // With 'Wait', TryWrite returns false when the queue is full, instead of dropping a call silently.
            FullMode = BoundedChannelFullMode.Wait,
        });
        this._pump = Task.Run(this.PumpAsync);
    }

    /// <summary>
    /// Occurs once when the queue ran full and started to drop calls.
    /// </summary>
    /// <remarks>
    /// It's raised on the thread of the dropped call, which may hold a lock of the game map,
    /// so handlers must not block.
    /// </remarks>
    public event Action? Overflowed;

    /// <summary>
    /// Gets the number of calls which wait for the browser.
    /// </summary>
    public int PendingCount => this._channel.Reader.Count;

    /// <inheritdoc />
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return this.InvokeAsync<TValue>(identifier, CancellationToken.None, args);
    }

    /// <inheritdoc />
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        if (typeof(TValue) != typeof(IJSVoidResult))
        {
            return this._inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
        }

        if (this._disposeCts.IsCancellationRequested
            || cancellationToken.IsCancellationRequested
            || !this._isBrowserConnected())
        {
            return default;
        }

        // After an overflow, nothing is queued until the rebuild discarded the rest. Moving or
        // removing an object whose addition was dropped would only confuse the browser.
        if (Volatile.Read(ref this._overflowed) == 0
            && !this._channel.Writer.TryWrite(new QueuedCall(identifier, args, cancellationToken))
            && Interlocked.Exchange(ref this._overflowed, 1) == 0)
        {
            this._logger.LogWarning("The browser doesn't keep up with the live map ({Capacity} calls pending); dropping calls until the map is resynchronized.", Capacity);
            this.Overflowed?.Invoke();
        }

        return default;
    }

    /// <summary>
    /// Throws away every call which wasn't delivered yet, and accepts calls again after an overflow.
    /// </summary>
    /// <remarks>
    /// Call it when the state of the browser is about to be rebuilt from scratch, before the
    /// first call of the rebuild is queued.
    /// </remarks>
    public void DiscardPending()
    {
        var discarded = 0;
        while (this._channel.Reader.TryRead(out _))
        {
            discarded++;
        }

        Interlocked.Exchange(ref this._overflowed, 0);
        this._logger.LogDebug("Discarded {Count} pending live map calls.", discarded);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._disposeCts.IsCancellationRequested)
        {
            return;
        }

        this._channel.Writer.TryComplete();
        await this._disposeCts.CancelAsync().ConfigureAwait(false);
        try
        {
            // Bounded, because a call which the browser never answers would hold the disposal forever.
            await this._pump.WaitAsync(PumpStopTimeout).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected
        }
        catch (TimeoutException)
        {
            // The pump ends with that call. The token source has to stay alive until then.
            return;
        }

        this._disposeCts.Dispose();
    }

    private async Task PumpAsync()
    {
        var disposeToken = this._disposeCts.Token;
        await foreach (var call in this._channel.Reader.ReadAllAsync(disposeToken).ConfigureAwait(false))
        {
            await this.DeliverAsync(call, disposeToken).ConfigureAwait(false);
        }
    }

    private async ValueTask DeliverAsync(QueuedCall call, CancellationToken disposeToken)
    {
        if (disposeToken.IsCancellationRequested || call.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await this._inner.InvokeVoidAsync(call.Identifier, call.CancellationToken, call.Args ?? []).ConfigureAwait(false);
        }
        catch (Exception e) when (e is OperationCanceledException or JSDisconnectedException or ObjectDisposedException)
        {
            // Cancelled, or the browser is away. The map is resynchronized when it's back.
        }
        catch (JSException e) when (e.Message.StartsWith("Could not find '", StringComparison.Ordinal))
        {
            // The map was already disposed on the browser side, so there is nothing to deliver it to.
            this._logger.LogDebug(e, "{Identifier} is not available on the live map.", call.Identifier);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Error when calling {Identifier} on the live map.", call.Identifier);
        }
    }

    private readonly record struct QueuedCall(string Identifier, object?[]? Args, CancellationToken CancellationToken);
}
