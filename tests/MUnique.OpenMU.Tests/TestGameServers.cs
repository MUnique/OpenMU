// <copyright file="TestGameServers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Creates game servers and configurations for the tests of the checks which a game server does before it starts.
/// </summary>
internal static class TestGameServers
{
    /// <summary>
    /// Creates a game server, without listeners, with the game configuration. Dispose it at the end of the test, because
    /// its game context starts timers.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The game server.</returns>
    public static GameServer Create(BasicModel.GameConfiguration gameConfiguration)
    {
        return new GameServer(
            new BasicModel.GameServerDefinition
            {
                ServerID = 1,
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new InMemoryPersistenceContextProvider(),
            new Mock<IFriendServer>().Object,
            NullLoggerFactory.Instance,
            new PlugInManager([], NullLoggerFactory.Instance, null, null),
            new ConfigurationChangeMediator());
    }

    /// <summary>
    /// Creates a game configuration which has what a game server checks before it starts: an item price definition and
    /// the item stat profile named <see cref="ItemStatProfile.DefaultName"/>. A test removes what it checks.
    /// </summary>
    /// <returns>The game configuration.</returns>
    public static BasicModel.GameConfiguration CreateStartableConfiguration()
    {
        var gameConfiguration = new BasicModel.GameConfiguration();
        gameConfiguration.ItemPriceDefinitions.Add(new BasicModel.ItemPriceDefinition { Name = "Jewel of Bless", BasePriceFormula = "9000000" });
        gameConfiguration.ItemStatProfiles.Add(new BasicModel.ItemStatProfile { Name = ItemStatProfile.DefaultName });
        return gameConfiguration;
    }
}
