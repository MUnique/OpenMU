// <copyright file="ServerStatistics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Calculates statistics about the <see cref="IManageableServer"/>s.
/// </summary>
public static class ServerStatistics
{
    /// <summary>
    /// Gets the number of players which are online on the game servers.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <returns>The number of players which are online on the game servers.</returns>
    public static int GetOnlinePlayerCount(IEnumerable<IManageableServer> servers)
    {
        return servers
            .Where(s => s.Type == ServerType.GameServer && s.ServerState != ServerState.Timeout)
            .Sum(s => s.CurrentConnections);
    }

    /// <summary>
    /// Gets the number of players which can be online on the started game servers.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <returns>
    /// The number of players which can be online on the started game servers;
    /// or <c>null</c>, if one of them doesn't limit the number of players.
    /// </returns>
    public static int? GetPlayerCapacity(IEnumerable<IManageableServer> servers)
    {
        long capacity = 0;
        foreach (var server in servers.Where(s => s.Type == ServerType.GameServer && s.ServerState == ServerState.Started))
        {
            if (server.MaximumConnections == int.MaxValue)
            {
                return null;
            }

            capacity += server.MaximumConnections;
        }

        return (int)Math.Min(capacity, int.MaxValue);
    }
}
