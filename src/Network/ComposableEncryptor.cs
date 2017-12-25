// <copyright file="ComposableEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.Collections.Generic;

    /// <summary>
    /// A encryptor which can be composed by several other encryptors.
    /// </summary>
    public class ComposableEncryptor : IEncryptor
    {
        private readonly ICollection<IEncryptor> encryptors = new List<IEncryptor>(2);

        /// <inheritdoc />
        public byte[] Encrypt(byte[] packet)
        {
            var result = packet;
            foreach (var encryptor in this.encryptors)
            {
                result = encryptor.Encrypt(result);
            }

            return result;
        }

        /// <inheritdoc />
        public void Reset()
        {
            foreach (var encryptor in this.encryptors)
            {
                encryptor.Reset();
            }
        }

        /// <summary>
        /// Adds the specified encryptor to the encryption chain.
        /// </summary>
        /// <param name="encryptor">The encryptor.</param>
        /// <returns>This instance, to allow chained calls.</returns>
        public ComposableEncryptor AddEncryptor(IEncryptor encryptor)
        {
            this.encryptors.Add(encryptor);
            return this;
        }
    }
}
