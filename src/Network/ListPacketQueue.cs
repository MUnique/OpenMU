// <copyright file="ListPacketQueue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Description of ListPacketQueue.
    /// </summary>
    internal class ListPacketQueue : List<byte>
    {
        private int currentIndex;

        /// <summary>
        /// Dequeues the next packet of the queue. Returns null if there is no next packet available.
        /// </summary>
        /// <returns>The next packet, or null if not available.</returns>
        public byte[] DequeueNextPacket()
        {
            var length = this.GetNextLength();
            if (length == 0)
            {
                return null;
            }

            if (this.Count < this.currentIndex + length)
            {
                return null;
            }

            var packet = this.GetNextPacket(length);
            this.currentIndex += length;
            if (this.Count <= this.currentIndex)
            {
                this.currentIndex = 0;
                this.Clear();
            }

            return packet;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.currentIndex = 0;
            this.Clear();
        }

        private byte[] GetNextPacket(int length)
        {
            var packet = new byte[length];
            for (int i = 0; i < length; i++)
            {
                packet[i] = this[i + this.currentIndex];
            }

            return packet;
        }

        private int GetNextLength()
        {
            if (this.Count < this.currentIndex + 2)
            {
                return 0;
            }

            switch (this[this.currentIndex])
            {
                case 0xC1:
                case 0xC3:
                    return this[this.currentIndex + 1];
                case 0xC2:
                case 0xC4:
                    return (this[this.currentIndex + 1] * 0x100) + this[this.currentIndex + 2];
                default:
                    throw new Exception("Unexpected byte value");
            }
        }
    }
}
