// <copyright file="PipelinedSimpleModulusEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.SimpleModulus;

using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

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

    private readonly PipeWriter _target;
    private readonly byte[] _inputBuffer;
    private readonly SimpleModulusKeys _encryptionKeys;

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
        : base(encryptionKeys.DecryptKey.Length == 4 ? Variant.New : Variant.Old)
    {
        this._target = target;
        this._encryptionKeys = encryptionKeys;
        this.Source = this.Pipe.Reader;
        this._inputBuffer = new byte[this.DecryptedBlockSize];
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public PipeWriter Writer => this.Pipe.Writer;

    /// <inheritdoc />
    protected override ValueTask OnCompleteAsync(Exception? exception)
    {
        return this._target.CompleteAsync(exception);
    }

    /// <inheritdoc />
    protected override async ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        packet.Slice(0, this.HeaderBuffer.Length).CopyTo(this.HeaderBuffer);

        if (this.HeaderBuffer[0] < 0xC3)
        {
            // we just have to write-through
            this.CopyDataIntoWriter(this._target, packet);
            return await this.TryFlushWriterAsync(this._target).ConfigureAwait(false);
        }

        this.EncryptAndWrite(packet);
        return await this.TryFlushWriterAsync(this._target).ConfigureAwait(false);
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
        var swapped = ReverseEndianness(result);
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
        var result = this._target.GetSpan(encryptedSize).Slice(0, encryptedSize);

        // setting up the header (packet type and size) in the result:
        result[0] = this.HeaderBuffer[0];
        result.SetPacketSize();

        // encrypting the content:
        var headerSize = this.HeaderBuffer.GetPacketHeaderSize();
        var input = packet.Slice(headerSize);
        this.EncryptPacketContent(input, result.Slice(headerSize));

        this._target.Advance(result.Length);
    }

    private int GetEncryptedSize(Span<byte> data)
    {
        var contentSize = this.GetContentSize(data, true);
        return (((contentSize / this.DecryptedBlockSize) + (((contentSize % this.DecryptedBlockSize) > 0) ? 1 : 0)) * this.EncryptedBlockSize) + data.GetPacketHeaderSize();
    }

    private void EncryptPacketContent(ReadOnlySequence<byte> input, Span<byte> result)
    {
        var sourceOffset = 0;
        var resultOffset = 0;
        var totalDecryptedSize = (int)input.Length;

        if (this.Counter != null)
        {
            // we process the first input block out of the loop, because we need to add the counter as prefix
            this._inputBuffer[0] = (byte)this.Counter.Count;
            if (totalDecryptedSize + 1 >= this.DecryptedBlockSize)
            {
                input.Slice(0, this.DecryptedBlockSize - 1).CopyTo(this._inputBuffer.AsSpan(1));
            }
            else
            {
                input.Slice(0, input.Length).CopyTo(this._inputBuffer.AsSpan(1));
                this._inputBuffer.AsSpan(totalDecryptedSize + 1).Clear();
            }

            var firstResultBlock = result.Slice(resultOffset, this.EncryptedBlockSize);
            var contentOfFirstBlockLength = Math.Min(this.DecryptedBlockSize, totalDecryptedSize + 1);
            this.EncryptBlock(firstResultBlock, contentOfFirstBlockLength);
            sourceOffset += this.DecryptedBlockSize - 1;
            resultOffset += this.EncryptedBlockSize;
        }

        // encrypt the rest of the blocks.
        while (sourceOffset < totalDecryptedSize)
        {
            var contentOfBlockLength = Math.Min(this.DecryptedBlockSize, totalDecryptedSize - sourceOffset);
            input.Slice(sourceOffset, contentOfBlockLength).CopyTo(this._inputBuffer);
            this._inputBuffer.AsSpan(contentOfBlockLength).Clear();
            var resultBlock = result.Slice(resultOffset, this.EncryptedBlockSize);
            this.EncryptBlock(resultBlock, contentOfBlockLength);
            sourceOffset += this.DecryptedBlockSize;
            resultOffset += this.EncryptedBlockSize;
        }

        this.Counter?.Increase();
    }

    private void EncryptBlock(Span<byte> outputBuffer, int blockSize)
    {
        outputBuffer.Clear(); // since the memory comes from the shared memory pool, it might not be initialized yet
        this.EncryptContent();
        for (int i = 0; i < this.EncryptionResult.Length; i++)
        {
            WriteResultToTarget(outputBuffer, i, this.EncryptionResult[i]);
        }

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
            checksum ^= this._inputBuffer[i];
        }

        size ^= checksum;
        outputBuffer[^2] = size;
        outputBuffer[^1] = checksum;
    }

    private void EncryptContent()
    {
        var keys = this._encryptionKeys;
        var input = MemoryMarshal.Cast<byte, ushort>(this._inputBuffer);

        this.EncryptionResult[0] = ((keys.XorKey[0] ^ input[0]) * keys.EncryptKey[0]) % keys.ModulusKey[0];
        for (int i = 1; i < this.EncryptionResult.Length; i++)
        {
            this.EncryptionResult[i] = ((keys.XorKey[i] ^ (input[i] ^ (this.EncryptionResult[i - 1] & 0xFFFF))) * keys.EncryptKey[i]) % keys.ModulusKey[i];
        }

        for (int i = 0; i < this.EncryptionResult.Length - 1; i++)
        {
            this.EncryptionResult[i] = this.EncryptionResult[i] ^ keys.XorKey[i] ^ (this.EncryptionResult[i + 1] & 0xFFFF);
        }
    }
}