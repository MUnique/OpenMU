// <copyright file="StreamExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO;

    /// <summary>
    /// Extensions for streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads an integer from the current position of a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The integer from the stream.</returns>
        /// <exception cref="EndOfStreamException">Length: {stream.Length}; Position: {stream.Position}.</exception>
        public static uint ReadInteger(this Stream stream)
        {
            if (stream.Length < stream.Position + sizeof(uint))
            {
                throw new EndOfStreamException($"Length: {stream.Length}; Position: {stream.Position}");
            }

            return NumberConversionExtensions.MakeDword((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
        }
    }
}
