// <copyright file="SimpleModulusDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using log4net;

    /// <summary>
    /// The standard decryptor (server-side) which decrypts with the "simple modulus"
    /// algorithm first, and then with the 32 byte XOR-key.
    /// </summary>
    public class SimpleModulusDecryptor : SimpleModulusBase, IDecryptor
    {
        /// <summary>
        /// The default server side decryption key. The corrsponding encryption key is <see cref="SimpleModulusEncryptor.DefaultClientKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultServerKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 128079, 164742, 70235, 106898, 31544, 2047, 57011, 10183, 48413, 46165, 15171, 37433 });

        /// <summary>
        /// The default client side decryption key. The corrsponding encryption key is <see cref="SimpleModulusEncryptor.DefaultServerKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultClientKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 73326, 109989, 98843, 171058, 18035, 30340, 24701, 11141, 62004, 64409, 35374, 64599 });

        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleModulusDecryptor));
        private readonly SimpleModulusKeys decryptionKeys;
        private readonly byte[] shiftArray = new byte[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusDecryptor"/> class with standard keys.
        /// </summary>
        public SimpleModulusDecryptor()
            : this(DefaultServerKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusDecryptor"/> class.
        /// </summary>
        /// <param name="decryptionKey">The decryption key.</param>
        public SimpleModulusDecryptor(uint[] decryptionKey)
            : this(SimpleModulusKeys.CreateDecryptionKeys(decryptionKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModulusDecryptor"/> class.
        /// </summary>
        /// <param name="decryptionKeys">The decryption keys.</param>
        public SimpleModulusDecryptor(SimpleModulusKeys decryptionKeys)
        {
            this.decryptionKeys = decryptionKeys;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this decryptor instance accepts wrong block checksum, or throws an exception in this case.
        /// </summary>
        public bool AcceptWrongBlockChecksum { get; set; }

        /// <inheritdoc/>
        public bool Decrypt(ref byte[] packet)
        {
            if (packet[0] < 0xC3)
            {
                return true;
            }

            bool result;
            lock (this.Counter)
            {
                result = this.Counter.Count == this.DecryptC3(ref packet);
                this.Counter.Increase();
            }

            return result;
        }

        private byte DecryptC3(ref byte[] data)
        {
            var contentSize = this.GetContentSize(data, false);
            var headerSize = data.GetPacketHeaderSize();
            var result = new byte[this.GetMaximumDecryptedSize(data)];
            var decryptedSize = this.DecodeBuffer(data, headerSize, contentSize, result);
            decryptedSize += headerSize - 1;
            var decryptedCount = result[headerSize - 1];
            result[0] = data[0];
            Array.Resize(ref result, decryptedSize);
            result.SetPacketSize();
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
            var keys = this.decryptionKeys;
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys.XorKey[2] ^ (this.RingBuffer[3] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys.XorKey[1] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys.XorKey[0] ^ (this.RingBuffer[1] & 0xFFFF);

            this.CryptBuffer[0] = (ushort)(keys.XorKey[0] ^ ((this.RingBuffer[0] * keys.DecryptKey[0]) % keys.ModulusKey[0]));
            this.CryptBuffer[1] = (ushort)(keys.XorKey[1] ^ ((this.RingBuffer[1] * keys.DecryptKey[1]) % keys.ModulusKey[1]) ^ (this.RingBuffer[0] & 0xFFFF));
            this.CryptBuffer[2] = (ushort)(keys.XorKey[2] ^ ((this.RingBuffer[2] * keys.DecryptKey[2]) % keys.ModulusKey[2]) ^ (this.RingBuffer[1] & 0xFFFF));
            this.CryptBuffer[3] = (ushort)(keys.XorKey[3] ^ ((this.RingBuffer[3] * keys.DecryptKey[3]) % keys.ModulusKey[3]) ^ (this.RingBuffer[2] & 0xFFFF));

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
            byte blockSize = (byte)(this.ShiftBuffer[0] ^ this.ShiftBuffer[1] ^ BlockSizeXorKey);
            Buffer.BlockCopy(this.CryptBuffer, 0, outputBuffer, 0, blockSize);
            byte checksum = BlockCheckSumXorKey;
            for (int i = 0; i < blockSize; i++)
            {
                checksum ^= outputBuffer[i];
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

            return blockSize;
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
            return ((this.GetContentSize(packet, false) / EncryptedBlockSize) * DecryptedBlockSize) + packet.GetPacketHeaderSize() - 1;
        }
    }
}
