// <copyright file="DuplexPipe.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System.IO.Pipelines;

    /// <summary>
    /// A simple implementation of a <see cref="IDuplexPipe"/> which is helpful for testing.
    /// It's pipes are configured to forward incoming data as soon as they are written.
    /// </summary>
    public class DuplexPipe : IDuplexPipe
    {
        /// <summary>
        /// Gets the send pipe.
        /// </summary>
        public Pipe SendPipe { get; } = new (new PipeOptions(pauseWriterThreshold: 1, resumeWriterThreshold: 1));

        /// <summary>
        /// Gets the receive pipe.
        /// </summary>
        public Pipe ReceivePipe { get; } = new (new PipeOptions(pauseWriterThreshold: 1, resumeWriterThreshold: 1));

        /// <summary>
        /// Gets the input pipe reader. It's the reader of the <see cref="ReceivePipe"/>.
        /// </summary>
        public PipeReader Input => this.ReceivePipe.Reader;

        /// <summary>
        /// Gets the output pipe writer. It's the writer of the <see cref="SendPipe"/>.
        /// </summary>
        public PipeWriter Output => this.SendPipe.Writer;
    }
}