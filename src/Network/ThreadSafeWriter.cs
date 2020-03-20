// <copyright file="ThreadSafeWriter.cs" company="MUnique">
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
    /// We use a readonly ref struct here, so we don't allocate more memory on the stack.
    /// </remarks>
    public readonly ref struct ThreadSafeWriter
    {
        private readonly IConnection connection;
        private readonly int expectedPacketSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeWriter" /> struct.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="packetType">Type of the packet (e.g. 0xC1).</param>
        /// <param name="expectedPacketSize">Size of the expected packet.</param>
        public ThreadSafeWriter(IConnection connection, byte packetType, int expectedPacketSize)
        {
            this.connection = connection;
            this.expectedPacketSize = expectedPacketSize;
            Monitor.Enter(connection);
            try
            {
                var span = this.Span;
                span.Clear();
                span[0] = packetType;
                span.SetPacketSize();
            }
            catch (InvalidOperationException)
            {
                Monitor.Exit(connection);
                throw;
            }
        }

        /// <summary>
        /// Gets the span to write at.
        /// </summary>
        public Span<byte> Span => this.connection.Output.GetSpan(this.expectedPacketSize).Slice(0, this.expectedPacketSize);

        /// <summary>
        /// Commits the data of the <see cref="Span"/> with the expected packet size.
        /// </summary>
        public void Commit() => this.Commit(this.expectedPacketSize);

        /// <summary>
        /// Commits the data of the <see cref="Span"/> with specified packet size.
        /// </summary>
        /// <param name="packetSize">Size of the packet.</param>
        public void Commit(int packetSize)
        {
            this.connection.Output.Advance(packetSize);
            this.connection.Output.FlushAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Monitor.Exit(this.connection);
        }
    }
}
