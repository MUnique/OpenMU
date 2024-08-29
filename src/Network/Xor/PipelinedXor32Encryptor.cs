// <copyright file="PipelinedXor32Encryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor;

using System.Buffers;
using System.IO.Pipelines;

/// <summary>
/// Pipelined encryptor which uses a 32 byte key for a xor encryption.
/// It's typically used to encrypt packet sent by the client to the server.
/// </summary>
public class PipelinedXor32Encryptor : PacketPipeReaderBase, IPipelinedEncryptor
{
    private readonly PipeWriter _target;
    private readonly Pipe _pipe = new();
    private readonly byte[] _xor32Key;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedXor32Encryptor"/> class.
    /// </summary>
    /// <param name="target">The target.</param>
    public PipelinedXor32Encryptor(PipeWriter target)
        : this(target, DefaultKeys.Xor32Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedXor32Encryptor"/> class.
    /// </summary>
    /// <param name="target">The target.</param>
    /// /// <param name="xor32Key">The xor32 key.</param>
    public PipelinedXor32Encryptor(PipeWriter target, byte[] xor32Key)
    {
        if (xor32Key.Length != 32)
        {
            throw new ArgumentException($"Xor32key must have a size of 32 bytes, but is {xor32Key.Length} bytes long.");
        }

        this.Source = this._pipe.Reader;
        this._target = target;
        this._xor32Key = xor32Key;
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public PipeWriter Writer => this._pipe.Writer;

    /// <inheritdoc />
    protected override ValueTask OnCompleteAsync(Exception? exception)
    {
        return this._target.CompleteAsync(exception);
    }

    /// <summary>
    /// Reads the mu online packet.
    /// Encrypts the packet and writes it into the target.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns><see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.</returns>
    protected override async ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        this.EncryptAndWrite(packet);
        return await this.TryFlushWriterAsync(this._target).ConfigureAwait(false);
    }

    private void EncryptAndWrite(ReadOnlySequence<byte> packet)
    {
        var span = this._target.GetSpan((int)packet.Length);
        var result = span.Slice(0, (int)packet.Length);
        packet.CopyTo(result);

        var headerSize = result.GetPacketHeaderSize();
        for (int i = headerSize + 1; i < packet.Length; i++)
        {
            result[i] = (byte)(result[i] ^ result[i - 1] ^ this._xor32Key[i % 32]);
        }

        this._target.Advance(result.Length);
    }
}