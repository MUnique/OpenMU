// <copyright file="DiscordMessageQueue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

/// <summary>
/// Queues <see cref="DiscordEmbed"/>s for one Discord channel and sends them in the background.
/// </summary>
/// <remarks>
/// Discord limits the number of messages per channel. So the embeds are queued, and the queued embeds
/// are combined into one message (up to <see cref="MaximumEmbedsPerMessage"/>) while the previous message is sent.
/// When the queue is full, the oldest embed is dropped.
/// </remarks>
public abstract class DiscordMessageQueue : IAsyncDisposable
{
    /// <summary>
    /// The maximum number of embeds which Discord accepts in one message.
    /// </summary>
    internal const int MaximumEmbedsPerMessage = 10;

    private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

    private readonly Channel<DiscordEmbed> _queue;
    private readonly CancellationTokenSource _cancellation = new();
    private Task? _worker;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordMessageQueue"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="capacity">The maximum number of queued embeds.</param>
    protected DiscordMessageQueue(ILogger logger, int capacity)
    {
        this.Logger = logger;
        this._queue = Channel.CreateBounded<DiscordEmbed>(
            new BoundedChannelOptions(Math.Max(capacity, 1))
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
            },
            dropped => this.Logger.LogWarning("The Discord message queue is full, dropped the message {title}.", dropped.Title));
    }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Queues the embed for sending.
    /// </summary>
    /// <param name="embed">The embed.</param>
    /// <returns><see langword="true"/>, if the embed was queued; <see langword="false"/>, if the queue is shutting down.</returns>
    public bool Enqueue(DiscordEmbed embed)
    {
        // The worker is started lazily, so that derived classes are fully initialized when it runs.
        if (this._worker is null)
        {
            lock (this._queue)
            {
                this._worker ??= Task.Run(this.RunAsync);
            }
        }

        return this._queue.Writer.TryWrite(embed);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // The queued messages are still sent, but the shutdown doesn't wait forever.
        this._queue.Writer.TryComplete();
        try
        {
            if (this._worker is { } worker)
            {
                await worker.WaitAsync(ShutdownTimeout).ConfigureAwait(false);
            }
        }
        catch (TimeoutException)
        {
            this.Logger.LogWarning("Not all Discord messages could be sent before the shutdown.");
        }
        finally
        {
            await this._cancellation.CancelAsync().ConfigureAwait(false);
            this._cancellation.Dispose();
        }
    }

    /// <summary>
    /// Sends one message with the specified embeds.
    /// </summary>
    /// <param name="embeds">The embeds; at least one and at most <see cref="MaximumEmbedsPerMessage"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    protected abstract Task SendAsync(IReadOnlyList<DiscordEmbed> embeds, CancellationToken cancellationToken);

    private async Task RunAsync()
    {
        var reader = this._queue.Reader;
        var cancellationToken = this._cancellation.Token;
        try
        {
            while (await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var batch = new List<DiscordEmbed>(MaximumEmbedsPerMessage);
                while (batch.Count < MaximumEmbedsPerMessage && reader.TryRead(out var embed))
                {
                    batch.Add(embed);
                }

                try
                {
                    await this.SendAsync(batch, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // One faulty message must not stop the whole queue.
                    this.Logger.LogError(ex, "Unexpected error when sending {count} message(s) to Discord; they are dropped.", batch.Count);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // shutting down
        }
    }
}
