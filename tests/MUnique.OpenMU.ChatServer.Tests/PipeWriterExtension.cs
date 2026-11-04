// <copyright file="PipeWriterExtension.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests;

using System.IO.Pipelines;

/// <summary>
/// Extensions for the <see cref="PipeWriter"/>, helpful for testing.
/// </summary>
internal static class PipeWriterExtension
{
    /// <summary>
    /// Flushes and waits until the written bytes are flushed.
    /// </summary>
    /// <param name="pipeWriter">The pipe writer.</param>
    /// <param name="data">The data which should be written.</param>
    public static async Task WriteAndWaitForFlushAsync(this PipeWriter pipeWriter, ReadOnlyMemory<byte> data)
    {
        await pipeWriter.WriteAsync(data).ConfigureAwait(false);
        await pipeWriter.WaitForFlushAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Waits until all <see cref="PipeWriter.UnflushedBytes"/> are flushed.
    /// </summary>
    /// <param name="pipeWriter">The pipe writer.</param>
    public static async Task WaitForFlushAsync(this PipeWriter pipeWriter)
    {
        do
        {
            await Task.Delay(200).ConfigureAwait(false);
        }
        while (pipeWriter.UnflushedBytes > 0);
        await Task.Delay(200).ConfigureAwait(false);
    }
}