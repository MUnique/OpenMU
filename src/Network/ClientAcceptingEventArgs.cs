// <copyright file="ClientAcceptingEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.ComponentModel;
using System.Net.Sockets;

/// <summary>
/// Event args for <see cref="Listener.ClientAccepting"/> which contains the connection of the client.
/// </summary>
public class ClientAcceptingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientAcceptingEventArgs"/> class.
    /// </summary>
    /// <param name="socket">The accepting socket.</param>
    public ClientAcceptingEventArgs(Socket socket)
    {
        this.AcceptingSocket = socket;
    }

    /// <summary>
    /// Gets the accepted connection.
    /// </summary>
    public Socket AcceptingSocket { get; }
}