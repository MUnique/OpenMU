// <copyright file="Field.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
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
        Short,

        /// <summary>
        /// The field is a short value and has a length of two bytes.
        /// It's encoded as big endian.
        /// </summary>
        ShortBigEndian,

        /// <summary>
        /// The field is an integer value and has a length of four bytes.
        /// It's encoded as little endian.
        /// </summary>
        Integer,

        /// <summary>
        /// The field is an integer value and has a length of four bytes.
        /// It's encoded as big endian.
        /// </summary>
        IntegerBigEndian,

        /// <summary>
        /// The field is a long integer value and has a length of eight bytes.
        /// It's encoded as little endian.
        /// </summary>
        Long,

        /// <summary>
        /// The field is a long integer value and has a length of eight bytes.
        /// It's encoded as big endian.
        /// </summary>
        LongBigEndian,

        /// <summary>
        /// The field is a binary data and has a variable length, specified by <see cref="Field.Length"/>.
        /// </summary>
        String,

        /// <summary>
        /// The field is a binary data and has a variable length, specified by <see cref="Field.Length"/>.
        /// </summary>
        Binary,
    }

    /// <summary>
    /// A field definition. Based on this definition, the analyzer can extract informations out of the packet byte array.
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
        /// Gets or sets the type of the field.
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the length of the field, when it's a string; Otherwise it's ignored.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Length"/> is specified.
        /// </summary>
        [XmlIgnore]
        public bool LengthSpecified { get; set; }
    }
}