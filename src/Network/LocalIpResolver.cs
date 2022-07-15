// <copyright file="LocalIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;

/// <summary>
/// Resolves the own ip address by resolving the local host name to get the local <see cref="IPAddress"/> over DNS.
/// </summary>
public class LocalIpResolver : IIpAddressResolver
{
    /// <inheritdoc/>
    public async ValueTask<IPAddress> ResolveIPv4Async()
    {
        if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            var localHostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName()).ConfigureAwait(false);
            var localAddress = localHostEntry.AddressList
                .FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (localAddress != null)
            {
                return localAddress;
            }
        }

        return IPAddress.Loopback;
    }
}