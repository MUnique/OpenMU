// <copyright file="IGameServerEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net;

/// <summary>
/// Interface for an entry of a gameserver in the connect server.
/// </summary>
public interface IGameServerEntry
{
    /// <summary>
    /// Gets the server identifier.
    /// </summary>
    ushort ServerId { get; }

    /// <summary>
    /// Gets the end point under which the server is accessible.
    /// </summary>
    IPEndPoint EndPoint { get; }

    /// <summary>
    /// Gets the server load percentage.
    /// </summary>
    byte ServerLoadPercentage { get; }

    /// <summary>
    /// Gets the count of current connections.
    /// </summary>
    int CurrentConnections { get; }
}