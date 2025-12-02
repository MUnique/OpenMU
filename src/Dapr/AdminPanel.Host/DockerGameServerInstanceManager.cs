// <copyright file="ServerRestarter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of <see cref="IGameServerInstanceManager"/>.
/// </summary>
public class DockerGameServerInstanceManager : IGameServerInstanceManager
{
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerGameServerInstanceManager"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public DockerGameServerInstanceManager(IServerProvider serverProvider)
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

    /// <inheritdoc />
    public async ValueTask InitializeGameServerAsync(byte serverId)
    {
        // TODO: Implement this... by starting a new docker container

    }

    /// <inheritdoc />
    public async ValueTask RemoveGameServerAsync(byte serverId)
    {
        var gameServer = this._serverProvider.Servers
            .Where(server => server.Type == ServerType.GameServer)
            .FirstOrDefault(server => server.Id == serverId);
        if (gameServer is not null)
        {
            await gameServer.ShutdownAsync().ConfigureAwait(false);
            // TODO: Remove the docker container
        }
    }
}