// <copyright file="Xor32Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor
{
    using System;

    /// <summary>
    /// Encryptor which uses a 32 byte key for a xor encryption.
    /// It's typically used to encrypt packet sent by the client to the server.
    /// </summary>
    public class Xor32Encryptor : IEncryptor
    {
        private readonly byte[] xor32Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="Xor32Encryptor"/> class
        /// using <see cref="DefaultKeys.Xor32Key"/>.
        /// </summary>
        public Xor32Encryptor()
            : this(DefaultKeys.Xor32Key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Xor32Encryptor"/> class.
        /// </summary>
        /// <param name="xor32Key">The xor32 key.</param>
        public Xor32Encryptor(byte[] xor32Key)
        {
            if (xor32Key.Length != 32)
            {
                throw new ArgumentException($"Xor32key must have a size of 32 bytes, but is {xor32Key.Length} bytes long.");
            }

            this.xor32Key = xor32Key;
        }

        /// <inheritdoc />
        public byte[] Encrypt(byte[] packet)
        {
            int headerSize = packet.GetPacketHeaderSize();
            for (int i = headerSize + 1; i < packet.Length; i++)
            {
                packet[i] = (byte)(packet[i] ^ packet[i - 1] ^ this.xor32Key[i % 32]);
            }

            return packet;
        }

        /// <inheritdoc />
        public void Reset()
        {
            // no reset required as this encryptor is stateless.
        }
    }
}
