// <copyright file="IGameServerInstanceManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for an instance which manages game servers.
/// </summary>
public interface IGameServerInstanceManager
{
    /// <summary>
    /// Restarts all servers of this container.
    /// </summary>
    /// <param name="onDatabaseInit">If set to <c>true</c>, this method is called during a database initialization.</param>
    /// <returns>A task representing the restart all operation.</returns>
    ValueTask RestartAllAsync(bool onDatabaseInit);

    /// <summary>
    /// Initializes a game server.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <returns>A task representing the initialize game operation.</returns>
    ValueTask InitializeGameServerAsync(byte serverId);

    /// <summary>
    /// Removes the game server instance.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <returns>A task representing the remove game server operation.</returns>
    ValueTask RemoveGameServerAsync(byte serverId);
}