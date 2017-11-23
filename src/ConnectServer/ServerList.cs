// <copyright file="ServerList.cs" company="MUnique">
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
            this.Servers = new List<ServerListItem>();
            this.ConnectInfos = new Dictionary<ushort, byte[]>();
        }

        /// <summary>
        /// Gets or sets the currently available servers.
        /// </summary>
        public IList<ServerListItem> Servers { get; set; }

        /// <summary>
        /// Gets or sets the cached connection infos.
        /// </summary>
        public IDictionary<ushort, byte[]> ConnectInfos { get; set; }

        /// <summary>
        /// Gets the cache of the complete connection infos.
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
            for (int i = 0; i < this.Servers.Count; ++i)
            {
                Buffer.BlockCopy(this.Servers[i].Data, 0, packet, 7 + (i * 4), 4);
                this.Servers[i].LoadIndex = 7 + 2 + (i * 4);
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
    }
}
