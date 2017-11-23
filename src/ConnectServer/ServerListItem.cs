// <copyright file="ServerListItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// A list item of an available server.
    /// </summary>
    internal class ServerListItem
    {
        private readonly byte[] data = new byte[4];
        private readonly ServerList owner;

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
        public ushort ServerId
        {
            get => (ushort)(this.data[0] | this.data[1] << 8);

            set
            {
                this.data[0] = (byte)(value & 0xFF);
                this.data[1] = (byte)((value >> 8) & 0xFF);
            }
        }

        /// <summary>
        /// Gets or sets the server load (usage rate).
        /// </summary>
        public byte ServerLoad
        {
            get => this.data[2];

            set
            {
                this.data[2] = value;
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
            get => new IPEndPoint(new IPAddress(this.ConnectInfo.Take(4).ToArray()), this.ConnectInfo[4] << 8 | this.ConnectInfo[5]);

            set
            {
                for (int i = 5; i < this.ConnectInfo.Length; i++)
                {
                    this.ConnectInfo[i] = 0;
                }

                var ip = value.Address.ToString();
                Encoding.ASCII.GetBytes(ip, 0, ip.Length, this.ConnectInfo, 5);

                this.ConnectInfo[this.ConnectInfo.Length - 1] = (byte)((value.Port >> 8) & 0xFF);
                this.ConnectInfo[this.ConnectInfo.Length - 2] = (byte)(value.Port & 0xFF);
            }
        }

        /// <summary>
        /// Gets the serialized data of server id and server load.
        /// </summary>
        public byte[] Data => this.data;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ServerId={this.ServerId}, ServerLoad={this.ServerLoad}";
        }
    }
}
