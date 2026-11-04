// <copyright file="PipeWriterExtension.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Diagnostics;
using System.IO.Pipelines;

/// <summary>
/// Extensions for <see cref="PipeWriter"/>.
/// </summary>
public static class PipeWriterExtension
{
    /// <summary>
    /// Advances the <see cref="PipeWriter"/> and catches possible errors.
    /// </summary>
    /// <param name="pipeWriter">The <see cref="PipeWriter"/>.</param>
    /// <param name="bytes">The number of data items written to the <see cref="T:System.Span`1" /> or <see cref="T:System.Memory`1" />.</param>
    public static void AdvanceSafely(this PipeWriter pipeWriter, int bytes)
    {
        try
        {
            pipeWriter.Advance(bytes);
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