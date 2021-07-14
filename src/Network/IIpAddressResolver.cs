// <copyright file="IIpAddressResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.Net;

    /// <summary>
    /// Interface for an <see cref="IPAddress"/> resolver.
    /// </summary>
    public interface IIpAddressResolver
    {
        /// <summary>
        /// Gets the resolved IPv4.
        /// </summary>
        /// <returns>The resolved IPv4.</returns>
        IPAddress ResolveIPv4();
    }
}
