// <copyright file="FieldExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using MUnique.OpenMU.Network.Packets;

    /// <summary>
    /// Extensions for <see cref="Field"/>s.
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        /// Gets the field size in bytes.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The size of the field in bytes.</returns>
        internal static int? GetFieldSizeInBytes(this Field field)
        {
            switch (field.Type)
            {
                case FieldType.Byte:
                case FieldType.Enum:
                case FieldType.Boolean:
                    return 1;
                case FieldType.Float:
                case FieldType.IntegerLittleEndian:
                case FieldType.IntegerBigEndian:
                    return sizeof(int);
                case FieldType.LongLittleEndian:
                case FieldType.LongBigEndian:
                    return sizeof(long);
                case FieldType.ShortLittleEndian:
                case FieldType.ShortBigEndian:
                    return sizeof(short);
                case FieldType.String:
                case FieldType.StructureArray:
                case FieldType.Binary:
                    return field.LengthSpecified ? field.Length : (int?)null;
                default:
                    return null;
            }
        }
    }
}