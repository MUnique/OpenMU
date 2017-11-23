// <copyright file="MainPacketHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// The configuration for a main packet handler.
    /// </summary>
    public class MainPacketHandlerConfiguration
    {
        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        /// <remarks>
        /// = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
        /// </remarks>
        public byte[] ClientVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client serial.
        /// </summary>
        /// <remarks>
        /// Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");
        /// </remarks>
        public byte[] ClientSerial
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the appearance serializer class.
        /// If no explicit class name is specified, a default one will be used.
        /// </summary>
        public string AppearanceSerializerClassName { get; set; }

        /// <summary>
        /// Gets or sets the packet handlers of this main packet handler.
        /// </summary>
        public virtual ICollection<PacketHandlerConfiguration> PacketHandlers { get; protected set; }
    }
}
