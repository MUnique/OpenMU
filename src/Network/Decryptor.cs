// <copyright file="Decryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.Network.Xor32;

    /// <summary>
    /// The default decryptor used by the server to decrypt incoming data packets.
    /// </summary>
    public class Decryptor : ComposableDecryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decryptor"/> class
        /// which uses default keys.
        /// </summary>
        public Decryptor()
            : this(SimpleModulusDecryptor.DefaultServerKey, DefaultKeys.Xor32Key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decryptor"/> class.
        /// </summary>
        /// <param name="decryptionKey">The decryption key.</param>
        /// <param name="xor32Key">The xor32 key.</param>
        public Decryptor(SimpleModulusKeys decryptionKey, byte[] xor32Key)
        {
            this.AddDecryptor(new SimpleModulusDecryptor(decryptionKey))
                .AddDecryptor(new Xor32Decryptor(xor32Key));
        }
    }
}
