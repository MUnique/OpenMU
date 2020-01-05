// <copyright file="Structure.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a structure which can be defined as packet header or part of a packet.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://www.munique.net/OpenMU/PacketDefinitions")]
    public class Structure
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement(DataType = "Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        [XmlArray(IsNullable = true)]
        [XmlArrayItem(IsNullable = false)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public Field[]? Fields { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}