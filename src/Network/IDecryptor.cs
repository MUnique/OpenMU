// <copyright file="IDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    /// <summary>
    /// This interface specifies a decryptor, which decrypts encrypted data packets.
    /// </summary>
    public interface IDecryptor
    {
        /// <summary>
        /// Decrypts the packet, and checks if the packet was valid.
        /// </summary>
        /// <param name="packet">The encrypted data packet. The decrypted result will be written back to the same reference.</param>
        /// <returns>True, if successful; False, otherwise.</returns>
        bool Decrypt(ref byte[] packet);

        /// <summary>
        /// Resets the state of the decryptor.
        /// </summary>
        void Reset();
    }
}
