// <copyright file="SimpleModulusEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;

    /// <summary>
    /// The standard encryptor (server-side) which encrypts 0xC3 and 0xC4-packets with the "simple modulus" algorithm.
    /// </summary>
    public class SimpleModulusEncryptor : SimpleModulusBase, IEncryptor
    {
        /// <summary>
        /// The default server side encryption key. The corrsponding encryption key is <see cref="SimpleModulusDecryptor.DefaultClientKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultServerKey = SimpleModulusKeys.CreateEncryptionKeys(new uint[] { 73326, 109989, 98843, 171058, 13169, 19036, 35482, 29587, 62004, 64409, 35374, 64599 });

        /// <summary>
        /// The default client side decryption key. The corrsponding encryption key is <see cref="SimpleModulusDecryptor.DefaultServerKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultClientKey = SimpleModulusKeys.CreateEncryptionKeys(new uint[] { 128079, 164742, 70235, 106898, 23489, 11911, 19816, 13647, 48413, 46165, 15171, 37433 });

        private readonly SimpleModulusKeys encryptionKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusEncryptor"/> class with the default key.
        /// </summary>
        public SimpleModulusEncryptor()
            : this(DefaultServerKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusEncryptor"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        public SimpleModulusEncryptor(uint[] encryptionKey)
            : this(SimpleModulusKeys.CreateEncryptionKeys(encryptionKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusEncryptor" /> class.
        /// </summary>
        /// <param name="encryptionKeys">The encryption keys.</param>
        public SimpleModulusEncryptor(SimpleModulusKeys encryptionKeys)
        {
            this.encryptionKeys = encryptionKeys;
        }

        /// <inheritdoc/>
        public byte[] Encrypt(byte[] packet)
        {
            if (packet[0] < 0xC3)
            {
                return packet;
            }

            lock (this.Counter)
            {
                var result = this.EncryptC3(packet);
                this.Counter.Increase();
                return result;
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
            var headerSize = data.GetPacketHeaderSize();
            var contentSize = this.GetContentSize(data, true);
            var contents = new byte[contentSize];
            contents[0] = (byte)this.Counter.Count;
            Buffer.BlockCopy(data, headerSize, contents, 1, contentSize - 1);
            var result = new byte[this.GetEncryptedSize(data)];
            this.EncodeBuffer(contents, headerSize, contentSize, result);
            result[0] = data[0];
            result.SetPacketSize();

            return result;
        }

        private int GetEncryptedSize(byte[] data)
        {
            var contentSize = this.GetContentSize(data, true);
            return (((contentSize / DecryptedBlockSize) + (((contentSize % DecryptedBlockSize) > 0) ? 1 : 0)) * EncryptedBlockSize) + data.GetPacketHeaderSize();
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
            byte size = (byte)(blockSize ^ BlockSizeXorKey);
            byte checksum = BlockCheckSumXorKey;
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
            this.RingBuffer[0] = ((keys.XorKey[0] ^ this.CryptBuffer[0]) * keys.EncryptKey[0]) % keys.ModulusKey[0];
            this.RingBuffer[1] = ((keys.XorKey[1] ^ (this.CryptBuffer[1] ^ (this.RingBuffer[0] & 0xFFFF))) * keys.EncryptKey[1]) % keys.ModulusKey[1];
            this.RingBuffer[2] = ((keys.XorKey[2] ^ (this.CryptBuffer[2] ^ (this.RingBuffer[1] & 0xFFFF))) * keys.EncryptKey[2]) % keys.ModulusKey[2];
            this.RingBuffer[3] = ((keys.XorKey[3] ^ (this.CryptBuffer[3] ^ (this.RingBuffer[2] & 0xFFFF))) * keys.EncryptKey[3]) % keys.ModulusKey[3];
            Buffer.BlockCopy(this.CryptBuffer, 0, inputBuffer, 0, blockSize);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys.XorKey[0] ^ (this.RingBuffer[1] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys.XorKey[1] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys.XorKey[2] ^ (this.RingBuffer[3] & 0xFFFF);
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
