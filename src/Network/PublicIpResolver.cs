// <copyright file="PublicIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Resolves the own ip address by calling an external API to get the public <see cref="IPAddress"/>.
/// </summary>
public class PublicIpResolver : IIpAddressResolver
{
    private readonly ILogger<PublicIpResolver> _logger;
    private readonly TimeSpan _maximumCachedAddressLifetime = new (0, 5, 0);
    private IPAddress? _publicIPv4;
    private DateTime _lastRequest = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicIpResolver"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public PublicIpResolver(ILogger<PublicIpResolver> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Gets the public IPv4 address with the help of the following api: https://www.ipify.org/.
    /// </summary>
    /// <returns>The public IPv4 address.</returns>
    public async ValueTask<IPAddress> ResolveIPv4Async()
    {
        if (this._lastRequest + this._maximumCachedAddressLifetime < DateTime.Now)
        {
            this._publicIPv4 = await this.InternalGetIPv4Async();
            this._lastRequest = DateTime.Now;
        }

        return this._publicIPv4!;
    }

    private async ValueTask<IPAddress> InternalGetIPv4Async()
    {
        const string url = "https://api.ipify.org/?format=text";
        this._logger.LogDebug("Start Requesting public ip from {url}", url);
        using var client = new System.Net.Http.HttpClient();
        var response = await client.GetStringAsync(url);

        var match = Regex.Match(response, @".*?(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}).*");
        if (match.Success)
        {
            var ipString = match.Groups[1].Value;
            this._logger.LogDebug("Request of public ip answered with: {ipString}", ipString);
            return IPAddress.Parse(ipString);
        }

        this._logger.LogDebug("Request of public ip answered with unknown format: {response}", response);
        throw new FormatException($"Request of public ip answered with unknown format: {response}");
    }
}