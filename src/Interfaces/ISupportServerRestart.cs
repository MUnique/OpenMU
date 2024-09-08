// <copyright file="ISupportServerRestart.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for a class which supports to restart a server.
/// </summary>
public interface ISupportServerRestart
{
    /// <summary>
    /// Restarts all servers of this container.
    /// </summary>
    /// <param name="onDatabaseInit">If set to <c>true</c>, this method is called during a database initialization.</param>
    /// <returns></returns>
    ValueTask RestartAllAsync(bool onDatabaseInit);
}