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
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A container which keeps all <see cref="Interfaces.IConnectServer"/>s in one <see cref="IHostedService"/>.
/// </summary>
public class ConnectServerContainer : ServerContainerBase, IEnumerable<IConnectServer>
{
    private readonly IList<IManageableServer> _servers;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly ConnectServerFactory _connectServerFactory;
    private readonly IList<IConnectServer> _connectServers = new List<IConnectServer>();
    private readonly IDictionary<GameClientDefinition, IGameServerStateObserver> _observers = new Dictionary<GameClientDefinition, IGameServerStateObserver>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerContainer" /> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="connectServerFactory">The connect server factory.</param>
    /// <param name="setupService">The setup service.</param>
    public ConnectServerContainer(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, ILogger<ConnectServerContainer> logger, ConnectServerFactory connectServerFactory, SetupService setupService)
        : base(setupService, logger)
    {
        this._servers = servers;
        this._persistenceContextProvider = persistenceContextProvider;
        this._connectServerFactory = connectServerFactory;
    }

    /// <summary>
    /// Gets the observer.
    /// </summary>
    /// <param name="gameClient">The game client.</param>
    /// <returns>The observer for the client.</returns>
    public IGameServerStateObserver GetObserver(GameClientDefinition gameClient)
    {
        if (!this._observers.TryGetValue(gameClient, out var observer))
        {
            // In this case, most probably the game server gets started before the connection server.
            // As a workaround, we just create a new multicast observer, on which the connect server will be added later.
            observer = new MulticastConnectionServerStateObserver();
            this._observers[gameClient] = observer;
        }

        return observer;
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

    /// <inheritdoc />
    protected override async Task StartListenersAsync(CancellationToken cancellationToken)
    {
        foreach (var server in this._connectServers)
        {
            await server.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async Task StartInnerAsync(CancellationToken cancellationToken)
    {
        using var persistenceContext = this._persistenceContextProvider.CreateNewConfigurationContext();
        foreach (var connectServerDefinition in await persistenceContext.GetAsync<ConnectServerDefinition>().ConfigureAwait(false))
        {
            var connectServer = this._connectServerFactory.CreateConnectServer(connectServerDefinition);
            this._servers.Add(connectServer);
            this._connectServers.Add(connectServer);
            var client = connectServerDefinition.Client!;
            if (this._observers.TryGetValue(client, out var observer))
            {
                if (observer is not MulticastConnectionServerStateObserver multicastObserver)
                {
                    multicastObserver = new MulticastConnectionServerStateObserver();
                    multicastObserver.AddObserver(observer);
                    this._observers[client] = multicastObserver;
                }

                multicastObserver.AddObserver(connectServer);
            }
            else
            {
                this._observers[client] = connectServer;
            }
        }
    }

    /// <inheritdoc />
    protected override async Task StopInnerAsync(CancellationToken cancellationToken)
    {
        foreach (var connectServer in this._connectServers)
        {
            await connectServer.StopAsync(cancellationToken).ConfigureAwait(false);
            this._servers.Remove(connectServer);
        }

        this._connectServers.Clear();
    }
}