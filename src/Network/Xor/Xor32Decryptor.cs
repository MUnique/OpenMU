// <copyright file="Xor32Decryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor
{
    /// <summary>
    /// Decryptor which uses a 32 byte key for a xor encryption.
    /// It's typically used to decrypt packets sent by the client to the server.
    /// </summary>
    public class Xor32Decryptor : IDecryptor
    {
        private readonly byte[] xor32Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="Xor32Decryptor"/> class
        /// using <see cref="DefaultKeys.Xor32Key"/>.
        /// </summary>
        public Xor32Decryptor()
            : this(DefaultKeys.Xor32Key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Xor32Decryptor"/> class.
        /// </summary>
        /// <param name="xor32Key">The xor32 key.</param>
        public Xor32Decryptor(byte[] xor32Key)
        {
            this.xor32Key = xor32Key;
        }

        /// <inheritdoc />
        public bool Decrypt(ref byte[] packet)
        {
            var headerSize = packet.GetPacketHeaderSize();
            for (var i = packet.Length - 1; i > headerSize; i--)
            {
                packet[i] = (byte)(packet[i] ^ packet[i - 1] ^ this.xor32Key[i % 32]);
            }

            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            // no reset required as this decryptor is stateless.
        }
    }
}
