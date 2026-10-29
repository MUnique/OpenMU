// <copyright file="GameServerItemStatProfilesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Interfaces;
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
        var gameConfiguration = TestGameServers.CreateStartableConfiguration();
        gameConfiguration.ItemStatProfiles.Clear();
        await using var gameServer = TestGameServers.Create(gameConfiguration);

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
        var gameConfiguration = TestGameServers.CreateStartableConfiguration();
        gameConfiguration.ItemStatProfiles.Clear();
        gameConfiguration.ItemStatProfiles.Add(new BasicModel.ItemStatProfile { Name = "armor" });
        await using var gameServer = TestGameServers.Create(gameConfiguration);

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
        await using var gameServer = TestGameServers.Create(TestGameServers.CreateStartableConfiguration());

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
        var gameConfiguration = TestGameServers.CreateStartableConfiguration();
        var defaultProfile = gameConfiguration.ItemStatProfiles.Single();
        var withoutProfile = new BasicModel.ItemDefinition { Group = 6, Number = 99, ItemSlot = new BasicModel.ItemSlotType() };
        gameConfiguration.Items.Add(withoutProfile);
        gameConfiguration.Items.Add(new BasicModel.ItemDefinition { Group = 6, Number = 98, ItemSlot = new BasicModel.ItemSlotType(), StatProfile = defaultProfile });
        gameConfiguration.Items.Add(new BasicModel.ItemDefinition { Group = 14, Number = 13 });
        await using var gameServer = TestGameServers.Create(gameConfiguration);

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(ResolvedItemStatProfile.GetWearableItemsWithoutProfile(gameConfiguration), Is.EqualTo(new[] { withoutProfile }));
            Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Started));
        });
    }
}
