// <copyright file="DiscordWebhookSender.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sends <see cref="DiscordEmbed"/>s to one Discord webhook, in the background.
/// </summary>
/// <remarks>
/// <para>
/// Discord limits the number of messages per webhook (about 30 per minute). So the embeds are queued,
/// and the queued embeds are combined into one message (up to <see cref="MaximumEmbedsPerMessage"/>)
/// while waiting for the rate limit. When the queue is full, the oldest embed is dropped.
/// </para>
/// <para>
/// The rate limit is respected by evaluating the rate limit headers of the responses and,
/// when it was hit anyway, the <c>Retry-After</c> header.
/// </para>
/// </remarks>
public sealed class DiscordWebhookSender : IAsyncDisposable
{
    /// <summary>
    /// The maximum number of embeds which Discord accepts in one message.
    /// </summary>
    internal const int MaximumEmbedsPerMessage = 10;

    /// <summary>
    /// The maximum number of attempts to send a message, when Discord isn't available or returns an error.
    /// </summary>
    private const int MaximumAttempts = 5;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

    private readonly Uri _webhookUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly TimeProvider _timeProvider;
    private readonly Channel<DiscordEmbed> _queue;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly Task _worker;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordWebhookSender"/> class.
    /// </summary>
    /// <param name="webhookUrl">The URL of the webhook.</param>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="capacity">The maximum number of queued embeds.</param>
    /// <param name="timeProvider">The time provider, which is used to wait for the rate limits.</param>
    public DiscordWebhookSender(Uri webhookUrl, HttpClient httpClient, ILogger logger, int capacity, TimeProvider? timeProvider = null)
    {
        this._webhookUrl = webhookUrl;
        this._httpClient = httpClient;
        this._logger = logger;
        this._timeProvider = timeProvider ?? TimeProvider.System;
        this._queue = Channel.CreateBounded<DiscordEmbed>(
            new BoundedChannelOptions(Math.Max(capacity, 1))
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
            },
            dropped => this._logger.LogWarning("The Discord message queue is full, dropped the message {title}.", dropped.Title));
        this._worker = Task.Run(this.RunAsync);
    }

    /// <summary>
    /// Queues the embed for sending.
    /// </summary>
    /// <param name="embed">The embed.</param>
    /// <returns><see langword="true"/>, if the embed was queued; <see langword="false"/>, if the sender is shutting down.</returns>
    public bool Enqueue(DiscordEmbed embed) => this._queue.Writer.TryWrite(embed);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // The queued messages are still sent, but the shutdown doesn't wait forever.
        this._queue.Writer.TryComplete();
        try
        {
            await this._worker.WaitAsync(ShutdownTimeout).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            this._logger.LogWarning("Not all Discord messages could be sent before the shutdown.");
        }
        finally
        {
            await this._cancellation.CancelAsync().ConfigureAwait(false);
            this._cancellation.Dispose();
        }
    }

    private static TimeSpan? GetDelay(HttpResponseMessage response, string headerName)
    {
        if (response.Headers.TryGetValues(headerName, out var values)
            && double.TryParse(values.FirstOrDefault(), NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
        {
            return TimeSpan.FromSeconds(Math.Max(seconds, 0));
        }

        return null;
    }

    /// <summary>
    /// Gets the delay after which a rate limited request can be retried.
    /// Discord sends it in seconds, as decimal number in the body and rounded up in the <c>Retry-After</c> header.
    /// </summary>
    private static async Task<TimeSpan> GetRetryDelayAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var body = await response.Content.ReadFromJsonAsync<RateLimitResponse>(SerializerOptions, cancellationToken).ConfigureAwait(false);
            if (body?.RetryAfter is { } seconds)
            {
                return TimeSpan.FromSeconds(Math.Max(seconds, 0));
            }
        }
        catch (JsonException)
        {
            // fall back to the header
        }

        return response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1);
    }

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
                    this._logger.LogError(ex, "Unexpected error when sending {count} message(s) to Discord; they are dropped.", batch.Count);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // shutting down
        }
    }

    private async Task SendAsync(IReadOnlyList<DiscordEmbed> batch, CancellationToken cancellationToken)
    {
        var message = new WebhookMessage(
            batch.Select(embed => new WebhookEmbed(
                embed.Title,
                embed.Description,
                embed.Color,
                embed.TimestampUtc.ToString("O", CultureInfo.InvariantCulture),
                embed.Footer is { } footer ? new WebhookFooter(footer) : null)).ToList(),
            new WebhookAllowedMentions([]));

        for (var attempt = 1; attempt <= MaximumAttempts; attempt++)
        {
            TimeSpan delay;
            try
            {
                using var response = await this._httpClient.PostAsJsonAsync(this._webhookUrl, message, SerializerOptions, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    // When the rate limit is exhausted, we wait until it resets, before sending the next message.
                    if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var remaining) && remaining.FirstOrDefault() == "0"
                        && GetDelay(response, "X-RateLimit-Reset-After") is { } resetAfter)
                    {
                        await Task.Delay(resetAfter, this._timeProvider, cancellationToken).ConfigureAwait(false);
                    }

                    return;
                }

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    delay = await GetRetryDelayAsync(response, cancellationToken).ConfigureAwait(false);
                    this._logger.LogDebug("Discord rate limit hit, retrying after {delay}.", delay);
                }
                else if ((int)response.StatusCode >= 500)
                {
                    delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    this._logger.LogWarning("Discord returned {statusCode}, retrying after {delay}.", response.StatusCode, delay);
                }
                else
                {
                    // E.g. the webhook was deleted. Retrying wouldn't help.
                    this._logger.LogError("Discord rejected the message with {statusCode}. Is the webhook URL valid?", response.StatusCode);
                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                this._logger.LogWarning(ex, "Discord isn't reachable, retrying after {delay}.", delay);
            }

            await Task.Delay(delay, this._timeProvider, cancellationToken).ConfigureAwait(false);
        }

        this._logger.LogError("Couldn't send {count} message(s) to Discord after {attempts} attempts; they are dropped.", batch.Count, MaximumAttempts);
    }

    private sealed record RateLimitResponse(double? RetryAfter);

    private sealed record WebhookMessage(IReadOnlyList<WebhookEmbed> Embeds, WebhookAllowedMentions AllowedMentions);

    private sealed record WebhookEmbed(string Title, string Description, int Color, string Timestamp, WebhookFooter? Footer);

    private sealed record WebhookFooter(string Text);

    /// <summary>
    /// Defines which mentions are allowed. An empty list prevents that texts of players can ping anybody, e.g. with <c>@everyone</c>.
    /// </summary>
    private sealed record WebhookAllowedMentions(IReadOnlyList<string> Parse);
}
