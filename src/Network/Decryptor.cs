// <copyright file="Decryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using log4net;

    /// <summary>
    /// The standard decryptor (server-side) which decrypts with the "simple modulus"
    /// algorithm first, and then with the 32 byte XOR-key.
    /// </summary>
    public class Decryptor : EncryptionBase, IDecryptor
    {
        private readonly uint[] decryptionKey;
        private readonly byte[] xor32Key;
        private readonly byte[] shiftArray = new byte[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="Decryptor"/> class with standard keys.
        /// </summary>
        public Decryptor()
            : this(DefaultKeys.DecryptionKey, DefaultKeys.Xor32Key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decryptor"/> class.
        /// </summary>
        /// <param name="decryptionKey">The decryption key.</param>
        /// <param name="xor32Key">The 32 byte XOR-key.</param>
        public Decryptor(uint[] decryptionKey, byte[] xor32Key)
        {
            this.decryptionKey = decryptionKey ?? DefaultKeys.DecryptionKey;
            this.xor32Key = xor32Key ?? DefaultKeys.Xor32Key;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this decryptor instance accepts wrong block checksum, or throws an exception in this case.
        /// </summary>
        public bool AcceptWrongBlockChecksum { get; set; }

        private static ILog Log { get; } = LogManager.GetLogger(typeof(Decryptor));

        /// <inheritdoc/>
        public bool Decrypt(ref byte[] packet)
        {
            try
            {
                var result = true;
                switch (packet[0])
                {
                    case 0xC1:
                    case 0xC2:
                        this.DecryptC1(packet);
                        break;
                    case 0xC3:
                    case 0xC4:
                        lock (this.Counter)
                        {
                            result = this.Counter.Count == this.DecryptC3(ref packet);
                            this.Counter.Increase();
                        }

                        break;
                    default:
                        Log.Error($"packet[0] is not 0xC1, 0xC2, 0xC3 or 0xC4, but {packet[0] :X2}");
                        result = false;
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        private byte DecryptC3(ref byte[] data)
        {
            var contentSize = this.GetContentSize(data, false);
            var headerSize = this.GetHeaderSize(data[0]);
            var result = new byte[this.GetMaximumDecryptedSize(data)];
            var decryptedSize = this.DecodeBuffer(data, headerSize, contentSize, result);
            decryptedSize += headerSize - 1;
            var decryptedCount = result[headerSize - 1];
            result[0] = data[0];
            this.SetPacketSize(result, decryptedSize);

            Array.Resize(ref result, decryptedSize);
            this.DecryptC1(result);
            data = result;

            return decryptedCount;
        }

        private int DecodeBuffer(byte[] inputBuffer, int offset, int size, byte[] result)
        {
            int sizeCounter = 0;
            if ((size % EncryptedBlockSize) != 0)
            {
                return sizeCounter;
            }

            for (int i = 0; i < size; i += EncryptedBlockSize)
            {
                Buffer.BlockCopy(inputBuffer, i + offset, this.EncryptedBlockBuffer, 0, EncryptedBlockSize);
                var blockSize = this.BlockDecode(this.DecryptedBlockBuffer, this.EncryptedBlockBuffer);
                if (blockSize != -1)
                {
                    Buffer.BlockCopy(this.DecryptedBlockBuffer, 0, result, (offset - 1) + sizeCounter, blockSize);
                    sizeCounter += blockSize;
                }
            }

            return sizeCounter;
        }

        /// <summary>
        /// Decodes the block.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <param name="inputBuffer">The input buffer array. Contains the data which should be decrypted.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int BlockDecode(byte[] outputBuffer, byte[] inputBuffer)
        {
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, inputBuffer, 0x00, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, inputBuffer, 0x10, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 0, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, inputBuffer, 0x12, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, inputBuffer, 0x22, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 4, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, inputBuffer, 0x24, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, inputBuffer, 0x34, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 8, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, inputBuffer, 0x36, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, inputBuffer, 0x46, 0x02);

            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 12, 4);
            var keys = this.decryptionKey;
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys[10] ^ (this.RingBuffer[3] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys[9] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys[8] ^ (this.RingBuffer[1] & 0xFFFF);

            this.CryptBuffer[0] = (ushort)(keys[8] ^ ((this.RingBuffer[0] * keys[4]) % keys[0]));
            this.CryptBuffer[1] = (ushort)(keys[9] ^ ((this.RingBuffer[1] * keys[5]) % keys[1]) ^ (this.RingBuffer[0] & 0xFFFF));
            this.CryptBuffer[2] = (ushort)(keys[10] ^ ((this.RingBuffer[2] * keys[6]) % keys[2]) ^ (this.RingBuffer[1] & 0xFFFF));
            this.CryptBuffer[3] = (ushort)(keys[11] ^ ((this.RingBuffer[3] * keys[7]) % keys[3]) ^ (this.RingBuffer[2] & 0xFFFF));

            return this.DecodeFinal(inputBuffer, outputBuffer);
        }

        /// <summary>
        /// Decodes the last block which contains the checksum and the block size.
        /// </summary>
        /// <param name="inputBuffer">The input buffer array. Contains the data which should be decrypted.</param>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int DecodeFinal(byte[] inputBuffer, byte[] outputBuffer)
        {
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, inputBuffer, 0x48, 0x10);

            // ShiftBuffer[0] -> block size (decrypted)
            // ShiftBuffer[1] -> checksum
            this.ShiftBuffer[0] ^= this.ShiftBuffer[1];
            this.ShiftBuffer[0] ^= 0x3D; // TODO: Magic number
            Buffer.BlockCopy(this.CryptBuffer, 0, outputBuffer, 0, this.ShiftBuffer[0]);
            byte checksum = 0xF8; // TODO: Magic number
            for (int i = 0; i < this.ShiftBuffer[0]; i++)
            {
                checksum = (byte)(checksum ^ outputBuffer[i]);
            }

            if (this.ShiftBuffer[1] != checksum)
            {
                if (!this.AcceptWrongBlockChecksum)
                {
                    throw new InvalidBlockChecksumException(this.ShiftBuffer[1], checksum);
                }

                if (Log.IsDebugEnabled)
                {
                    var message = $"Block checksum invalid. Expected: {checksum}. Actual: {this.ShiftBuffer[1]}.";
                    Log.Debug(message);
                }
            }

            return this.ShiftBuffer[0];
        }

        private void ShiftBytes(byte[] outputBuffer, int outputOffset, byte[] inputBuffer, int shiftOffset, int length)
        {
            int size = this.GetShiftSize(length, shiftOffset);
            this.shiftArray[1] = 0;
            this.shiftArray[2] = 0;
            this.shiftArray[3] = 0;
            Array.Copy(inputBuffer, shiftOffset / DecryptedBlockSize, this.shiftArray, 0, size);

            var tempShift = (length + shiftOffset) & 0x7;
            if (tempShift != 0)
            {
                this.shiftArray[size - 1] = (byte)(this.shiftArray[size - 1] & 0xFF << (8 - tempShift));
            }

            this.InternalShiftBytes(outputBuffer, outputOffset, this.shiftArray, shiftOffset, size);
        }

        /// <summary>
        /// Returns the maximum packet size of the packet in decrypted state.
        /// (The exact size needs to be decrypted first)
        /// </summary>
        /// <param name="packet">The encrypted packet.</param>
        /// <returns>The maximum packet size of the packet in decrypted state.</returns>
        private int GetMaximumDecryptedSize(byte[] packet)
        {
            return ((this.GetContentSize(packet, false) / EncryptedBlockSize) * DecryptedBlockSize) + this.GetHeaderSize(packet[0]) - 1;
        }

        /// <summary>
        /// Decrypts the data with the XOR 32-byte key.
        /// </summary>
        /// <param name="data">The data which should be decrypted.</param>
        private void DecryptC1(byte[] data)
        {
            var headerSize = this.GetHeaderSize(data[0]);
            for (var i = data.Length - 1; i > headerSize; i--)
            {
                data[i] = (byte)(data[i] ^ data[i - 1] ^ this.xor32Key[i % 32]);
            }
        }
    }
}
