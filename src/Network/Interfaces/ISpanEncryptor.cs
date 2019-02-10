// <copyright file="ISpanEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;

    /// <summary>
    /// This interface specifies an encryptor, which encrypts span data packets in-place.
    /// </summary>
    public interface ISpanEncryptor
    {
        /// <summary>
        /// Encrypts the packet.
        /// </summary>
        /// <param name="packet">The decrypted data packet.</param>
        void Encrypt(Span<byte> packet);
    }
}