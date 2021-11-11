// <copyright file="IAfterSocketAcceptPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net.Sockets;

/// <summary>
/// Plugin which is executed when a client socket got accepted by the listener.
/// </summary>
internal interface IAfterSocketAcceptPlugin
{
    /// <summary>
    /// Called after the client socket got accepted by the listener.
    /// </summary>
    /// <param name="socket">The socket.</param>
    /// <returns>Flag that indicates if the socket is allowed to connect.</returns>
    bool OnAfterSocketAccept(Socket socket);
}