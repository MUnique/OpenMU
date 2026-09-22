// <copyright file="IConnectionSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// Interface for a server which can provide the connections of its clients, so that their
/// traffic can be captured.
/// </summary>
/// <remarks>
/// It's implemented by the servers themselves. A server which doesn't implement it, e.g.
/// because it's just a proxy to a server in another process, is simply not listed.
/// </remarks>
public interface IConnectionSource
{
    /// <summary>
    /// Gets the currently connected clients of this server.
    /// </summary>
    /// <returns>The currently connected clients of this server.</returns>
    ValueTask<IReadOnlyList<ICapturedConnectionInfo>> GetConnectionsAsync();
}
