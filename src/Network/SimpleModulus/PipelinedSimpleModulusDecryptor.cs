// <copyright file="PipelinedSimpleModulusDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus;

using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

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

    private readonly SimpleModulusKeys _decryptionKeys;
    private readonly byte[] _inputBuffer;

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
        : base(decryptionKeys.DecryptKey.Length == 4 ? Variant.New : Variant.Old)
    {
        this.Source = source;
        this._decryptionKeys = decryptionKeys;
        this._inputBuffer = new byte[this.EncryptedBlockSize];
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public PipeReader Reader => this.Pipe.Reader;

    /// <summary>
    /// Gets or sets a value indicating whether this decryptor instance accepts wrong block checksum, or throws an exception in this case.
    /// </summary>
    public bool AcceptWrongBlockChecksum { get; set; }

    /// <inheritdoc />
    protected override ValueTask OnCompleteAsync(Exception? exception)
    {
        return this.Pipe.Writer.CompleteAsync(exception);
    }

    /// <summary>
    /// Reads the mu online packet.
    /// Decrypts the packet and writes it into our pipe.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns><see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.</returns>
    protected override async ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        // The next line is getting a span from the writer which is at least as big as the packet.
        // As I found out, it's initially about 2 kb in size and gets smaller within further
        // usage. If the previous span was used up, a new piece of memory is getting provided for us.
        packet.Slice(0, 3).CopyTo(this.HeaderBuffer);

        if (this.HeaderBuffer[0] < 0xC3)
        {
            // we just have to write-through
            this.CopyDataIntoWriter(this.Pipe.Writer, packet);
            return await this.TryFlushWriterAsync(this.Pipe.Writer).ConfigureAwait(false);
        }
        else
        {
            var contentSize = this.GetContentSize(this.HeaderBuffer, false);
            if ((contentSize % this.EncryptedBlockSize) != 0)
            {
                throw new ArgumentException(
                    $"The packet has an unexpected content size. It must be a multiple of {this.EncryptedBlockSize}",
                    nameof(packet));
            }

            this.DecryptAndWrite(packet);
            return await this.TryFlushWriterAsync(this.Pipe.Writer).ConfigureAwait(false);
        }
    }

    private void DecryptAndWrite(ReadOnlySequence<byte> packet)
    {
        var maximumDecryptedSize = this.GetMaximumDecryptedSize(this.HeaderBuffer);
        var headerSize = this.HeaderBuffer.GetPacketHeaderSize();
        var counterSize = this.Counter is null ? 0 : 1;

        var span = this.Pipe.Writer.GetSpan(maximumDecryptedSize);

        // we just want to work on a span with the exact size of the packet.
        var decrypted = span.Slice(0, maximumDecryptedSize);
        var decryptedContentSize = this.DecryptPacketContent(packet.Slice(headerSize), decrypted.Slice(headerSize - counterSize)); // if we have a counter, we trick a bit by passing in a bigger span
        decrypted[0] = this.HeaderBuffer[0];
        decrypted = decrypted.Slice(0, decryptedContentSize + headerSize - counterSize);
        decrypted.SetPacketSize();

        this.Pipe.Writer.Advance(decrypted.Length);
    }

    private int DecryptPacketContent(ReadOnlySequence<byte> input, Span<byte> output)
    {
        int sizeCounter = 0;
        var rest = input;
        do
        {
            rest.Slice(0, this.EncryptedBlockSize).CopyTo(this._inputBuffer);
            var outputBlock = output.Slice(sizeCounter, this.DecryptedBlockSize);
            var blockSize = this.DecryptBlock(outputBlock);
            if (this.Counter != null && sizeCounter == 0 && outputBlock[0] != this.Counter.Count)
            {
                throw new InvalidPacketCounterException(outputBlock[0], (byte)this.Counter.Count);
            }

            if (blockSize != -1)
            {
                sizeCounter += blockSize;
            }

            rest = rest.Slice(this.EncryptedBlockSize);
        }
        while (rest.Length > 0);

        this.Counter?.Increase();
        return sizeCounter;
    }

    /// <summary>
    /// Decrypts the block.
    /// </summary>
    /// <param name="outputBuffer">The output buffer array.</param>
    /// <returns>The decrypted length of the block.</returns>
    private int DecryptBlock(Span<byte> outputBuffer)
    {
        for (int i = 0; i < this.EncryptionResult.Length; i++)
        {
            this.EncryptionResult[i] = this.ReadInputBuffer(i);
        }

        this.DecryptContent(outputBuffer);

        return this.DecodeFinal(outputBuffer);
    }

    private void DecryptContent(Span<byte> outputBuffer)
    {
        var keys = this._decryptionKeys;
        for (int i = this.EncryptionResult.Length - 1; i > 0; i--)
        {
            this.EncryptionResult[i - 1] = this.EncryptionResult[i - 1] ^ keys.XorKey[i - 1] ^ (this.EncryptionResult[i] & 0xFFFF);
        }

        var output = MemoryMarshal.Cast<byte, ushort>(outputBuffer);
        for (int i = 0; i < this.EncryptionResult.Length; i++)
        {
            uint result = keys.XorKey[i] ^ ((this.EncryptionResult[i] * keys.DecryptKey[i]) % keys.ModulusKey[i]);
            if (i > 0)
            {
                result ^= this.EncryptionResult[i - 1] & 0xFFFF;
            }

            output[i] = (ushort)result;
        }
    }

    private uint ReadInputBuffer(int resultIndex)
    {
        var byteOffset = GetByteOffset(resultIndex);
        var bitOffset = GetBitOffset(resultIndex);
        var firstMask = GetFirstBitMask(resultIndex);
        uint result = 0;
        result += (uint)((this._inputBuffer[byteOffset++] & firstMask) << (24 + bitOffset));
        result += (uint)(this._inputBuffer[byteOffset++] << (16 + bitOffset));
        result += (uint)((this._inputBuffer[byteOffset] & (0xFF << (8 - bitOffset))) << (8 + bitOffset));

        result = ReverseEndianness(result);
        var remainderMask = GetRemainderBitMask(resultIndex);
        var remainder = (byte)(this._inputBuffer[byteOffset] & remainderMask);
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
        var blockSuffix = this._inputBuffer.AsSpan(this.EncryptedBlockSize - 2, 2);
        //// blockSuffix[0] -> block size (encrypted)
        //// blockSuffix[1] -> checksum

        byte blockSize = (byte)(blockSuffix[0] ^ blockSuffix[1] ^ BlockSizeXorKey);

        if (blockSize > this.DecryptedBlockSize)
        {
            throw new InvalidBlockSizeException(blockSize, this.DecryptedBlockSize);
        }

        byte checksum = BlockCheckSumXorKey;
        for (int i = 0; i < this.DecryptedBlockSize; i++)
        {
            checksum ^= outputBuffer[i];
        }

        if (blockSuffix[1] != checksum)
        {
            if (!this.AcceptWrongBlockChecksum)
            {
                throw new InvalidBlockChecksumException(blockSuffix[1], checksum);
            }

            Debug.WriteLine($"Block checksum invalid. Expected: {checksum}. Actual: {blockSuffix[1]}.");
        }

        return blockSize;
    }

    /// <summary>
    /// Returns the maximum packet size of the packet in decrypted state.
    /// (The exact size needs to be decrypted first).
    /// </summary>
    /// <param name="packet">The encrypted packet.</param>
    /// <returns>The maximum packet size of the packet in decrypted state.</returns>
    private int GetMaximumDecryptedSize(Span<byte> packet)
    {
        int contentSize = packet.GetPacketSize() - packet.GetPacketHeaderSize();
        if (this.Counter != null)
        {
            // as we don't forward the counter value, we can subtract one
            contentSize--;
        }

        return (contentSize * this.DecryptedBlockSize / this.EncryptedBlockSize) + packet.GetPacketHeaderSize();
    }
}