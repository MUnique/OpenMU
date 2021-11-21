// <copyright file="DuplexPipe.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.IO.Pipelines;

/// <summary>
/// A simple implementation of a <see cref="IDuplexPipe"/> which is helpful for testing.
/// It's pipes are configured to forward incoming data as soon as they are written.
/// </summary>
public class DuplexPipe : IDuplexPipe
{
    /// <summary>
    /// Initializes a new instance of <see cref="DuplexPipe"/>.
    /// </summary>
    /// <param name="options">The optional pipe options. If null, the default options are applied.</param>
    public DuplexPipe(PipeOptions? options = null)
    {
        this.SendPipe = new Pipe(options ?? PipeOptions.Default);
        this.ReceivePipe = new Pipe(options ?? PipeOptions.Default);
    }

    /// <summary>
    /// Gets the send pipe.
    /// </summary>
    public Pipe SendPipe { get; }

    /// <summary>
    /// Gets the receive pipe.
    /// </summary>
    public Pipe ReceivePipe { get; }

    /// <summary>
    /// Gets the input pipe reader. It's the reader of the <see cref="ReceivePipe"/>.
    /// </summary>
    public PipeReader Input => this.ReceivePipe.Reader;

    /// <summary>
    /// Gets the output pipe writer. It's the writer of the <see cref="SendPipe"/>.
    /// </summary>
    public PipeWriter Output => this.SendPipe.Writer;
}