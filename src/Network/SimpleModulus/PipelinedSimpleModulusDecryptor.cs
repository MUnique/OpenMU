// <copyright file="PipelinedSimpleModulusDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using log4net;

    /// <summary>
    /// A decryptor which decrypts 0xC3 and 0xC4-packets with the "simple modulus" algorithm.
    /// </summary>
    public class PipelinedSimpleModulusDecryptor : PipelinedSimpleModulusBase, IPipelinedDecryptor
    {
        /// <summary>
        /// The default server side decryption key. The corresponding encryption key is <see cref="PipelinedSimpleModulusEncryptor.DefaultClientKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultServerKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 128079, 164742, 70235, 106898, 31544, 2047, 57011, 10183, 48413, 46165, 15171, 37433 });

        /// <summary>
        /// The default client side decryption key. The corresponding encryption key is <see cref="PipelinedSimpleModulusEncryptor.DefaultServerKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultClientKey = SimpleModulusKeys.CreateDecryptionKeys(new uint[] { 73326, 109989, 98843, 171058, 18035, 30340, 24701, 11141, 62004, 64409, 35374, 64599 });

        private static readonly ILog Log = LogManager.GetLogger(typeof(PipelinedSimpleModulusDecryptor));
        private readonly SimpleModulusKeys decryptionKeys;
        private readonly byte[] inputBuffer = new byte[EncryptedBlockSize];

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
        public PipeReader Reader => this.Pipe.Reader;

        /// <summary>
        /// Gets or sets a value indicating whether this decryptor instance accepts wrong block checksum, or throws an exception in this case.
        /// </summary>
        public bool AcceptWrongBlockChecksum { get; set; }

        /// <inheritdoc />
        protected override void OnComplete(Exception exception)
        {
            this.Pipe.Writer.Complete(exception);
        }

        /// <summary>
        /// Reads the mu online packet.
        /// Decrypts the packet and writes it into our pipe.
        /// </summary>
        /// <param name="packet">The mu online packet</param>
        /// <returns>The async task.</returns>
        protected override async Task ReadPacket(ReadOnlySequence<byte> packet)
        {
            // The next line is getting a span from the writer which is at least as big as the packet.
            // As I found out, it's initially about 2 kb in size and gets smaller within further
            // usage. If the previous span was used up, a new piece of memory is getting provided for us.
            packet.Slice(0, 3).CopyTo(this.HeaderBuffer);

            if (this.HeaderBuffer[0] < 0xC3)
            {
                // we just have to write-through
                this.CopyDataIntoWriter(this.Pipe.Writer, packet);
                await this.Pipe.Writer.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                var contentSize = this.GetContentSize(this.HeaderBuffer, false);
                if ((contentSize % EncryptedBlockSize) != 0)
                {
                    throw new ArgumentException(
                        $"The packet has an unexpected content size. It must be a multiple of ${EncryptedBlockSize}",
                        nameof(packet));
                }

                this.DecryptAndWrite(packet);
                await this.Pipe.Writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private void DecryptAndWrite(ReadOnlySequence<byte> packet)
        {
            var maximumDecryptedSize = this.GetMaximumDecryptedSize(this.HeaderBuffer);
            var encryptedHeaderSize = this.HeaderBuffer.GetPacketHeaderSize();
            var decryptedHeaderSize = encryptedHeaderSize - 1;
            var span = this.Pipe.Writer.GetSpan(maximumDecryptedSize);

            // we just want to work on a span with the exact size of the packet.
            var decrypted = span.Slice(0, maximumDecryptedSize);
            var decryptedContentSize = this.DecryptPacketContent(packet.Slice(encryptedHeaderSize), decrypted.Slice(decryptedHeaderSize));
            decrypted[0] = this.HeaderBuffer[0];
            decrypted = decrypted.Slice(0, decryptedContentSize + decryptedHeaderSize);
            decrypted.SetPacketSize();

            this.Pipe.Writer.Advance(decrypted.Length);
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
                if (sizeCounter == 0 && outputBlock[0] != this.Counter.Count)
                {
                    throw new InvalidPacketCounterException(outputBlock[0], (byte)this.Counter.Count);
                }

                if (blockSize != -1)
                {
                    sizeCounter += blockSize;
                }

                rest = rest.Slice(EncryptedBlockSize);
            }
            while (rest.Length > 0);

            this.Counter.Increase();
            return sizeCounter;
        }

        /// <summary>
        /// Decodes the block.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int BlockDecode(Span<byte> outputBuffer)
        {
            this.RingBuffer.AsSpan().Clear();
            var shiftResult = MemoryMarshal.Cast<uint, byte>(this.RingBuffer);
            this.ShiftBytes(shiftResult.Slice(0, 4), 0x00, 0x00, 0x10);
            this.ShiftBytes(shiftResult.Slice(0, 4), 0x16, 0x10, 0x02);
            this.ShiftBytes(shiftResult.Slice(4, 4), 0x00, 0x12, 0x10);
            this.ShiftBytes(shiftResult.Slice(4, 4), 0x16, 0x22, 0x02);
            this.ShiftBytes(shiftResult.Slice(8, 4), 0x00, 0x24, 0x10);
            this.ShiftBytes(shiftResult.Slice(8, 4), 0x16, 0x34, 0x02);
            this.ShiftBytes(shiftResult.Slice(12, 4), 0x00, 0x36, 0x10);
            this.ShiftBytes(shiftResult.Slice(12, 4), 0x16, 0x46, 0x02);

            var keys = this.decryptionKeys;
            this.RingBuffer[2] = this.RingBuffer[2] ^ keys.XorKey[2] ^ (this.RingBuffer[3] & 0xFFFF);
            this.RingBuffer[1] = this.RingBuffer[1] ^ keys.XorKey[1] ^ (this.RingBuffer[2] & 0xFFFF);
            this.RingBuffer[0] = this.RingBuffer[0] ^ keys.XorKey[0] ^ (this.RingBuffer[1] & 0xFFFF);

            var output = MemoryMarshal.Cast<byte, ushort>(outputBuffer);
            output[0] = (ushort)(keys.XorKey[0] ^ ((this.RingBuffer[0] * keys.DecryptKey[0]) % keys.ModulusKey[0]));
            output[1] = (ushort)(keys.XorKey[1] ^ ((this.RingBuffer[1] * keys.DecryptKey[1]) % keys.ModulusKey[1]) ^ (this.RingBuffer[0] & 0xFFFF));
            output[2] = (ushort)(keys.XorKey[2] ^ ((this.RingBuffer[2] * keys.DecryptKey[2]) % keys.ModulusKey[2]) ^ (this.RingBuffer[1] & 0xFFFF));
            output[3] = (ushort)(keys.XorKey[3] ^ ((this.RingBuffer[3] * keys.DecryptKey[3]) % keys.ModulusKey[3]) ^ (this.RingBuffer[2] & 0xFFFF));

            return this.DecodeFinal(outputBuffer);
        }

        /// <summary>
        /// Decodes the last block which contains the checksum and the block size.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int DecodeFinal(Span<byte> outputBuffer)
        {
            var blockSuffix = this.inputBuffer.AsSpan(EncryptedBlockSize - 2, 2);
            // blockSuffix[0] -> block size (encrypted)
            // blockSuffix[1] -> checksum

            byte blockSize = (byte)(blockSuffix[0] ^ blockSuffix[1] ^ BlockSizeXorKey);
            byte checksum = BlockCheckSumXorKey;
            for (int i = 0; i < DecryptedBlockSize; i++)
            {
                checksum ^= outputBuffer[i];
            }

            if (blockSuffix[1] != checksum)
            {
                if (!this.AcceptWrongBlockChecksum)
                {
                    throw new InvalidBlockChecksumException(blockSuffix[1], checksum);
                }

                if (Log.IsDebugEnabled)
                {
                    var message = $"Block checksum invalid. Expected: {checksum}. Actual: {blockSuffix[1]}.";
                    Log.Debug(message);
                }
            }

            return blockSize;
        }

        private void ShiftBytes(Span<byte> outputBuffer, int outputOffset, int shiftOffset, int length)
        {
            int size = this.GetShiftSize(length, shiftOffset);
            Span<byte> shiftBuffer = stackalloc byte[4];
            this.inputBuffer.AsSpan(shiftOffset / DecryptedBlockSize, size).CopyTo(shiftBuffer);

            var tempShift = (length + shiftOffset) & 0x7;
            if (tempShift != 0)
            {
                shiftBuffer[size - 1] = (byte)(shiftBuffer[size - 1] & 0xFF << (8 - tempShift));
            }

            this.InternalShiftBytes(outputBuffer, outputOffset, shiftBuffer, shiftOffset, size);
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
