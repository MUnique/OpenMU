// <copyright file="SimpleModulusBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    /// <summary>
    /// The base class for the "simple modulus" encryption.
    /// </summary>
    public abstract class SimpleModulusBase
    {
        /// <summary>
        /// The decrypted block size.
        /// </summary>
        protected const int DecryptedBlockSize = 8;

        /// <summary>
        /// The encrypted block size.
        /// It is bigger than the decrypted size, because it contains the length of the actual data of the block and a checksum.
        /// </summary>
        protected const int EncryptedBlockSize = 11;

        /// <summary>
        /// The xor key which is used as to 'encrypt' the size of each block.
        /// </summary>
        protected const byte BlockSizeXorKey = 0x3D;

        /// <summary>
        /// The xor key which is used as to 'encrypt' the checksum of each encrypted block.
        /// </summary>
        protected const byte BlockCheckSumXorKey = 0xF8;

        /// <summary>
        /// Gets the counter.
        /// </summary>
        protected Counter Counter { get; } = new Counter();

        /// <summary>
        /// Gets the decrypted block buffer.
        /// </summary>
        protected byte[] DecryptedBlockBuffer { get; } = new byte[DecryptedBlockSize];

        /// <summary>
        /// Gets the encrypted block buffer.
        /// </summary>
        protected byte[] EncryptedBlockBuffer { get; } = new byte[EncryptedBlockSize];

        /// <summary>
        /// Gets the ring buffer.
        /// </summary>
        protected uint[] RingBuffer { get; } = new uint[4];

        /// <summary>
        /// Gets the shift buffer.
        /// </summary>
        protected byte[] ShiftBuffer { get; } = new byte[4];

        /// <summary>
        /// Gets the crypt buffer.
        /// </summary>
        protected ushort[] CryptBuffer { get; } = new ushort[4];

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Counter.Reset();
        }

        /// <summary>
        /// Does some shifting.
        /// </summary>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="shiftArray">The shift array with the input data.</param>
        /// <param name="shiftOffset">The shift offset.</param>
        /// <param name="size">The size of the input data.</param>
        protected void InternalShiftBytes(byte[] outputBuffer, int outputOffset, byte[] shiftArray, int shiftOffset, int size)
        {
            shiftOffset &= 0x7;
            ShiftRight(shiftArray, size, shiftOffset);
            ShiftLeft(shiftArray, size + 1, outputOffset & 0x7);
            if ((outputOffset & 0x7) > shiftOffset)
            {
                size++;
            }

            var offset = outputOffset / DecryptedBlockSize;
            for (var i = 0; i < size; i++)
            {
                outputBuffer[i + offset] |= shiftArray[i];
            }
        }

        /// <summary>
        /// Gets the size of the content.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="decrypted">if set to <c>true</c> it is decrypted. Encrypted packets additionally contain a counter.</param>
        /// <returns>The size of the actual content.</returns>
        protected int GetContentSize(byte[] packet, bool decrypted)
        {
            return packet.GetPacketSize() - packet.GetPacketHeaderSize() + (decrypted ? 1 : 0);
        }

        /// <summary>
        /// Gets the number of bytes to shift.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="shiftOffset">The shift offset.</param>
        /// <returns>The number of bytes to shift</returns>
        protected int GetShiftSize(int length, int shiftOffset)
        {
            return ((length + shiftOffset - 1) / DecryptedBlockSize) + (1 - (shiftOffset / DecryptedBlockSize));
        }

        /// <summary>
        /// Clears the shift buffer.
        /// </summary>
        protected void ClearShiftBuffer()
        {
            this.ShiftBuffer[0] = 0;
            this.ShiftBuffer[1] = 0;
            this.ShiftBuffer[2] = 0;
            this.ShiftBuffer[3] = 0;
        }

        private static void ShiftLeft(byte[] data, int size, int shift)
        {
            if (shift == 0)
            {
                return;
            }

            for (var i = 1; i < size; i++)
            {
                data[size - i] = (byte)((data[size - i] >> shift) | (data[size - i - 1] << (8 - shift)));
            }

            data[0] >>= shift;
        }

        private static void ShiftRight(byte[] data, int size, int shift)
        {
            if (shift == 0)
            {
                return;
            }

            for (var i = 1; i < size; i++)
            {
                data[i - 1] = (byte)((data[i - 1] << shift) | (data[i] >> (8 - shift)));
            }

            data[size - 1] <<= shift;
        }
    }
}
