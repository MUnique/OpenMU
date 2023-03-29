// <copyright file="LocalIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;

/// <summary>
/// Resolves the own ip address by resolving the local host name to get the local <see cref="IPAddress"/> over DNS.
/// </summary>
public class LocalIpResolver : HostNameIpResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalIpResolver"/> class.
    /// </summary>
    public LocalIpResolver()
        : base(Dns.GetHostName())
    {
    }

    /// <inheritdoc />
    public override async ValueTask<IPAddress> ResolveIPv4Async()
    {
        return await this.ResolveAsync().ConfigureAwait(false)
               ?? LoopbackIpResolver.LoopbackAddress;
    }
}