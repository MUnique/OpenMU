// <copyright file="GameServerItemStatProfilesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that a game server doesn't start without the item stat profile named <see cref="ItemStatProfile.DefaultName"/>,
/// because the item formulas read the profiles, and the mandatory update which adds them to existing databases is
/// missing then.
/// </summary>
[TestFixture]
public class GameServerItemStatProfilesTest
{
    /// <summary>
    /// Tests that the server stays stopped, when the configuration has no item stat profiles.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task DoesNotStartWithoutItemStatProfilesAsync()
    {
        var gameServer = CreateGameServer(CreateConfiguration());

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Stopped));
    }

    /// <summary>
    /// Tests that the server stays stopped, when the configuration has item stat profiles, but not the default one.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task DoesNotStartWithoutTheDefaultProfileAsync()
    {
        var gameConfiguration = CreateConfiguration();
        gameConfiguration.ItemStatProfiles.Add(new BasicModel.ItemStatProfile { Name = "armor" });
        var gameServer = CreateGameServer(gameConfiguration);

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Stopped));
    }

    /// <summary>
    /// Tests that the server starts, when the configuration has the default profile.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task StartsWithTheDefaultProfileAsync()
    {
        var gameConfiguration = CreateConfiguration();
        gameConfiguration.ItemStatProfiles.Add(new BasicModel.ItemStatProfile { Name = ItemStatProfile.DefaultName });
        var gameServer = CreateGameServer(gameConfiguration);

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Started));
    }

    /// <summary>
    /// Tests that the items which can be worn, but have no stat profile, are found for the warning at the start, and that
    /// the server starts with them.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task StartsWithWearableItemsWithoutProfileAsync()
    {
        var gameConfiguration = CreateConfiguration();
        var defaultProfile = new BasicModel.ItemStatProfile { Name = ItemStatProfile.DefaultName };
        gameConfiguration.ItemStatProfiles.Add(defaultProfile);
        var withoutProfile = new BasicModel.ItemDefinition { Group = 6, Number = 99, ItemSlot = new BasicModel.ItemSlotType() };
        gameConfiguration.Items.Add(withoutProfile);
        gameConfiguration.Items.Add(new BasicModel.ItemDefinition { Group = 6, Number = 98, ItemSlot = new BasicModel.ItemSlotType(), StatProfile = defaultProfile });
        gameConfiguration.Items.Add(new BasicModel.ItemDefinition { Group = 14, Number = 13 });
        await using var gameServer = CreateGameServer(gameConfiguration);

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(ResolvedItemStatProfile.GetWearableItemsWithoutProfile(gameConfiguration), Is.EqualTo(new[] { withoutProfile }));
            Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Started));
        });
    }

    /// <summary>
    /// Creates a configuration which has what the game server checks before it starts, except the item stat profiles.
    /// </summary>
    private static BasicModel.GameConfiguration CreateConfiguration()
    {
        var gameConfiguration = new BasicModel.GameConfiguration();
        gameConfiguration.ItemPriceDefinitions.Add(new BasicModel.ItemPriceDefinition { Name = "Jewel of Bless", BasePriceFormula = "9000000" });
        return gameConfiguration;
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
