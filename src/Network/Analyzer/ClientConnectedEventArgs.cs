// <copyright file="ClientConnectedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;

    /// <summary>
    /// The event args for the <see cref="LiveConnectionListener.ClientConnected"/>.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectedEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ClientConnectedEventArgs(LiveConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public LiveConnection Connection { get; }
    }
}