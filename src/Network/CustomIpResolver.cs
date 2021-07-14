// <copyright file="CustomIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.Net;

    /// <summary>
    /// Resolver which always returns the provided custom ip address.
    /// </summary>
    public class CustomIpResolver : IIpAddressResolver
    {
        private readonly IPAddress address;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomIpResolver"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public CustomIpResolver(IPAddress address)
        {
            this.address = address;
        }

        /// <inheritdoc />
        public IPAddress ResolveIPv4() => this.address;
    }
}