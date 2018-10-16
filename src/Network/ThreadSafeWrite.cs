// <copyright file="ThreadSafeWrite.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Threading;

    /// <summary>
    /// A helper struct to write safely to a <see cref="IConnection.Output" />.
    /// </summary>
    /// <remarks>
    /// We use a struct here, so we don't allocate more memory on the stack. This has some limitations, e.g. that the <see cref="exactPacketSize"/> can't be changed after the write has started.
    /// </remarks>
    public struct ThreadSafeWrite : IDisposable
    {
        private readonly IConnection connection;
        private readonly int exactPacketSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeWrite"/> struct.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="exactPacketSize">Size of the exact packet.</param>
        public ThreadSafeWrite(IConnection connection, int exactPacketSize)
        {
            this.connection = connection;
            this.exactPacketSize = exactPacketSize;
            Monitor.Enter(connection);
        }

        /// <summary>
        /// Gets the span to write at.
        /// </summary>
        public Span<byte> Span => this.connection.Output.GetSpan(this.exactPacketSize).Slice(0, this.exactPacketSize);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.connection.Output.Advance(this.exactPacketSize);
            this.connection.Output.FlushAsync();
            Monitor.Exit(this.connection);
        }
    }
}
