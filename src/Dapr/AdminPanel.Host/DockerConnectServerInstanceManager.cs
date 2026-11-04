// <copyright file="DockerConnectServerInstanceManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Host;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of <see cref="IConnectServerInstanceManager"/>.
/// </summary>
public class DockerConnectServerInstanceManager : IConnectServerInstanceManager
{
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConnectServerInstanceManager"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    public DockerConnectServerInstanceManager(IServerProvider serverProvider)
    {
        this._serverProvider = serverProvider;
    }

    /// <inheritdoc />
    public async ValueTask InitializeConnectServerAsync(Guid connectServerDefinitionId)
    {
        // TODO: Implement this... by starting a new docker container
    }

    /// <inheritdoc />
    public async ValueTask RemoveConnectServerAsync(Guid connectServerDefinitionId)
    {
        var connectServers = this._serverProvider.Servers
            .Where(server => server.Type == ServerType.ConnectServer)
            .FirstOrDefault(server => server.ConfigurationId == connectServerDefinitionId);
        if (connectServers is not null)
        {
            await connectServers.ShutdownAsync().ConfigureAwait(false);

            // TODO: Remove the docker container
        }
    }
}