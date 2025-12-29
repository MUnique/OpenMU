// <copyright file="PipelinedHackCheckDecryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.HackCheck;

using System.Buffers;
using System.IO.Pipelines;
using MUnique.OpenMU.Network;

/// <summary>
/// Decryptor which applies the HackCheck stream transformation.
/// </summary>
public sealed class PipelinedHackCheckDecryptor : IPipelinedDecryptor
{
    private readonly Pipe _pipe = new();
    private readonly PipeReader _source;
    private readonly HackCheckKeys _keys;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedHackCheckDecryptor"/> class.
    /// </summary>
    /// <param name="source">The source reader.</param>
    /// <param name="keys">The HackCheck keys.</param>
    public PipelinedHackCheckDecryptor(PipeReader source, HackCheckKeys keys)
    {
        this._source = source;
        this._keys = keys;
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public PipeReader Reader => this._pipe.Reader;

    private async Task ReadSourceAsync()
    {
        Exception? error = null;

        try
        {
            while (true)
            {
                var result = await this._source.ReadAsync().ConfigureAwait(false);
                var buffer = result.Buffer;

                if (!buffer.IsEmpty)
                {
                    this.WriteDecrypted(buffer);
                    this._source.AdvanceTo(buffer.End);

                    var flushResult = await this._pipe.Writer.FlushAsync().ConfigureAwait(false);
                    if (flushResult.IsCompleted || flushResult.IsCanceled)
                    {
                        break;
                    }
                }
                else
                {
                    this._source.AdvanceTo(buffer.End);
                }

                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            error = ex;
        }

        await this._pipe.Writer.CompleteAsync(error).ConfigureAwait(false);
        await this._source.CompleteAsync(error).ConfigureAwait(false);
    }

    private void WriteDecrypted(ReadOnlySequence<byte> buffer)
    {
        foreach (var segment in buffer)
        {
            var span = this._pipe.Writer.GetSpan(segment.Length);
            var destination = span.Slice(0, segment.Length);
            segment.Span.CopyTo(destination);
            HackCheckCrypto.Decrypt(destination, this._keys);
            this._pipe.Writer.Advance(segment.Length);
        }
    }
}
