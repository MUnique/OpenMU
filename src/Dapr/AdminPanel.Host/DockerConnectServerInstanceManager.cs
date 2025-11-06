// <copyright file="DockerConnectServerInstanceManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of <see cref="IConnectServerInstanceManager"/>.
/// </summary>
public class DockerConnectServerInstanceManager : IConnectServerInstanceManager
{
    private readonly IServerProvider _serverProvider;
    private readonly IDockerClient _dockerClient;
    private readonly ILogger<DockerConnectServerInstanceManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConnectServerInstanceManager"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="dockerClient">The Docker client.</param>
    /// <param name="logger">The logger.</param>
    public DockerConnectServerInstanceManager(IServerProvider serverProvider, IDockerClient dockerClient, ILogger<DockerConnectServerInstanceManager> logger)
    {
        this._serverProvider = serverProvider;
        this._dockerClient = dockerClient;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask InitializeConnectServerAsync(Guid connectServerDefinitionId)
    {
        try
        {
            this._logger.LogInformation("Starting Docker container for Connect Server with definition ID: {DefinitionId}", connectServerDefinitionId);

            var createContainerParameters = new CreateContainerParameters
            {
                Image = "openmu-connectserver",
                Name = $"connectserver-{connectServerDefinitionId}",
                Env = new[]
                {
                    $"CONNECT_SERVER_DEFINITION_ID={connectServerDefinitionId}",
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

            this._logger.LogInformation("Successfully started Docker container {ContainerId} for Connect Server {DefinitionId}", response.ID, connectServerDefinitionId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to start Docker container for Connect Server {DefinitionId}", connectServerDefinitionId);
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask RemoveConnectServerAsync(Guid connectServerDefinitionId)
    {
        try
        {
            var connectServers = this._serverProvider.Servers
                .Where(server => server.Type == ServerType.ConnectServer)
                .FirstOrDefault(server => server.ConfigurationId == connectServerDefinitionId);
            if (connectServers is not null)
            {
                await connectServers.ShutdownAsync().ConfigureAwait(false);
            }

            // Find and remove the Docker container
            var containerName = $"connectserver-{connectServerDefinitionId}";
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
                this._logger.LogInformation("Stopping and removing Docker container {ContainerName} for Connect Server {DefinitionId}", containerName, connectServerDefinitionId);

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
            this._logger.LogError(ex, "Failed to remove Docker container for Connect Server {DefinitionId}", connectServerDefinitionId);
            throw;
        }
    }
}