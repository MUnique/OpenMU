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
    using Interfaces;
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
                var blockSize = this.DecryptBlock(outputBlock);
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
        /// Decrypts the block.
        /// </summary>
        /// <param name="outputBuffer">The output buffer array.</param>
        /// <returns>The decrypted length of the block.</returns>
        private int DecryptBlock(Span<byte> outputBuffer)
        {
            this.EncryptionResult[0] = this.ReadInputBuffer(0);
            this.EncryptionResult[1] = this.ReadInputBuffer(1);
            this.EncryptionResult[2] = this.ReadInputBuffer(2);
            this.EncryptionResult[3] = this.ReadInputBuffer(3);

            this.DecryptContent(outputBuffer);

            return this.DecodeFinal(outputBuffer);
        }

        private void DecryptContent(Span<byte> outputBuffer)
        {
            var keys = this.decryptionKeys;
            this.EncryptionResult[2] = this.EncryptionResult[2] ^ keys.XorKey[2] ^ (this.EncryptionResult[3] & 0xFFFF);
            this.EncryptionResult[1] = this.EncryptionResult[1] ^ keys.XorKey[1] ^ (this.EncryptionResult[2] & 0xFFFF);
            this.EncryptionResult[0] = this.EncryptionResult[0] ^ keys.XorKey[0] ^ (this.EncryptionResult[1] & 0xFFFF);

            var output = MemoryMarshal.Cast<byte, ushort>(outputBuffer);
            output[0] = (ushort)(keys.XorKey[0] ^ ((this.EncryptionResult[0] * keys.DecryptKey[0]) % keys.ModulusKey[0]));
            output[1] = (ushort)(keys.XorKey[1] ^ ((this.EncryptionResult[1] * keys.DecryptKey[1]) % keys.ModulusKey[1]) ^ (this.EncryptionResult[0] & 0xFFFF));
            output[2] = (ushort)(keys.XorKey[2] ^ ((this.EncryptionResult[2] * keys.DecryptKey[2]) % keys.ModulusKey[2]) ^ (this.EncryptionResult[1] & 0xFFFF));
            output[3] = (ushort)(keys.XorKey[3] ^ ((this.EncryptionResult[3] * keys.DecryptKey[3]) % keys.ModulusKey[3]) ^ (this.EncryptionResult[2] & 0xFFFF));
        }

        private uint ReadInputBuffer(int resultIndex)
        {
            var byteOffset = GetByteOffset(resultIndex);
            var bitOffset = GetBitOffset(resultIndex);
            var firstMask = GetFirstBitMask(resultIndex);
            uint result = 0;
            result += (uint)((this.inputBuffer[byteOffset++] & firstMask) << (24 + bitOffset));
            result += (uint)(this.inputBuffer[byteOffset++] << (16 + bitOffset));
            result += (uint)((this.inputBuffer[byteOffset] & (0xFF << (8 - bitOffset))) << (8 + bitOffset));

            result = result.SwapBytes();
            var remainderMask = GetRemainderBitMask(resultIndex);
            var remainder = (byte)(this.inputBuffer[byteOffset] & remainderMask);
            result += (uint)(remainder << 16) >> (6 - bitOffset);

            return result;
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
