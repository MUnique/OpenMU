// <copyright file="ServerListItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Net;
    using System.Text;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// A list item of an available server.
    /// </summary>
    internal class ServerListItem
    {
        private const byte IpStartIndex = 4;
        private readonly byte[] data = new byte[4];
        private readonly ServerList owner;

        private byte serverLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListItem"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public ServerListItem(ServerList owner)
        {
            this.LoadIndex = -1;
            this.owner = owner;
            this.ConnectInfo = new byte[] { 0xC1, 0x16, 0xF4, 0x03, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        /// <summary>
        /// Gets or sets the index of the server in the load (usage rate) array.
        /// </summary>
        /// <value>
        /// The index of the server in the load (usage rate) array.
        /// </value>
        public int LoadIndex { get; set; }

        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        /// <value>
        /// The server identifier.
        /// </value>
        public ushort ServerId { get; set; }

        /// <summary>
        /// Gets or sets the server load (usage rate).
        /// </summary>
        public byte ServerLoad
        {
            get => this.serverLoad;

            set
            {
                this.serverLoad = value;
                var cache = this.owner.Cache;
                if (cache != null && this.LoadIndex != -1)
                {
                    cache[this.LoadIndex] = value;
                }
            }
        }

        /// <summary>
        /// Gets the connect information.
        /// </summary>
        public byte[] ConnectInfo { get; }

        /// <summary>
        /// Gets or sets the ip end point.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                var ip = IPAddress.Parse(this.ConnectInfo.ExtractString(IpStartIndex, 16, Encoding.UTF8));
                var port = this.ConnectInfo.MakeWordBigEndian(this.ConnectInfo.Length - 2);

                return new IPEndPoint(ip, port);
            }

            set
            {
                for (int i = IpStartIndex; i < this.ConnectInfo.Length; i++)
                {
                    this.ConnectInfo[i] = 0;
                }

                var ip = value.Address.ToString();
                Encoding.ASCII.GetBytes(ip, 0, ip.Length, this.ConnectInfo, IpStartIndex);
                this.ConnectInfo.SetShortBigEndian((ushort)value.Port, this.ConnectInfo.Length - 2);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ServerId={this.ServerId}, ServerLoad={this.ServerLoad}";
        }
    }
}
