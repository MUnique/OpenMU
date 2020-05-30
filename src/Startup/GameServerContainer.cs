// <copyright file="GameServerContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A container which keeps all <see cref="IGameServer"/>s in one <see cref="IHostedService"/>.
    /// </summary>
    public sealed class GameServerContainer : IHostedService, IDisposable
    {
        private readonly ILogger<GameServerContainer> logger;
        private readonly IList<IManageableServer> servers;
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly ConnectServerContainer connectServerContainer;
        private readonly IGuildServer guildServer;
        private readonly ILoginServer loginServer;
        private readonly IFriendServer friendServer;
        private readonly IIpAddressResolver ipResolver;
        private readonly IDictionary<int, IGameServer> gameServers;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerContainer" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="servers">The servers.</param>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="connectServerContainer">The connect server container.</param>
        /// <param name="guildServer">The guild server.</param>
        /// <param name="loginServer">The login server.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <param name="ipResolver">The ip resolver.</param>
        public GameServerContainer(
            ILogger<GameServerContainer> logger,
            IList<IManageableServer> servers,
            IDictionary<int, IGameServer> gameServers,
            IPersistenceContextProvider persistenceContextProvider,
            ConnectServerContainer connectServerContainer,
            IGuildServer guildServer,
            ILoginServer loginServer,
            IFriendServer friendServer,
            IIpAddressResolver ipResolver)
        {
            this.logger = logger;
            this.servers = servers;
            this.gameServers = gameServers;
            this.persistenceContextProvider = persistenceContextProvider;
            this.connectServerContainer = connectServerContainer;
            this.guildServer = guildServer;
            this.loginServer = loginServer;
            this.friendServer = friendServer;
            this.ipResolver = ipResolver;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var persistenceContext = this.persistenceContextProvider.CreateNewConfigurationContext();
            foreach (var gameServerDefinition in persistenceContext.Get<GameServerDefinition>())
            {
                using var loggerScope = this.logger.BeginScope("GameServer: {0}", gameServerDefinition.ServerID);
                var gameServer = new GameServer(gameServerDefinition, this.guildServer, this.loginServer, this.persistenceContextProvider, this.friendServer);
                foreach (var endpoint in gameServerDefinition.Endpoints)
                {
                    gameServer.AddListener(new DefaultTcpGameServerListener(endpoint, gameServer.ServerInfo, gameServer.Context, this.connectServerContainer.GetObserver(endpoint.Client), this.ipResolver));
                }

                this.servers.Add(gameServer);
                this.gameServers.Add(gameServer.Id, gameServer);
                this.logger.LogInformation($"Game Server {gameServer.Id} - [{gameServer.Description}] initialized");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var gameServer in this.gameServers.Values)
            {
                await gameServer.StopAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var gameServer in this.gameServers.Values)
            {
                (gameServer as IDisposable)?.Dispose();
            }
        }
    }
}