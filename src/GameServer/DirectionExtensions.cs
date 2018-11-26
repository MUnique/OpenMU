// <copyright file="DirectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Extensions for <see cref="Direction"/> which concern the communication with the game client.
    /// </summary>
    internal static class DirectionExtensions
    {
        /// <summary>
        /// Gets the packet byte value for the given direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The packet byte value for the given direction.</returns>
        public static byte ToPacketByte(this Direction direction)
        {
            if (direction != Direction.Undefined)
            {
                return (byte)(direction - 1);
            }

            return 0;
        }

        /// <summary>
        /// Parses as direction byte value into a <see cref="Direction"/>.
        /// </summary>
        /// <param name="packetByte">The packet byte.</param>
        /// <returns>The direction.</returns>
        public static Direction ParseAsDirection(this byte packetByte)
        {
            return (Direction)(packetByte + 1);
        }
    }
}
