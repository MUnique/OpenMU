// <copyright file="ClientAcceptedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;

    /// <summary>
    /// Event args for <see cref="Listener.ClientAccepted"/> which contains the connection of the client.
    /// </summary>
    public class ClientAcceptedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAcceptedEventArgs"/> class.
        /// </summary>
        /// <param name="acceptedConnection">The accepted connection.</param>
        public ClientAcceptedEventArgs(IConnection acceptedConnection)
        {
            this.AcceptedConnection = acceptedConnection;
        }

        /// <summary>
        /// Gets the accepted connection.
        /// </summary>
        public IConnection AcceptedConnection { get; }
    }
}