// <copyright file="ClientConnectionCounter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Counts the connections per ip address.
    /// </summary>
    internal class ClientConnectionCounter
    {
        private readonly IDictionary<IPAddress, int> connections = new Dictionary<IPAddress, int>();
        private readonly object syncRoot = new ();

        /// <summary>
        /// Gets the connection count of the specified ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>The counted connections of the ip address.</returns>
        public int GetConnectionCount(IPAddress ipAddress)
        {
            int count;
            lock (this.syncRoot)
            {
                this.connections.TryGetValue(ipAddress, out count);
            }

            return count;
        }

        /// <summary>
        /// Adds the connection and increases its count for the specified ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        public void AddConnection(IPAddress ipAddress)
        {
            lock (this.syncRoot)
            {
                if (this.connections.ContainsKey(ipAddress))
                {
                    this.connections[ipAddress]++;
                }
                else
                {
                    this.connections.Add(ipAddress, 1);
                }
            }
        }

        /// <summary>
        /// Removes the connection and decreases its count for the specified ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        public void RemoveConnection(IPAddress ipAddress)
        {
            lock (this.syncRoot)
            {
                if (!this.connections.ContainsKey(ipAddress))
                {
                    return;
                }

                this.connections[ipAddress]--;
                if (this.connections[ipAddress] == 0)
                {
                    this.connections.Remove(ipAddress);
                }
            }
        }
    }
}
