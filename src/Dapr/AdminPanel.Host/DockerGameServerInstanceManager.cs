// <copyright file="DockerGameServerInstanceManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of <see cref="IGameServerInstanceManager"/>.
/// </summary>
public class DockerGameServerInstanceManager : IGameServerInstanceManager
{
    private readonly IServerProvider _serverProvider;
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<DockerGameServerInstanceManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerGameServerInstanceManager"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="dockerClient">The Docker client.</param>
    /// <param name="logger">The logger.</param>
    public DockerGameServerInstanceManager(IServerProvider serverProvider, IDockerClient dockerClient, ILogger<DockerGameServerInstanceManager> logger)
    {
        this._serverProvider = serverProvider;
        this._dockerClient = dockerClient;
        this._logger = logger;
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
        try
        {
            this._logger.LogInformation("Starting Docker container for Game Server with ID: {ServerId}", serverId);

            var createContainerParameters = new CreateContainerParameters
            {
                Image = "openmu-gameserver",
                Name = $"gameserver-{serverId}",
                Env = new[]
                {
                    $"GAME_SERVER_ID={serverId}",
                    "ASPNETCORE_ENVIRONMENT=Production",
                },
                HostConfig = new HostConfig
                {
                    RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.UnlessStopped },
                    NetworkMode = "openmu-network",
                },
            };

            var response = await this._dockerClient.Containers.CreateContainerAsync(createContainerParameters).ConfigureAwait(false);
            await this._dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters()).ConfigureAwait(false);

            this._logger.LogInformation("Successfully started Docker container {ContainerId} for Game Server {ServerId}", response.ID, serverId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to start Docker container for Game Server {ServerId}", serverId);
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask RemoveGameServerAsync(byte serverId)
    {
        try
        {
            var gameServer = this._serverProvider.Servers
                .Where(server => server.Type == ServerType.GameServer)
                .FirstOrDefault(server => server.Id == serverId);
            if (gameServer is not null)
            {
                await gameServer.ShutdownAsync().ConfigureAwait(false);
            }

            // Find and remove the Docker container
            var containerName = $"gameserver-{serverId}";
            var containers = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["name"] = new Dictionary<string, bool> { [containerName] = true },
                },
            }).ConfigureAwait(false);

            foreach (var container in containers)
            {
                this._logger.LogInformation("Stopping and removing Docker container {ContainerName} for Game Server {ServerId}", containerName, serverId);

                // Stop the container
                await this._dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 10,
                }).ConfigureAwait(false);

                // Remove the container
                await this._dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                {
                    Force = true,
                }).ConfigureAwait(false);

                this._logger.LogInformation("Successfully removed Docker container {ContainerName}", containerName);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to remove Docker container for Game Server {ServerId}", serverId);
            throw;
        }
    }
}