// <copyright file="IPacketTwister.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PacketTwister
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a packet twister of a specific packet type.
    /// </summary>
    internal interface IPacketTwister
    {
        /// <summary>
        /// Twists (encrypts) the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Twist(IList<byte> data);

        /// <summary>
        /// Corrects (decrypts) the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Correct(IList<byte> data);
    }
}