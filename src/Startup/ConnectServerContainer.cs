// <copyright file="ConnectServerContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
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
        private readonly IList<IManageableServer> servers;
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly ConnectServerFactory connectServerFactory;
        private readonly ILogger<ConnectServerContainer> logger;
        private readonly IList<IConnectServer> connectServers = new List<IConnectServer>();
        private readonly IDictionary<GameClientDefinition, IGameServerStateObserver> observers = new Dictionary<GameClientDefinition, IGameServerStateObserver>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServerContainer" /> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="connectServerFactory">The connect server factory.</param>
        public ConnectServerContainer(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, ILogger<ConnectServerContainer> logger, ConnectServerFactory connectServerFactory)
        {
            this.servers = servers;
            this.persistenceContextProvider = persistenceContextProvider;
            this.connectServerFactory = connectServerFactory;
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var persistenceContext = this.persistenceContextProvider.CreateNewConfigurationContext();
            foreach (var connectServerDefinition in persistenceContext.Get<ConnectServerDefinition>())
            {
                var clientVersion = new ClientVersion(connectServerDefinition.Client.Season, connectServerDefinition.Client.Episode, connectServerDefinition.Client.Language);
                var connectServer = this.connectServerFactory.CreateConnectServer(connectServerDefinition, clientVersion, connectServerDefinition.GetId());
                this.servers.Add(connectServer);
                this.connectServers.Add(connectServer);

                if (this.observers.TryGetValue(connectServerDefinition.Client, out var observer))
                {
                    this.logger.LogWarning($"Multiple connect servers for game client '{connectServerDefinition.Client.Description}' configured. Only one per client makes sense.");
                    if (!(observer is MulticastConnectionServerStateObserver))
                    {
                        var multicastObserver = new MulticastConnectionServerStateObserver();
                        multicastObserver.AddObserver(observer);
                        multicastObserver.AddObserver(connectServer);
                        this.observers[connectServerDefinition.Client] = multicastObserver;
                    }
                }
                else
                {
                    this.observers[connectServerDefinition.Client] = connectServer;
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var connectServer in this.connectServers)
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
            return this.observers[gameClient];
        }

        /// <inheritdoc />
        public IEnumerator<IConnectServer> GetEnumerator()
        {
            return this.connectServers.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}