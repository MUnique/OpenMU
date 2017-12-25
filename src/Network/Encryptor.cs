// <copyright file="Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using MUnique.OpenMU.Network.SimpleModulus;

    /// <summary>
    /// The default encryptor used by the server to encrypt outgoing data packets.
    /// </summary>
    public class Encryptor : ComposableEncryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Encryptor"/> class
        /// which uses default keys.
        /// </summary>
        public Encryptor()
            : this(SimpleModulusEncryptor.DefaultServerKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Encryptor"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        public Encryptor(SimpleModulusKeys encryptionKey)
        {
            this.AddEncryptor(new SimpleModulusEncryptor(encryptionKey));
        }
    }
}
