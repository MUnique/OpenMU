// <copyright file="ClientConnectionCounter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net;

/// <summary>
/// Counts the connections per ip address.
/// </summary>
internal class ClientConnectionCounter
{
    private readonly IDictionary<IPAddress, int> _connections = new Dictionary<IPAddress, int>();
    private readonly object _syncRoot = new();

    /// <summary>
    /// Gets the connection count of the specified ip address.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns>The counted connections of the ip address.</returns>
    public int GetConnectionCount(IPAddress ipAddress)
    {
        int count;
        lock (this._syncRoot)
        {
            this._connections.TryGetValue(ipAddress, out count);
        }

        return count;
    }

    /// <summary>
    /// Adds the connection and increases its count for the specified ip address.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    public void AddConnection(IPAddress ipAddress)
    {
        lock (this._syncRoot)
        {
            if (this._connections.ContainsKey(ipAddress))
            {
                this._connections[ipAddress]++;
            }
            else
            {
                this._connections.Add(ipAddress, 1);
            }
        }
    }

    /// <summary>
    /// Removes the connection and decreases its count for the specified ip address.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    public void RemoveConnection(IPAddress ipAddress)
    {
        lock (this._syncRoot)
        {
            if (!this._connections.ContainsKey(ipAddress))
            {
                return;
            }

            this._connections[ipAddress]--;
            if (this._connections[ipAddress] == 0)
            {
                this._connections.Remove(ipAddress);
            }
        }
    }
}