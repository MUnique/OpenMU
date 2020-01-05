// <copyright file="Enum.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Xml serializable class for enum definitions.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public class Enum
    {
        /// <summary>
        ///  Gets or sets the name of the enum.
        /// </summary>
        [XmlElement(DataType = "Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this enum.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the possible values of this enum.
        /// </summary>
        [XmlArrayItem(IsNullable = false)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public EnumValue[]? Values { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}