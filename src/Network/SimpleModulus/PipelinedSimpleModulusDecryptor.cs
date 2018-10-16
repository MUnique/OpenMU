// <copyright file="PipelinedSimpleModulusDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using log4net;

    /// <summary>
    /// A pipelined implementation of a <see cref="SimpleModulusDecryptor"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.SimpleModulus.PipelinedSimpleModulusBase" />
    public class PipelinedSimpleModulusDecryptor : PipelinedSimpleModulusBase, IPipelinedDecryptor
    {
        /// <summary>
        /// The default server side decryption key. The corresponding encryption key is <see cref="SimpleModulusEncryptor.DefaultClientKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultServerKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 128079, 164742, 70235, 106898, 31544, 2047, 57011, 10183, 48413, 46165, 15171, 37433 });

        /// <summary>
        /// The default client side decryption key. The corresponding encryption key is <see cref="SimpleModulusEncryptor.DefaultServerKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultClientKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 73326, 109989, 98843, 171058, 18035, 30340, 24701, 11141, 62004, 64409, 35374, 64599 });

        private static readonly ILog Log = LogManager.GetLogger(typeof(PipelinedSimpleModulusDecryptor));
        private readonly SimpleModulusKeys decryptionKeys;
        private readonly byte[] shiftArray = new byte[4];
        private readonly byte[] headerBuffer = new byte[3];
        private readonly byte[] inputBuffer = new byte[EncryptedBlockSize];

        private readonly Pipe pipe = new Pipe();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusDecryptor"/> class with standard keys.
        /// </summary>
        /// <param name="source">The source.</param>
        public PipelinedSimpleModulusDecryptor(PipeReader source)
            : this(source, DefaultServerKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusDecryptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="decryptionKey">The decryption key.</param>
        public PipelinedSimpleModulusDecryptor(PipeReader source, uint[] decryptionKey)
            : this(source, SimpleModulusKeys.CreateDecryptionKeys(decryptionKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusDecryptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="decryptionKeys">The decryption keys.</param>
        public PipelinedSimpleModulusDecryptor(PipeReader source, SimpleModulusKeys decryptionKeys)
        {
            this.Source = source;
            this.decryptionKeys = decryptionKeys;
            this.ReadSource().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public PipeReader Reader => this.pipe.Reader;

        /// <summary>
        /// Gets or sets a value indicating whether this decryptor instance accepts wrong block checksum, or throws an exception in this case.
        /// </summary>
        public bool AcceptWrongBlockChecksum { get; set; }

        /// <inheritdoc />
        protected override void OnComplete()
        {
            this.pipe.Writer.Complete();
        }

        /// <summary>
        /// Reads the mu online packet.
        /// Decrypts the packet and writes it into our pipe.
        /// </summary>
        /// <param name="packet">The mu online packet</param>
        protected override void ReadPacket(ReadOnlySequence<byte> packet)
        {
            // The next line is getting a span from the writer which is at least as big as the packet.
            // As I found out, it's initially about 2 kb in size and gets smaller within further
            // usage. If the previous span was used up, a new piece of memory is getting provided for us.
            packet.Slice(0, 3).CopyTo(this.headerBuffer);

            if (this.headerBuffer[0] < 0xC3)
            {
                // we just have to write-through
                var packetSize = this.headerBuffer.GetPacketSize();
                var data = this.pipe.Writer.GetSpan(packetSize).Slice(0, packetSize);
                packet.CopyTo(data);
                this.pipe.Writer.Advance(packetSize);
                this.pipe.Writer.FlushAsync();
                return;
            }

            var contentSize = this.GetContentSize(this.headerBuffer, false);
            if ((contentSize % EncryptedBlockSize) != 0)
            {
                throw new ArgumentException($"The packet has an unexpected content size. It must be a multiple of ${EncryptedBlockSize}", nameof(packet));
            }

            var maximumDecryptedSize = this.GetMaximumDecryptedSize(this.headerBuffer);
            var encryptedHeaderSize = this.headerBuffer.GetPacketHeaderSize();
            var decryptedHeaderSize = encryptedHeaderSize - 1;
            var span = this.pipe.Writer.GetSpan(maximumDecryptedSize);

            // we just want to work on a span with the exact size of the packet.
            var decrypted = span.Slice(0, maximumDecryptedSize);
            var decryptedContentSize = this.DecryptPacketContent(packet.Slice(encryptedHeaderSize), decrypted.Slice(decryptedHeaderSize));
            decrypted[0] = this.headerBuffer[0];
            decrypted = decrypted.Slice(0, decryptedContentSize + decryptedHeaderSize);
            decrypted.SetPacketSize();

            this.pipe.Writer.Advance(decrypted.Length);
            this.pipe.Writer.FlushAsync();
        }

        private int DecryptPacketContent(ReadOnlySequence<byte> input, Span<byte> output)
        {
            int sizeCounter = 0;
            var rest = input;
            do
            {
                rest.Slice(0, EncryptedBlockSize).CopyTo(this.inputBuffer);
                var outputBlock = output.Slice(sizeCounter, DecryptedBlockSize);
                var blockSize = this.BlockDecode(outputBlock);
                if (blockSize != -1)
                {
                    sizeCounter += blockSize;
                }

                rest = rest.Slice(EncryptedBlockSize);
            }
            while (rest.Length > 0);

            return sizeCounter;
        }

        /// <summary>
        /// Decodes the block.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int BlockDecode(Span<byte> outputBuffer)
        {
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, this.inputBuffer, 0x00, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, this.inputBuffer, 0x10, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 0, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, this.inputBuffer, 0x12, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, this.inputBuffer, 0x22, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 4, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, this.inputBuffer, 0x24, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, this.inputBuffer, 0x34, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 8, 4);
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, this.inputBuffer, 0x36, 0x10);
            this.ShiftBytes(this.ShiftBuffer, 0x16, this.inputBuffer, 0x46, 0x02);
            Buffer.BlockCopy(this.ShiftBuffer, 0, this.RingBuffer, 12, 4);
            var keys = this.decryptionKeys;
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys.XorKey[2] ^ (this.RingBuffer[3] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys.XorKey[1] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys.XorKey[0] ^ (this.RingBuffer[1] & 0xFFFF);

            this.CryptBuffer[0] = (ushort)(keys.XorKey[0] ^ ((this.RingBuffer[0] * keys.DecryptKey[0]) % keys.ModulusKey[0]));
            this.CryptBuffer[1] = (ushort)(keys.XorKey[1] ^ ((this.RingBuffer[1] * keys.DecryptKey[1]) % keys.ModulusKey[1]) ^ (this.RingBuffer[0] & 0xFFFF));
            this.CryptBuffer[2] = (ushort)(keys.XorKey[2] ^ ((this.RingBuffer[2] * keys.DecryptKey[2]) % keys.ModulusKey[2]) ^ (this.RingBuffer[1] & 0xFFFF));
            this.CryptBuffer[3] = (ushort)(keys.XorKey[3] ^ ((this.RingBuffer[3] * keys.DecryptKey[3]) % keys.ModulusKey[3]) ^ (this.RingBuffer[2] & 0xFFFF));

            return this.DecodeFinal(outputBuffer);
        }

        /// <summary>
        /// Decodes the last block which contains the checksum and the block size.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int DecodeFinal(Span<byte> outputBuffer)
        {
            this.ClearShiftBuffer();
            this.ShiftBytes(this.ShiftBuffer, 0x00, this.inputBuffer, 0x48, 0x10);

            // ShiftBuffer[0] -> block size (decrypted)
            // ShiftBuffer[1] -> checksum
            byte blockSize = (byte)(this.ShiftBuffer[0] ^ this.ShiftBuffer[1] ^ BlockSizeXorKey);
            MemoryMarshal.Cast<ushort, byte>(this.CryptBuffer).CopyTo(outputBuffer);
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

        private void ShiftBytes(Span<byte> outputBuffer, int outputOffset, Span<byte> inputBuffer, int shiftOffset, int length)
        {
            int size = this.GetShiftSize(length, shiftOffset);
            this.shiftArray[1] = 0;
            this.shiftArray[2] = 0;
            this.shiftArray[3] = 0;
            inputBuffer.Slice(shiftOffset / DecryptedBlockSize, size).CopyTo(this.shiftArray);

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
        private int GetMaximumDecryptedSize(Span<byte> packet)
        {
            return ((this.GetContentSize(packet, false) / EncryptedBlockSize) * DecryptedBlockSize) + packet.GetPacketHeaderSize() - 1;
        }
    }
}
