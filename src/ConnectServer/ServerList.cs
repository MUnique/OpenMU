// <copyright file="ServerList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using MUnique.OpenMU.Network.Packets.ConnectServer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The server list.
/// </summary>
internal class ServerList
{
    private readonly ClientVersion _clientVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerList" /> class.
    /// </summary>
    /// <param name="clientVersion">The client version.</param>
    public ServerList(ClientVersion clientVersion)
    {
        this._clientVersion = clientVersion;
        this.Servers = new SortedSet<ServerListItem>(new ServerListItemComparer());
    }

    /// <summary>
    /// Gets or sets the currently available servers.
    /// </summary>
    public ICollection<ServerListItem> Servers { get; set; }

    /// <summary>
    /// Gets the cache of the available servers.
    /// </summary>
    public byte[]? Cache { get; private set; }

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

        byte[] packet;
        if (this._clientVersion.Season == 0)
        {
            packet = new byte[ServerListResponseOld.GetRequiredSize(this.Servers.Count)];
            var response = new ServerListResponseOld(packet)
            {
                ServerCount = (byte)this.Servers.Count,
            };
            var i = 0;
            foreach (var server in this.Servers)
            {
                var serverBlock = response[i];
                serverBlock.ServerId = (byte)server.ServerId;
                serverBlock.LoadPercentage = server.ServerLoadPercentage;
                i++;
            }
        }
        else
        {
            packet = new byte[ServerListResponse.GetRequiredSize(this.Servers.Count)];
            var response = new ServerListResponse(packet)
            {
                ServerCount = (ushort)this.Servers.Count,
            };
            var i = 0;
            foreach (var server in this.Servers)
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

    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    public void InvalidateCache()
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