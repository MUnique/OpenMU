// <copyright file="Debouncer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a reusable debounce mechanism that delays execution of an async action
/// until a specified quiet period has elapsed since the last invocation.
/// </summary>
/// <remarks>
/// Thread-safe with respect to rapid sequential calls. Only the last invocation
/// within the debounce window will execute.
/// </remarks>
public sealed class Debouncer : IDisposable
{
    private readonly int _delayMs;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _cts;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Debouncer"/> class.
    /// </summary>
    /// <param name="delayMs">The debounce delay in milliseconds. Must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="delayMs"/> is less than or equal to zero.
    /// </exception>
    public Debouncer(int delayMs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(delayMs, 0);
        this._delayMs = delayMs;
    }

    /// <summary>
    /// Debounces the specified async action. If called again before the delay elapses,
    /// the previous pending invocation is cancelled and the timer restarts.
    /// </summary>
    /// <param name="action">The async action to execute after the debounce delay.</param>
    /// <returns>A task that completes when the debounce cycle finishes (either executed or cancelled).</returns>
    public async Task DebounceAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (this._disposed)
        {
            return;
        }

        CancellationToken token;

        await this._lock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (this._disposed)
            {
                return;
            }

            this.CancelPending();

            var cts = new CancellationTokenSource();
            this._cts = cts;
            token = cts.Token;
        }
        finally
        {
            this._lock.Release();
        }

        try
        {
            await Task.Delay(this._delayMs, token).ConfigureAwait(false);
            await action().ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Expected when a newer invocation cancels a pending debounce.
        }
    }

    /// <summary>
    /// Debounces the specified async action, providing a <see cref="CancellationToken"/>
    /// that the action can observe for cooperative cancellation.
    /// </summary>
    /// <param name="action">
    /// The async action to execute after the debounce delay.
    /// Receives a cancellation token linked to the debounce lifecycle.
    /// </param>
    /// <returns>A task that completes when the debounce cycle finishes.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the debouncer has been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public async Task DebounceAsync(Func<CancellationToken, Task> action)
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        CancellationToken token;

        await this._lock.WaitAsync().ConfigureAwait(false);
        try
        {
            this.CancelPending();

            var cts = new CancellationTokenSource();
            this._cts = cts;
            token = cts.Token;
        }
        finally
        {
            this._lock.Release();
        }

        try
        {
            await Task.Delay(this._delayMs, token).ConfigureAwait(false);
            await action(token).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Expected when a newer invocation cancels a pending debounce.
        }
    }

    /// <summary>
    /// Cancels any currently pending debounced action without disposing the debouncer.
    /// </summary>
    public void Cancel()
    {
        if (this._disposed)
        {
            return;
        }

        this._lock.Wait();
        try
        {
            this.CancelPending();
        }
        finally
        {
            this._lock.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        this._lock.Wait();
        try
        {
            this.CancelPending();
        }
        finally
        {
            this._lock.Release();
        }

        this._lock.Dispose();
    }

    /// <summary>
    /// Cancels and disposes the current <see cref="CancellationTokenSource"/>.
    /// Must be called within the lock.
    /// </summary>
    private void CancelPending()
    {
        if (this._cts is { } existing)
        {
            existing.Cancel();
            existing.Dispose();
            this._cts = null;
        }
    }
}