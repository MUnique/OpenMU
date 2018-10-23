// <copyright file="ISpanDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;

    /// <summary>
    /// This interface specifies an decryptor, which decrypts span data packets in-place.
    /// </summary>
    public interface ISpanDecryptor
    {
        /// <summary>
        /// Decrypts the packet.
        /// </summary>
        /// <param name="packet">The encrypted data packet. The decrypted result will be written back to the same span.</param>
        void Decrypt(Span<byte> packet);
    }
}
