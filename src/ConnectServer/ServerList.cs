// <copyright file="ServerList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Threading;
using MUnique.OpenMU.Network.Packets.ConnectServer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The server list.
/// </summary>
internal class ServerList
{
    private readonly ReaderWriterLockSlim _lock = new();

    private readonly ICollection<ServerListItem> _servers = new SortedSet<ServerListItem>(new ServerListItemComparer());

    private readonly ClientVersion _clientVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerList" /> class.
    /// </summary>
    /// <param name="clientVersion">The client version.</param>
    public ServerList(ClientVersion clientVersion)
    {
        this._clientVersion = clientVersion;
    }

    /// <summary>
    /// Gets the total connection count.
    /// </summary>
    public int TotalConnectionCount
    {
        get
        {
            this._lock.EnterReadLock();
            try
            {
                return this._servers.Sum(s => s.CurrentConnections);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Gets the cache of the available servers.
    /// </summary>
    public byte[]? Cache { get; private set; }

    /// <summary>
    /// Gets the <see cref="IGameServerEntry"/>s of this list.
    /// </summary>
    public IEnumerable<IGameServerEntry> Items
    {
        get
        {
            this._lock.EnterReadLock();
            try
            {
                return this._servers.ToList();
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Adds the specified item to this instance.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(ServerListItem item)
    {
        this._lock.EnterWriteLock();
        try
        {
            this._servers.Add(item);
            this.InvalidateCache();
        }
        finally
        {
            this._lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Removes the specified item from this instance.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Remove(ServerListItem item)
    {
        this._lock.EnterWriteLock();
        try
        {
            this._servers.Remove(item);
            this.InvalidateCache();
        }
        finally
        {
            this._lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets the <see cref="ServerListItem"/> of the specified server id.
    /// </summary>
    /// <param name="gameServerId">The game server identifier.</param>
    /// <returns>The found <see cref="ServerListItem"/>.</returns>
    public ServerListItem? GetItem(ushort gameServerId)
    {
        this._lock.EnterReadLock();
        try
        {
            return this._servers.FirstOrDefault(s => s.ServerId == gameServerId);
        }
        finally
        {
            this._lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Serializes this instance to a server list packet, which can be sent to the client.
    /// </summary>
    /// <returns>The serialized server list.</returns>
    public byte[] Serialize()
    {
        var result = this.Cache;
        if (result != null)
        {
            return result;
        }

        this._lock.EnterReadLock();
        try
        {
            result = this.Cache;
            if (result != null)
            {
                return result;
            }

            byte[] packet;
            if (this._clientVersion.Season == 0)
            {
                packet = new byte[ServerListResponseOld.GetRequiredSize(this._servers.Count)];
                var response = new ServerListResponseOld(packet)
                {
                    ServerCount = (byte)this._servers.Count,
                };
                var i = 0;
                foreach (var server in this._servers)
                {
                    var serverBlock = response[i];
                    serverBlock.ServerId = (byte)server.ServerId;
                    serverBlock.LoadPercentage = server.ServerLoadPercentage;
                    i++;
                }
            }
            else
            {
                packet = new byte[ServerListResponse.GetRequiredSize(this._servers.Count)];
                var response = new ServerListResponse(packet)
                {
                    ServerCount = (ushort)this._servers.Count,
                };
                var i = 0;
                foreach (var server in this._servers)
                {
                    var serverBlock = response[i];
                    serverBlock.ServerId = server.ServerId;
                    serverBlock.LoadPercentage = server.ServerLoadPercentage;
                    i++;
                }
            }

            this.Cache = packet;
            return packet;
        }
        finally
        {
            this._lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    private void InvalidateCache()
    {
        this.Cache = null;
    }

    /// <summary>
    /// Comparer for <see cref="ServerListItem"/>s.
    /// </summary>
    private class ServerListItemComparer : IComparer<ServerListItem>
    {
        /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(ServerListItem? x, ServerListItem? y)
        {
            return x?.ServerId.CompareTo(y?.ServerId) ?? int.MinValue;
        }
    }
}