// <copyright file="ServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The state of the server.
/// </summary>
public enum ServerState
{
    /// <summary>
    /// The server has finished stopping.
    /// </summary>
    Stopped,

    /// <summary>
    /// The server is currently starting, but has not yet finished initialization.
    /// </summary>
    Starting,

    /// <summary>
    /// The server started and is available.
    /// </summary>
    Started,

    /// <summary>
    /// The server is not available anymore and is stopping it's services.
    /// </summary>
    Stopping,

    /// <summary>
    /// The server state is unknown, because of a timeout.
    /// </summary>
    Timeout,
}
