// <copyright file="GameEventWebhookSender.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Posts the <see cref="GameEventWebhookDelivery"/>s in the background, signed with the shared secret.
/// </summary>
/// <remarks>
/// The queue is bounded and drops the oldest delivery when it's full, so a receiver which is down never makes
/// the server hold more than <see cref="QueueCapacity"/> events. Failed posts are retried with a doubling delay.
/// </remarks>
public sealed class GameEventWebhookSender
{
    /// <summary>
    /// The name of the header with the signature, <c>sha256=</c> followed by the lowercase hex of the HMAC.
    /// </summary>
    public const string SignatureHeader = "X-OpenMU-Signature";

    /// <summary>
    /// The name of the header with the time of the request, in seconds since the Unix epoch, which is part of the signature.
    /// </summary>
    public const string TimestampHeader = "X-OpenMU-Timestamp";

    /// <summary>
    /// The maximum number of events which wait to be sent.
    /// </summary>
    /// <remarks>It's not configurable: it only bounds the memory while the receiver is unreachable.</remarks>
    internal const int QueueCapacity = 1000;

    /// <summary>
    /// The number of attempts to post one event, before it's given up.
    /// </summary>
    internal const int MaximumAttempts = 5;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static readonly HttpClient SharedHttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    private readonly Channel<GameEventWebhookDelivery> _queue;
    private readonly HttpClient _httpClient;
    private readonly TimeProvider _timeProvider;
    private readonly TimeSpan _firstRetryDelay;
    private readonly ILogger _logger;
    private int _workerStarted;
    private long _droppedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventWebhookSender"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClient">The HTTP client; a shared one with a timeout of 10 seconds by default.</param>
    /// <param name="timeProvider">The time provider of the timestamps and the retry delays.</param>
    /// <param name="firstRetryDelay">The delay before the first retry; it doubles with every further one. 2 seconds by default.</param>
    public GameEventWebhookSender(ILogger logger, HttpClient? httpClient = null, TimeProvider? timeProvider = null, TimeSpan? firstRetryDelay = null)
    {
        this._logger = logger;
        this._httpClient = httpClient ?? SharedHttpClient;
        this._timeProvider = timeProvider ?? TimeProvider.System;
        this._firstRetryDelay = firstRetryDelay ?? TimeSpan.FromSeconds(2);
        this._queue = Channel.CreateBounded<GameEventWebhookDelivery>(
            new BoundedChannelOptions(QueueCapacity) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true },
            _ => Interlocked.Increment(ref this._droppedCount));
    }

    /// <summary>
    /// Gets the number of events which were dropped because the queue was full.
    /// </summary>
    public long DroppedCount => Interlocked.Read(ref this._droppedCount);

    /// <summary>
    /// Gets the number of events which wait to be sent.
    /// </summary>
    public int QueuedCount => this._queue.Reader.Count;

    /// <summary>
    /// Computes the signature of a request: HMAC-SHA256 with the secret over the timestamp, a dot and the body.
    /// </summary>
    /// <param name="secret">The shared secret.</param>
    /// <param name="timestamp">The timestamp, in seconds since the Unix epoch.</param>
    /// <param name="body">The body of the request.</param>
    /// <returns>The value of the <see cref="SignatureHeader"/>.</returns>
    public static string Sign(string secret, long timestamp, string body)
    {
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes($"{timestamp.ToString(CultureInfo.InvariantCulture)}.{body}"));
        return "sha256=" + Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// Serializes the body of a delivery: its id and the event, with its type in <c>$type</c>.
    /// </summary>
    /// <param name="id">The identifier of the delivery, which stays the same for its retries.</param>
    /// <param name="gameEvent">The game event.</param>
    /// <returns>The JSON body.</returns>
    public static string Serialize(Guid id, GameEvent gameEvent)
    {
        return JsonSerializer.Serialize(new GameEventWebhookMessage(id, gameEvent), SerializerOptions);
    }

    /// <summary>
    /// Queues the delivery and returns at once; it never blocks and never throws.
    /// </summary>
    /// <param name="delivery">The delivery.</param>
    public void Enqueue(GameEventWebhookDelivery delivery)
    {
        // A bounded channel which drops the oldest item always accepts the new one.
        this._queue.Writer.TryWrite(delivery);
        if (Interlocked.Exchange(ref this._workerStarted, 1) == 0)
        {
            _ = Task.Run(this.ProcessQueueAsync);
        }
    }

    /// <summary>
    /// Posts one delivery, with retries.
    /// </summary>
    /// <param name="delivery">The delivery.</param>
    /// <returns><c>True</c>, if the receiver accepted it; Otherwise, <c>false</c>.</returns>
    internal async ValueTask<bool> DeliverAsync(GameEventWebhookDelivery delivery)
    {
        var body = Serialize(delivery.Id, delivery.Event);
        var delay = this._firstRetryDelay;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var request = this.CreateRequest(delivery, body);
                using var response = await this._httpClient.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (!IsTransient(response.StatusCode))
                {
                    this._logger.LogWarning("The game event webhook refused the event {id} with {status}; it's not sent again.", delivery.Id, (int)response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Attempt {attempt} to post the game event {id} failed.", attempt, delivery.Id);
            }

            if (attempt >= MaximumAttempts)
            {
                this._logger.LogWarning("The game event {id} couldn't be posted to the webhook after {attempts} attempts; it's dropped.", delivery.Id, attempt);
                return false;
            }

            await Task.Delay(delay, this._timeProvider).ConfigureAwait(false);
            delay *= 2;
        }
    }

    private static bool IsTransient(HttpStatusCode status)
    {
        return status is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests || (int)status >= 500;
    }

    private HttpRequestMessage CreateRequest(GameEventWebhookDelivery delivery, string body)
    {
        // Each attempt is signed with its own time, so a late retry still passes the receiver's replay window.
        var timestamp = this._timeProvider.GetUtcNow().ToUnixTimeSeconds();
        var request = new HttpRequestMessage(HttpMethod.Post, delivery.Url)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };
        request.Headers.Add(TimestampHeader, timestamp.ToString(CultureInfo.InvariantCulture));
        request.Headers.Add(SignatureHeader, Sign(delivery.Secret, timestamp, body));
        return request;
    }

    private async Task ProcessQueueAsync()
    {
        await foreach (var delivery in this._queue.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                await this.DeliverAsync(delivery).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Unexpected error when posting the game event {id}.", delivery.Id);
            }
        }
    }
}
