// <copyright file="GameServerItemPriceDefinitionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;

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
        var gameConfiguration = TestGameServers.CreateStartableConfiguration();
        gameConfiguration.ItemPriceDefinitions.Clear();
        await using var gameServer = TestGameServers.Create(gameConfiguration);

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
        await using var gameServer = TestGameServers.Create(TestGameServers.CreateStartableConfiguration());

        await gameServer.StartAsync().ConfigureAwait(false);

        Assert.That(gameServer.ServerState, Is.EqualTo(ServerState.Started));
    }
}
