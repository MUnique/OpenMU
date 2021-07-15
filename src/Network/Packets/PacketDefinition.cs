// <copyright file="PacketDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
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
    /// Defines the direction from which to which endpoint a data packet flows.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    public enum Direction
    {
        /// <summary>
        /// The data packet is sent from the (game) server to the game client.
        /// </summary>
        ServerToClient,

        /// <summary>
        /// The data packet is sent from the game client to the (game) server.
        /// </summary>
        ClientToServer,

        /// <summary>
        /// The data packet is sent in both directions.
        /// </summary>
        Bidirectional,
    }

    /// <summary>
    /// The definition of one packet type.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public class PacketDefinition
    {
        private string? headerType;

        private PacketType? type;

        /// <summary>
        /// Gets or sets the type of the header.
        /// </summary>
        /// <value>
        /// The type of the header.
        /// </value>
        public string? HeaderType
        {
            get => this.headerType;
            set
            {
                this.headerType = value;
                this.type = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the packet, represented by its first byte value.
        /// </summary>
        [XmlIgnore]
        public PacketType Type
        {
            get
            {
                if (!this.type.HasValue)
                {
                    this.type = this.HeaderType is null ? default : (PacketType)byte.Parse(this.HeaderType.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return this.type.Value;
            }

            set
            {
                this.type = value;
                var newHeaderType = ((byte)value).ToString("X", CultureInfo.InvariantCulture) + "Header";
                if (this.SubCodeSpecified)
                {
                    newHeaderType += "WithSubCode";
                }

                this.HeaderType = newHeaderType;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Code"/> as hexadecimal string.
        /// </summary>
        [XmlElement(nameof(Code))]
        public string CodeAsHex
        {
            get => this.Code.ToString("X", CultureInfo.InvariantCulture);
            set => this.Code = byte.Parse(value, NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// Gets or sets the <see cref="SubCode"/> as hexadecimal string.
        /// </summary>
        [XmlElement(nameof(SubCode))]
        public string SubCodeAsHex
        {
            get => this.SubCode.ToString("X", CultureInfo.InvariantCulture);
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
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the direction of the packet.
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Gets or sets the 'sent when' description of the packet.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string? SentWhen { get; set; }

        /// <summary>
        /// Gets or sets the 'caused reaction' description of the packet.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string? CausedReaction { get; set; }

        /// <summary>
        /// Gets or sets the field definitions of the packets.
        /// </summary>
        [XmlArray(IsNullable = true)]
        [XmlArrayItem("Field", IsNullable = false)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public Field[]? Fields { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

        /// <summary>
        /// Gets or sets the structures which are exclusively used by this packet.
        /// </summary>
        [XmlArrayItem("Structure", IsNullable = false)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public Structure[]? Structures { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

        /// <summary>
        /// Gets or sets the enums which are exclusively used by this packet.
        /// </summary>
        [XmlArrayItem(IsNullable = false)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public Enum[]? Enums { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}