// <copyright file="GameServerItemPriceDefinitionsTest.cs" company="MUnique">
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
/// Tests that a game server doesn't start without <see cref="ItemPriceDefinition"/>s,
/// because the mandatory update which adds them to existing databases is missing then.
/// </summary>
[TestFixture]
public class GameServerItemPriceDefinitionsTest
{
    /// <summary>
    /// Tests that the server stays stopped, when the configuration has no item price definitions.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task DoesNotStartWithoutItemPriceDefinitionsAsync()
    {
        var gameServer = CreateGameServer(new BasicModel.GameConfiguration());

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Stopped));
    }

    /// <summary>
    /// Tests that the server starts, when the configuration has item price definitions.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task StartsWithItemPriceDefinitionsAsync()
    {
        var gameConfiguration = new BasicModel.GameConfiguration();
        gameConfiguration.ItemPriceDefinitions.Add(new BasicModel.ItemPriceDefinition { Name = "Jewel of Bless", BasePriceFormula = "9000000" });
        var gameServer = CreateGameServer(gameConfiguration);

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Started));
    }

    private static GameServer CreateGameServer(BasicModel.GameConfiguration gameConfiguration)
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
}
