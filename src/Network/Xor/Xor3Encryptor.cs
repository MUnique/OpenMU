// <copyright file="Xor3Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor
{
    /// <summary>
    /// An encryptor which XOR-encrypts data using a 3-byte key.
    /// </summary>
    public class Xor3Encryptor : IEncryptor
    {
        private readonly byte[] xor3Keys;

        private readonly int startOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Xor3Encryptor"/> class.
        /// </summary>
        /// <param name="startOffset">The start offset.</param>
        public Xor3Encryptor(int startOffset)
        {
            this.startOffset = startOffset;
            this.xor3Keys = DefaultKeys.Xor3Keys;
        }

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] data)
        {
            return this.InternalEncrypt(data);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            // nothing needed
        }

        /// <summary>
        /// Internal encrypt function. XORs each byte with one byte of the 3-byte key.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The encrypted data.</returns>
        protected byte[] InternalEncrypt(byte[] data)
        {
            for (var i = 0; i < data.Length - this.startOffset; i++)
            {
                data[i + this.startOffset] ^= this.xor3Keys[i % 3];
            }

            return data;
        }
    }
}
