// <copyright file="Field.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The data type of the field.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public enum FieldType
    {
        /// <summary>
        /// The field is a byte value and has a length of one byte.
        /// </summary>
        Byte,

        /// <summary>
        /// The field is a short value and has a length of two bytes.
        /// It's encoded as little endian.
        /// </summary>
        ShortLittleEndian,

        /// <summary>
        /// The field is a short value and has a length of two bytes.
        /// It's encoded as big endian.
        /// </summary>
        ShortBigEndian,

        /// <summary>
        /// The field is an integer value and has a length of four bytes.
        /// It's encoded as little endian.
        /// </summary>
        IntegerLittleEndian,

        /// <summary>
        /// The field is an integer value and has a length of four bytes.
        /// It's encoded as big endian.
        /// </summary>
        IntegerBigEndian,

        /// <summary>
        /// The field is a long integer value and has a length of eight bytes.
        /// It's encoded as little endian.
        /// </summary>
        LongLittleEndian,

        /// <summary>
        /// The field is a long integer value and has a length of eight bytes.
        /// It's encoded as big endian.
        /// </summary>
        LongBigEndian,

        /// <summary>
        /// The field is a string and has a variable length, specified by <see cref="Field.Length"/>.
        /// </summary>
        String,

        /// <summary>
        /// The field is a binary data and has a variable length, specified by <see cref="Field.Length"/>.
        /// </summary>
        Binary,

        /// <summary>
        /// The field is a enum value and has a length of one byte.
        /// </summary>
        Enum,

        /// <summary>
        /// The field is a boolean value and has a length of one byte.
        /// </summary>
        Boolean,

        /// <summary>
        /// The field is a float value and has a length of four bytes.
        /// </summary>
        Float,

        /// <summary>
        /// The field is a structure and has the length of the structure.
        /// </summary>
        [XmlEnum("Structure[]")]
        StructureArray,
    }

    /// <summary>
    /// A field definition. Based on this definition, the analyzer can extract information out of the packet byte array.
    /// </summary>
    [XmlType(Namespace = PacketDefinitions.XmlNamespace)]
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Gets or sets the index of the field in the packet byte array.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the number of bits by which the field is left shifted at the specified index.
        /// Only relevant for field types which are just 1 byte long, such as <see cref="FieldType.Byte"/>
        /// and <see cref="FieldType.Enum"/>.
        /// </summary>
        public int LeftShifted { get; set; }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// Relevant when <see cref="Type"/> is a <see cref="FieldType.Enum"/> or <see cref="FieldType.StructureArray"/>.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string? TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the length/size of the field, when it's a string or enum; Otherwise it's ignored.
        /// In case of strings, it defines the length in bytes.
        /// In case of enums, it defines the size in bits. If not specified, a size of 8 bits is assumed.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the default value of this field. It gets set when a packet is initialized.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the item count field, if this field is a structure array.
        /// </summary>
        /// <value>The item count field.</value>
        public string? ItemCountField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this field uses a custom implementation for the indexer, if this field is a struct array.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this field uses a custom implementation for the indexer, if this field is a struct array; otherwise, <c>false</c>.
        /// </value>
        public bool UseCustomIndexer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Length"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool LengthSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="LeftShifted"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool LeftShiftedSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="UseCustomIndexer"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool UseCustomIndexerSpecified { get; set; }
    }
}