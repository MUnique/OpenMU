// <copyright file="InvalidBlockSizeException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;

    /// <summary>
    /// Exception which occurs when a block contains an invalid size.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class InvalidBlockSizeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBlockSizeException"/> class.
        /// </summary>
        /// <param name="actualSize">The actual size found in the block.</param>
        /// <param name="maximumSize">The expected maximum size.</param>
        public InvalidBlockSizeException(byte actualSize, int maximumSize)
            : base(CreateMessage(actualSize, maximumSize))
        {
        }

        private static string CreateMessage(byte actualSize, int maximumSize)
        {
            return $"Block size invalid. Maximum expected: {maximumSize}. Actual: {actualSize}.";
        }
    }
}