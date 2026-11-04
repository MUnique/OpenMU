// <copyright file="DiscordHttpClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Net.Http;

/// <summary>
/// Creates the HTTP client for the webhooks.
/// </summary>
public static class DiscordHttpClient
{
    /// <summary>
    /// The time after which a request to Discord is aborted.
    /// A sender waits for its request, so a hanging connection would stall its queue.
    /// The senders retry a request which timed out.
    /// </summary>
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Creates the HTTP client. It lives as long as the process, so it isn't disposed.
    /// </summary>
    /// <returns>The HTTP client.</returns>
    public static HttpClient Create() => new() { Timeout = Timeout };
}
