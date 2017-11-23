// <copyright file="PacketHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration of a packet handler.
    /// </summary>
    public class PacketHandlerConfiguration
    {
        /// <summary>
        /// Gets or sets the packet identifier.
        /// </summary>
        public byte PacketIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the name of the packet handler class. It is used to create the packet handler.
        /// </summary>
        public string PacketHandlerClassName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the packet which is handled by this packet handler needs to be encrypted.
        /// </summary>
        public bool NeedsToBeEncrypted { get; set; }

        /// <summary>
        /// Gets or sets the sub packet handlers, if a packet handler has child packet handler.
        /// </summary>
        /// <remarks>
        /// E.g. 0xF3 and Character management related packets:
        ///     - 0xF3 0x00: Request character list
        ///     - 0xF3 0x01: Create character
        ///     - 0xF3 0x02: Delete character
        ///     - 0xF3 0x03: Select character
        ///     - and so on...
        /// </remarks>
        public virtual ICollection<PacketHandlerConfiguration> SubPacketHandlers { get; protected set; }
    }
}
