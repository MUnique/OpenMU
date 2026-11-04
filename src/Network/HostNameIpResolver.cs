// <copyright file="HostNameIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;

/// <summary>
/// Resolves the ip address by resolving a host name to get the <see cref="IPAddress"/> over DNS.
/// </summary>
public class HostNameIpResolver : IIpAddressResolver
{
    private readonly string _hostName;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostNameIpResolver"/> class.
    /// </summary>
    /// <param name="hostName">Name of the host.</param>
    public HostNameIpResolver(string hostName)
    {
        this._hostName = hostName;
    }

    /// <inheritdoc />
    public virtual async ValueTask<IPAddress> ResolveIPv4Async()
    {
        return await this.ResolveAsync().ConfigureAwait(false)
            ?? throw new InvalidOperationException("The host name could not be resolved.");
    }

    /// <summary>
    /// Gets the resolved IPv4, if available.
    /// </summary>
    /// <returns>The resolved IPv4.</returns>
    protected async ValueTask<IPAddress?> ResolveAsync()
    {
        if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            return null;
        }

        var hostEntry = await Dns.GetHostEntryAsync(this._hostName).ConfigureAwait(false);
        var ipAddress = hostEntry.AddressList
            .FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                       && !IPAddress.IsLoopback(address));

        return ipAddress;
    }
}