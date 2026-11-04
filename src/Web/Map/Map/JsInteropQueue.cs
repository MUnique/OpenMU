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
/// When the browser can't keep up and the queue is full, further calls are dropped. As soon as the
/// browser received the calls which were queued before, <see cref="ResynchronizationRequired"/> is
/// raised, and the owner rebuilds the state of the browser from scratch (see <see cref="DiscardPending"/>),
/// because that's the only state which is known to be right after a gap. Waiting for the browser to
/// catch up first keeps a browser which doesn't answer at all from causing one rebuild after another.
/// Calls which expect a result are passed through to the inner runtime.
/// </remarks>
public sealed class JsInteropQueue : IJSRuntime, IAsyncDisposable
{
    private const int NotOverflowed = 0;

    private const int Overflowed = 1;

    private const int CaughtUp = 2;

    /// <summary>
    /// The time which the disposal waits for the call which the pump is currently delivering.
    /// </summary>
    private static readonly TimeSpan PumpStopTimeout = TimeSpan.FromSeconds(1);

    private readonly IJSRuntime _inner;
    private readonly ILogger _logger;
    private readonly CircuitConnectionState _circuitState;
    private readonly Channel<QueuedCall> _channel;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly Task _pump;

    /// <summary>
    /// The overflow state, one of <see cref="NotOverflowed"/>, <see cref="Overflowed"/> and <see cref="CaughtUp"/>.
    /// </summary>
    private int _overflowState;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsInteropQueue"/> class.
    /// </summary>
    /// <param name="inner">The runtime which actually calls the browser.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="circuitState">The connection state of the browser circuit. While the browser isn't reachable, calls are dropped.</param>
    public JsInteropQueue(IJSRuntime inner, ILogger logger, CircuitConnectionState circuitState)
    {
        this._inner = inner;
        this._logger = logger;
        this._circuitState = circuitState;
        this._channel = Channel.CreateBounded<QueuedCall>(new BoundedChannelOptions(Capacity)
        {
            SingleReader = true,
            SingleWriter = false,

            // With 'Wait', TryWrite returns false when the queue is full, instead of dropping a call silently.
            FullMode = BoundedChannelFullMode.Wait,
        });
        this._pump = Task.Run(() => this.PumpAsync().AsTask());
    }

    /// <summary>
    /// Occurs once after the queue ran full, when the browser received the calls which were queued before.
    /// </summary>
    /// <remarks>
    /// It's raised on the pump, which doesn't deliver further calls until the handlers returned,
    /// so handlers must not block.
    /// </remarks>
    public event Action? ResynchronizationRequired;

    /// <summary>
    /// Gets the maximum number of calls which wait for the browser. A browser which reaches it stopped
    /// reading, because even the initial population of a crowded map takes far less calls.
    /// </summary>
    public static int Capacity => 4096;

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
            || !this._circuitState.IsConnected)
        {
            return default;
        }

        // After an overflow, nothing is queued until the rebuild discarded the rest. Moving or
        // removing an object whose addition was dropped would only confuse the browser.
        if (Volatile.Read(ref this._overflowState) == NotOverflowed
            && !this._channel.Writer.TryWrite(new QueuedCall(identifier, args, cancellationToken))
            && !this._disposeCts.IsCancellationRequested
            && Interlocked.CompareExchange(ref this._overflowState, Overflowed, NotOverflowed) == NotOverflowed)
        {
            this._logger.LogWarning("The browser doesn't keep up with the live map ({Capacity} calls pending); dropping calls until the map is resynchronized.", Capacity);
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

        Interlocked.Exchange(ref this._overflowState, NotOverflowed);
        this._logger.LogDebug("Discarded {Count} pending live map calls.", discarded);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._disposeCts.IsCancellationRequested)
        {
            return;
        }

        // Cancelled first, so that a call which fails to be written into the completed channel
        // isn't taken for an overflow.
        await this._disposeCts.CancelAsync().ConfigureAwait(false);
        this._channel.Writer.TryComplete();
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
            _ = this._pump.ContinueWith(_ => this._disposeCts.Dispose(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return;
        }

        this._disposeCts.Dispose();
    }

    private async ValueTask PumpAsync()
    {
        var disposeToken = this._disposeCts.Token;
        await foreach (var call in this._channel.Reader.ReadAllAsync(disposeToken).ConfigureAwait(false))
        {
            await this.DeliverAsync(call, disposeToken).ConfigureAwait(false);

            if (this._channel.Reader.Count == 0
                && Interlocked.CompareExchange(ref this._overflowState, CaughtUp, Overflowed) == Overflowed)
            {
                this.ResynchronizationRequired?.Invoke();
            }
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
