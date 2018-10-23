﻿// <copyright file="ServerList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The server list.
    /// </summary>
    internal class ServerList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerList"/> class.
        /// </summary>
        public ServerList()
        {
            this.Servers = new SortedSet<ServerListItem>(new ServerListItemComparer());
            this.ConnectInfos = new Dictionary<ushort, byte[]>();
        }

        /// <summary>
        /// Gets or sets the currently available servers.
        /// </summary>
        public ICollection<ServerListItem> Servers { get; set; }

        /// <summary>
        /// Gets or sets the cached connection infos.
        /// </summary>
        public IDictionary<ushort, byte[]> ConnectInfos { get; set; }

        /// <summary>
        /// Gets the cache of the available servers.
        /// </summary>
        public byte[] Cache { get; private set; }

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

            byte[] packet = new byte[(this.Servers.Count * 4) + 7];
            packet[0] = 0xC2;
            packet[1] = (byte)((packet.Length >> 8) & 0xFF);
            packet[2] = (byte)(packet.Length & 0xFF);
            packet[3] = 0xF4;
            packet[4] = 0x06;
            packet[5] = (byte)((this.Servers.Count >> 8) & 0xFF);
            packet[6] = (byte)(this.Servers.Count & 0xFF);
            var i = 0;
            foreach (var server in this.Servers)
            {
                Buffer.BlockCopy(server.Data, 0, packet, 7 + (i * 4), 4);
                server.LoadIndex = 7 + 2 + (i * 4);
                i++;
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
            public int Compare(ServerListItem x, ServerListItem y)
            {
                return x.ServerId.CompareTo(y.ServerId);
            }
        }
    }
}
