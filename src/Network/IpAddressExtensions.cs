// <copyright file="IpAddressExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Diagnostics.CodeAnalysis;
using System.Net;

/// <summary>
/// Extensions for <see cref="IPAddress"/>.
/// </summary>
public static class IpAddressExtensions
{
    private static HashSet<IPAddress>? _localIpAddresses;

    /// <summary>
    /// Determines whether the address in on same (this) host.
    /// </summary>
    /// <param name="address">The next server address.</param>
    /// <returns>
    ///   <c>true</c> if the specified address is on same host as this machine; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsOnSameHost([NotNullWhen(true)] this IPAddress? address)
    {
        if (address is null)
        {
            return false;
        }

        if (IPAddress.IsLoopback(address))
        {
            return true;
        }
        
        _localIpAddresses ??= Dns.GetHostAddresses(Dns.GetHostName()).ToHashSet();
        return _localIpAddresses.Contains(address);
    }
}