// <copyright file="PipelinedSimpleModulusEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// The standard encryptor (server-side) which encrypts 0xC3 and 0xC4-packets with the "simple modulus" algorithm.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Network.SimpleModulus.PipelinedSimpleModulusBase" />
    public class PipelinedSimpleModulusEncryptor : PipelinedSimpleModulusBase, IPipelinedEncryptor
    {
        /// <summary>
        /// The default server side encryption key. The corresponding encryption key is <see cref="PipelinedSimpleModulusDecryptor.DefaultClientKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultServerKey = SimpleModulusKeys.CreateEncryptionKeys(new uint[] { 73326, 109989, 98843, 171058, 13169, 19036, 35482, 29587, 62004, 64409, 35374, 64599 });

        /// <summary>
        /// The default client side decryption key. The corresponding encryption key is <see cref="PipelinedSimpleModulusDecryptor.DefaultServerKey"/>.
        /// </summary>
        public static readonly SimpleModulusKeys DefaultClientKey = SimpleModulusKeys.CreateEncryptionKeys(new uint[] { 128079, 164742, 70235, 106898, 23489, 11911, 19816, 13647, 48413, 46165, 15171, 37433 });

        private readonly PipeWriter target;
        private readonly byte[] inputBuffer = new byte[DecryptedBlockSize];
        private readonly SimpleModulusKeys encryptionKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        public PipelinedSimpleModulusEncryptor(PipeWriter target)
            : this(target, DefaultServerKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusEncryptor"/> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        /// <param name="encryptionKeys">The encryption keys.</param>
        public PipelinedSimpleModulusEncryptor(PipeWriter target, uint[] encryptionKeys)
            : this(target, SimpleModulusKeys.CreateEncryptionKeys(encryptionKeys))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelinedSimpleModulusEncryptor" /> class.
        /// </summary>
        /// <param name="target">The target pipe writer.</param>
        /// <param name="encryptionKeys">The encryption keys.</param>
        public PipelinedSimpleModulusEncryptor(PipeWriter target, SimpleModulusKeys encryptionKeys)
        {
            this.target = target;
            this.encryptionKeys = encryptionKeys;
            this.Source = this.Pipe.Reader;
            this.ReadSource().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public PipeWriter Writer => this.Pipe.Writer;

        /// <inheritdoc />
        protected override void OnComplete(Exception exception)
        {
            this.target.Complete(exception);
        }

        /// <inheritdoc />
        protected override async Task ReadPacket(ReadOnlySequence<byte> packet)
        {
            packet.Slice(0, this.HeaderBuffer.Length).CopyTo(this.HeaderBuffer);

            if (this.HeaderBuffer[0] < 0xC3)
            {
                // we just have to write-through
                this.CopyDataIntoWriter(this.target, packet);
                await this.target.FlushAsync().ConfigureAwait(false);
                return;
            }

            this.EncryptAndWrite(packet);
            await this.target.FlushAsync();
        }

        /// <summary>
        /// Writes the encryption result into the target span.
        /// It basically squeezes the result (4 bytes) into 2 bytes and 2 bits (=18 bits).
        /// </summary>
        /// <param name="target">The target span.</param>
        /// <param name="resultIndex">Index of the result.</param>
        /// <param name="result">The encryption result value.</param>
        private static void WriteResultToTarget(Span<byte> target, int resultIndex, uint result)
        {
            var byteOffset = GetByteOffset(resultIndex);
            var bitOffset = GetBitOffset(resultIndex);
            var firstMask = GetFirstBitMask(resultIndex);
            var swapped = result.SwapBytes();
            target[byteOffset++] |= (byte)(swapped >> (24 + bitOffset) & firstMask);
            target[byteOffset++] = (byte)(swapped >> (16 + bitOffset));
            target[byteOffset] = (byte)((swapped >> (8 + bitOffset)) & (0xFF << (8 - bitOffset)));
            var remainderMask = GetRemainderBitMask(resultIndex);
            var remainder = (result >> 16) << (6 - bitOffset);
            target[byteOffset] |= (byte)(remainder & remainderMask);
        }

        private void EncryptAndWrite(ReadOnlySequence<byte> packet)
        {
            var encryptedSize = this.GetEncryptedSize(this.HeaderBuffer);
            var result = this.target.GetSpan(encryptedSize).Slice(0, encryptedSize);

            // setting up the header (packet type and size) in the result:
            result[0] = this.HeaderBuffer[0];
            result.SetPacketSize();

            // encrypting the content:
            var headerSize = this.HeaderBuffer.GetPacketHeaderSize();
            var input = packet.Slice(headerSize);
            this.EncryptPacketContent(input, result.Slice(headerSize));

            this.target.Advance(result.Length);
        }

        private int GetEncryptedSize(Span<byte> data)
        {
            var contentSize = this.GetContentSize(data, true);
            return (((contentSize / DecryptedBlockSize) + (((contentSize % DecryptedBlockSize) > 0) ? 1 : 0)) * EncryptedBlockSize) + data.GetPacketHeaderSize();
        }

        private void EncryptPacketContent(ReadOnlySequence<byte> input, Span<byte> result)
        {
            var i = 0;
            var sizeCounter = 0;
            var size = (int)input.Length + 1; // plus one for the counter

            // we process the first input block out of the loop, because we need to add the counter as prefix
            this.inputBuffer[0] = (byte)this.Counter.Count;
            if (size >= DecryptedBlockSize)
            {
                input.Slice(0, DecryptedBlockSize - 1).CopyTo(this.inputBuffer.AsSpan(1));
            }
            else
            {
                input.Slice(0, input.Length).CopyTo(this.inputBuffer.AsSpan(1));
                this.inputBuffer.AsSpan(size).Clear();
            }

            var firstResultBlock = result.Slice(sizeCounter, EncryptedBlockSize);
            var contentOfFirstBlockLength = Math.Min(DecryptedBlockSize, size);
            this.EncryptBlock(firstResultBlock, contentOfFirstBlockLength);
            i += DecryptedBlockSize;
            sizeCounter += EncryptedBlockSize;

            // encrypt the rest of the blocks.
            while (i < size)
            {
                var contentOfBlockLength = Math.Min(DecryptedBlockSize, size - i);
                input.Slice(i - 1, contentOfBlockLength).CopyTo(this.inputBuffer);
                this.inputBuffer.AsSpan(contentOfBlockLength).Clear();
                var resultBlock = result.Slice(sizeCounter, EncryptedBlockSize);
                this.EncryptBlock(resultBlock, contentOfBlockLength);
                i += DecryptedBlockSize;
                sizeCounter += EncryptedBlockSize;
            }

            this.Counter.Increase();
        }

        private void EncryptBlock(Span<byte> outputBuffer, int blockSize)
        {
            outputBuffer.Clear(); // since the memory comes from the shared memory pool, it might not be initialized yet
            this.EncryptContent();
            WriteResultToTarget(outputBuffer, 0, this.EncryptionResult[0]);
            WriteResultToTarget(outputBuffer, 1, this.EncryptionResult[1]);
            WriteResultToTarget(outputBuffer, 2, this.EncryptionResult[2]);
            WriteResultToTarget(outputBuffer, 3, this.EncryptionResult[3]);
            this.EncryptFinalBlockByte(blockSize, outputBuffer);
        }

        /// <summary>
        /// Encodes the final part of the block. It contains a checksum and the length of the block, which is needed for decryption.
        /// </summary>
        /// <param name="blockSize">The size of the block of decrypted data in bytes.</param>
        /// <param name="outputBuffer">The output buffer to which the encrypted result will be written.</param>
        private void EncryptFinalBlockByte(int blockSize, Span<byte> outputBuffer)
        {
            byte size = (byte)(blockSize ^ BlockSizeXorKey);
            byte checksum = BlockCheckSumXorKey;
            for (var i = 0; i < blockSize; i++)
            {
                checksum ^= this.inputBuffer[i];
            }

            size ^= checksum;
            outputBuffer[outputBuffer.Length - 2] = size;
            outputBuffer[outputBuffer.Length - 1] = checksum;
        }

        private void EncryptContent()
        {
            var keys = this.encryptionKeys;
            var input = MemoryMarshal.Cast<byte, ushort>(this.inputBuffer);

            this.EncryptionResult[0] = ((keys.XorKey[0] ^ input[0]) * keys.EncryptKey[0]) % keys.ModulusKey[0];
            this.EncryptionResult[1] = ((keys.XorKey[1] ^ (input[1] ^ (this.EncryptionResult[0] & 0xFFFF))) * keys.EncryptKey[1]) % keys.ModulusKey[1];
            this.EncryptionResult[2] = ((keys.XorKey[2] ^ (input[2] ^ (this.EncryptionResult[1] & 0xFFFF))) * keys.EncryptKey[2]) % keys.ModulusKey[2];
            this.EncryptionResult[3] = ((keys.XorKey[3] ^ (input[3] ^ (this.EncryptionResult[2] & 0xFFFF))) * keys.EncryptKey[3]) % keys.ModulusKey[3];

            this.EncryptionResult[0] = this.EncryptionResult[0] ^ keys.XorKey[0] ^ (this.EncryptionResult[1] & 0xFFFF);
            this.EncryptionResult[1] = this.EncryptionResult[1] ^ keys.XorKey[1] ^ (this.EncryptionResult[2] & 0xFFFF);
            this.EncryptionResult[2] = this.EncryptionResult[2] ^ keys.XorKey[2] ^ (this.EncryptionResult[3] & 0xFFFF);
        }
    }
}
