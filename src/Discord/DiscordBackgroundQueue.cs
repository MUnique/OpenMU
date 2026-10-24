// <copyright file="DiscordBackgroundQueue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

/// <summary>
/// Runs bounded background work for one connection lifetime of the bot.
/// </summary>
internal sealed class DiscordBackgroundQueue : IAsyncDisposable
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly ILogger _logger;
    private readonly Task _worker;
    private int _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordBackgroundQueue"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="capacity">The maximum number of pending operations.</param>
    internal DiscordBackgroundQueue(ILogger logger, int capacity = 100)
    {
        this._logger = logger;
        this._queue = Channel.CreateBounded<Func<CancellationToken, Task>>(new BoundedChannelOptions(capacity) { SingleReader = true });
        this._worker = Task.Run(this.RunAsync);
    }

    /// <inheritdoc />
    ValueTask IAsyncDisposable.DisposeAsync() => this.DisposeAsync();

    /// <summary>
    /// Queues an operation, or rejects it when the queue is full or stopped.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns>Whether the operation was queued.</returns>
    internal bool Enqueue(Func<CancellationToken, Task> operation)
    {
        if (this._queue.Writer.TryWrite(operation))
        {
            return true;
        }

        this._logger.LogWarning("A Discord background notification was dropped because the queue is full or stopped.");
        return false;
    }

    /// <summary>
    /// Cancels the connection's operations and waits for the worker to stop.
    /// </summary>
    /// <returns>The asynchronous shutdown operation.</returns>
    internal async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this._isDisposed, 1) != 0)
        {
            return;
        }

        this._queue.Writer.TryComplete();
        await this._cancellation.CancelAsync().ConfigureAwait(false);
        try
        {
            await this._worker.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            this._logger.LogWarning("A Discord background operation did not finish within the shutdown timeout.");
        }
        finally
        {
            this._cancellation.Dispose();
        }
    }

    private async Task RunAsync()
    {
        var cancellationToken = this._cancellation.Token;
        try
        {
            await foreach (var operation in this._queue.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
                {
                    this._logger.LogWarning(ex, "A Discord background notification failed.");
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Shutdown cancels waits for Ready as well as requests to Discord.
        }
    }
}
