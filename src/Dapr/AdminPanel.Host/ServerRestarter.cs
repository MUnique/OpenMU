// <copyright file="ServerRestarter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of <see cref="ISupportServerRestart"/>.
/// It restarts the server by stopping and starting it.
/// </summary>
public class ServerRestarter : ISupportServerRestart
{
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerRestarter"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public ServerRestarter(IServerProvider serverProvider)
    {
        this._serverProvider = serverProvider;
    }

    /// <inheritdoc />
    public async ValueTask RestartAllAsync(bool onDatabaseInit)
    {
        var gameServers = this._serverProvider.Servers.Where(server => server.Type == ServerType.GameServer).ToList();
        foreach (var gameServer in gameServers)
        {
            await gameServer.ShutdownAsync().ConfigureAwait(false);
            // It's started again automatically by the docker host.
        }
    }
}