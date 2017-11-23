// <copyright file="Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;

    /// <summary>
    /// The standard encryptor (server-side) which encrypts 0xC3 and 0xC4-packets with the "simple modulus" algorithm.
    /// </summary>
    public class Encryptor : EncryptionBase, IEncryptor
    {
        private readonly uint[] encryptionKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="Encryptor"/> class with the default key.
        /// </summary>
        public Encryptor()
            : this(DefaultKeys.EncryptionKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Encryptor"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        public Encryptor(uint[] encryptionKey)
        {
            this.encryptionKeys = encryptionKey;
        }

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] packet)
        {
            switch (packet[0])
            {
                case 0xC1:
                case 0xC2:
                    // no encryption needed from server to client
                    return packet;
                case 0xC3:
                case 0xC4:
                    lock (this.Counter)
                    {
                        var result = this.EncryptC3(packet);
                        this.Counter.Increase();
                        return result;
                    }

                default:
                    throw new Exception("packet[0] is not 0xC1, 0xC2, 0xC3 or 0xC4, but " + packet[0].ToString("X2"));
            }
        }

        private static void CopyIntToArray(byte[] targetArray, uint value, int valueOffset, int size)
        {
            var targetIndex = 0;
            for (int i = valueOffset; i < valueOffset + size; i++)
            {
                targetArray[targetIndex] = (byte)((value >> (8 * i)) & 0xFF);
                targetIndex++;
            }
        }

        private byte[] EncryptC3(byte[] data)
        {
            var headerSize = this.GetHeaderSize(data[0]);
            var contentSize = this.GetContentSize(data, true);
            var contents = new byte[contentSize];
            contents[0] = (byte)this.Counter.Count;
            Buffer.BlockCopy(data, headerSize, contents, 1, contentSize - 1);
            var result = new byte[this.GetEncryptedSize(data)];
            this.EncodeBuffer(contents, headerSize, contentSize, result);
            result[0] = data[0];
            this.SetPacketSize(result, result.Length);

            return result;
        }

        private int GetEncryptedSize(byte[] data)
        {
            var contentSize = this.GetContentSize(data, true);
            return (((contentSize / DecryptedBlockSize) + (((contentSize % DecryptedBlockSize) > 0) ? 1 : 0)) * EncryptedBlockSize) + this.GetHeaderSize(data[0]);
        }

        private void EncodeBuffer(byte[] inputBuffer, int offset, int size, byte[] result)
        {
            var i = 0;
            var sizeCounter = 0;
            while (i < size)
            {
                Array.Clear(this.EncryptedBlockBuffer, 0, this.EncryptedBlockBuffer.Length);
                if (i + DecryptedBlockSize < size)
                {
                    Buffer.BlockCopy(inputBuffer, i, this.DecryptedBlockBuffer, 0, DecryptedBlockSize);
                    this.BlockEncode(this.EncryptedBlockBuffer, this.DecryptedBlockBuffer, DecryptedBlockSize);
                }
                else
                {
                    Buffer.BlockCopy(inputBuffer, i, this.DecryptedBlockBuffer, 0, size - i);
                    this.BlockEncode(this.EncryptedBlockBuffer, this.DecryptedBlockBuffer, size - i);
                }

                Buffer.BlockCopy(this.EncryptedBlockBuffer, 0, result, offset + sizeCounter, EncryptedBlockSize);
                i += DecryptedBlockSize;
                sizeCounter += EncryptedBlockSize;
            }
        }

        private void BlockEncode(byte[] outputBuffer, byte[] inputBuffer, int blockSize)
        {
            this.SetRingBuffer(inputBuffer, blockSize);
            this.ShiftBytes(outputBuffer, 0x00, this.RingBuffer[0], 0x00, 0x10);
            this.ShiftBytes(outputBuffer, 0x10, this.RingBuffer[0], 0x16, 0x02);
            this.ShiftBytes(outputBuffer, 0x12, this.RingBuffer[1], 0x00, 0x10);
            this.ShiftBytes(outputBuffer, 0x22, this.RingBuffer[1], 0x16, 0x02);
            this.ShiftBytes(outputBuffer, 0x24, this.RingBuffer[2], 0x00, 0x10);
            this.ShiftBytes(outputBuffer, 0x34, this.RingBuffer[2], 0x16, 0x02);
            this.ShiftBytes(outputBuffer, 0x36, this.RingBuffer[3], 0x00, 0x10);
            this.ShiftBytes(outputBuffer, 0x46, this.RingBuffer[3], 0x16, 0x02);
            this.EncodeFinal(blockSize, inputBuffer, outputBuffer);
        }

        /// <summary>
        /// Encodes the final part of the block. It contains a checksum and the length of the block, which is needed for decryption.
        /// </summary>
        /// <param name="blockSize">The size of the block of decrypted data in bytes.</param>
        /// <param name="inputBuffer">The input buffer with the incoming decrypted data..</param>
        /// <param name="outputBuffer">The output buffer to which the encrypted result will be written.</param>
        private void EncodeFinal(int blockSize, byte[] inputBuffer, byte[] outputBuffer)
        {
            byte size = (byte)(blockSize ^ 0x3D); // TODO: Magic number
            byte checksum = 0xF8; // TODO: Magic number
            for (var i = 0; i < blockSize; i++)
            {
                checksum ^= inputBuffer[i];
            }

            size ^= checksum;

            this.ShiftBytes(outputBuffer, 0x48, (uint)(checksum << 8 | size), 0x00, 0x10);
        }

        private void SetRingBuffer(byte[] inputBuffer, int blockSize)
        {
            var keys = this.encryptionKeys;
            Array.Clear(this.CryptBuffer, blockSize / 2, this.CryptBuffer.Length - (blockSize / 2)); // we don't need to clear the whole array since parts are getting overriden by the input buffer
            Buffer.BlockCopy(inputBuffer, 0, this.CryptBuffer, 0, blockSize);
            this.RingBuffer[0] = ((keys[8] ^ this.CryptBuffer[0]) * keys[4]) % keys[0];
            this.RingBuffer[1] = ((keys[9] ^ (this.CryptBuffer[1] ^ (this.RingBuffer[0] & 0xFFFF))) * keys[5]) % keys[1];
            this.RingBuffer[2] = ((keys[10] ^ (this.CryptBuffer[2] ^ (this.RingBuffer[1] & 0xFFFF))) * keys[6]) % keys[2];
            this.RingBuffer[3] = ((keys[11] ^ (this.CryptBuffer[3] ^ (this.RingBuffer[2] & 0xFFFF))) * keys[7]) % keys[3];
            Buffer.BlockCopy(this.CryptBuffer, 0, inputBuffer, 0, blockSize);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys[8] ^ (this.RingBuffer[1] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys[9] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys[10] ^ (this.RingBuffer[3] & 0xFFFF);
        }

        private void ShiftBytes(byte[] outputBuffer, int outputOffset, uint shift, int shiftOffset, int length)
        {
            int size = this.GetShiftSize(length, shiftOffset);
            this.ShiftBuffer[2] = 0; // the first two bytes will be set at the next statement
            CopyIntToArray(this.ShiftBuffer, shift, shiftOffset / DecryptedBlockSize, size);
            this.InternalShiftBytes(outputBuffer, outputOffset, this.ShiftBuffer, shiftOffset, size);
        }
    }
}
