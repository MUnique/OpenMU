using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;

namespace MUnique.OpenMU.GameServer.Host;

public class GameServerInitializer
{
    private readonly GameServer _gameServer;
    private readonly GameServerDefinition _definition;
    private readonly IIpAddressResolver _ipResolver;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IGameServerStateObserver _stateObserver;
    private readonly IPersistenceContextProvider _contextProvider;

    public GameServerInitializer(GameServer gameServer, GameServerDefinition definition, IIpAddressResolver ipResolver, ILoggerFactory loggerFactory, IGameServerStateObserver stateObserver, IPersistenceContextProvider contextProvider)
    {
        this._gameServer = gameServer;
        this._definition = definition;
        this._ipResolver = ipResolver;
        this._loggerFactory = loggerFactory;
        this._stateObserver = stateObserver;
        this._contextProvider = contextProvider;
    }

    public void Initialize()
    {
        foreach (var endpoint in _definition.Endpoints)
        {
            this._gameServer.AddListener(new DefaultTcpGameServerListener(
                endpoint,
                this._gameServer.CreateServerInfo(),
                this._gameServer.Context,
                this._stateObserver,
                this._ipResolver,
                this._loggerFactory));
        }

        using var context = this._contextProvider.CreateNewConfigurationContext();
        this.LoadGameClientDefinitions(context);
    }

    private void LoadGameClientDefinitions(IContext persistenceContext)
    {
        var versions = persistenceContext.Get<GameClientDefinition>().ToList();
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