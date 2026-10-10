// <copyright file="DiscordWebhookSenderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord;

/// <summary>
/// Tests for the <see cref="DiscordWebhookSender"/>.
/// </summary>
[TestFixture]
public class DiscordWebhookSenderTest
{
    private static readonly Uri WebhookUrl = new("https://discord.example/api/webhooks/1/token");

    /// <summary>
    /// Tests that a message is posted with its embed, and that mentions are disabled.
    /// </summary>
    [Test]
    public async Task MessageIsPostedWithoutMentionsAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        await using var sender = CreateSender(handler);

        sender.Enqueue(CreateEmbed("@everyone hello"));
        await handler.WaitForRequestsAsync(1).ConfigureAwait(false);

        var message = handler.Requests[0].RootElement;
        Assert.That(message.GetProperty("embeds")[0].GetProperty("title").GetString(), Is.EqualTo("@everyone hello"));
        Assert.That(message.GetProperty("embeds")[0].GetProperty("footer").GetProperty("text").GetString(), Is.EqualTo("Server: Test"));
        Assert.That(message.GetProperty("allowed_mentions").GetProperty("parse").GetArrayLength(), Is.Zero);
    }

    /// <summary>
    /// Tests that the embeds which are queued while a message is sent, are combined into the next message.
    /// </summary>
    [Test]
    public async Task QueuedEmbedsAreCombinedAsync()
    {
        var release = new TaskCompletionSource();
        var handler = new RecordingHttpMessageHandler { BlockFirstRequest = release.Task };
        await using var sender = CreateSender(handler);

        sender.Enqueue(CreateEmbed("1"));
        await WaitUntilAsync(() => handler.Requests.Count == 1).ConfigureAwait(false);
        sender.Enqueue(CreateEmbed("2"));
        sender.Enqueue(CreateEmbed("3"));
        release.SetResult();
        await handler.WaitForRequestsAsync(2).ConfigureAwait(false);

        Assert.That(handler.Requests, Has.Count.EqualTo(2));
        Assert.That(handler.Requests[1].RootElement.GetProperty("embeds").GetArrayLength(), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that a message is sent again after the rate limit was hit.
    /// </summary>
    [Test]
    public async Task MessageIsRetriedAfterRateLimitAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        handler.AddResponse(() =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{\"message\": \"You are being rate limited.\", \"retry_after\": 0.01, \"global\": false}"),
            };
            response.Headers.Add("Retry-After", "1");
            return response;
        });
        await using var sender = CreateSender(handler);

        sender.Enqueue(CreateEmbed("1"));
        await handler.WaitForRequestsAsync(2).ConfigureAwait(false);

        Assert.That(handler.Requests[1].RootElement.GetProperty("embeds")[0].GetProperty("title").GetString(), Is.EqualTo("1"));
    }

    /// <summary>
    /// Tests that a message which Discord rejects, e.g. because the webhook doesn't exist, isn't sent again.
    /// </summary>
    [Test]
    public async Task RejectedMessageIsNotRetriedAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        handler.AddResponse(() => new HttpResponseMessage(HttpStatusCode.NotFound));
        await using var sender = CreateSender(handler);

        sender.Enqueue(CreateEmbed("1"));
        sender.Enqueue(CreateEmbed("2"));
        await handler.WaitForRequestsAsync(1).ConfigureAwait(false);
        await Task.Delay(200).ConfigureAwait(false);

        // The second embed was either combined into the rejected message, or sent separately - but nothing is repeated.
        var titles = handler.Requests.SelectMany(r => r.RootElement.GetProperty("embeds").EnumerateArray()).Select(e => e.GetProperty("title").GetString());
        Assert.That(titles, Is.Unique);
    }

    /// <summary>
    /// Tests that the oldest messages are dropped when the queue is full.
    /// </summary>
    [Test]
    public async Task OldestMessagesAreDroppedWhenQueueIsFullAsync()
    {
        var release = new TaskCompletionSource();
        var handler = new RecordingHttpMessageHandler { BlockFirstRequest = release.Task };
        await using var sender = CreateSender(handler, capacity: 2);

        sender.Enqueue(CreateEmbed("1"));
        await WaitUntilAsync(() => handler.Requests.Count == 1).ConfigureAwait(false);
        sender.Enqueue(CreateEmbed("2"));
        sender.Enqueue(CreateEmbed("3"));
        sender.Enqueue(CreateEmbed("4"));
        release.SetResult();
        await handler.WaitForRequestsAsync(2).ConfigureAwait(false);

        var titles = handler.Requests[1].RootElement.GetProperty("embeds").EnumerateArray().Select(e => e.GetProperty("title").GetString());
        Assert.That(titles, Is.EqualTo(new[] { "3", "4" }));
    }

    private static DiscordWebhookSender CreateSender(RecordingHttpMessageHandler handler, int capacity = 100)
    {
        return new DiscordWebhookSender(WebhookUrl, new HttpClient(handler), NullLogger.Instance, capacity);
    }

    private static DiscordEmbed CreateEmbed(string title) => new(title, "Description", 0x123456, DateTime.UtcNow, "Server: Test");

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        Assert.That(condition(), Is.True, "The condition wasn't met in time.");
    }
}
