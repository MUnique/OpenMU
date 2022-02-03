// <copyright file="ConnectServerContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Collections;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ConnectServer;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A container which keeps all <see cref="Interfaces.IConnectServer"/>s in one <see cref="IHostedService"/>.
/// </summary>
public class ConnectServerContainer : IHostedService, IEnumerable<IConnectServer>
{
    private readonly IList<IManageableServer> _servers;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly ConnectServerFactory _connectServerFactory;
    private readonly ILogger<ConnectServerContainer> _logger;
    private readonly IList<IConnectServer> _connectServers = new List<IConnectServer>();
    private readonly IDictionary<GameClientDefinition, IGameServerStateObserver> _observers = new Dictionary<GameClientDefinition, IGameServerStateObserver>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerContainer" /> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="connectServerFactory">The connect server factory.</param>
    public ConnectServerContainer(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, ILogger<ConnectServerContainer> logger, ConnectServerFactory connectServerFactory)
    {
        this._servers = servers;
        this._persistenceContextProvider = persistenceContextProvider;
        this._connectServerFactory = connectServerFactory;
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var persistenceContext = this._persistenceContextProvider.CreateNewConfigurationContext();
        foreach (var connectServerDefinition in persistenceContext.Get<ConnectServerDefinition>())
        {
            var connectServer = this._connectServerFactory.CreateConnectServer(connectServerDefinition);
            this._servers.Add(connectServer);
            this._connectServers.Add(connectServer);
            var client = connectServerDefinition.Client!;
            if (this._observers.TryGetValue(client, out var observer))
            {
                this._logger.LogWarning($"Multiple connect servers for game client '{client.Description}' configured. Only one per client makes sense.");
                if (observer is not MulticastConnectionServerStateObserver)
                {
                    var multicastObserver = new MulticastConnectionServerStateObserver();
                    multicastObserver.AddObserver(observer);
                    multicastObserver.AddObserver(connectServer);
                    this._observers[client] = multicastObserver;
                }
            }
            else
            {
                this._observers[client] = connectServer;
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var connectServer in this._connectServers)
        {
            await connectServer.StopAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Gets the observer.
    /// </summary>
    /// <param name="gameClient">The game client.</param>
    /// <returns>The observer for the client.</returns>
    public IGameServerStateObserver GetObserver(GameClientDefinition gameClient)
    {
        return this._observers[gameClient];
    }

    /// <inheritdoc />
    public IEnumerator<IConnectServer> GetEnumerator()
    {
        return this._connectServers.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}