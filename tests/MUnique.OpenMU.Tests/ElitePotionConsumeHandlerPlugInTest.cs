// <copyright file="ElitePotionConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="ElitePotionConsumeHandlerPlugIn"/>s.
/// </summary>
[TestFixture]
public class ElitePotionConsumeHandlerPlugInTest
{
    private const byte PotionSlot = 12;

    /// <summary>
    /// Tests that a potion recovers its percentage of the maximum value immediately and uses up one piece.
    /// </summary>
    /// <param name="handlerType">The type of the consume handler.</param>
    /// <param name="currentAttributeName">The name of the attribute which is recovered.</param>
    /// <param name="maximumAttributeName">The name of the attribute which holds the maximum value.</param>
    /// <param name="percentage">The expected percentage which is recovered.</param>
    [TestCase(typeof(EliteHealingPotionConsumeHandlerPlugIn), nameof(Stats.CurrentHealth), nameof(Stats.MaximumHealth), 100)]
    [TestCase(typeof(EliteManaPotionConsumeHandlerPlugIn), nameof(Stats.CurrentMana), nameof(Stats.MaximumMana), 100)]
    [TestCase(typeof(MediumEliteHealingPotionConsumeHandlerPlugIn), nameof(Stats.CurrentHealth), nameof(Stats.MaximumHealth), 65)]
    [TestCase(typeof(EliteShieldPotionConsumeHandlerPlugIn), nameof(Stats.CurrentShield), nameof(Stats.MaximumShield), 65)]
    public async Task RecoversImmediatelyAsync(Type handlerType, string currentAttributeName, string maximumAttributeName, int percentage)
    {
        var currentAttribute = GetAttribute(currentAttributeName);
        var maximum = GetAttribute(maximumAttributeName);
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![currentAttribute] = 0;
        var potion = await AddPotionAsync(player, 2).ConfigureAwait(false);
        var handler = (ElitePotionConsumeHandlerPlugIn)Activator.CreateInstance(handlerType)!;

        var consumed = await handler.ConsumeItemAsync(player, potion, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(player.Attributes[maximum], Is.GreaterThan(0));
        Assert.That(consumed, Is.True);
        Assert.That(player.Attributes[currentAttribute], Is.EqualTo((uint)(player.Attributes[maximum] * percentage / 100.0)));
        Assert.That(potion.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the recovered percentage comes from the configuration of the plugin.
    /// </summary>
    [Test]
    public async Task RecoversConfiguredPercentageAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 0;
        var potion = await AddPotionAsync(player, 1).ConfigureAwait(false);
        var handler = new EliteHealingPotionConsumeHandlerPlugIn();
        handler.Configuration = (RecoverConsumeHandlerConfiguration)handler.CreateDefaultConfig();
        handler.Configuration.TotalRecoverPercentage = 10;

        var consumed = await handler.ConsumeItemAsync(player, potion, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(player.Attributes[Stats.CurrentHealth], Is.EqualTo((uint)(player.Attributes[Stats.MaximumHealth] / 10.0)));
    }

    /// <summary>
    /// Tests that a potion can't be used again during the potion cooldown.
    /// </summary>
    [Test]
    public async Task NotUsedDuringCooldownAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var potion = await AddPotionAsync(player, 2).ConfigureAwait(false);
        var handler = new EliteManaPotionConsumeHandlerPlugIn();

        var first = await handler.ConsumeItemAsync(player, potion, null, FruitUsage.Undefined).ConfigureAwait(false);
        var second = await handler.ConsumeItemAsync(player, potion, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(first, Is.True);
        Assert.That(second, Is.False);
        Assert.That(potion.Durability, Is.EqualTo(1));
    }

    private static AttributeDefinition GetAttribute(string name)
    {
        return (AttributeDefinition)typeof(Stats).GetProperty(name)!.GetValue(null)!;
    }

    private static async Task<Item> AddPotionAsync(Player player, byte durability)
    {
        var potion = new Item
        {
            Definition = new ItemDefinition { Group = 14, Number = 70, Width = 1, Height = 1, Durability = 50 },
            Durability = durability,
            ItemSlot = PotionSlot,
        };
        await player.Inventory!.AddItemAsync(PotionSlot, potion).ConfigureAwait(false);
        return potion;
    }
}
