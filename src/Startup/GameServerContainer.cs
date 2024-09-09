// <copyright file="GameServerContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A container which keeps all <see cref="IGameServer"/>s in one <see cref="IHostedService"/>.
/// </summary>
public sealed class GameServerContainer : ServerContainerBase, IGameServerInstanceManager, IDisposable
{
    private readonly ILogger<GameServerContainer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IList<IManageableServer> _servers;
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly ConnectServerContainer _connectServerContainer;
    private readonly IGuildServer _guildServer;
    private readonly ILoginServer _loginServer;
    private readonly IFriendServer _friendServer;
    private readonly IIpAddressResolver _ipResolver;
    private readonly PlugInManager _plugInManager;
    private readonly IConfigurationChangeMediator _changeMediator;
    private readonly IDictionary<int, IGameServer> _gameServers;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerContainer" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="servers">The servers.</param>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="connectServerContainer">The connect server container.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="ipResolver">The ip resolver.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="setupService">The setup service.</param>
    /// <param name="changeMediator">The change mediator.</param>
    public GameServerContainer(
        ILoggerFactory loggerFactory,
        IList<IManageableServer> servers,
        IDictionary<int, IGameServer> gameServers,
        IPersistenceContextProvider persistenceContextProvider,
        ConnectServerContainer connectServerContainer,
        IGuildServer guildServer,
        ILoginServer loginServer,
        IFriendServer friendServer,
        IIpAddressResolver ipResolver,
        PlugInManager plugInManager,
        SetupService setupService,
        IConfigurationChangeMediator changeMediator)
        : base(setupService, loggerFactory.CreateLogger<GameServerContainer>())
    {
        this._loggerFactory = loggerFactory;
        this._servers = servers;
        this._gameServers = gameServers;
        this._persistenceContextProvider = persistenceContextProvider;
        this._connectServerContainer = connectServerContainer;
        this._guildServer = guildServer;
        this._loginServer = loginServer;
        this._friendServer = friendServer;
        this._ipResolver = ipResolver;
        this._plugInManager = plugInManager;
        this._changeMediator = changeMediator;

        this._logger = this._loggerFactory.CreateLogger<GameServerContainer>();
        this._eventPublisher = new InMemoryEventPublisher(this._gameServers, this._friendServer, this._guildServer);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            (gameServer as IDisposable)?.Dispose();
        }
    }

    /// <inheritdoc />
    public async ValueTask InitializeGameServerAsync(byte serverId)
    {
        using var persistenceContext = this._persistenceContextProvider.CreateNewConfigurationContext();
        var gameServerDefinitions = await persistenceContext.GetAsync<GameServerDefinition>().ConfigureAwait(false);
        var gameServerDefinition = gameServerDefinitions.FirstOrDefault(def => def.ServerID == serverId)
                                   ?? throw new InvalidOperationException($"GameServerDefinition of server {serverId} was not found.");

        this.InitializeGameServer(gameServerDefinition);
    }

    /// <inheritdoc />
    public async ValueTask RemoveGameServerAsync(byte serverId)
    {
        using var loggerScope = this._logger.BeginScope("GameServer: {0}", serverId);
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            await gameServer.ShutdownAsync().ConfigureAwait(false);
            this._gameServers.Remove(serverId);
            this._servers.Remove(gameServer);
            this._logger.LogInformation($"Game Server {gameServer.Id} - [{gameServer.Description}] removed");
        }
        else
        {
            this._logger.LogInformation($"Game Server {serverId} not found");
        }
    }

    /// <inheritdoc />
    protected override async ValueTask BeforeStartAsync(bool onDatabaseInit, CancellationToken cancellationToken)
    {
        await base.BeforeStartAsync(onDatabaseInit, cancellationToken);
        if (!onDatabaseInit)
        {
            (this._persistenceContextProvider as IMigratableDatabaseContextProvider)?.ResetCache();
        }
    }

    /// <inheritdoc />
    protected override async Task StartInnerAsync(CancellationToken cancellationToken)
    {
        using var persistenceContext = this._persistenceContextProvider.CreateNewConfigurationContext();
        await this.LoadGameClientDefinitionsAsync(persistenceContext).ConfigureAwait(false);

        var gameServerDefinitions = await persistenceContext.GetAsync<GameServerDefinition>(cancellationToken).ConfigureAwait(false);
        foreach (var gameServerDefinition in gameServerDefinitions)
        {
            this.InitializeGameServer(gameServerDefinition);
        }
    }

    /// <inheritdoc />
    protected override async Task StartListenersAsync(CancellationToken cancellationToken)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            await gameServer.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async Task StopInnerAsync(CancellationToken cancellationToken)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            await gameServer.StopAsync(cancellationToken).ConfigureAwait(false);
            this._servers.Remove(gameServer);
        }

        this._gameServers.Clear();
    }

    private void InitializeGameServer(GameServerDefinition gameServerDefinition)
    {
        using var loggerScope = this._logger.BeginScope("GameServer: {0}", gameServerDefinition.ServerID);
        var gameServer = new GameServer(gameServerDefinition, this._guildServer, this._eventPublisher, this._loginServer, this._persistenceContextProvider, this._friendServer, this._loggerFactory, this._plugInManager, this._changeMediator);
        foreach (var endpoint in gameServerDefinition.Endpoints)
        {
            gameServer.AddListener(new DefaultTcpGameServerListener(endpoint, gameServer.CreateServerInfo(), gameServer.Context, this._connectServerContainer.GetObserver(endpoint.Client!), this._ipResolver, this._loggerFactory));
        }

        this._servers.Add(gameServer);
        this._gameServers.Add(gameServer.Id, gameServer);
        this._logger.LogInformation($"Game Server {gameServer.Id} - [{gameServer.Description}] initialized");
    }

    private async ValueTask LoadGameClientDefinitionsAsync(IContext persistenceContext)
    {
        var versions = (await persistenceContext.GetAsync<GameClientDefinition>().ConfigureAwait(false)).ToList();
        foreach (var gameClientDefinition in versions)
        {
            ClientVersionResolver.Register(
                gameClientDefinition.Version,
                new ClientVersion(gameClientDefinition.Season, gameClientDefinition.Episode, gameClientDefinition.Language));
        }

        if (versions.FirstOrDefault() is { } firstVersion)
        {
            ClientVersionResolver.DefaultVersion = ClientVersionResolver.Resolve(firstVersion.Version);
        }
    }
}