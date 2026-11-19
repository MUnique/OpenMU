// <copyright file="InventoryStoragePowerUpConcurrencyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using static MUnique.OpenMU.DataModel.InventoryConstants;

/// <summary>
/// Verifies that concurrent equipped-item and durability notifications keep item power-ups consistent.
/// </summary>
[TestFixture]
public class InventoryStoragePowerUpConcurrencyTests
{
    /// <summary>
    /// Ensures overlapping durability updates do not lose, duplicate, or leave stale item power-ups.
    /// </summary>
    [Test]
    public async Task ConcurrentDurabilityUpdatesKeepEquippedItemPowerUpsConsistentAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = player.Inventory!;
        var storage = inventory.AsInventoryStorage!;
        var attributes = player.Attributes!;
        var initialBonus = attributes[Stats.AttackDamageIncrease];

        var firstItem = CreateEquippedItem(player, 10);
        var secondItem = CreateEquippedItem(player, 20);

        Assert.That(await inventory.AddItemAsync(Ring1Slot, firstItem).ConfigureAwait(false), Is.True);
        Assert.That(await inventory.AddItemAsync(Ring2Slot, secondItem).ConfigureAwait(false), Is.True);

        const int updatesPerItem = 64;
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var updates = Enumerable.Range(0, updatesPerItem * 2)
            .Select(index => Task.Run(async () =>
            {
                await start.Task.ConfigureAwait(false);
                var item = index % 2 == 0 ? firstItem : secondItem;
                await storage.RaiseEquippedItemsChangedAsync(item, true, hasDurabilityFactorChanged: true).ConfigureAwait(false);
            }))
            .ToArray();

        start.TrySetResult();
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(updates).ConfigureAwait(false));

        var equippedItems = storage.EquippedItems.ToHashSet();
        Assert.Multiple(() =>
        {
            Assert.That(attributes.ItemPowerUps.Keys, Is.EquivalentTo(equippedItems),
                "Every equipped item must have exactly one power-up entry, and unequipped items must have none.");
            Assert.That(attributes.ItemPowerUps.Count, Is.EqualTo(equippedItems.Count));
            Assert.That(attributes.ItemPowerUps[firstItem], Has.Count.EqualTo(1));
            Assert.That(attributes.ItemPowerUps[secondItem], Has.Count.EqualTo(1));
            Assert.That(attributes[Stats.AttackDamageIncrease], Is.EqualTo(initialBonus + 30).Within(0.001f),
                "The final bonus must equal the sum of the currently equipped items.");
        });
    }

    private static MUnique.OpenMU.DataModel.Entities.Item CreateEquippedItem(Player player, float bonus)
    {
        var definition = player.PersistenceContext.CreateNew<ItemDefinition>();
        definition.Width = 1;
        definition.Height = 1;
        definition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition
        {
            TargetAttribute = Stats.AttackDamageIncrease,
            BaseValue = bonus,
            AggregateType = AggregateType.AddRaw,
        });

        var item = player.PersistenceContext.CreateNew<MUnique.OpenMU.DataModel.Entities.Item>();
        item.Definition = definition;
        item.Durability = 1;
        return item;
    }
}
