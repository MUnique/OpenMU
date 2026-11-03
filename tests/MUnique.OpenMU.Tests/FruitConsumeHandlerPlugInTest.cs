// <copyright file="FruitConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="FruitConsumeHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class FruitConsumeHandlerPlugInTest
{
    private const byte FruitSlot = 12;

    /// <summary>
    /// The item level of the fruit of strength.
    /// </summary>
    private const byte StrengthFruitLevel = 3;

    /// <summary>
    /// Tests that the points which a fruit removes are returned as level up points, like the client shows it.
    /// The first ten removed points always succeed.
    /// </summary>
    [Test]
    public async Task RemovedPointsAreReturnedAsLevelUpPointsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.Level] = 100;
        player.Attributes[Stats.BaseStrength] = 100;
        var fruit = new Item
        {
            Definition = new ItemDefinition { Group = 13, Number = 15, Width = 1, Height = 1, Durability = 1 },
            Durability = 1,
            Level = StrengthFruitLevel,
            ItemSlot = FruitSlot,
        };
        await player.Inventory!.AddItemAsync(FruitSlot, fruit).ConfigureAwait(false);

        var consumed = await new FruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.RemovePoints).ConfigureAwait(false);

        var removedPoints = 100 - (int)player.Attributes[Stats.BaseStrength];
        Assert.That(consumed, Is.True);
        Assert.That(removedPoints, Is.InRange(1, 9));
        Assert.That(player.SelectedCharacter!.UsedNegFruitPoints, Is.EqualTo(removedPoints));
        Assert.That(player.SelectedCharacter.LevelUpPoints, Is.EqualTo(removedPoints));
    }
}
