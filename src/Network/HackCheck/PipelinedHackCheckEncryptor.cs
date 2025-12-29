// <copyright file="PipelinedHackCheckEncryptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.HackCheck;

using System.Buffers;
using System.IO.Pipelines;
using MUnique.OpenMU.Network;

/// <summary>
/// Encryptor which applies the HackCheck stream transformation.
/// </summary>
public sealed class PipelinedHackCheckEncryptor : IPipelinedEncryptor
{
    private readonly Pipe _pipe = new();
    private readonly PipeWriter _target;
    private readonly HackCheckKeys _keys;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedHackCheckEncryptor"/> class.
    /// </summary>
    /// <param name="target">The target writer.</param>
    /// <param name="keys">The HackCheck keys.</param>
    public PipelinedHackCheckEncryptor(PipeWriter target, HackCheckKeys keys)
    {
        this._target = target;
        this._keys = keys;
        _ = this.ReadSourceAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public PipeWriter Writer => this._pipe.Writer;

    private async Task ReadSourceAsync()
    {
        var source = this._pipe.Reader;
        Exception? error = null;

        try
        {
            while (true)
            {
                var result = await source.ReadAsync().ConfigureAwait(false);
                var buffer = result.Buffer;

                if (!buffer.IsEmpty)
                {
                    this.WriteEncrypted(buffer);
                    source.AdvanceTo(buffer.End);

                    var flushResult = await this._target.FlushAsync().ConfigureAwait(false);
                    if (flushResult.IsCompleted || flushResult.IsCanceled)
                    {
                        break;
                    }
                }
                else
                {
                    source.AdvanceTo(buffer.End);
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

        await this._target.CompleteAsync(error).ConfigureAwait(false);
        await source.CompleteAsync(error).ConfigureAwait(false);
    }

    private void WriteEncrypted(ReadOnlySequence<byte> buffer)
    {
        foreach (var segment in buffer)
        {
            var span = this._target.GetSpan(segment.Length);
            var destination = span.Slice(0, segment.Length);
            segment.Span.CopyTo(destination);
            HackCheckCrypto.Encrypt(destination, this._keys);
            this._target.Advance(segment.Length);
        }
    }
}
