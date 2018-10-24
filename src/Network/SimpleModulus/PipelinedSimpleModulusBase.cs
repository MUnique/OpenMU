// <copyright file="PipelinedSimpleModulusBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The base class for the "simple modulus" encryption.
    /// </summary>
    public abstract class PipelinedSimpleModulusBase : PacketPipeReaderBase
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

        private const int BitsPerByte = 8;

        private const int BitsPerValue = (BitsPerByte * 2) + 2;

        /// <summary>
        /// Gets the counter.
        /// </summary>
        protected Counter Counter { get; } = new Counter();

        /// <summary>
        /// Gets the ring buffer.
        /// </summary>
        protected uint[] EncryptionResult { get; } = new uint[4];

        /// <summary>
        /// Gets the header buffer of the currently read packet.
        /// </summary>
        protected byte[] HeaderBuffer { get; } = new byte[3];

        /// <summary>
        /// Gets the pipe which is either the target (for the encryptor) or source (for the decryptor) for or of the encrypted packets.
        /// </summary>
        protected Pipe Pipe { get; } = new Pipe();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Counter.Reset();
        }

        /// <summary>
        /// Gets the byte offset in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.
        /// </summary>
        /// <param name="resultIndex">Index of the <see cref="EncryptionResult"/>.</param>
        /// <returns>The byte offset in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int GetByteOffset(int resultIndex) => GetBitIndex(resultIndex) / BitsPerByte;

        /// <summary>
        /// Gets the bit offset in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.
        /// </summary>
        /// <param name="resultIndex">Index of the <see cref="EncryptionResult"/>.</param>
        /// <returns>The bit offset in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int GetBitOffset(int resultIndex) => GetBitIndex(resultIndex) % BitsPerByte;

        /// <summary>
        /// Gets the bit mask of the first byte (at the index of <see cref="GetByteOffset(int)"/>) in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.
        /// </summary>
        /// <param name="resultIndex">Index of the <see cref="EncryptionResult"/>.</param>
        /// <returns>The the bit mask of the first byte (at the index of <see cref="GetByteOffset(int)"/>) in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int GetFirstBitMask(int resultIndex) => 0xFF >> GetBitOffset(resultIndex);

        /// <summary>
        /// Gets the bit mask of the last byte (at the index of <see cref="GetByteOffset(int)"/>) in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>,
        /// just for the last remainder of the encryption result value. The remainder is basically the first 2 bits, e.g. when the value is <c>0x2FFFF</c>, the remainder is the 2 in front.
        /// </summary>
        /// <param name="resultIndex">Index of the <see cref="EncryptionResult"/>.</param>
        /// <returns>The the bit mask of the first byte (at the index of <see cref="GetByteOffset(int)"/>) in the encrypted block buffer based on the index of <see cref="EncryptionResult"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int GetRemainderBitMask(int resultIndex) => (0xFF << (6 - GetBitOffset(resultIndex)) & 0xFF) - ((0xFF << (8 - GetBitOffset(resultIndex))) & 0xFF);

        /// <summary>
        /// Copies the data of the packet into the target writer, without flushing it yet.
        /// </summary>
        /// <param name="target">The target writer.</param>
        /// <param name="packet">The packet.</param>
        protected void CopyDataIntoWriter(PipeWriter target, ReadOnlySequence<byte> packet)
        {
            var packetSize = this.HeaderBuffer.GetPacketSize();
            var data = target.GetSpan(packetSize).Slice(0, packetSize);
            packet.CopyTo(data);
            target.Advance(packetSize);
        }

        /// <summary>
        /// Gets the size of the content.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="decrypted">if set to <c>true</c> it is decrypted. Encrypted packets additionally contain a counter.</param>
        /// <returns>The size of the actual content.</returns>
        protected int GetContentSize(Span<byte> packet, bool decrypted)
        {
            return packet.GetPacketSize() - packet.GetPacketHeaderSize() + (decrypted ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBitIndex(int resultIndex) => resultIndex * BitsPerValue;
    }
}
