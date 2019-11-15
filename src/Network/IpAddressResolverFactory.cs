// <copyright file="IpAddressResolverFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Factory which creates an <see cref="IIpAddressResolver"/> depending on command line arguments.
    /// </summary>
    public static class IpAddressResolverFactory
    {
        /// <summary>
        /// Determines the ip resolver based on command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The determined resolver.</returns>
        public static IIpAddressResolver DetermineIpResolver(string[] args)
        {
            const string resolveParameterPrefix = "-resolveIP:";
            const string publicIpResolve = "-resolveIP:public";
            const string localIpResolve = "-resolveIP:local";
            var parameter = args.FirstOrDefault(a => a.StartsWith(resolveParameterPrefix, StringComparison.InvariantCultureIgnoreCase));

            switch (parameter)
            {
                case null:
                case string p when p.StartsWith(publicIpResolve, StringComparison.InvariantCultureIgnoreCase):
                    return new PublicIpResolver();
                case string p when p.StartsWith(localIpResolve, StringComparison.InvariantCultureIgnoreCase):
                    return new LocalIpResolver();
                default:
                    IPAddress.TryParse(parameter.Substring(parameter.IndexOf(':') + 1), out var ip);
                    return new CustomIpResolver(ip);
            }
        }
    }
}
