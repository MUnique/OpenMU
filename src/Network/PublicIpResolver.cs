// <copyright file="PublicIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using log4net;

    /// <summary>
    /// Resolves the own ip address by calling an external API to get the public <see cref="IPAddress"/>.
    /// </summary>
    public class PublicIpResolver : IIpAddressResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PublicIpResolver));
        private readonly TimeSpan maximumCachedAddressLifetime = new TimeSpan(0, 5, 0);
        private IPAddress publicIPv4;
        private DateTime lastRequest = DateTime.MinValue;

        /// <summary>
        /// Gets the public IPv4 address with the help of the following api: https://www.ipify.org/.
        /// </summary>
        /// <returns>The public IPv4 address.</returns>
        public IPAddress GetIPv4()
        {
            if (this.lastRequest + this.maximumCachedAddressLifetime < DateTime.Now)
            {
                this.publicIPv4 = InternalGetIPv4();
                this.lastRequest = DateTime.Now;
            }

            return this.publicIPv4;
        }

        private static IPAddress InternalGetIPv4()
        {
            const string url = "https://api.ipify.org/?format=text";
            Log.Debug($"Start Requesting public ip from {url}");
            using (var client = new System.Net.Http.HttpClient())
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                var response = task.Result;
                var match = Regex.Match(response, @".*?(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}).*");
                if (match.Success)
                {
                    var ipString = match.Groups[1].Value;
                    Log.Debug($"Request of public ip answered with: {ipString}");
                    return IPAddress.Parse(ipString);
                }

                Log.Debug($"Request of public ip answered with unknown format: {response}");
                throw new FormatException($"Request of public ip answered with unknown format: {response}");
            }
        }
    }
}
