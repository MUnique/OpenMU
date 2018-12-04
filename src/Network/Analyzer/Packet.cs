// <copyright file="Packet.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Text;

    /// <summary>
    /// A captured data packet.
    /// </summary>
    public class Packet
    {
        private readonly byte[] innerData;

        private string dataAsString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="toServer">If set to <c>true</c>, the packet was sent to server; Otherwise it was sent to the client.</param>
        public Packet(byte[] data, bool toServer)
        {
            this.innerData = data;
            this.Timestamp = DateTime.Now;
            this.ToServer = toServer;
            this.Direction = this.ToServer ? "C->S" : "S->C";
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets the type of the packet, which is the first byte of the packet byte array.
        /// </summary>
        public byte Type => this.innerData[0];

        /// <summary>
        /// Gets the code of the packet, which specifies the kind of the message.
        /// </summary>
        public byte Code => this.innerData.AsSpan().GetPacketType();

        /// <summary>
        /// Gets the sub code of the packet, which further specifies the kind of the message, if specified.
        /// </summary>
        public byte SubCode => this.innerData.AsSpan().GetPacketSubType();

        /// <summary>
        /// Gets the direction as string.
        /// </summary>
        public string Direction { get; }

        /// <summary>
        /// Gets a value indicating whether the packet was sent to the server; Otherwise it was sent to the client.
        /// </summary>
        public bool ToServer { get; }

        /// <summary>
        /// Gets the size of the packet byte array.
        /// </summary>
        public int Size => this.innerData.Length;

        /// <summary>
        /// Gets the packet data as binary string.
        /// </summary>
        public string PacketData => this.dataAsString ?? (this.dataAsString = BitConverter.ToString(this.innerData).Replace('-', ' '));

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Direction}: {this.PacketData}";
        }

        /// <summary>
        /// Extracts the field value from this packet.
        /// </summary>
        /// <param name="field">The field definition.</param>
        /// <returns>The value of the field.</returns>
        internal string ExtractFieldValue(Field field)
        {
            var fieldSize = this.GetFieldSizeInBytes(field) ?? int.MaxValue;
            if (field.Type == FieldType.String && field.Index < this.innerData.Length)
            {
                return this.innerData.ExtractString(field.Index, fieldSize, Encoding.UTF8);
            }

            if (field.Type == FieldType.Binary && field.Index < this.innerData.Length)
            {
                return BitConverter.ToString(this.innerData, field.Index, field.Length).Replace('-', ' ');
            }

            if (this.innerData.Length < field.Index + fieldSize)
            {
                return string.Empty;
            }

            switch (field.Type)
            {
                case FieldType.Byte:
                    return this.innerData[field.Index].ToString();
                case FieldType.Integer:
                    return this.innerData.MakeDwordSmallEndian(field.Index).ToString();
                case FieldType.IntegerBigEndian:
                    return this.innerData.MakeDwordBigEndian(field.Index).ToString();
                case FieldType.Short:
                    return this.innerData.MakeWordSmallEndian(field.Index).ToString();
                case FieldType.ShortBigEndian:
                    return this.innerData.MakeWordBigEndian(field.Index).ToString();
                case FieldType.Long:
                    return this.innerData.MakeQword(field.Index).ToString();
                case FieldType.LongBigEndian:
                    return this.innerData.MakeQwordBigEndian(field.Index).ToString();
                default:
                    return string.Empty;
            }
        }

        private int? GetFieldSizeInBytes(Field field)
        {
            switch (field.Type)
            {
                case FieldType.Byte:
                    return 1;
                case FieldType.Integer:
                case FieldType.IntegerBigEndian:
                    return sizeof(int);
                case FieldType.Long:
                case FieldType.LongBigEndian:
                    return sizeof(long);
                case FieldType.Short:
                case FieldType.ShortBigEndian:
                    return sizeof(short);
                case FieldType.String:
                    return field.LengthSpecified ? field.Length : (int?)null;
                default:
                    return null;
            }
        }
    }
}
