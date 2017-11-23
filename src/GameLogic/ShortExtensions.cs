// <copyright file="ShortExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// Extension methods for <see cref="short"/>.
    /// </summary>
    public static class ShortExtensions
    {
        /// <summary>
        /// Converts the signed short to an unsigned short.
        /// </summary>
        /// <param name="value">The signed value.</param>
        /// <returns>The unsigned value.</returns>
        public static ushort ToUnsigned(this short value)
        {
            return unchecked((ushort)value);
        }

        /// <summary>
        /// Converts the signed short to an unsigned short.
        /// </summary>
        /// <param name="value">The signed value.</param>
        /// <returns>The unsigned value.</returns>
        public static short ToSigned(this ushort value)
        {
            return unchecked((short)value);
        }
    }
}
