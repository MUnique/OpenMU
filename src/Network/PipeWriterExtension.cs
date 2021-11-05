// <copyright file="PipeWriterExtension.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Diagnostics;
    using System.IO.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for <see cref="PipeWriter"/>.
    /// </summary>
    public static class PipeWriterExtension
    {
        /// <summary>
        /// Tries to advance and to flush the <see cref="PipeWriter"/>.
        /// </summary>
        /// <param name="pipeWriter">The <see cref="PipeWriter"/>.</param>
        /// <param name="bytes">The number of data items written to the <see cref="T:System.Span`1" /> or <see cref="T:System.Memory`1" />.</param>
        public static void AdvanceAndFlushSafely(this PipeWriter pipeWriter, int bytes)
        {
            try
            {
                pipeWriter.Advance(bytes);

                Task.Run(async () =>
                {
                    try
                    {
                        await pipeWriter.FlushAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsLogging())
                        {
                            Debug.Fail(ex.ToString());
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                if (Debugger.IsLogging())
                {
                    Debug.Fail(ex.ToString());
                }
            }
        }
    }
}