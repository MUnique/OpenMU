// <copyright file="Utilities.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;

    /// <summary>
    /// Some utility functions.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Converts a byte array to a hexadecimal string, each byte separated by space.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The hexadecimal string.</returns>
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }
    }
}
