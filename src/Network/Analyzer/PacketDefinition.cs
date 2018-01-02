// <copyright file="PacketDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// The packet type, which is specified by the first byte of the packet byte array.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public enum PacketType : byte
    {
        /// <summary>
        /// The C1 type, which has a one byte length field and is not encrypted by simple modulus.
        /// </summary>
        C1 = 0xC1,

        /// <summary>
        /// The C2 type, which has a two byte length field and is not encrypted by simple modulus.
        /// </summary>
        C2 = 0xC2,

        /// <summary>
        /// The C3 type, which has a one byte length field and is encrypted by simple modulus.
        /// </summary>
        C3 = 0xC3,

        /// <summary>
        /// The C4 type, which has a two byte length field and is encrypted by simple modulus.
        /// </summary>
        C4 = 0xC4,
    }

    /// <summary>
    /// The definiton of one packet type.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public class PacketDefinition
    {
        /// <summary>
        /// Gets or sets the type of the packet, represented by its first byte value.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Code"/> as hexadecimal string.
        /// </summary>
        [XmlElement(nameof(Code))]
        public string CodeAsHex
        {
            get => this.Code.ToString("X");
            set => this.Code = byte.Parse(value, NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// Gets or sets the <see cref="SubCode"/> as hexadecimal string.
        /// </summary>
        [XmlElement(nameof(SubCode))]
        public string SubCodeAsHex
        {
            get => this.SubCode.ToString("X");
            set => this.SubCode = byte.Parse(value, NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// Gets or sets the code (type) of a packet.
        /// </summary>
        [XmlIgnore]
        public byte Code { get; set; }

        /// <summary>
        /// Gets or sets the sub code of a packet, usually the byte which follows the <see cref="Code"/>.
        /// </summary>
        [XmlIgnore]
        public byte SubCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="SubCode"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool SubCodeSpecified
        {
            get => this.SubCodeAsHexSpecified;
            set => this.SubCodeAsHexSpecified = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="SubCodeAsHex"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool SubCodeAsHexSpecified { get; set; }

        /// <summary>
        /// Gets or sets the name of the packet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the packet.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field definitions of the packets.
        /// </summary>
        [XmlArray(IsNullable = true)]
        [XmlArrayItem("Field", IsNullable = false)]
        public Field[] Fields { get; set; }
    }
}