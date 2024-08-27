// <copyright file="PipelinedXor32Decryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Xor;

using System.Buffers;
using System.IO.Pipelines;

/// <summary>
/// Pipelined decryptor which uses a 32 byte key for a xor encryption.
/// It's typically used to decrypt packets sent by the client to the server.
/// </summary>
public class PipelinedXor32Decryptor : PacketPipeReaderBase, IPipelinedDecryptor
{
    private readonly Pipe _pipe = new();
    private readonly byte[] _xor32Key;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedXor32Decryptor"/> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public PipelinedXor32Decryptor(PipeReader source)
        : this(source, DefaultKeys.Xor32Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedXor32Decryptor"/> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="xor32Key">The xor32 key.</param>
    public PipelinedXor32Decryptor(PipeReader source, byte[] xor32Key)
    {
        if (xor32Key.Length != 32)
        {
            throw new ArgumentException($"Xor32key must have a size of 32 bytes, but is {xor32Key.Length} bytes long.");
        }

        this.Source = source;
        this._xor32Key = xor32Key;
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public PipeReader Reader => this._pipe.Reader;

    /// <inheritdoc />
    protected override ValueTask OnCompleteAsync(Exception? exception)
    {
        return this._pipe.Writer.CompleteAsync(exception);
    }

    /// <summary>
    /// Reads the mu online packet.
    /// Decrypts the packet and writes it into our pipe.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns><see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.</returns>
    protected override async ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        this.DecryptAndWrite(packet);
        return await this.TryFlushWriterAsync(this._pipe.Writer).ConfigureAwait(false);
    }

    private void DecryptAndWrite(ReadOnlySequence<byte> packet)
    {
        // The next line is getting a span from the writer which is at least as big as the packet.
        // As I found out, it's initially about 2 kb in size and gets smaller within further
        // usage. If the previous span was used up, a new piece of memory is getting provided for us.
        var span = this._pipe.Writer.GetSpan((int)packet.Length);

        // we just want to work on a span with the exact size of the packet.
        var target = span.Slice(0, (int)packet.Length);
        packet.CopyTo(target);

        var headerSize = target.GetPacketHeaderSize();
        for (var i = target.Length - 1; i > headerSize; i--)
        {
            target[i] = (byte)(target[i] ^ target[i - 1] ^ this._xor32Key[i % 32]);
        }

        this._pipe.Writer.Advance(target.Length);
    }
}