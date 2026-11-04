// <copyright file="DiscordChatQueue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Text;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sends the chat messages of a channel in the background. When messages come in faster than Discord
/// accepts them, they are combined into one Discord message.
/// </summary>
public sealed class DiscordChatQueue : IAsyncDisposable
{
    /// <summary>
    /// The maximum length of a Discord message.
    /// </summary>
    internal const int MaximumMessageLength = 2000;

    private readonly ulong _channelId;
    private readonly IDiscordChatMessenger _messenger;
    private readonly ILogger _logger;
    private readonly Channel<string> _queue;
    private readonly CancellationTokenSource _cancellation = new();
    private Task? _worker;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordChatQueue"/> class.
    /// </summary>
    /// <param name="channelId">The identifier of the channel.</param>
    /// <param name="messenger">The messenger.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="capacity">The maximum number of queued messages. When there are more, the oldest ones are dropped.</param>
    public DiscordChatQueue(ulong channelId, IDiscordChatMessenger messenger, ILogger logger, int capacity)
    {
        this._channelId = channelId;
        this._messenger = messenger;
        this._logger = logger;
        this._queue = Channel.CreateBounded<string>(new BoundedChannelOptions(Math.Max(capacity, 1))
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
        });
    }

    /// <summary>
    /// Adds a line of text to the queue.
    /// </summary>
    /// <param name="line">The line.</param>
    public void Enqueue(string line)
    {
        if (this._worker is null)
        {
            lock (this._queue)
            {
                this._worker ??= Task.Run(this.RunAsync);
            }
        }

        this._queue.Writer.TryWrite(line.Length > MaximumMessageLength ? line[..MaximumMessageLength] : line);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._queue.Writer.TryComplete();
        await this._cancellation.CancelAsync().ConfigureAwait(false);
        if (this._worker is { } worker)
        {
            await worker.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }

        this._cancellation.Dispose();
    }

    private async Task RunAsync()
    {
        var reader = this._queue.Reader;
        var cancellationToken = this._cancellation.Token;
        string? pending = null;
        try
        {
            while (pending is not null || await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var text = new StringBuilder();
                while ((pending ?? (reader.TryRead(out var line) ? line : null)) is { } next)
                {
                    if (text.Length > 0 && text.Length + 1 + next.Length > MaximumMessageLength)
                    {
                        pending = next;
                        break;
                    }

                    pending = null;
                    text.Append(text.Length > 0 ? "\n" : string.Empty).Append(next);
                }

                try
                {
                    await this._messenger.SendTextAsync(this._channelId, text.ToString(), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    this._logger.LogError(ex, "Unexpected error when sending chat messages to the Discord channel {channelId}; they are dropped.", this._channelId);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // shutting down
        }
    }
}
