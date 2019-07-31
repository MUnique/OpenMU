// <copyright file="DefaultKeys.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor
{
    /// <summary>
    /// The default xor keys for encryption and decryption.
    /// </summary>
    public static class DefaultKeys
    {
        /// <summary>
        /// Gets the default 3 byte long XOR key.
        /// </summary>
        /// <value>
        /// The default 3 byte long XOR key.
        /// </value>
        public static byte[] Xor3Keys { get; } = { 0xFC, 0xCF, 0xAB };

        /// <summary>
        /// Gets the default 32 byte long XOR key.
        /// </summary>
        /// <value>
        /// The default 32 byte long XOR key.
        /// </value>
        public static byte[] Xor32Key { get; } =
        {
            0xAB, 0x11, 0xCD, 0xFE, 0x18, 0x23, 0xC5, 0xA3,
            0xCA, 0x33, 0xC1, 0xCC, 0x66, 0x67, 0x21, 0xF3,
            0x32, 0x12, 0x15, 0x35, 0x29, 0xFF, 0xFE, 0x1D,
            0x44, 0xEF, 0xCD, 0x41, 0x26, 0x3C, 0x4E, 0x4D,
        };
    }
}
