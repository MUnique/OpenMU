// <copyright file="GameServerInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Initialization of a <see cref="GameServer"/> which is executed before starting it.
/// </summary>
public class GameServerInitializer
{
    private readonly GameServer _gameServer;
    private readonly GameServerDefinition _definition;
    private readonly IIpAddressResolver _ipResolver;
    private readonly ILoggerFactory _loggerFactory;
    private readonly GameServerStatePublisher _statePublisher;
    private readonly IPersistenceContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerInitializer"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    /// <param name="definition">The definition.</param>
    /// <param name="ipResolver">The ip resolver.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="statePublisher">The state publisher.</param>
    /// <param name="contextProvider">The context provider.</param>
    public GameServerInitializer(GameServer gameServer, GameServerDefinition definition, IIpAddressResolver ipResolver, ILoggerFactory loggerFactory, GameServerStatePublisher statePublisher, IPersistenceContextProvider contextProvider)
    {
        this._gameServer = gameServer;
        this._definition = definition;
        this._ipResolver = ipResolver;
        this._loggerFactory = loggerFactory;
        this._statePublisher = statePublisher;
        this._contextProvider = contextProvider;
    }

    /// <summary>
    /// Initializes the game server.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        // When GS_LISTENER_PORT is set, the game server listens on this port for the first client version,
        // on the next port for the second one, and so on - regardless of the configured ports. This way,
        // every game server container can use the same ports, e.g. all pods of a Kubernetes StatefulSet.
        var listenerPortBase = int.TryParse(Environment.GetEnvironmentVariable("GS_LISTENER_PORT"), out var port) ? port : (int?)null;
        var index = 0;
        foreach (var endpoint in this._definition.Endpoints.OrderBy(ep => ep.NetworkPort))
        {
            this._gameServer.AddListener(new DefaultTcpGameServerListener(
                endpoint,
                this._gameServer.CreateServerInfo(),
                this._gameServer.Context,
                this._statePublisher.ForEndpoint(endpoint),
                this._ipResolver,
                this._loggerFactory,
                listenerPortBase + index));
            index++;
        }

        using var context = this._contextProvider.CreateNewConfigurationContext();
        await this.LoadGameClientDefinitionsAsync(context).ConfigureAwait(false);
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