// <copyright file="IEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    /// <summary>
    /// This interface specifies an encryptor, which encrypts data packets.
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypts the packet.
        /// </summary>
        /// <param name="packet">The decrypted data packet.</param>
        /// <returns>The encrypted data packet.</returns>
        byte[] Encrypt(byte[] packet);

        /// <summary>
        /// Resets the state of the encryptor.
        /// </summary>
        void Reset();
    }
}
