// <copyright file="InvalidBlockChecksumException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;

    /// <summary>
    /// Exception which occurs when a block contains an invalid checksum.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class InvalidBlockChecksumException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBlockChecksumException"/> class.
        /// </summary>
        /// <param name="actualChecksum">The actual checksum.</param>
        /// <param name="expectedChecksum">The expected checksum.</param>
        public InvalidBlockChecksumException(byte actualChecksum, byte expectedChecksum)
            : base(CreateMessage(actualChecksum, expectedChecksum))
        {
        }

        private static string CreateMessage(byte actualChecksum, byte expectedChecksum)
        {
            return $"Block checksum invalid. Expected: {expectedChecksum}. Actual: {actualChecksum}.";
        }
    }
}
