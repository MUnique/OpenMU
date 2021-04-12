// <copyright file="EncodingExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System.Text;

    /// <summary>
    /// Extensions for an <see cref="Encoding"/>.
    /// </summary>
    public static class EncodingExtensions
    {
        /// <summary>
        /// Gets the maximum character count of the given text which fits in the specified maximum byte count.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="text">The text.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>The maximum character count of the given text which fits in the specified maximum byte count.</returns>
        public static int GetCharacterCountOfMaxByteCount(this Encoding encoding, string text, int maximum)
        {
            if (encoding.GetByteCount(text) <= maximum)
            {
                return text.Length;
            }

            var current = maximum;
            while (encoding.GetByteCount(text, 0, current) > maximum)
            {
                --current;
            }

            return current;
        }
    }
}
