// <copyright file="LoginTestSetup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Creates the game server context for the tests of the login, with mocks for the other servers.
/// </summary>
internal static class LoginTestSetup
{
    /// <summary>
    /// Creates a persistence context provider whose player contexts are the given one.
    /// </summary>
    /// <param name="playerContext">The player context.</param>
    /// <returns>The persistence context provider.</returns>
    public static IPersistenceContextProvider CreatePersistence(IPlayerContext playerContext)
    {
        var persistence = new Mock<IPersistenceContextProvider> { DefaultValue = DefaultValue.Mock };
        persistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(playerContext);
        return persistence.Object;
    }

    /// <summary>
    /// Creates a game server context with one map. It has to be disposed, which stops its timers.
    /// </summary>
    /// <param name="persistence">The persistence context provider.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="eventPublisher">The event publisher; a mock, when it's <c>null</c>.</param>
    /// <param name="maximumPlayers">The maximum players of the game server.</param>
    /// <returns>The game server context.</returns>
    public static GameServerContext CreateGameServerContext(IPersistenceContextProvider persistence, ILoginServer loginServer, IEventPublisher? eventPublisher = null, short maximumPlayers = 10)
    {
        var gameConfiguration = new Persistence.BasicModel.GameConfiguration();
        gameConfiguration.Maps.Add(new Persistence.BasicModel.GameMapDefinition { TerrainData = new byte[ushort.MaxValue + 3] });
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new Persistence.BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new Persistence.BasicModel.GameServerConfiguration { MaximumPlayers = maximumPlayers },
            },
            new Mock<IGuildServer>().Object,
            eventPublisher ?? new Mock<IEventPublisher>().Object,
            loginServer,
            new Mock<IFriendServer>().Object,
            persistence,
            mapInitializer,
            NullLoggerFactory.Instance,
            new PlugInManager(new List<PlugInConfiguration>(), NullLoggerFactory.Instance, null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;
        return gameServerContext;
    }
}
