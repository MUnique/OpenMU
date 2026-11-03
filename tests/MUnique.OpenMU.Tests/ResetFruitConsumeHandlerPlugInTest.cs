// <copyright file="ResetFruitConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="ResetFruitConsumeHandlerPlugIn"/>s.
/// </summary>
[TestFixture]
public class ResetFruitConsumeHandlerPlugInTest
{
    private const byte FruitSlot = 12;

    /// <summary>
    /// The base strength of the character class of the test player.
    /// </summary>
    private const int BaseStrength = 28;

    /// <summary>
    /// Tests that a fruit takes back ten points per durability and returns them as level up points.
    /// </summary>
    [Test]
    public async Task TakesBackTenPointsPerDurabilityAsync()
    {
        var player = await CreatePlayerAsync(100).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 2).ConfigureAwait(false);

        var consumed = await new StrengthResetFruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(player.Attributes![Stats.BaseStrength], Is.EqualTo(80));
        Assert.That(player.SelectedCharacter!.LevelUpPoints, Is.EqualTo(20));
        Assert.That(player.SelectedCharacter.UsedNegFruitPoints, Is.Zero);
        Assert.That(fruit.Durability, Is.Zero, "The fruit is used up.");
    }

    /// <summary>
    /// Tests that the points per durability are taken from the configuration of the plugin.
    /// </summary>
    [Test]
    public async Task TakesBackConfiguredPointsPerDurabilityAsync()
    {
        var player = await CreatePlayerAsync(100).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 2).ConfigureAwait(false);
        var plugIn = new StrengthResetFruitConsumeHandlerPlugIn { Configuration = new ResetFruitConsumeHandlerConfiguration { PointsPerDurability = 5 } };

        var consumed = await plugIn.ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(player.Attributes![Stats.BaseStrength], Is.EqualTo(90));
        Assert.That(player.SelectedCharacter!.LevelUpPoints, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that a fruit doesn't take back more points than are invested.
    /// </summary>
    [Test]
    public async Task TakesBackAtMostTheInvestedPointsAsync()
    {
        var player = await CreatePlayerAsync(BaseStrength + 5).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 1).ConfigureAwait(false);

        var consumed = await new StrengthResetFruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(player.Attributes![Stats.BaseStrength], Is.EqualTo(BaseStrength));
        Assert.That(player.SelectedCharacter!.LevelUpPoints, Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that a fruit isn't used when no points are invested.
    /// </summary>
    [Test]
    public async Task NotUsedWithoutInvestedPointsAsync()
    {
        var player = await CreatePlayerAsync(BaseStrength).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 1).ConfigureAwait(false);

        var consumed = await new StrengthResetFruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(fruit.Durability, Is.EqualTo(1));
        Assert.That(player.SelectedCharacter!.LevelUpPoints, Is.Zero);
    }

    /// <summary>
    /// Tests that the fruit of command can't be used by a character class without the command stat.
    /// </summary>
    [Test]
    public async Task CommandFruitNeedsTheCommandStatAsync()
    {
        var player = await CreatePlayerAsync(100).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 1).ConfigureAwait(false);

        var consumed = await new CommandResetFruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(fruit.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a fruit can't be used while an item is equipped.
    /// </summary>
    [Test]
    public async Task NotUsedWithEquippedItemsAsync()
    {
        var player = await CreatePlayerAsync(100).ConfigureAwait(false);
        var fruit = await AddFruitAsync(player, 1).ConfigureAwait(false);
        var helm = new Item { Definition = new ItemDefinition { Group = 7, Width = 2, Height = 2 }, Durability = 1, ItemSlot = InventoryConstants.HelmSlot };
        await player.Inventory!.AddItemAsync(InventoryConstants.HelmSlot, helm).ConfigureAwait(false);

        var consumed = await new StrengthResetFruitConsumeHandlerPlugIn().ConsumeItemAsync(player, fruit, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(player.Attributes![Stats.BaseStrength], Is.EqualTo(100));
    }

    private static async Task<Player> CreatePlayerAsync(int strength)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.BaseStrength] = strength;
        return player;
    }

    private static async Task<Item> AddFruitAsync(Player player, byte durability)
    {
        var fruit = new Item
        {
            Definition = new ItemDefinition { Group = 13, Number = 54, Width = 1, Height = 1, Durability = 1 },
            Durability = durability,
            ItemSlot = FruitSlot,
        };
        await player.Inventory!.AddItemAsync(FruitSlot, fruit).ConfigureAwait(false);
        return fruit;
    }
}
