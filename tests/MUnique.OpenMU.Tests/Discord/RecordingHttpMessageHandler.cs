// <copyright file="RecordingHttpMessageHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

/// <summary>
/// A <see cref="HttpMessageHandler"/> which records the posted messages and answers with configurable responses.
/// </summary>
internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpResponseMessage>> _responses = new();
    private readonly SemaphoreSlim _requestReceived = new(0);

    /// <summary>
    /// Gets the bodies of the received requests.
    /// </summary>
    public List<JsonDocument> Requests { get; } = new();

    /// <summary>
    /// Gets or sets a task which has to complete before the first request is answered.
    /// </summary>
    public Task? BlockFirstRequest { get; set; }

    /// <summary>
    /// Adds a response, which is returned for the next request. When there are no more responses, 204 is returned.
    /// </summary>
    /// <param name="response">The function which creates the response.</param>
    public void AddResponse(Func<HttpResponseMessage> response) => this._responses.Enqueue(response);

    /// <summary>
    /// Waits until the specified number of requests has been received.
    /// </summary>
    /// <param name="count">The number of requests.</param>
    public async Task WaitForRequestsAsync(int count)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (this.Requests.Count < count && DateTime.UtcNow < deadline)
        {
            await this._requestReceived.WaitAsync(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
        }

        Assert.That(this.Requests, Has.Count.GreaterThanOrEqualTo(count), "Not enough requests were received in time.");
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = await request.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        Func<HttpResponseMessage>? response;
        lock (this.Requests)
        {
            this.Requests.Add(JsonDocument.Parse(body));
            this._responses.TryDequeue(out response);
        }

        if (this.Requests.Count == 1 && this.BlockFirstRequest is { } block)
        {
            await block.ConfigureAwait(false);
        }

        this._requestReceived.Release();
        return response?.Invoke() ?? new HttpResponseMessage(HttpStatusCode.NoContent);
    }
}
