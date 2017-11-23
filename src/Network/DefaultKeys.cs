// <copyright file="DefaultKeys.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    /// <summary>
    /// The default keys for encryption and decryption.
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
        /// Gets the default encryption key.
        /// </summary>
        /// <value>
        /// The default encryption key.
        /// </value>
        public static uint[] EncryptionKey { get; } = { 73326, 109989, 98843, 171058, 13169, 19036, 35482, 29587, 62004, 64409, 35374, 64599 };

        /// <summary>
        /// Gets the default decryption key.
        /// </summary>
        /// <value>
        /// The default decryption key.
        /// </value>
        public static uint[] DecryptionKey { get; } = { 128079, 164742, 70235, 106898, 31544, 2047, 57011, 10183, 48413, 46165, 15171, 37433 };

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
            0x44, 0xEF, 0xCD, 0x41, 0x26, 0x3C, 0x4E, 0x4D
        };
    }
}
