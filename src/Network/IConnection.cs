// <copyright file="IConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;

    /// <summary>
    /// A delegate which is executed when a packet gets received from a connection.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="packet">The packet.</param>
    public delegate void PacketReceivedHandler(object sender, byte[] packet);

    /// <summary>
    /// A delegate which is executed when the connection got disconnected.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public delegate void DisconnectedHandler(object sender, EventArgs e);

    /// <summary>
    /// A connection from the client to the server.
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Occurs when a new packet is received.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Reducing needless EventArgs heap allocations.")]
        event PacketReceivedHandler PacketReceived;

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
        /// Sends the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        void Send(byte[] packet);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Begins receiving from the client.
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();
    }
}
