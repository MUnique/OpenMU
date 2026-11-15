// <copyright file="TalismanOfMobilityConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.Pathfinding;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="TalismanOfMobilityConsumeHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class TalismanOfMobilityConsumeHandlerPlugInTest
{
    private static readonly Point SavedPosition = new(100, 120);

    /// <summary>
    /// The first usage saves the place and uses up one durability, the second one warps the character back to it.
    /// </summary>
    [Test]
    public async Task SavesPlaceAndReturnsToItAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.EqualTo(1));

        player.Position = new Point(30, 40);
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.Zero);
        Assert.That((player.SelectedCharacter!.PositionX, player.SelectedCharacter.PositionY), Is.EqualTo((SavedPosition.X, SavedPosition.Y)));
    }

    /// <summary>
    /// A used talisman without a saved place isn't used up.
    /// </summary>
    [Test]
    public async Task UsedTalismanWithoutSavedPlaceIsKeptAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        talisman.Durability = 1;

        Assert.That(await new TalismanOfMobilityConsumeHandlerPlugIn().ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
    }

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.Maps.Single().IsActive = true;
        return player;
    }

    private static Item CreateTalisman()
    {
        var definition = new BasicModel.ItemDefinition { Group = 13, Number = 70, Width = 1, Height = 1, Durability = 2 };
        return new BasicModel.Item { Definition = definition, Durability = 2 };
    }
}
