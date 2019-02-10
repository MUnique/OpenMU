// <copyright file="IConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// A delegate which is executed when a packet gets received from a connection.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="packet">The packet.</param>
    public delegate void PipedPacketReceivedHandler(object sender, ReadOnlySequence<byte> packet);

    /// <summary>
    /// A delegate which is executed when the connection got disconnected.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public delegate void DisconnectedHandler(object sender, EventArgs e);

    /// <summary>
    /// Interface for a connection.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Occurs when a new packet got received.
        /// </summary>
        /// <remarks>
        /// Remove and implement <see cref="IDuplexPipe"/> instead?
        /// </remarks>
        event PipedPacketReceivedHandler PacketReceived;

        /// <summary>
        /// Occurs when the client disconnected.
        /// </summary>
        event DisconnectedHandler Disconnected;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IConnection"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets the pipe writer to send data.
        /// </summary>
        /// <value>
        /// The pipe writer.
        /// </value>
        PipeWriter Output { get; }

        /// <summary>
        /// Begins receiving from the client.
        /// </summary>
        /// <returns>The async task.</returns>
        Task BeginReceive();

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();
    }
}
